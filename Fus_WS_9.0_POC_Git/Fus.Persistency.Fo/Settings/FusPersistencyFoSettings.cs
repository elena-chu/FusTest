using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Settings
{
    class FusPersistencyFoSettings
    {
        public Dictionary<string, DicomSearchServiceSettings> DicomServices { get; set; } = new Dictionary<string, DicomSearchServiceSettings>();

        public FileSysSearchServiceSettings FileSysService = new FileSysSearchServiceSettings();

        public int CStoreScpPort { get; set; } = 104;

        public double StorageHiWaterMarkGb { get; set; } = 0.1;
        public double StorageLoWaterMarkGb { get; set; } = 0.05;
    }
}
