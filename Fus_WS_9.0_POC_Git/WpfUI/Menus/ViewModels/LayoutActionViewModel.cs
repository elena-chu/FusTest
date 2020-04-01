using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;
using WpfUI.Menus.Models;
using WpfUI.ViewModels;
using Ws.Fus.DicomViewer.Interfaces.Controllers;
using Ws.Fus.DicomViewer.Interfaces.Entities;

namespace WpfUI.Menus.ViewModels
{
    /// <summary>
    /// Class for layout selection action
    /// </summary>
    public class LayoutActionViewModel : ActionViewModelBase<LayoutActionViewModel>
    {
        private readonly IStripsViewerLayoutController _layoutController;

        public LayoutActionViewModel(
            IStripsViewerLayoutController layoutController,
            Func<LayoutActionViewModel> factory)
            :base(factory)
        {
            _layoutController = layoutController;
        }

        public StripsViewerLayout Layout { get; set; }

        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }


        public override void ExecuteActionSpecific()
        {
            IsActive = true;
            _layoutController.SwitchLayout(Layout.Name);
            
            //Temp
            //var layout = _layoutController.Layouts.First(el => el.Name == Name);
            //_layoutController.SwitchLayout(Name);
        }

        public override void ChildActionUpdated(IActionViewModel child)
        {
            _isUpdating = true;
            foreach(var childAction in ChildActions)
            {
                childAction.IsActive = childAction == child;
            }
            _isUpdating = false;
        }

        /// <summary>
        /// Initializes Action with specific parameters defining  Name, tree structure etc.
        /// </summary>
        /// <param name="param"></param>
        public override void Initialize(ActionInitializeParams param)
        {
            Debug.Assert(param is LayoutActionInitializeParams, "In Layouts ActionInitializeParams should be of type LayoutActionInitializeParams");
            base.Initialize(param);
            Layout = ((LayoutActionInitializeParams)param).Layout;
        }
    }
}
