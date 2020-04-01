using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Regions;
using Fus.Strips.Wpf.Views;

namespace Fus.Strips.Wpf.Module
{
    public class FusStripsWpfModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion("StripsMenu", typeof(StripsMenuView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
