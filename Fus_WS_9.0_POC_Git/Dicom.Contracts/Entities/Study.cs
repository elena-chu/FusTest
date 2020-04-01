using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Interfaces.Entities
{
    public class Study : DicomObj, IEquatable<Study>
    {
        [DicomTag(DicomTags.StudyInstanceUID)]
        [Required]
        public virtual string StudyInstanceUid { get; set; }

        [DicomTag(DicomTags.StudyID)]
        public virtual string StudyId { get; set; }

        [DicomTag(DicomTags.PatientID)]
        public virtual string PatientId { get; set; }

        [DicomTag(DicomTags.PatientName)]
        public virtual string PatientName { get; set; }

        [DicomTag(DicomTags.StudyDate)]
        public virtual DateTime? StudyDate { get; set; }

        [DicomTag(DicomTags.StudyDescription)]
        public virtual string StudyDescription { get; set; }

        [DicomTag(DicomTags.ModalitiesInStudy)]
        public virtual string ModalitiesInStudy { get; set; }

        [DicomTag(DicomTags.NumberOfStudyRelatedSeries)]
        public virtual uint? NumberOfStudyRelatedSeries { get; set; }

        [DicomTag(DicomTags.NumberOfStudyRelatedInstances)]
        public virtual uint? NumberOfStudyRelatedInstances { get; set; }

        public bool Equals(Study other)
        {
            if (other == null)
                return false;

            return StudyInstanceUid == other.StudyInstanceUid;
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

            return Equals((Study)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode() => StudyInstanceUid.GetHashCode();
    }
}
