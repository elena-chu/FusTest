using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;
using WpfUI.Menus.Enums;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Class for Draw Overlays action
    /// </summary>
    public class MeasureActionViewModel : ActionViewModelBase<MeasureActionViewModel>
    {
        private bool _canEnterMode;
        private bool _canEnterSubUiMode;
        protected readonly IUiModeChanges _uiModeService;

        public MeasureActionViewModel(IUiModeChanges uiModeService, 
            Func<MeasureActionViewModel> factory) : base(factory)
        {
            _uiModeService = uiModeService;
            CanEnterSubUiMode = true;
        }

        public UiMode UiMode { get; protected set; }

        public string SubUiMode { get; protected set; }

        public bool CanEnterMode
        {
            get { return _canEnterMode; }
            set
            {
                if(SetProperty(ref _canEnterMode, value))
                {
                    ActionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanEnterSubUiMode
        {
            get { return _canEnterSubUiMode; }
            set
            {
                if (SetProperty(ref _canEnterSubUiMode, value))
                {
                    ActionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Indicates whether the Action can be executed. 
        /// For action with SubUiMode affected by two conditions: generic for UiMode and specific for UiMode+SubUiMode
        /// </summary>
        /// <returns></returns>
        public override bool ActionCanExecute()
        {
            bool canExecute = string.IsNullOrWhiteSpace(SubUiMode)
                ? CanEnterMode
                : CanEnterMode && CanEnterSubUiMode;
            return canExecute;
        }

        /// <summary>
        /// Calls for enter or exit drawing Overlays mode
        /// </summary>
        public override void ExecuteActionSpecific()
        {
            RaisePropertyChanged(nameof(HasActiveChildActions));

            if (IsActive || NodeType == NodeType.ChildExecuteAlways)
            {
                _uiModeService.EnterMode(UiMode, SubUiMode);
            }
            else
            {
                _uiModeService.ExitMode(UiMode);
            }
        }

        /// <summary>
        /// Executes action of button with SubMenu/SubActions.
        /// Status here in view model changed before calling action if click was not handled in GUI. For example: on GUI click was done on Selected button
        /// but in code below we are dealing already with changed status (IsActive == false)
        /// For drawing Overlay buttons if current GUI state of button is Unselected, so click on button
        /// just opens SubMenu and children are executing the real action - entering their own drawing mode.
        /// If current GUI state of button is Selected click on button changes first of all status of button in VM (IsActive == false in VM) and then calls action for exit the drawing mode.
        /// In Draw Overlay buttons Children actions have the same UiMode as parent
        /// </summary>
        protected override void ExecuteParentAction()
        {
            //Debug.WriteLine("MeasureActionViewModel ExecuteParentAction");
            if (!IsActive)//Status already changed to unselected
            {
                _uiModeService.ExitMode(UiMode);
            }
        }
        
        /// <summary>
        /// Initializes Action with specific parameters defining  Name, tree structure etc.
        /// Subscribes to events
        /// </summary>
        /// <param name="param"></param>
        public override void Initialize(ActionInitializeParams param)
        {
            Debug.Assert(param is LayerActionInitializeParams, "In Measure Menu ActionInitializeParams should be of type LayerActionInitializeParams");
            UiMode = ((LayerActionInitializeParams)param).Layer;
            SubUiMode = ((LayerActionInitializeParams)param).SubUiMode;

            base.Initialize(param);

            _uiModeService.CanEnterModeChanged += (_, ea) =>
            {
                if (ea.Mode == UiMode)
                {
                    //regular notification
                    if (string.IsNullOrWhiteSpace(ea.SubUiMode))
                    {
                        CanEnterMode = ea.CanEnter;
                    }
                    //specific notification for SubUiMode
                    if(!string.IsNullOrWhiteSpace(ea.SubUiMode) 
                        && !string.IsNullOrWhiteSpace(SubUiMode)
                        && ea.SubUiMode.ToUpper() == SubUiMode.ToUpper())
                    {
                        CanEnterSubUiMode = ea.CanEnter;
                    }
                }
            };
            _uiModeService.ModeChanged += (___, ea) =>
            {
                if (NodeType != NodeType.ContainerParentType)//doesn't have its indication status
                {
                    IsActive = ea.NewMode == UiMode;
                }
            };
            
            //Set initial state
            CanEnterMode = _uiModeService.CanEnterMode(UiMode, "");
            if(!string.IsNullOrWhiteSpace(SubUiMode))
            {
                CanEnterSubUiMode = _uiModeService.CanEnterMode(UiMode, SubUiMode);
            }
        }


    }
}
