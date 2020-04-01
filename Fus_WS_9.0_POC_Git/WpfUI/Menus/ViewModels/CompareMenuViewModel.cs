using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Builders;
using Ws.Fus.DicomViewer.Interfaces.Controllers;
using Ws.Fus.DicomViewer.Secret;

namespace WpfUI.Menus.ViewModels
{
    public class CompareMenuViewModel : ToolsMenuViewModelBase
    {
        private readonly IStripsViewerLayoutController _layoutController;
        private readonly CompareActionBuilder _builder;

        public CompareMenuViewModel(IStripsViewerLayoutController layoutController, CompareActionBuilder builder)
        {
            _layoutController = layoutController;
            _builder = builder;
            Initialize();
        }

        public CompareActionViewModel Flicker { get; set; }

        public CompareActionViewModel Curtain { get; set; }

        public CompareActionViewModel Fusion { get; set; }

        //protected override void CreateChildActions()
        //{
        //    Flicker = _builder.Build(CompareMode.Flicker);
        //    Actions.Add(Flicker);

        //    Curtain = _builder.Build(CompareMode.Curtain);
        //    Actions.Add(Curtain);

        //    Fusion = _builder.Build(CompareMode.Fusion);
        //    Actions.Add(Fusion);
        //}

        protected override void CreateChildActions()
        {
            // So far only slider implemented
            var modes = _layoutController.Modes;

            var curtainMode = modes.First(m => (m as ICompareModeAware).Mode == CompareMode.Curtain);

            Flicker = _builder.Build(curtainMode);
            Actions.Add(Flicker);

            Curtain = _builder.Build(curtainMode);
            Actions.Add(Curtain);

            Fusion = _builder.Build(curtainMode);
            Actions.Add(Fusion);
        }
    }
}
