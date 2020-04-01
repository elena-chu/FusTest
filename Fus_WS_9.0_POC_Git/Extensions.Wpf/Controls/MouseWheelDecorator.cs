using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ws.Extensions.UI.Wpf.Controls
{
	public class MouseWheelDecorator : Decorator
	{
		private static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MouseWheelDecorator));
		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		private static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MouseWheelDecorator));
		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		private static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MouseWheelDecorator),new FrameworkPropertyMetadata(0.0,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			Value = Math.Max(Minimum, Math.Min(Maximum, Value + e.Delta));			
			base.OnMouseWheel(e);
		}
	}
}
