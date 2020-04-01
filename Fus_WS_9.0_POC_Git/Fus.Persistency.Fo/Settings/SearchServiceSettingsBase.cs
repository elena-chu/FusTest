using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Settings
{
    class SearchServiceSettingsBase
    {
        public int MaxFindStudiesRequests { get; set; } = 1;
        public int MaxGetStudySeriesRequests { get; set; } = 10;
        public int MaxGetSeriesImageRequests { get; set; } = 2;
    }
}
