using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Settings
{
    class FileSysSearchServiceSettings
    {
        public SearchServiceSettingsBase BaseSettings { get; set; } = new SearchServiceSettingsBase();

        public int DirScanTimeoutSec { get; set; } = 60;
    }
}
