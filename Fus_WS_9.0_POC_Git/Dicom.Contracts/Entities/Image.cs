using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Interfaces.Entities
{
    public abstract class Image : DicomObj
    {
        public abstract bool BadImage { get; }

        [DicomTag(DicomTags.InstanceNumber)]
        public virtual uint InstanceNumber { get; set; }

        [DicomTag(DicomTags.ModalitiesInStudy)]
        public virtual string ModalitiesInStudy { get; set; }

        [DicomTag(DicomTags.SeriesInstanceUID)]
        public virtual string SeriesInstanceUid { get; set; }

        [DicomTag(DicomTags.SOPInstanceUID)]
        public virtual string SopInstanceUid { get; set; }

        [DicomTag(DicomTags.StudyDate)]
        public virtual DateTime? StudyDate { get; set; }

        [DicomTag(DicomTags.StudyDescription)]
        public virtual string StudyDescription { get; set; }

        [DicomTag(DicomTags.StudyInstanceUID)]
        public virtual string StudyInstanceUid { get; set; }

        public virtual FDCSeriesOrientation Orientation { get; set; } = FDCSeriesOrientation.eFDC_NO_ORIENTATION;

        //[DicomTag(DicomTags.NumberOfStudyRelatedSeries)]
        //public abstract uint? NumberOfStudyRelatedSeries { get; set; }

        //[DicomTag(DicomTags.NumberOfStudyRelatedInstances)]
        //public abstract uint? NumberOfStudyRelatedInstances { get; set; }
        public abstract I ImageAs<I>();
    }
}
