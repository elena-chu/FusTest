using Ws.Extensions.Mvvm.Events;
using Ws.Extensions.Mvvm.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Menus.Interfaces;

namespace WpfUI.Menus.ViewModels
{
    public class ToolsMenusContainerViewModel : ViewModelBase
    {
        private bool _isStayOpen;

        protected readonly IEventAggregator _eventAggregator;

        public ToolsMenusContainerViewModel(
            IEventAggregator eventAggregator,
            OverlaysMenuViewModel overlaysMenu,
            MeasureMenuViewModel measureMenu,
            ViewMenuViewModel viewMenu,
            CompareMenuViewModel compareMenu
            )
        {
            OverlaysMenu = overlaysMenu;
            MeasureMenu = measureMenu;
            ViewMenu = viewMenu;
            CompareMenu = compareMenu;

            CreateMenues();

            //Support for closing submenus on Mouse.Click of working space
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AppWorkingEvent>().Subscribe(() => TryToClose());
        }

        public bool IsStayOpen
        {
            get { return _isStayOpen; }
            set
            {
                SetProperty(ref _isStayOpen, value);
                Menus.ForEach(el => el.IsStayOpen = value);
            }
        }

        public List<IToolsMenuViewModel> Menus { get; private set; }

        private IToolsMenuViewModel _overlaysMenu;
        public IToolsMenuViewModel OverlaysMenu
        {
            get { return _overlaysMenu; }
            private set { SetProperty(ref _overlaysMenu, value); }
        }

        private IToolsMenuViewModel _measureMenu;
        public IToolsMenuViewModel MeasureMenu
        {
            get { return _measureMenu; }
            set { SetProperty(ref _measureMenu, value); }
        }

        private IToolsMenuViewModel _viewMenu;
        public IToolsMenuViewModel ViewMenu
        {
            get { return _viewMenu; }
            set { SetProperty(ref _viewMenu, value); }
        }

        private IToolsMenuViewModel _compareMenu;
        public IToolsMenuViewModel CompareMenu
        {
            get { return _compareMenu; }
            set { SetProperty(ref _compareMenu, value); }
        }


        public void Initialize()
        {
        }

        public void TryToClose()
        {
            foreach (var menu in Menus)
            {
                menu.TryToClose();
            }
        }

        private void CreateMenues()
        {
            Menus = new List<IToolsMenuViewModel>();
            Menus.Add(OverlaysMenu);
            Menus.Add(MeasureMenu);
            Menus.Add(ViewMenu);
            Menus.Add(CompareMenu);

            Menus.ForEach(el => el.IsOpening += OnChildMenuIsOpening);
        }

        private void OnChildMenuIsOpening(object sender, EventArgs e)
        {
            foreach (var menu in Menus)
            {
                if (menu != sender)
                {
                    menu.TryToClose();
                }
            }
        }
    }
}
