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
    /// Menu for drawing overlay tools
    /// </summary>
    public class MeasureMenuViewModel : ToolsMenuViewModelBase
    {
        private readonly MeasureActionBuilder _builder;

        public MeasureMenuViewModel(MeasureActionBuilder builder)
        {
            _builder = builder;
            Initialize();
        }

        public MeasureActionViewModel FiducialsOverlay { get; set; }
        public MeasureActionViewModel MeasurementDistanceOverlay { get; set; }
        public MeasureActionViewModel MeasurementAreaOverlay { get; set; }
        public MeasureActionViewModel MeasurementAngleOverlay { get; set; }

        public DeleteOverlayActionViewModel DeleteOverlay { get; set; }
        public DeleteOverlayActionViewModel DeleteAllOverlay { get; set; }

        //With Sub Actions
        public MeasureActionViewModel RigidNPROverlay { get; set; }
        //public MeasureActionViewModel AnnotationMeasurement { get; set; }


        protected override void CreateChildActions()
        {
            FiducialsOverlay = _builder.Build(UiMode.FiducialsOverlay);
            Actions.Add(FiducialsOverlay);

            MeasurementDistanceOverlay = _builder.Build(UiMode.MeasurementDistanceOverlay);
            Actions.Add(MeasurementDistanceOverlay);

            MeasurementAreaOverlay = _builder.Build(UiMode.MeasurementAreaOverlay);
            Actions.Add(MeasurementAreaOverlay);

            MeasurementAngleOverlay = _builder.Build(UiMode.MeasurementAngleOverlay);
            Actions.Add(MeasurementAngleOverlay);

            RigidNPROverlay = _builder.Build(UiMode.RigidNPROverlay);
            Actions.Add(RigidNPROverlay);

            DeleteOverlay = _builder.BuildDelete(false);
            Actions.Add(DeleteOverlay);

            DeleteAllOverlay = _builder.BuildDelete(true);
            Actions.Add(DeleteAllOverlay);
        }

    }
}
