using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Settings
{
    class DicomSeriesImageServiceSettings
    {
        public int MaxParallelImageQueries { get; set; } = 1;
        public int MaxParallelMoveRequests { get; set; } = 5;
        public int SeriesImageTakeImages { get; set; } = 3;
        public int GetImageBulkSize { get; set; } = 10;
        public int GetImageBulkTriggerIntervalMs { get; set; } = 5000;
        public int GetImageTimeoutMs { get; set; } = 5000;
    }
}
