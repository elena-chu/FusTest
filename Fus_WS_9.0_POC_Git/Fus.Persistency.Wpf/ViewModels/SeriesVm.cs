using Ws.Dicom.Interfaces.Entities;
using Ws.Extensions.Mvvm.ViewModels;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ws.Dicom.Persistency.UI.Wpf.ViewModels
{
    /// <summary>
    /// Bindable extension for <see cref="Series"/>
    /// </summary>
    class SeriesVm : BindableWrapper<Series>
    {
        private static readonly ILogger _logger = Log.ForContext<SeriesVm>();

        private object _thumbnail;
        private bool _isBadImage;
        private bool _isSelected;

        public SeriesVm(Series series) : base(series)
        {
        }

        public Series Series => Value;

        public string SeriesInstanceUid => Series.SeriesInstanceUid;
        public string StudyInstanceUid => Series.StudyInstanceUid;
        public int? SeriesNumber => Series.SeriesNumber;
        public DateTime? SeriesDate => Series.SeriesDate;
        public string SeriesDescription => Series.SeriesDescription;
        public string Modality => Series.Modality;
        public int NumberOfSeriesRelatedInstances => Series.NumberOfSeriesRelatedInstances;
        public FDCSeriesOrientation Orientation => Series.Orientation;

        public object Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (SetProperty(ref _thumbnail, value))
                    RaisePropertyChanged(nameof(Orientation));
            }
        }

        public bool IsBadImage
        {
            get { return _isBadImage; }
            set { SetProperty(ref _isBadImage, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public void SetImage(Image image)
        {
            if (image.BadImage)
            {
                IsBadImage = true;
            }
            else
            {
                try
                {
                    Thumbnail = image.ImageAs<WriteableBitmap>();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to convert dicom object to image");
                    IsBadImage = true;
                }
            }
        }
    }
}
