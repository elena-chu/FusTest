using Ws.Extensions.Mvvm.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfUI.Menus.Enums;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Base Class for all actions(buttons). Has tree structure to support Submenus
    /// </summary>
    /// <typeparam name="T">Type of inherited class</typeparam>
    public abstract class ActionViewModelBase<T> : ViewModelBase, IActionViewModel where T: IActionViewModel
    {
        protected readonly Func<T> _factory;

        protected bool _isUpdating;
        protected bool _isActive;
        protected bool _isInitialized = false;
        protected List<IActionViewModel> _childActions;
        protected bool _childrenShown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory">Factory for instantiating the specific inheriting class </param>
        public ActionViewModelBase(Func<T> factory)
        {
            _factory = factory;

            ActionCommand = new DelegateCommand(ActionExecute, ActionCanExecute);
            SetAllChildActionsActiveCommand = new DelegateCommand<bool?>(SetAllChildActionsActiveExecute, SetAllChildActionsActiveCanExecute);

            ChildActions = new List<IActionViewModel>();
        }

        #region BaseFunctionality

        #region Commands

        /// <summary>
        /// Implementation specific action executed on buttons without SubMenus/SubActions
        /// </summary>
        public abstract void ExecuteActionSpecific();

        /// <summary>
        /// Main button command
        /// </summary>
        public DelegateCommand ActionCommand { get; protected set; }
        public virtual void ActionExecute()
        {
            if (!ActionCanExecute()) //common
            {
                return;
            }

            if (HasChildActions) //Tree support
            {
                ExecuteParentAction();
            }
            else //Base action
            {
                ExecuteActionSpecific(); //Implementation specific
                if (Parent != null)
                {
                    Parent.ChildActionUpdated(this);
                }
            }

            RaisePropertyChanged(nameof(HasActiveChildActions));
        }

        public virtual bool ActionCanExecute()
        {
            bool canExecute = true;
            return canExecute;
        }

        #endregion

        /// <summary>
        /// Indicates whether current action is in Selected state 
        /// </summary>
        public virtual bool IsActive
        {
            get { return _isActive; }
            set
            {
                if(SetProperty(ref _isActive, value) && Parent != null)
                {
                    Parent.ChildActionUpdated(this);
                }
            }
        }

        public string Name { get; set; }

        /// <summary>
        /// Initializes Action with specific parameters defining  Name, tree structure etc.
        /// </summary>
        /// <param name="param"></param>
        public virtual void Initialize(ActionInitializeParams param)
        {
            Parent = param.Parent;
            Name = param.Name;
            NodeType = param.NodeType;
            CreateChildActions(param);
            _isInitialized = true;

            RaisePropertyChanged(nameof(HasChildActions));
            RaisePropertyChanged(nameof(HasActiveChildActions));
        }

        #endregion

        #region TreeSupport

        #region Commands
        
        /// <summary>
        /// Tree Support. SetAllChildActionsActive Command used in submenus with option "Select"/"Unselect All"
        /// </summary>
        public DelegateCommand<bool?> SetAllChildActionsActiveCommand { get; protected set; }

        public virtual bool SetAllChildActionsActiveCanExecute(bool? show)
        {
            return true;
        }

        public virtual void SetAllChildActionsActiveExecute(bool? active)
        {
            bool activeVal;
            if (active.HasValue)
            {
                activeVal = active.Value;
            }
            else
            {
                activeVal = HasActiveChildActions == false;
            }
            SetAllChildActionsActive(active.Value);
        }

        internal virtual void SetAllChildActionsActive(bool active)
        {
            if (!HasChildActions)
            {
                IsActive = active;
                return;
            }

            _isUpdating = true;

            foreach (var action in ChildActions)
            {
                ((ActionViewModelBase<T>)action).SetAllChildActionsActive(active);
            }

            _isUpdating = false;
            RaisePropertyChanged(nameof(HasActiveChildActions));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Tree Support. Return value: true - all active, null - any(not all) active, else - no active
        /// </summary>
        public bool? HasActiveChildActions
        {
            get
            {
                if (!HasChildActions)
                {
                    return false;
                }
                bool? ret;
                if (ChildActions.All(el => el.IsActive))
                {
                    ret = true;
                }
                else if (ChildActions.Any(el => el.IsActive))
                {
                    ret = null;
                }
                else
                {
                    ret = false;
                }
                return ret;
            }
        }

        /// <summary>
        /// Tree Support. Parent Action
        /// </summary>
        public IActionViewModel Parent { get; protected set; }

        /// <summary>
        /// Used to specify node behavior.
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// SubActions/ SubMenus actions
        /// </summary>
        public List<IActionViewModel> ChildActions
        {
            get { return _childActions; }
            set { SetProperty(ref _childActions, value); }
        }

        /// <summary>
        /// Indicates whether current Action has SubActions/SubMenus
        /// </summary>
        public virtual bool HasChildActions
        {
            get { return ChildActions != null && ChildActions.Any(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called from child action to notify UI about possible status update
        /// </summary>
        public virtual void ChildActionUpdated(IActionViewModel child)
        {
            if (!_isUpdating)
            {
                RaisePropertyChanged(nameof(HasActiveChildActions));
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Creates all child actions from params
        /// </summary>
        /// <param name="param"></param>
        protected virtual void CreateChildActions(ActionInitializeParams param)
        {
            if (param.ChildActions == null || !param.ChildActions.Any())
            {
                return;
            }

            foreach (var childParam in param.ChildActions)
            {
                childParam.Parent = this;
                IActionViewModel action = _factory(); //Creating type specific instance
                action.Initialize(childParam);
                ChildActions.Add(action);
            }
        }

        /// <summary>
        /// Executes action of button with SubMenu/SubActions.
        /// </summary>
        protected virtual void ExecuteParentAction()
        {
        }

        #endregion

        #endregion



    }
}
