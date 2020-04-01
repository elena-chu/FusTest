using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Settings
{
    public class DicomServerSettings
    {
        public const string DefaultQRServerType = "Default";

        public string QRServerType { get; set; } = DefaultQRServerType;
        public string QRServerHost { get; set; }
        public int QRServerPort { get; set; } = 104;
        public string QRServerAET { get; set; } = "CLEARC";
        public string AET { get; set; } = "andreyb";

        public int EchoTimeoutMs { get; set; } = 5000;
    }
}
