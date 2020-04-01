using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ws.Extensions.UI.Wpf.Converters
{
    public class GridLengthMultiConverter : IMultiValueConverter
    {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
            double par = double.Parse(values[1].ToString());
            double val = 0;
            double.TryParse(values[0]?.ToString(), out val);
            var res = new GridLength(val * par, GridUnitType.Pixel);
            //Debug.WriteLine(res);
            return res;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }
}
