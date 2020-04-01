using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ws.Extensions.UI.Wpf.Behaviors
{
    public class OpenMenuOnClick
    {
        public static DependencyProperty PrepareMenuCommandProperty = DependencyProperty.RegisterAttached("PrepareMenuCommand",
            typeof(ICommand), typeof(OpenMenuOnClick), new PropertyMetadata(PrepareMenuCommandChangedCallback));

        public static void SetPrepareMenuCommand(FrameworkElement obj, ICommand value)
        {
            obj.SetValue(PrepareMenuCommandProperty, value);
        }

        public static ICommand GetPrepareMenuCommand(FrameworkElement obj)
        {
            return (ICommand)obj.GetValue(PrepareMenuCommandProperty);
        }

        private static void PrepareMenuCommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;
            element.PreviewMouseDown += (sender, ea) =>
            {
                if (e.NewValue != null) ((ICommand)e.NewValue).Execute(null);
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.IsOpen = true;
            };
        }
    }
}
