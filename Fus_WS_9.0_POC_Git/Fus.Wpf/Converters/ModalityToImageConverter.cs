using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ws.Fus.UI.Wpf.Converters
{
    public class ModalityToImageConverter : IMultiValueConverter
    {
        private static WriteableBitmap MrScannerImage { get; } = new WriteableBitmap(
            new BitmapImage(new Uri($"pack://application:,,,/{typeof(ModalityToImageConverter).Assembly.GetName().Name};component/Images/MR.jpg")));
        private static WriteableBitmap CtScannerImage { get; } = new WriteableBitmap(
            new BitmapImage(new Uri($"pack://application:,,,/{typeof(ModalityToImageConverter).Assembly.GetName().Name};component/Images/CT.jpg")));
        private static WriteableBitmap BadImage { get; } = new WriteableBitmap(
           new BitmapImage(new Uri($"pack://application:,,,/{typeof(ModalityToImageConverter).Assembly.GetName().Name};component/Images/bad_image.png")));

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var image = values[0];
            if (image != null)
            {
                var buffer = image as byte[];
                if (buffer != null)
                    return CreateBitmap(buffer);

                return image;
            }

            var isBadImage = values[1];
            if (isBadImage is bool)
                if ((bool)isBadImage)
                    return BadImage;

            var modality = values[2] as string;
            if (string.IsNullOrWhiteSpace(modality))
                return null;

            switch (modality.ToUpper())
            {
                case "MR":
                    return MrScannerImage;
                case "CT":
                    return CtScannerImage;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static WriteableBitmap CreateBitmap(byte[] buffer)
        {
            try
            {
                var rgb24buf = new byte[buffer.Length * 3];
                for (int i = 0; i < buffer.Length; i++)
                {
                    var j = i * 3;
                    rgb24buf[j] = buffer[i];
                    rgb24buf[j + 1] = rgb24buf[j + 2] = 0;
                }

                //var pf = PixelFormats.Gray8;
                var pf = PixelFormats.Rgb24;
                var stride = (512 * pf.BitsPerPixel + 7) / 8;
                
                var bitmap = BitmapSource.Create(512, 512, 96, 96, pf, null, rgb24buf, stride);
                
                var wbitmap = new WriteableBitmap(bitmap);

                return wbitmap;
            }
            catch (Exception ex)
            {
                return BadImage;
            }
        }
    }
}
