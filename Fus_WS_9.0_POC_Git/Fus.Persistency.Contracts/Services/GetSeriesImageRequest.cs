using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public class GetSeriesImageRequest
    {
        public event EventHandler<Image> ImageGot;

        public IEnumerable<Series> Series { get; set; } = new Series[0];

        internal virtual void RaiseImageGot(Image image)
        {
            try
            {
                ImageGot?.Invoke(this, image);
            }
            catch
            { /* ignore exception */ }
        }
    }
}
