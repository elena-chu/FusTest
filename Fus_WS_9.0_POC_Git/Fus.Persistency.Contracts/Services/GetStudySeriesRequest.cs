using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public class GetStudySeriesRequest
    {
        public event EventHandler<Series> SeriesGot;

        public Study Study { get; set; }

        internal virtual void RaiseSeriesGot(Series series)
        {
            try
            {
                SeriesGot?.Invoke(this, series);
            }
            catch
            { /* ignore exception */ }
        }
    }
}
