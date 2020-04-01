using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using WpfUI.Views;
using Prism.Regions;
using WpfUI.Calibration.Views;
using WpfUI.Overlays.Views;
using Prism.Mvvm;
using WpfUI.Menus.Views;
using WpfUI.Menus.ViewModels;

namespace WpfUI.Module
{
    class WpfUiModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("Measure", typeof(CalibrationControlView));
            regionManager.RegisterViewWithRegion("Overlays", typeof(OverlayShowHideView));
            regionManager.RegisterViewWithRegion("UpperMenu", typeof(ToolMenusContainerView));
            regionManager.RegisterViewWithRegion("LeftMenu", typeof(ToolboxesContainerView));
            regionManager.RequestNavigate("MainView", "MainView");

            return;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView>();

            ViewModelLocationProvider.Register<ToolMenusContainerView, ToolsMenusContainerViewModel>();
            ViewModelLocationProvider.Register<ToolboxesContainerView, ToolboxesContainerViewModel>();
        }
    }
}
