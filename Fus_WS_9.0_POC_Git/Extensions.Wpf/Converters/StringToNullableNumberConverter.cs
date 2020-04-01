using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ws.Extensions.UI.Wpf.Converters
{
    public class StringToNullableNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                if (targetType == typeof(int?))
                {
                    int result;
                    if (int.TryParse(stringValue, out result))
                        return result;

                    return null;
                }

                if (targetType == typeof(decimal?))
                {
                    decimal result;
                    if (decimal.TryParse(stringValue, out result))
                        return result;

                    return null;
                }

                if (targetType == typeof(long?))
                {
                    long result;
                    if (long.TryParse(stringValue, out result))
                        return result;

                    return null;
                }

                if (targetType == typeof(double?))
                {
                    double result;
                    if (double.TryParse(stringValue, out result))
                        return result;

                    return null;
                }

                if (targetType == typeof(float?))
                {
                    float result;
                    if (float.TryParse(stringValue, out result))
                        return result;

                    return null;
                }
            }

            return value;
        }
    }
}
