using Dicom;
using Dicom.Imaging;
using Dicom.Network;
using Ws.Dicom.Interfaces.Entities;
using Ws.Dicom.Persistency.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    static class SeriesExtensions
    {
        public static DicomCMoveRequest CreateCMoveRequest(this Series series, DicomServerSettings settings) =>
            new DicomCMoveRequest(
                settings.AET,
                series.StudyInstanceUid,
                series.SeriesInstanceUid);
    }
}
