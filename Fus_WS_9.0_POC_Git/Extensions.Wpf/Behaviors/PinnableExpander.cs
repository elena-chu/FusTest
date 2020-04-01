using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Ws.Extensions.UI.Wpf.Utils;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Ws.Extensions.UI.Wpf.Behaviors
{
    /// <summary>
    /// This behavior supports pinnable expander and goes with conjunction with specific style "PinableExpanderStyle"
    /// using specific UI elements defined in control's template
    /// </summary>
    public class PinnableExpander
    {
        public static DependencyProperty IsPinnableProperty = DependencyProperty.RegisterAttached("IsPinnable",
            typeof(bool), typeof(PinnableExpander), new PropertyMetadata(IsPinnableChangedCallback));

        public static void SetIsPinnable(Expander obj, bool value)
        {
            obj.SetValue(IsPinnableProperty, value);
        }

        public static bool GetIsPinnable(Expander obj)
        {
            return (bool)obj.GetValue(IsPinnableProperty);
        }

        private static void IsPinnableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue || !(d is Expander))
            {
                return;
            }

            var expander = (Expander)d;

            RoutedEventHandler onChecked = (_o, __o) =>
            {
                expander.IsExpanded = true;
                expander.SetValue(PinnableExpander.IsPinnedProperty, true);
            };

            RoutedEventHandler onUnchecked  = (_o, __o) =>
            {
                expander.IsExpanded = false;
                expander.SetValue(PinnableExpander.IsPinnedProperty, false);
            };

            MouseEventHandler onMouseEnter = (_o, __o) =>
            {
                if (!expander.IsExpanded)
                {
                    expander.IsExpanded = true;
                }
            };

            //MouseEventHandler onMouseLeave = (_o, __o) =>
            //{
            //    if (!GetIsPinned(expander) && expander.IsExpanded)
            //    {
            //        expander.IsExpanded = false;
            //    }
            //};

            ToggleButton pinButton = null;
            Rectangle headerBkg = null;
            expander.Loaded += (_, __) =>
            {
                pinButton = expander.ChildrenOfType<ToggleButton>().FirstOrDefault(el => el.Name == "PinButton_PART");
                if(pinButton !=  null)
                {
                    pinButton.Checked += onChecked;
                    pinButton.Unchecked += onUnchecked;
                }

                headerBkg = expander.ChildrenOfType<Rectangle>().FirstOrDefault(el => el.Name == "HeaderBkg_PART");
                if(headerBkg != null)
                {
                    headerBkg.MouseEnter += onMouseEnter;
                }

                //Support for closing expander on Mouse leave
                //expander.MouseLeave += onMouseLeave;
            };

            expander.Unloaded += (_, __) =>
            {
                if (pinButton != null)
                {
                    pinButton.Checked -= onChecked;
                    pinButton.Unchecked -= onUnchecked;
                }

                if (headerBkg != null)
                {
                    headerBkg.MouseEnter -= onMouseEnter;
                }

                //Support for closing expander on Mouse leave
                //expander.MouseLeave -= onMouseLeave;
            };
        }




        public static DependencyProperty IsPinnedProperty = DependencyProperty.RegisterAttached("IsPinned",
            typeof(bool), typeof(PinnableExpander), new PropertyMetadata(IsPinnedChangedCallback));

        public static void SetIsPinned(Expander obj, bool value)
        {
            obj.SetValue(IsPinnedProperty, value);
        }

        public static bool GetIsPinned(Expander obj)
        {
            return (bool)obj.GetValue(IsPinnedProperty);
        }

        private static void IsPinnedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

    }
}
