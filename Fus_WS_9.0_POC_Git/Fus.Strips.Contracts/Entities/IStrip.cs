using Ws.Dicom.Interfaces.Entities;
using Ws.Extensions.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Ws.Fus.Strips.Interfaces.Entities
{
    public interface IStrip : IWrapper<Series>
    {
        StripType StripType { get; }
        string StripName { get; }
        long ImageId { get; }
        BitmapSource Image { get; }


        Series Series { get; }
    }
}
