using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using Prism.Regions;
using Ws.Fus.Strips.Interfaces.Services;
using Ws.Dicom.Persistency.Interfaces.Services;
using System.Windows.Threading;
using Prism.Mvvm;
using Prism.Commands;
using Ws.Dicom.Interfaces.Entities;
using Ws.Fus.Interfaces.Overlays;
using Ws.Fus.Interfaces.Calibration;
using Ws.Fus.Interfaces.Coordinates;
using Ws.Fus.Interfaces.MovementDetection;
using WpfUI.Calibration.ViewModels;
using WpfUI.MovementDetection.ViewModels;
using Ws.Fus.DicomViewer.Interfaces.Controllers;
using Ws.Fus.DicomViewer.Interfaces.Entities;
using System.Collections.ObjectModel;
using Prism.Events;
using Ws.Dicom.Persistency.Interfaces.Controllers;
using System.Diagnostics;
using Ws.Extensions.Mvvm.Events;

namespace WpfUI.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly Dispatcher _dispatcher;

        private readonly IUiModeChanges _uiModeModel;
        private readonly ISeriesSelectorController _seriesSelectorController;
        internal readonly IStripsViewerLayoutController _viewerLayoutController;
        private readonly IStripsManager _stripsManager;
        private readonly IRigidNPR _rigidNPR;
        private readonly IEventAggregator _eventAggregator;

        public event UiModeChangedEventHandler UiModeChanged;

        public event LayerVisiblityChangedEventHandler LayerVisibilityChanged;

        internal event EventHandler SeriesSelected;

        public DelegateCommand<StripsViewerLayout> SwitchLayoutCommand { get; }

        public DelegateCommand AppWorkingOcurredCommand { get; }

        public ObservableCollection<StripsViewerLayout> StripsViewerLayouts { get; }

        public TargetViewModel ACPC { get; private set; }

        public MDViewModel MD { get; private set; }

        public XDCalibViewModel XDCalib { get; private set; }


        public MainViewModel(
            Dispatcher dispatcher,
            IEventAggregator eventAggregator,
            ISeriesSelectorController seriesSelectorController,
            IUiModeChanges uiModeModel,
            ITargetLocation targetModel, 
            ITransformation transformationInterface,
            IStripsViewerLayoutController viewerLayoutController,
            ICalibration calibrationInterface,
            IMovementDetection movementDetectionModel,
            ILocateXD locateXDModel,
            IStripsManager stripsManager,
            IACPC  acpcInterface,
            IRigidNPR rigidNPRInterface)
        {
            _dispatcher = dispatcher;
            _eventAggregator = eventAggregator;
            _seriesSelectorController = seriesSelectorController;

            _uiModeModel = uiModeModel;
            _uiModeModel.ModeChanged += (s, a) => UiModeChanged?.Invoke(s, a);
            _uiModeModel.LayerVisiblityChanged += (s, a) => LayerVisibilityChanged?.Invoke(s, a);

            _viewerLayoutController = viewerLayoutController;
            _stripsManager = stripsManager;
            _rigidNPR = rigidNPRInterface;

            ACPC = new TargetViewModel(targetModel, transformationInterface, calibrationInterface, acpcInterface);

            MD = new MDViewModel(movementDetectionModel);
            XDCalib = new XDCalibViewModel(locateXDModel);

            SwitchLayoutCommand = new DelegateCommand<StripsViewerLayout>(SwitchLayout);
            AppWorkingOcurredCommand = new DelegateCommand(() => _eventAggregator.GetEvent<AppWorkingEvent>().Publish());

            StripsViewerLayouts = new ObservableCollection<StripsViewerLayout>(_viewerLayoutController.Layouts);

            _seriesSelectorController.SeriesSelected += OnSeriesSelected;
            _stripsManager.Changed += OnStripsManagerChanged;
        }        

        public void SwitchLayout() => SwitchLayout(StripsViewerLayouts.ElementAt(0));

        public void SwitchLayout(StripsViewerLayout selectedLayout) => _viewerLayoutController.SwitchLayout(selectedLayout.Name);

        public void EnterUIMode(UiMode guiMode, string subGuiMode)
        {
            _uiModeModel.EnterMode(guiMode, subGuiMode);
        }

        public void ExitUIMode(UiMode guiMode)
        {
            _uiModeModel.ExitMode(guiMode);
        }

        public bool CanEnterUIMode(UiMode guiMode)
        {
            return _uiModeModel.CanEnterMode(guiMode, null);
        }

        public UiMode GetUIMode()
        {
            return _uiModeModel.GetMode();
        }

        public void SubscribeOnCanEnterMode(UiMode guiMode, CanEnterUiModeChangedEventHandler handler)
        {
            _uiModeModel.CanEnterModeChanged += (_,ea)=> 
            {
                if (ea.Mode == guiMode) handler(this, ea);
            };
        }

        //***DIANA-TBD?
        public void SubscribeModeChanged(UiMode guiMode, UiModeChangedEventHandler handler)
        {
            _uiModeModel.ModeChanged += (_, ea) =>
            {
                if (ea.NewMode == guiMode) handler(this, ea);
            };
        }

        public void ShowLayer(UiMode layer)
        {
            if (!_uiModeModel.IsLayerVisible(layer))
                _uiModeModel.ShowLayer(layer);
        }

        public void HideLayer(UiMode layer)
        {
            if (_uiModeModel.IsLayerVisible(layer))
                _uiModeModel.HideLayer(layer);

        }

        public bool CanShowHideLayer(UiMode layer)
        {
            return _uiModeModel.CanShowHideLayer(layer);
        }

        public bool IsLayerVisible(UiMode layer)
        {
            return _uiModeModel.IsLayerVisible(layer);
        }

        public void SubscribeOnCanShowHideLayer(UiMode layer, CanShowHideLayerChangedEventHandler handler)
        {
            _uiModeModel.CanShowHideLayerChanged += (_, ea) =>
            {
                if (ea.Layer == layer) handler(this, ea);
            };
        }

        public void SubscribeOnLayerVisiblityChanged(UiMode layer, LayerVisiblityChangedEventHandler handler)
        {
            _uiModeModel.LayerVisiblityChanged += (_, ea) =>
            {
                if (ea.Layer == layer) handler(this, ea);
            };
        }

        private async void OnSeriesSelected(object sender, ICollection<Series> selectedSeries)
        {
            await Task.Run(() =>
            {
                SeriesSelected?.Invoke(this, EventArgs.Empty);
            });

            await _dispatcher.BeginInvoke((Action)(() =>
            {
                if (selectedSeries.Any())
                    _stripsManager.AddSeries(selectedSeries);
            }), DispatcherPriority.ApplicationIdle);
        }

        private void OnStripsManagerChanged(object sender, EventArgs e)
        {
            _seriesSelectorController.LoadedSeries.Clear();

            var strips = _stripsManager.GetPlanningStrips();

            foreach (var strip in strips)
                _seriesSelectorController.LoadedSeries.Add(new KeyValuePair<Series, System.Windows.Media.Imaging.BitmapSource>(strip.Series, strip.Image));
        }
    }
}
