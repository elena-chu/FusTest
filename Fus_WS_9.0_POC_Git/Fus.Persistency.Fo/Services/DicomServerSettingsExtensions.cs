using Dicom.Network.Client;
using Ws.Dicom.Persistency.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Services
{
    static class DicomServerSettingsExtensions
    {
        public static IDicomClient CreateClient(this DicomServerSettings settings)
        {
            var client = new DicomClient(
                settings.QRServerHost, settings.QRServerPort, false, settings.AET, settings.QRServerAET);
            client.NegotiateAsyncOps();
            return client;
        }
    }
}
