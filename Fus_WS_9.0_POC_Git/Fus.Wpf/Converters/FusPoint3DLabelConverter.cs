using Ws.Fus.UI.Wpf.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ws.Fus.UI.Wpf.Converters
{
    public class FusPoint3DLabelConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var value = (double?)values[0];
                var mode = (FusPoint3DMode)values[1];
                var custom = (string)values[2];
                var coord = (string)parameter;

                switch (mode)
                {
                    case FusPoint3DMode.Xyz:
                        return ConvertXyz(value, coord);
                    case FusPoint3DMode.Uvw:
                        return ConvertUvw(value, coord);
                    case FusPoint3DMode.Ras:
                        return ConvertRas(value, coord);
                    case FusPoint3DMode.AcPc:
                        return ConvertAcPc(value, coord);
                    case FusPoint3DMode.Custom:
                        return custom;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return DependencyProperty.UnsetValue;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object ConvertXyz(double? value, string coord)
        {
            return coord?.ToUpper() ?? DependencyProperty.UnsetValue;
        }

        private static object ConvertUvw(double? value, string coord)
        {
            switch (coord.ToUpper())
            {
                case "X":
                    return "U";
                case "Y":
                    return "V";
                case "Z":
                    return "W";
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        private static object ConvertRas(double? value, string coord)
        {
            switch (coord.ToUpper())
            {
                case "X":
                    return value.GetValueOrDefault() < 0 ? "L" : "R";
                case "Y":
                    return value.GetValueOrDefault() < 0 ? "P" : "A";
                case "Z":
                    return value.GetValueOrDefault() < 0 ? "I" : "S";
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        private static object ConvertAcPc(double? value, string coord)
        {
            switch (coord.ToUpper())
            {
                case "X":
                    return "ML";
                case "Y":
                    return "AP";
                case "Z":
                    return "SI";
                default:
                    return DependencyProperty.UnsetValue;
            }
        }
    }

}
