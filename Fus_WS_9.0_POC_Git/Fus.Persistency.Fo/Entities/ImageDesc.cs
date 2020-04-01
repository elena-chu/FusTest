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
    static class ImageDescExtensions
    {
        public static DicomCMoveRequest CreateCMoveRequest(this ImageDesc imageDesc, DicomServerSettings settings) =>
            new DicomCMoveRequest(
                settings.AET,
                imageDesc.StudyInstanceUid,
                imageDesc.SeriesInstanceUid,
                imageDesc.SopInstanceUid);

    }

    class ImageDesc : IEquatable<ImageDesc>
    {
        [DicomTag(DicomTags.InstanceNumber)]
        public uint InstanceNumber { get; set; }

        [DicomTag(DicomTags.SeriesInstanceUID)]
        public string SeriesInstanceUid { get; set; }

        [DicomTag(DicomTags.SOPInstanceUID)]
        public string SopInstanceUid { get; set; }

        [DicomTag(DicomTags.StudyInstanceUID)]
        public string StudyInstanceUid { get; set; }

        public bool Equals(ImageDesc other)
        {
            if (other == null)
                return false;

            return SopInstanceUid == other.SopInstanceUid;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((ImageDesc)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode() => SopInstanceUid.GetHashCode();
    }
}
