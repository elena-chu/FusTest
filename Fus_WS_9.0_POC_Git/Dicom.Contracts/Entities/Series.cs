using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Interfaces.Entities
{
    [SeriesValidation]
    public class Series : DicomObj, IEquatable<Series>
    {
        public Series()
        {
        }

        public Series(Study study)
        {
            Study = study;
        }

        [DicomTag(DicomTags.SeriesInstanceUID)]
        [Required]
        public virtual string SeriesInstanceUid { get; set; }

        [DicomTag(DicomTags.StudyInstanceUID)]
        [Required]
        public virtual string StudyInstanceUid { get; set; }

        [DicomTag(DicomTags.Modality)]
        [Required]
        public virtual string Modality { get; set; }

        [DicomTag(DicomTags.SeriesNumber)]
        public virtual int? SeriesNumber { get; set; }

        [DicomTag(DicomTags.SeriesDate)]
        public virtual DateTime? SeriesDate { get; set; }

        [DicomTag(DicomTags.SeriesDescription)]
        public virtual string SeriesDescription { get; set; }

        //[DicomTag(DicomTags.SeriesDescriptionCodeSequence)]
        public virtual string SeriesCode { get; set; }

        [DicomTag(DicomTags.SeriesType)]
        public virtual string SeriesType { get; set; }        

        [DicomTag(DicomTags.NumberOfSeriesRelatedInstances)]
        public virtual int NumberOfSeriesRelatedInstances { get; set; }

        public virtual Study Study { get; set; }

        public virtual FDCSeriesOrientation Orientation { get; set; } = FDCSeriesOrientation.eFDC_NO_ORIENTATION;

        public virtual string ImagesUri { get; set; }

        public bool Equals(Series other)
        {
            if (other == null)
                return false;

            return SeriesInstanceUid == other.SeriesInstanceUid;
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

            return Equals((Series)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode() => SeriesInstanceUid?.GetHashCode() ?? string.Empty.GetHashCode();
    }
}
