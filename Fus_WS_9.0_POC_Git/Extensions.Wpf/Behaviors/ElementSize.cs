using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ws.Extensions.UI.Wpf.Behaviors
{
	public class ElementSize
	{
		public static DependencyProperty ActualSizeEnabledProperty = DependencyProperty.RegisterAttached("ActualSizeEnabled",
			typeof(bool), typeof(ElementSize), new PropertyMetadata(ActualSizeEnabledChangedCallback));

		public static void SetActualSizeEnabled(FrameworkElement obj, bool value)
		{
			obj.SetValue(ActualSizeEnabledProperty, value);
		}

		public static bool GetActualSizeEnabled(FrameworkElement obj)
		{
			return (bool)obj.GetValue(ActualSizeEnabledProperty);
		}

		private static void ActualSizeEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var elem = (FrameworkElement)d;
			elem.SizeChanged += (_, ea) => SetActualSize(elem, (Size)ea.NewSize);
		}

		public static DependencyProperty ActualSizeProperty = DependencyProperty.RegisterAttached("ActualSize",
			typeof(Size), typeof(ElementSize), new FrameworkPropertyMetadata(new Size()));

		public static void SetActualSize(FrameworkElement obj, Size value)
		{
			obj.SetValue(ActualSizeProperty, value);
		}

		public static Size GetActualSize(FrameworkElement obj)
		{
			return (Size)obj.GetValue(ActualSizeProperty);
		}

	}
}
