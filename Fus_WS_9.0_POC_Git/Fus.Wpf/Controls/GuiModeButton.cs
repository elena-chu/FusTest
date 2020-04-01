using Ws.Fus.UI.Wpf.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ws.Fus.Interfaces.Overlays;

namespace Ws.Fus.UI.Wpf.Views
{
    public class GuiModeButton : ToggleButton
    {
        public static readonly DependencyProperty GuiModeProperty =
            DependencyProperty.Register("GuiMode", typeof(UiMode), typeof(GuiModeButton), new PropertyMetadata(OnGuiModeChanged));

        public static readonly DependencyProperty SubGuiModeProperty =
            DependencyProperty.Register("SubGuiMode", typeof(string), typeof(GuiModeButton), new PropertyMetadata(null));

        private GuiModeButtonViewModel _dataContext;

        public GuiModeButton()
        {
            SetValue(ViewModelLocator.AutoWireViewModelProperty, true);

            Loaded += (s, e) =>
            {
                _dataContext = DataContext as GuiModeButtonViewModel;
            };
        }

        public UiMode GuiMode
        {
            get { return (UiMode)GetValue(GuiModeProperty); }
            set { SetValue(GuiModeProperty, value); }
        }

        public string SubGuiMode
        {
            get { return (string)GetValue(SubGuiModeProperty); }
            set { SetValue(SubGuiModeProperty, value); }
        }

        private IUiModeChanges UiModeChanges => _dataContext?.UiModeChanges;

        private static void OnGuiModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            (d as GuiModeButton)?.OnGuiModeChanged(e);

        private static void OnContextMenuClosed(object sender, RoutedEventArgs e) =>
            ((sender as ContextMenu).PlacementTarget as GuiModeButton).OnContextMenuClosed(e);

        private static void OnMenuItemClick(object sender, RoutedEventArgs e) =>
            ((sender as ContextMenu).PlacementTarget as GuiModeButton).OnMenuItemClick(e);

        private void OnGuiModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UiModeChanges == null)
                return;
            if (e.NewValue == null)
                return;

            var guiMode = (UiMode)e.NewValue;

            Checked += (_, __) =>
            {
                if (ContextMenu != null)
                {
                    ContextMenu.AddHandler(MenuItem.ClickEvent, (RoutedEventHandler)OnMenuItemClick);
                    ContextMenu.Closed += OnContextMenuClosed;
                    ContextMenu.PlacementTarget = this;
                    ContextMenu.IsOpen = true;
                }
                else
                {
                    UiModeChanges.EnterMode(guiMode, SubGuiMode);
                }
            };

            Unchecked += (_, __) =>
            {
                UiModeChanges.ExitMode(guiMode);
            };

            Loaded += (_, __) =>
            {
                UiModeChanges.CanEnterModeChanged += (___, ea) =>
                {
                    if (ea.Mode == guiMode) IsEnabled = ea.CanEnter;
                };


                UiModeChanges.ModeChanged += (___, ea) =>
                {
                    IsChecked = ea.NewMode == guiMode;
                };

                IsEnabled = UiModeChanges.CanEnterMode(guiMode, null);
            };
        }

        private void OnContextMenuClosed(RoutedEventArgs e)
        {
            ContextMenu.RemoveHandler(MenuItem.ClickEvent, (RoutedEventHandler)OnMenuItemClick);
            ContextMenu.Closed -= OnContextMenuClosed;
            if (GuiMode != UiModeChanges.GetMode())
            {
                IsChecked = false;
            }
        }

        private void OnMenuItemClick(RoutedEventArgs e)
        {
            var menuItem = (MenuItem)e.OriginalSource;
            UiModeChanges.EnterMode(GuiMode, menuItem.Header.ToString());
        }
    }
}
