using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Class for Show/Hide Overlays actions
    /// </summary>
    public class OverlayActionViewModel : ActionViewModelBase<OverlayActionViewModel>
    {
        private readonly IUiModeChanges _uiModeService;
        private bool _canShowHideLayer;

        public OverlayActionViewModel(IUiModeChanges uiModeService, Func<OverlayActionViewModel> factory)
            : base(factory)
        {
            _uiModeService = uiModeService;
        }

        /// <summary>
        /// Show/Hide UIMode
        /// </summary>
        public UiMode Layer { get; protected set; }

        public bool CanShowHideLayer
        {
            get { return _canShowHideLayer; }
            set
            {
                if (SetProperty(ref _canShowHideLayer, value))
                {
                    ActionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public override bool ActionCanExecute()
        {
            bool canExecute = CanShowHideLayer;
            return canExecute;
        }

        /// <summary>
        /// Implementation specific action executed on buttons without SubMenus/SubActions
        /// </summary>
        public override void ExecuteActionSpecific()
        {

            if (IsActive)
            {
                _uiModeService.ShowLayer(Layer);
            }
            else
            {
                _uiModeService.HideLayer(Layer);
            }
        }

        /// <summary>
        /// Executes action of button with SubMenu/SubActions.
        /// Depending on defined node's behavior executes different actions
        /// </summary>
        protected override void ExecuteParentAction()
        {
            //Debug.WriteLine("OverlayActionViewModel ExecuteParentAction");
            _isUpdating = true;
            foreach (var action in ChildActions)
            {

                if (NodeType == Enums.NodeType.ProxyContainerParentType)
                {
                    action.IsActive = IsActive;
                    action.ActionCommand.Execute();
                }
                else
                {
                    if (action.IsActive)
                    {
                        //TODO: Execute ActionCommand with Parent's IsActive 
                        //action.IsActive = IsActive;
                        //action.ActionCommand.Execute();
                        //action.IsActive = true;
                    }
                    //Temp for demo enable Text show/hide
                    if(Layer == UiMode.TextOverlay)
                    {
                        ExecuteActionSpecific();
                    }
                }
            }
            _isUpdating = false;
        }

        /// <summary>
        /// Initializes Action with specific parameters defining  Name, tree structure etc.
        /// Subscribes to events
        /// </summary>
        /// <param name="param"></param>
        public override void Initialize(ActionInitializeParams param)
        {
            Debug.Assert(param is LayerActionInitializeParams, "In Overlays ActionInitializeParams should be of type LayerActionInitializeParams");
            Layer = ((LayerActionInitializeParams)param).Layer;

            base.Initialize(param);

            _uiModeService.CanShowHideLayerChanged += (_, ea) =>
            {
                if (ea.Layer == Layer)
                {
                    CanShowHideLayer = ea.CanShowHide;
                }
            };
            _uiModeService.LayerVisiblityChanged += (___, ea) =>
            {
                if (Layer != UiMode.None)
                {
                    IsActive = _uiModeService.IsLayerVisible(Layer);
                }
            };

            //Set initial state
            CanShowHideLayer = _uiModeService.CanShowHideLayer(Layer);
        }

    }
}
