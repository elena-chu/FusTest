using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WpfUI.ViewModels;
using Ws.Fus.Interfaces.Overlays;

namespace WpfUI.UIServices
{

	public class GuiModeButton
	{
		public static DependencyProperty GuiModeProperty = DependencyProperty.RegisterAttached("GuiMode",
			typeof(UiMode), typeof(GuiModeButton), new PropertyMetadata(GuiModeChangedCallback));

		public static void SetGuiMode(ToggleButton obj, UiMode value)
		{
			obj.SetValue(GuiModeProperty, value);
		}

		public static UiMode GetGuiMode(ToggleButton obj)
		{
			return (UiMode)obj.GetValue(GuiModeProperty);
		}

		private static void GuiModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var button = (ToggleButton)d;
 			var guiMode = (UiMode)e.NewValue;

			button.Checked += (_, __) =>
			{
                if (button.ContextMenu != null)
                {
                    button.ContextMenu.AddHandler(MenuItem.ClickEvent, (RoutedEventHandler)MenuItemClick);
                    button.ContextMenu.Closed += ContextMenuClosed;
                    button.ContextMenu.PlacementTarget = button;
                    button.ContextMenu.IsOpen = true;
                }
                else
                {
                    var vm = FindMainViewModel(button);
                    vm.EnterUIMode(guiMode, GetSubGuiMode(button));
                }
			};

			button.Unchecked += (_, __) =>
			{
				var vm = FindMainViewModel(button);
				vm.ExitUIMode(guiMode);
			};

			button.Loaded += (_, __) =>
			{
				var vm = FindMainViewModel(button);
				vm.SubscribeOnCanEnterMode(guiMode, (___, ea) => 
					button.IsEnabled= ea.CanEnter);
				vm.UiModeChanged += (___, ea) =>
					button.IsChecked = ea.NewMode == guiMode;
				button.IsEnabled = vm.CanEnterUIMode(guiMode);
            };

 		}

        private static void ContextMenuClosed(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            menu.RemoveHandler(MenuItem.ClickEvent, (RoutedEventHandler)MenuItemClick);
            menu.Closed -= ContextMenuClosed;
            var button = (ToggleButton)menu.PlacementTarget;
            var vm = FindMainViewModel(button);
            if (GetGuiMode(button) != vm.GetUIMode())
            {
                button.IsChecked = false; 
            }

        }

        private static void MenuItemClick(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            //var menuItem = (MenuItem)e.Source;
            var menuItem = (MenuItem)e.OriginalSource;
            var button = (ToggleButton)menu.PlacementTarget;
            var vm = FindMainViewModel(button);
            //vm.EnterUIMode(GetGuiMode(button), GetSubGuiMode(menuItem));
            vm.EnterUIMode(GetGuiMode(button), menuItem.Header.ToString());
        }

        public static DependencyProperty SubGuiModeProperty = DependencyProperty.RegisterAttached("SubGuiMode",
            typeof(string), typeof(GuiModeButton), new PropertyMetadata(null));

        public static void SetSubGuiMode(FrameworkElement obj, string value)
        {
            obj.SetValue(SubGuiModeProperty, value);
        }

        public static string GetSubGuiMode(FrameworkElement obj)
        {
            return (string)obj.GetValue(SubGuiModeProperty);
        }



        public static DependencyProperty ShowHideProperty = DependencyProperty.RegisterAttached("ShowHide",
			typeof(UiMode), typeof(GuiModeButton), new PropertyMetadata(ShowHideChangedCallback));

		public static void SetShowHide(ToggleButton obj, UiMode value)
		{
			obj.SetValue(ShowHideProperty, value);
		}

		public static UiMode GetShowHide(ToggleButton obj)
		{
			return (UiMode)obj.GetValue(ShowHideProperty);
		}

		private static void ShowHideChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var button = (ToggleButton)d;
			var layer = (UiMode)e.NewValue;

			button.Checked += (_, __) =>
			{
				var vm = FindMainViewModel(button);
				vm.ShowLayer(layer);
			};

			button.Unchecked += (_, __) =>
			{
				var vm = FindMainViewModel(button);
				vm.HideLayer(layer);
			};

			button.Loaded += (_, __) =>
			{
				var vm = FindMainViewModel(button);
				vm.SubscribeOnCanShowHideLayer(layer, (___, ea) =>
					button.IsEnabled = ea.CanShowHide);
                vm.LayerVisibilityChanged += (___, ea) =>
                    button.IsChecked= vm.IsLayerVisible(layer);
                button.IsEnabled = vm.CanShowHideLayer(layer);
            };

		}







		private static MainViewModel FindMainViewModel(FrameworkElement obj)
		{
			while (obj != null && !(obj.DataContext is MainViewModel))
				obj = VisualTreeHelper.GetParent(obj) as FrameworkElement;
			return (MainViewModel)obj?.DataContext;
		}

	}
}
