using Ws.Extensions.Mvvm.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Specialized;
using Serilog;
using Prism.Regions;
using Ws.Extensions.AppSettings.Patterns;
using Ws.Dicom.Persistency.UI.Wpf.Properties;
using Ws.Dicom.Persistency.Interfaces.Services;
using Ws.Dicom.Interfaces.Entities;
using Prism.Events;
using Ws.Dicom.Persistency.Interfaces.Controllers;
using System.Windows.Media.Imaging;
using Prism.Commands;
using System.Windows.Forms;

namespace Ws.Dicom.Persistency.UI.Wpf.ViewModels
{
    

    [AppSettingsClass(typeof(Settings))]
    class SeriesSelectorViewModel : BindableBase, ISeriesSelectorController
    {
        private static readonly ILogger _logger = Log.ForContext<SeriesSelectorViewModel>();

        private readonly Dispatcher _dispatcher;
        private readonly IEventAggregator _eventAggregator;
        private readonly Task _getAvailableServersTask;
        private readonly ISearchServiceFactory _searchServiceFactory;

        private readonly StudiesVm _foundStudiesVm;

        private readonly AsyncCommand _findStudiesCommand;
        private readonly AsyncCommand<StudyVm> _studySelectedCommand;
        private readonly AsyncCommand<bool?> _approveSelectedSeriesCommand;
        private readonly DelegateCommand _selectDicomDirCommand;

        private ISearchService _searchService;

        private ActionBlock<StudyVm> _loadSeriesImagesBlock;
        private TransformBlock<StudyVm, StudyVm> _loadStudySeriesBlock;
        private CancellationTokenSource _pipelineCts;

        private string _selectedDicomServer;
        private string _fileSysSearchServiceDir;
        private int _downloadImagesProgress;

        public event EventHandler<ICollection<Series>> SeriesSelected;

        public FindStudiesRequest Request { get; } = new FindStudiesRequest();
        public ObservableCollection<string> DicomServers { get; } = new ObservableCollection<string>();
        public ObservableCollection<SeriesVm> SelectedSeries { get; } = new ObservableCollection<SeriesVm>();
        private ObservableCollection<KeyValuePair<Series, BitmapSource>> LoadedSeries { get; } = new ObservableCollection<KeyValuePair<Series, BitmapSource>>();

        public SeriesSelectorViewModel(Dispatcher dispatcher, IEventAggregator ea, ISearchServiceFactory searchServiceFactory)
        {
            _dispatcher = dispatcher;
            _eventAggregator = ea;

            _searchServiceFactory = searchServiceFactory;

            _findStudiesCommand = new AsyncCommand(FindStudies, CanFindStudies);
            _studySelectedCommand = new AsyncCommand<StudyVm>(GetStudyDetails);
            _approveSelectedSeriesCommand = new AsyncCommand<bool?>(ApproveSelectedSeries);
            _selectDicomDirCommand = new DelegateCommand(SelectDicomDir);

            _foundStudiesVm = new StudiesVm(_studySelectedCommand);

            _getAvailableServersTask = Task.Run(GetAvailableServersAsync);

            SelectedSeries.CollectionChanged += OnSelectedSeriesCollectionChanged;
            LoadedSeries.CollectionChanged += OnLoadedSeriesCollectionChanged;
        }        

        ICollection<KeyValuePair<Series, BitmapSource>> ISeriesSelectorController.LoadedSeries => LoadedSeries;

