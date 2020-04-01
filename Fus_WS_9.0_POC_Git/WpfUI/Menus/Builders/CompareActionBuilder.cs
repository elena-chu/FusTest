using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Models;
using WpfUI.Menus.ViewModels;
using Ws.Fus.DicomViewer;
using Ws.Fus.DicomViewer.Interfaces.Entities;

namespace WpfUI.Menus.Builders
{
    public class CompareActionBuilder
    {
        private readonly Func<CompareActionViewModel> _factory;

        public CompareActionBuilder(Func<CompareActionViewModel> factory)
        {
            _factory = factory;
        }

        //public CompareActionViewModel Build(CompareMode compareMode)
        //{
        //    Debug.Assert(compareMode > 0, "CompareMode can't be None");
        //    var vm = _factory();
        //    vm.CompareMode = compareMode;
        //    var param = new ActionInitializeParams();

        //    switch (compareMode)
        //    {
        //        case CompareMode.Flicker:
        //            param.Name = "Flickering";
        //            break;
        //        case CompareMode.Curtain:
        //            param.Name = "Curtain";
        //            break;
        //        case CompareMode.Fusion:
        //            param.Name = "Fusion";
        //            break;
        //        default:
        //            throw new NotSupportedException($"The {compareMode} should be active");
        //    }

        //    vm.Initialize(param);
        //    return vm;
        //}

        public CompareActionViewModel Build(IStripsViewerMode mode)
        {
            var vm = _factory();
            vm.CompareMode = mode;
            var param = new ActionInitializeParams
            {
                Name = mode.Description
            };

            vm.Initialize(param);
            return vm;
        }
    }
}
