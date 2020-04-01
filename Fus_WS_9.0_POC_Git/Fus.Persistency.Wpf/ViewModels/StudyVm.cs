using Ws.Dicom.Interfaces.Entities;
using Ws.Extensions.Mvvm.ViewModels;
using Ws.Dicom.Persistency.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Threading;

namespace Ws.Dicom.Persistency.UI.Wpf.ViewModels
{
    class StudyVm : BindableWrapper<Study>
    {
        private readonly Dispatcher _dispatcher;

        private bool _loading;
        private object _loadingLock = new object();

        public ObservableCollection<SeriesVm> Series { get; } = new ObservableCollection<SeriesVm>();

        public StudyVm(Dispatcher dispatcher, Study study) : base(study)
        {
            _dispatcher = dispatcher;
            Series.CollectionChanged += Series_CollectionChanged;
        }        

        public Study Study => Value;

        public string StudyInstanceUid => Study.StudyInstanceUid;
        public string StudyId => Study.StudyId;
        public string PatientId => Study.PatientId;
        public string PatientName => Study.PatientName;
        public DateTime? StudyDate => Study.StudyDate;
        public string StudyDescription => Study.StudyDescription;
        public string ModalitiesInStudy => Study.ModalitiesInStudy;
        public uint? NumberOfStudyRelatedSeries => Study.NumberOfStudyRelatedSeries;
        public uint? NumberOfStudyRelatedInstances => Study.NumberOfStudyRelatedInstances;

        public bool HasSelectedSeries => Series.Any(s => s.IsSelected);

        public async Task<bool> LoadSeriesAsync(ISearchService searchService, CancellationToken ct)
        {
            if (_loading)
                return false;

            lock (_loadingLock)
            {
                if (_loading)
                    return false;

                _loading = true;
            }

            var addSeriesBlock = new ActionBlock<Series>(async series =>
            {
                if (ct.IsCancellationRequested)
                    return;

                await _dispatcher.InvokeAsync(() =>
                {
                    Series.Add(new SeriesVm(series));
                });
            },
            new ExecutionDataflowBlockOptions { CancellationToken = ct });

            var request = new GetStudySeriesRequest { Study = Study };
            request.SeriesGot += (s, e) =>
            {
                addSeriesBlock.Post(e);
            };

            await searchService.GetStudySeriesAsync(request, ct);//.ConfigureAwait(false);

            addSeriesBlock.Complete();

            await addSeriesBlock.Completion;

            return true;
        }

        public async Task LoadSeriesImages(ISearchService searchService, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return;

            var request = new GetSeriesImageRequest { Series = Series.Select(s => s.Series) };
            request.ImageGot += (s, e) =>
            {
                if (ct.IsCancellationRequested)
                    return;

                var series = Series.FirstOrDefault(ser => ser.SeriesInstanceUid == e.SeriesInstanceUid);
                if (series == null)
                    return;

                _dispatcher.InvokeAsync(() =>
                {
                    series.SetImage(e);
                });
            };

            await searchService.GetSeriesImageAsync(request, ct).ConfigureAwait(false);
        }

        private void Series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        ((SeriesVm)item).PropertyChanged += SeriesVm_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        ((SeriesVm)item).PropertyChanged -= SeriesVm_PropertyChanged;
                    break;
                default:
                    break;
            }
        }

        private void SeriesVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.PropertyName) || e.PropertyName == nameof(SeriesVm.IsSelected))
                RaisePropertyChanged(nameof(HasSelectedSeries));
        }
    }
}
