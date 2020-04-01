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
    public abstract class ParametrizedConverter<T> : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register(
            nameof(Parameter),
            typeof(T),
            typeof(ParametrizedConverter<T>),
            new PropertyMetadata(default(T)));

        public T Parameter
        {
            get { return (T)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
