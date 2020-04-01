using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Models;
using WpfUI.Menus.ViewModels;
using WpfUI.ViewModels;
using Ws.Fus.DicomViewer.Interfaces.Controllers;

namespace WpfUI.Menus.Builders
{
    /// Actions Builder for View menu
    public class ViewActionBuilder
    {
        private readonly IStripsViewerLayoutController _layoutController;
        private readonly Func<LayoutActionViewModel> _layoutFactory;

        public ViewActionBuilder(IStripsViewerLayoutController layoutController, Func<LayoutActionViewModel> layoutFactory)
        {
            _layoutController = layoutController;
            _layoutFactory = layoutFactory;
        }

        public LayoutActionViewModel BuildLayout()
        {
            LayoutActionViewModel vm = _layoutFactory();
            var layoutParam = new LayoutActionInitializeParams();
            layoutParam.ChildActions = _layoutController.Layouts
                .Select(el =>
                {
                    var param = new LayoutActionInitializeParams() { Name = el.Description, Layout = el };
                    return param;
                })
                .Cast<ActionInitializeParams>()
                .ToList();
            vm.Initialize(layoutParam);
            vm.ChildActions[0].IsActive = true;

            return vm;
        }
    }
}
