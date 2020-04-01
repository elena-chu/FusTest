using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ws.Extensions.UI.Wpf.Utils
{
    public static class ImageUtils
    {
        public static byte[] SetAlfa(byte[] array, int bytesPerPixel)
        {
            if (array == null || array.Length <= bytesPerPixel)
            {
                return array;
            }

            DateTime startTime = DateTime.Now;
            byte[] targ = new byte[array.Length];
            for (int i = 0; i < array.Length; i += bytesPerPixel)
            {
                targ[i] = array[i];
                targ[i + 1] = array[i + 1];
                targ[i + 2] = array[i + 2];
                if ((array[i] & array[i + 1] & array[i + 2]) == array[i])
                {
                    targ[i + 3] = (byte)(255 - array[i]);
                }
                else
                {
                    targ[i + 3] = array[i + 3];
                }
            }
            DateTime endTime = DateTime.Now;
            //Debug.WriteLine($"Alfa duration={(endTime - startTime).TotalMilliseconds}ms");
            return targ;
        }

        public static bool AreImagesEqual(byte[] array1, byte[] array2)
        {
            return ByteUtils.Compare_point(array1, array2);
        }

        public static byte[] FusionTwoImages(byte[] array1, byte[] array2, Color color1, Color color2, double coeff = 0 )
        {
            if(array1 == null || array2==null || array1.Length != array2.Length)
            {
                return array1;
            }
            return default(byte[]);
        }
    }
}
