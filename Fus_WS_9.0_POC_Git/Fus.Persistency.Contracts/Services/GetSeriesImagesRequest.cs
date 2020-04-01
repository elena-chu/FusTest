using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public class GetSeriesImagesRequest
    {
        public event EventHandler<int> Progress;
        public event EventHandler<Series> SeriesDone;

        public IEnumerable<Series> Series { get; set; } = new Series[0];

        internal virtual void RaiseProgress(int progress)
        {
            try
            {
                Progress?.Invoke(this, progress);
            }
            catch
            { /* ignore exception */ }
        }

        internal virtual void RaiseSeriesDone(Series series)
        {
            try
            {
                SeriesDone?.Invoke(this, series);
            }
            catch
            { /* ignore exception */ }
        }
    }
}
