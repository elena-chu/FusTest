using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ws.Extensions.UI.Wpf.Behaviors
{
	public class GridExtentions
	{
		public static DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns",
			typeof(int), typeof(GridExtentions), new PropertyMetadata(ColumnsChangedCallback));

        public static void SetColumns(Grid obj, int value)
        {
            obj.SetValue(ColumnsProperty, value);
        }

        public static int GetColumns(Grid obj)
        {
            return (int)obj.GetValue(ColumnsProperty);
        }

        private static void ColumnsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = (Grid)d;
			int newCount = (int)e.NewValue;

			grid.ColumnDefinitions.Clear();
			for(int i=0;i<newCount;++i)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}
		}

		public static DependencyProperty RowsProperty = DependencyProperty.RegisterAttached("Rows",
			typeof(int), typeof(GridExtentions), new PropertyMetadata(RowsChangedCallback));

        public static void SetRows(Grid obj, int value)
        {
            obj.SetValue(RowsProperty, value);
        }

        public static int GetRows(Grid obj)
        {
            return (int)obj.GetValue(RowsProperty);
        }

        private static void RowsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = (Grid)d;
			int newCount = (int)e.NewValue;

			grid.RowDefinitions.Clear();
			for (int i = 0; i < newCount; ++i)
			{
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
			}
		}

	}
}
