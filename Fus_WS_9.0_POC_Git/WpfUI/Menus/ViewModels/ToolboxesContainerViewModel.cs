using WpfUI.Menus.Builders;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Menu for specific stage actions
    /// </summary>
    public class ToolboxesContainerViewModel: ToolsMenuViewModelBase
    {
        private readonly MeasureActionBuilder _mesureBuilder;
        private readonly OverlayActionBuilder _overayBuilder;

        public ToolboxesContainerViewModel(MeasureActionBuilder mesureBuilder,
            OverlayActionBuilder overayBuilder)
        {
            _mesureBuilder = mesureBuilder;
            _overayBuilder = overayBuilder;
            Initialize();
        }

        public MeasureActionViewModel TargetOverlay { get; set; }
        public MeasureActionViewModel RegionalOverlay { get; set; }
        public MeasureActionViewModel NPRPolygonsOverlay { get; set; }
        public MeasureActionViewModel RigidNPROverlay { get; set; }
        public MeasureActionViewModel ACPCOverlay { get; set; }
        public MeasureActionViewModel ACPCAngle90Overlay { get; set; }


        protected override void CreateChildActions()
        {
            TargetOverlay = _mesureBuilder.Build(UiMode.TargetOverlay);
            Actions.Add(TargetOverlay);

            RegionalOverlay = _mesureBuilder.Build(UiMode.RegionsOverlay);
            Actions.Add(RegionalOverlay);

            NPRPolygonsOverlay = _mesureBuilder.Build(UiMode.NPRPolygonsOverlay);
            Actions.Add(NPRPolygonsOverlay);

            RigidNPROverlay = _mesureBuilder.Build(UiMode.RigidNPROverlay);
            Actions.Add(RigidNPROverlay);

            ACPCOverlay = _mesureBuilder.Build(UiMode.ACPCOverlay);
            Actions.Add(ACPCOverlay);

            ACPCAngle90Overlay = _mesureBuilder.Build(UiMode.MeasurementAngleOverlay, MeasureActionBuilder.ACPC90);
            Actions.Add(ACPCAngle90Overlay);
        }
    }
}
