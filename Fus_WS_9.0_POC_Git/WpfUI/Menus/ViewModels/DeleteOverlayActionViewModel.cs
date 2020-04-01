using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Models;
using WpfUI.Menus.Interfaces;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Class for Delete Overlay action
    /// </summary>
    public class DeleteOverlayActionViewModel : ActionViewModelBase<DeleteOverlayActionViewModel>
    {
        protected bool _canDelete;
        protected readonly IUiModeChanges _uiModeService;

        public DeleteOverlayActionViewModel(IUiModeChanges uiModeService, 
            Func<DeleteOverlayActionViewModel> factory)
            : base(factory)
        {
            _uiModeService = uiModeService;
        }

        /// <summary>
        /// The Flag specifying whether action is regular Delete(false) or Delete All(true)
        /// </summary>
        public bool IsDeleteAll { get; set; }

        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                if (SetProperty(ref _canDelete, value))
                {
                    ActionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public override bool ActionCanExecute()
        {
            bool canExecute = CanDelete;
            return canExecute;
        }

        public override void ExecuteActionSpecific()
        {
            if(IsDeleteAll)
            {
                _uiModeService.DeleteAll();
            }
            else
            {
                _uiModeService.Delete();
            }
        }

        public override void Initialize(ActionInitializeParams param)
        {
            base.Initialize(param);

            _uiModeService.SelectedOverlaysChanged += (_, ea) =>
            {
                GetCanDelete();
            };
        }

        private void GetCanDelete()
        {
            CanDelete = IsDeleteAll
                ? _uiModeService.IsDeleteAllEnabled()
                : _uiModeService.IsDeleteEnabled();
        }
    }
}
