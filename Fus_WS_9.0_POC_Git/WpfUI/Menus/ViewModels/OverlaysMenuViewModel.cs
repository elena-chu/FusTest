using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Builders;
using WpfUI.Menus.Enums;
using WpfUI.Menus.Models;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Menu for Show/Hide overlay tools
    /// </summary>
    public class OverlaysMenuViewModel : ToolsMenuViewModelBase
    {
        private bool _isShown;

        private readonly OverlayActionBuilder _builder;

        public OverlaysMenuViewModel(OverlayActionBuilder builder)
        {
            _builder = builder;
            Initialize();
        }

        #region Commands

        /// <summary>
        /// Show All: set all shown - not toggle 
        /// </summary>
        public DelegateCommand ShowAllCommand { get; private set; }

        /// <summary>
        /// Hide All: hide all - not toggle
        /// </summary>
        public DelegateCommand HideAllCommand { get; private set; }

        /// <summary>
        /// Show/Hide active selections - toggle
        /// </summary>
        public DelegateCommand<bool> ShowHideCommand { get; private set; }

        #endregion

        /// <summary>
        /// Status of Show/Hide toggle. TODO: Check if necessary
        /// </summary>
        public bool IsShown
        {
            get { return _isShown; }
            set
            {
                SetProperty(ref _isShown, value);
            }
        }

        public OverlayActionViewModel TargetOverlay { get; set; }
        public OverlayActionViewModel FiducialsOverlay { get; set; }
        public OverlayActionViewModel MeasurementsOverlay { get; set; }
        public OverlayActionViewModel RegionsOverlay { get; set; }
        public OverlayActionViewModel BathLimitsOverlay { get; set; }
        public OverlayActionViewModel CalibrationDataOverlay { get; set; }
        public OverlayActionViewModel CtMaskOverlay { get; set; }
        public OverlayActionViewModel ScanGridOverlay { get; set; }


        //With Sub Actions
        public OverlayActionViewModel AnnotationOverlay { get; set; }
        public OverlayActionViewModel NprRegionsOverlay { get; set; }


        protected override void CreateChildActions()
        {
            TargetOverlay = _builder.Build(UiMode.TargetOverlay);
            Actions.Add(TargetOverlay);

            FiducialsOverlay = _builder.Build(UiMode.FiducialsOverlay);
            Actions.Add(FiducialsOverlay);

            MeasurementsOverlay = _builder.Build(UiMode.MeasurementOverlay);
            Actions.Add(MeasurementsOverlay);

            RegionsOverlay = _builder.Build(UiMode.RegionsOverlay);
            Actions.Add(RegionsOverlay);

            BathLimitsOverlay = _builder.Build(UiMode.BathLimitsOverlay);
            Actions.Add(BathLimitsOverlay);

            CalibrationDataOverlay = _builder.Build(UiMode.CalibrationDataOverlay);
            Actions.Add(CalibrationDataOverlay);

            CtMaskOverlay = _builder.Build(UiMode.PreOpSegmentationOverlay);
            Actions.Add(CtMaskOverlay);

            ScanGridOverlay = _builder.Build(UiMode.ScanGridOverlay);
            Actions.Add(ScanGridOverlay);

            //Temp - still no implementation
            AnnotationOverlay = _builder.Build(UiMode.None, OverlayActionBuilder.ANNOTATION);
            Actions.Add(AnnotationOverlay);

            NprRegionsOverlay = _builder.Build(UiMode.NPRPolygonsOverlay);
            Actions.Add(NprRegionsOverlay);
        }
    }

}
