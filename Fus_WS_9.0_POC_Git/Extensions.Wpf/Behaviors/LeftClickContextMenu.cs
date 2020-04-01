using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Ws.Extensions.UI.Wpf.Behaviors
{
    public class LeftClickContextMenu
    {
        /// <summary>
        /// Indicates whether to open Contex menu on Left Button click. 
        /// </summary>
        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty.RegisterAttached(
        "IsLeftClickEnabled",
        typeof(bool),
        typeof(LeftClickContextMenu),
        new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLeftClickEnabledProperty);
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null)
            {
                return;
            }

            bool IsEnabled = e.NewValue is bool && (bool)e.NewValue;

            if (IsEnabled)
            {
                if (uiElement is ButtonBase)
                {
                    if (GetUsePreviewEvent(uiElement))
                        ((ButtonBase)uiElement).PreviewMouseLeftButtonDown += OnMouseLeftButtonUp;
                    else
                        ((ButtonBase)uiElement).Click += OnMouseLeftButtonUp;
                }
                else
                {
                    uiElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
            }
            else
            {
                if (uiElement is ButtonBase)
                {
                    ((ButtonBase)uiElement).PreviewMouseLeftButtonDown -= OnMouseLeftButtonUp;
                    ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
                }
                else
                {
                    uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            //Debug.Print("OnMouseLeftButtonUp");
            var fe = sender as FrameworkElement;
            if (fe != null)
            {
                if (GetOpeningFlag(fe) == false)
                {
                    return;
                }

                // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
                // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control, although I'm not sure)
                // so we have to set up ContextMenu.DataContext manually here
                if (fe.ContextMenu.DataContext == null)
                {
                    fe.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
                }

                fe.ContextMenu.PlacementTarget = fe;
                fe.ContextMenu.IsOpen = true;

                e.Handled = false;
            }
        }

        /// <summary>
        /// OpeningFlag indicates condition for opening Context menu even Click was executed
        /// </summary>
        public static readonly DependencyProperty OpeningFlagProperty = DependencyProperty.RegisterAttached(
        "OpeningFlag",
        typeof(bool?),
        typeof(LeftClickContextMenu),
        new UIPropertyMetadata(null, null));

        public static bool? GetOpeningFlag(DependencyObject obj)
        {
            return (bool?)obj.GetValue(OpeningFlagProperty);
        }

        public static void SetOpeningFlag(DependencyObject obj, bool? value)
        {
            obj.SetValue(OpeningFlagProperty, value);
        }

        public static readonly DependencyProperty UsePreviewEventProperty = DependencyProperty.RegisterAttached(
        "UsePreviewEvent",
        typeof(bool),
        typeof(LeftClickContextMenu),
        new UIPropertyMetadata(false, OnUsePreviewEventPropertyChanged));

        public static bool GetUsePreviewEvent(DependencyObject obj)
        {
            return (bool)obj.GetValue(UsePreviewEventProperty);
        }

        public static void SetUsePreviewEvent(DependencyObject obj, bool value)
        {
            obj.SetValue(UsePreviewEventProperty, value);
        }

        private static void OnUsePreviewEventPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null)
            {
                return;
            }

            if (uiElement is ButtonBase)
            {
                ((ButtonBase)uiElement).PreviewMouseLeftButtonDown -= OnMouseLeftButtonUp;
                ((ButtonBase)uiElement).Click -= OnMouseLeftButtonUp;
            }
            else
            {
                uiElement.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            }

            OnIsLeftClickEnabledChanged(sender, e);
        }

    }
}
