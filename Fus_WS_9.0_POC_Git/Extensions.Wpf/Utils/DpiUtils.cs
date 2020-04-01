using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Ws.Extensions.UI.Wpf.Utils
{
    public static class DpiUtils
    {
        public static Matrix GetToDeviceMatrix(Visual visual)
        {
            Matrix matrix;
            PresentationSource source = null;
            
            if (visual != null)
                source = PresentationSource.FromVisual(visual);

            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            return matrix;
        }

        public static void GetDpi(Visual visual, out double dpiX, out double dpiY)
        {
            var matrix = GetToDeviceMatrix(visual);
            dpiX = 96.0 * matrix.M11;
            dpiY = 96.0 * matrix.M22;
        }

        public static void Convert(Visual visual, Size size, out int width, out int height)
        {
            var matrix = GetToDeviceMatrix(visual);
            width = (int)(matrix.M11 * size.Width);
            height = (int)(matrix.M11 * size.Height);
        }

        public static void ConvertMousePosition(UIElement el, MouseEventArgs e, out int posX, out int posY)
        {
            var matrix = GetToDeviceMatrix(el);
            var devicePoint = matrix.Transform(e.GetPosition(el));
            posX = (int)(devicePoint.X);
            posY = (int)(devicePoint.Y);
        }
    }
}
