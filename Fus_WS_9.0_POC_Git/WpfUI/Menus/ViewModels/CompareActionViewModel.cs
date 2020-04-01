using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Models;
using Ws.Fus.DicomViewer;
using Ws.Fus.DicomViewer.Interfaces;
using Ws.Fus.DicomViewer.Interfaces.Controllers;
using Ws.Fus.DicomViewer.Interfaces.Entities;
using Ws.Fus.DicomViewer.UI.Wpf;

namespace WpfUI.Menus.ViewModels
{
    public class CompareActionViewModel : ActionViewModelBase<CompareActionViewModel>
    {
        private readonly IStripsViewerLayoutController _layoutController;

        public CompareActionViewModel(
            IStripsViewerLayoutController layoutController,
            Func<CompareActionViewModel> factory)
            :base(factory)
        {
            _layoutController = layoutController;
        }

        //public CompareMode CompareMode { get; set; }
        public IStripsViewerMode CompareMode { get; set; }

        public override void ExecuteActionSpecific()
        {
            //_layoutController.SwitchMode(IsActive ? CompareMode : CompareMode.None);
            if (IsActive)
                _layoutController.SwitchMode(CompareMode);
            else
                _layoutController.SwitchMode();
        }

        public override void Initialize(ActionInitializeParams param)
        {
            base.Initialize(param);

            //_layoutController.CompareModeChanged += OnCompareModeChanged;
            //IsActive = _layoutController.GetActiveCompareMode() == CompareMode;

            _layoutController.CurrentModeChanged += OnCompareModeChanged;
            IsActive = _layoutController.CurrentMode == CompareMode;
        }

        //private void OnCompareModeChanged(object sender, CompareModeChangedEventArgs args)
        //{
        //    IsActive = CompareMode == args.CompareMode;
        //}

        private void OnCompareModeChanged(object sender, IStripsViewerMode mode) => IsActive = CompareMode == mode;
    }
}
