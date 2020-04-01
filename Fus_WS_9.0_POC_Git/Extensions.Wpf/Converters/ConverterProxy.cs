using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ws.Extensions.UI.Wpf.Converters
{
    public class ConverterProxy : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (typeof(IValueConverter).IsAssignableFrom(values[0].GetType()))
                {
                    var realConverter = (IValueConverter)values[0];
                    return realConverter.Convert(values[1], targetType, parameter, culture);

                }
                else if (typeof(IMultiValueConverter).IsAssignableFrom(values[0].GetType()))
                {
                    var realConverter = (IMultiValueConverter)values[0];
                    return realConverter.Convert(values.Skip(1).ToArray(), targetType, parameter, culture);
                }
            }
            catch (Exception ex)
            {
                /// log
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
