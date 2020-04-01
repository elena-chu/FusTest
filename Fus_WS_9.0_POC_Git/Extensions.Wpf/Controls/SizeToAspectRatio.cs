using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ws.Extensions.UI.Wpf.Controls
{
	public class SizeToAspectRatio : Decorator
	{
		public static DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns",
			typeof(int), typeof(SizeToAspectRatio), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static DependencyProperty RowsProperty = DependencyProperty.Register("Rows",
			typeof(int), typeof(SizeToAspectRatio), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public int Columns
		{
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		public int Rows
		{
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}


		protected override Size MeasureOverride(Size constraint)
		{
			return base.MeasureOverride(constraint);
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var calculatedWidth = arrangeSize.Height * Columns / Rows;
			var calculatedHeight = arrangeSize.Width * Rows / Columns;

			if (double.IsNaN(calculatedWidth) || double.IsNaN(calculatedHeight))
			{
				Child.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
				return arrangeSize;
			}

			if(calculatedWidth<=arrangeSize.Width)
			{
				Child.Arrange(new Rect((arrangeSize.Width - calculatedWidth) / 2, 0, calculatedWidth, arrangeSize.Height));
			}
			else
			{
				Child.Arrange(new Rect(0, (arrangeSize.Height- calculatedHeight) / 2, arrangeSize.Width, calculatedHeight));
			}
			return arrangeSize;
		}
	}
}
