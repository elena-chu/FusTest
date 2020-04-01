using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Ws.Extensions.UI.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : MarkupExtension, IValueConverter
    {
        //private static readonly TraceSource m_trace = new TraceSource(Assembly.GetExecutingAssembly().GetName().Name);

        private static readonly Brush DefaultUnsetBrush = Brushes.Transparent;
        private static readonly Brush DefaultSetBrush = Brushes.Black;

        private Brush m_unsetBrush = DefaultUnsetBrush;
        private Brush m_setBrush = DefaultSetBrush;

        public string UnsetBrush
        {
            get
            {
                return m_unsetBrush.ToString();
            }
            set
            {
                m_unsetBrush = BrushFromString(value, DefaultUnsetBrush);
            }
        }

        public string SetBrush
        {
            get
            {
                return m_setBrush.ToString();
            }
            set
            {
                m_setBrush = BrushFromString(value, DefaultSetBrush);
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
            {
                return DependencyProperty.UnsetValue;
            }

            bool boolValue = (bool)value;

            return boolValue ? m_setBrush : m_unsetBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static Brush BrushFromString(string name, Brush defaultBrush)
        {
            if (string.IsNullOrEmpty(name))
                return defaultBrush;

            var pi = typeof(Brushes).GetProperty(name);

            if (pi == null)
            {
                //m_trace.TraceEvent(TraceEventType.Warning, 0, "Got unknown brush name: ", name);
                return defaultBrush;
            }

            return (Brush)pi.GetValue(null);
        }
    }
}
