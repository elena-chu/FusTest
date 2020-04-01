using Dicom.Imaging;
using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    class ImageImp : Image, IEquatable<ImageImp>
    {
        private readonly DicomImage _dicomImage;

        public ImageImp() : this(null)
        {
        }

        public ImageImp(DicomImage dicomImage)
        {
            if (dicomImage == null)
                BadImage = true;
            else
                _dicomImage = dicomImage;
        }

        public override bool BadImage { get; }

        public bool Equals(ImageImp other)
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

            return Equals((ImageImp)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode() => SopInstanceUid.GetHashCode();

        public override I ImageAs<I>()
        {
            return _dicomImage == null ? default(I) : _dicomImage.RenderImage().As<I>();
        }
    }
}
