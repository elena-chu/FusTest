using Dicom;
using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    class FileSysSeriesImp : Series
    {
        public string SeriesDir { get; }

        public FileSysSeriesImp(string seriesDir)
        {
            SeriesDir = seriesDir;
        }
    }
}
