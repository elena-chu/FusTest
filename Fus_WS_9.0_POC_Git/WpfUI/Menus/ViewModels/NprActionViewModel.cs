using FusInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;

namespace WpfUI.Menus.ViewModels
{
    public class NprActionViewModel : ActionViewModelBase
    {
        private readonly IRigidNPR _rigidNPRService;

        public NprActionViewModel(IRigidNPR rigidNPRService)
        {
            _rigidNPRService = rigidNPRService;
        }

        public UiMode UiMode { get; protected set; }
        public string NPRRadius { get; set; }


        public override IActionViewModel CreateChildOfType(ActionInitializeParams param)
        {
            var action = new NprActionViewModel(_rigidNPRService);
            action.Initialize(param);
            return action;
        }

        public override void ExecuteActionSpecific()
        {
            //TODO: Check what service to use when on Overlays menu and Measure
            
            //RaisePropertyChanged(nameof(HasActiveChildActions));
            //if (IsContainerMode || HasChildActions)
            //{
            //    return;
            //}

            //if (IsActive)
            //{
            //    _uiModeService.ShowLayer(Layer);
            //}
            //else
            //{
            //    _uiModeService.HideLayer(Layer);
            //}
        }

        public override void Initialize(ActionInitializeParams param)
        {
            Debug.Assert(param is NprActionInitialiseParams, "In NPR ActionInitializeParams should be of type NprActionInitialiseParams");

            NPRRadius = ((NprActionInitialiseParams)param).NPRRadius;
            UiMode = ((NprActionInitialiseParams)param).Layer;

            base.Initialize(param);
        }

    }
}