        [AppSettingsProp(nameof(Settings.FileSysSearchServiceDir))]
        public string FileSysSearchServiceDir
        {
            get { return _fileSysSearchServiceDir; }
            set
            {
                if (SetProperty(ref _fileSysSearchServiceDir, value))
                {
                    if (IsFileSysSearchService)
                    {
                        _findStudiesCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public StudiesVm FoundStudies => _foundStudiesVm;

        public StudiesVm LoadedStudies
        {
            get
            {
                StudiesVm studiesVm = new StudiesVm();
                if (!LoadedSeries.Any())
                    return studiesVm;

                foreach (var series in LoadedSeries)
                {
                    var seriesVm = new SeriesVm(series.Key)
                    {
                        Thumbnail = series.Value // set image buffer as thumbnail
                    };

                    var study = studiesVm.Studies.FirstOrDefault(st => st.StudyInstanceUid == series.Key.Study.StudyInstanceUid);
                    if (study == null)
                    {
                        study = new StudyVm(_dispatcher, series.Key.Study);
                        studiesVm.Studies.Add(study);
                    }

                    study.Series.Add(seriesVm);

                }

                return studiesVm;
            }
        }

        public bool IsFileSysSearchService => _searchServiceFactory.IsFileSysService(SelectedDicomServer);

        public string SelectedDicomServer
        {
            get { return _selectedDicomServer; }
            set
            {
                if (value != SelectedDicomServer)
                {
                    _selectedDicomServer = value;

                    RaisePropertyChanged(nameof(IsFileSysSearchService));

                    if (!IsFileSysSearchService)
                        _searchService = string.IsNullOrWhiteSpace(value) ? null : _searchServiceFactory.CreateService(value);

                    _findStudiesCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand FindStudiesCommand => _findStudiesCommand;

        public ICommand StudySelectedCommand => _studySelectedCommand;

        public ICommand ApproveSelectedSeriesCommand => _approveSelectedSeriesCommand;

        public ICommand SelectDicomDirCommand => _selectDicomDirCommand;


        public int DownloadImagesProgress
        {
            get { return _downloadImagesProgress; }
            set { SetProperty(ref _downloadImagesProgress, value); }
        }

        private IFileSysSearchService FileSysSearchService => _searchService as IFileSysSearchService;

        private bool CanFindStudies() =>
            IsFileSysSearchService ? !string.IsNullOrWhiteSpace(FileSysSearchServiceDir) : !string.IsNullOrWhiteSpace(SelectedDicomServer);

        private async Task FindStudies(CancellationToken ct)
        {
            FoundStudies.Studies.Clear();

            await CancelPipeline(false);

            if (IsFileSysSearchService)
            {
                if (FileSysSearchService == null || FileSysSearchService.SearchDir != FileSysSearchServiceDir)
                    _searchService = _searchServiceFactory.CreateFileSysSearchService(FileSysSearchServiceDir);
            }

            CreatePipilene();

            EventHandler<Study> eventHandler = (s, e) =>
            {
                _dispatcher.InvokeAsync(() =>
                {
                    if (ct.IsCancellationRequested)
                        return;

                    FoundStudies.Studies.Add(new StudyVm(_dispatcher, e));
                });
            };

            Request.StudyFound += eventHandler;

            try
            {
                await _searchService.FindStudiesAsync(Request, ct).ConfigureAwait(false);
            }
            finally
            {
                Request.StudyFound -= eventHandler;
            }
        }        

        private async Task GetAvailableServersAsync()
        {
            var progress = new Progress<string>(server =>
            {
                _dispatcher.InvokeAsync(() => DicomServers.Add(server));
            });

            await _searchServiceFactory.GetAvailableServersAsync(progress).ConfigureAwait(false);
        }

        private async Task GetStudyDetails(StudyVm study)
        {
            if (study == null)
                return;

            await _loadStudySeriesBlock.SendAsync(study);
        }

        private async Task ApproveSelectedSeries(bool? approve)
        {
            await CancelPipeline();

            if (!approve.GetValueOrDefault() || !SelectedSeries.Any())
            {
                SeriesSelected?.Invoke(this, new Series[] { });
                return;
            }

            DownloadImagesProgress = 0;

            var selectedSeries = SelectedSeries.Select(vm => vm.Series); // unwrap

            var request = new GetSeriesImagesRequest
            {
                Series = selectedSeries
            };

            request.Progress += (s, e) =>
            {
                DownloadImagesProgress = e;
            };

            await _searchService.GetSeriesImagesAsync(request, CancellationToken.None);

            SeriesSelected?.Invoke(this, selectedSeries.ToArray());

            SelectedSeries.Clear();

            DownloadImagesProgress = 0;
        }

        private void SelectDicomDir()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    FileSysSearchServiceDir = fbd.SelectedPath;
            }
        }

        private void CreatePipilene()
        {
            _pipelineCts = new CancellationTokenSource();
            _loadStudySeriesBlock = new TransformBlock<StudyVm, StudyVm>(async study =>
            {
                var seriesLoaded = false;
                try
                {
                    seriesLoaded = await study.LoadSeriesAsync(_searchService, _pipelineCts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to load series for study {StudyInstanceUid}", study.StudyInstanceUid);
                }

                return seriesLoaded ? study : null;
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = _pipelineCts.Token,
                MaxDegreeOfParallelism = _searchService.MaxGetStudySeriesRequests
            });

            _loadSeriesImagesBlock = new ActionBlock<StudyVm>(async study =>
            {
                if (study == null)
                    return;

                try
                {
                    await study.LoadSeriesImages(_searchService, _pipelineCts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to load series images of study {StudyInstanceUid}", study.StudyInstanceUid);
                }
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = _pipelineCts.Token,
                MaxDegreeOfParallelism = _searchService.MaxGetSeriesImageRequests
            });

            _loadStudySeriesBlock.LinkTo(_loadSeriesImagesBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        private async Task CancelPipeline(bool recreate)
        {
            if (_pipelineCts != null)
            {
                _pipelineCts.Cancel();

                try
                {
                    await _loadSeriesImagesBlock.Completion;
                }
                catch (OperationCanceledException ex)
                {
                }
            }

            if (recreate)
                CreatePipilene();
        }

        private async Task CancelPipeline() => await CancelPipeline(true);

        private void OnSelectedSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        ((SeriesVm)item).IsSelected = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        ((SeriesVm)item).IsSelected = false;
                    break;
                default:
                    break;
            }
        }

        private void OnLoadedSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RaisePropertyChanged(nameof(LoadedStudies));
    }
}
