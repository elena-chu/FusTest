using Ws.Extensions.Mvvm.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;

namespace WpfUI.Menus.ViewModels
{
    public abstract class ToolsMenuViewModelBase : ViewModelBase, IToolsMenuViewModel
    {
        protected bool _isSingleSelect;
        protected bool _isOpen;
        protected bool _isStayOpen;

        public ToolsMenuViewModelBase()
        {
        }

        protected abstract void CreateChildActions();

        public event EventHandler IsOpening;
        protected virtual void OnIsOpening(EventArgs e)
        {
            EventHandler handler = IsOpening;
            handler?.Invoke(this, e);
        }

        public IList<IActionViewModel> Actions { get; } = new List<IActionViewModel>();

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (SetProperty(ref _isOpen, value) && value)
                {
                    OnIsOpening(EventArgs.Empty);
                }
            }
        }

        public bool IsStayOpen
        {
            get { return _isStayOpen; }
            set
            {
                SetProperty(ref _isStayOpen, value);
                IsOpen = value;
            }
        }

        public void TryToClose()
        {
            if (IsStayOpen)
            {
                return;
            }
            IsOpen = false;
        }

        public virtual void Initialize()
        {
            CreateChildActions();
        }

    }
}
