using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Interfaces.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SeriesValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var series = value as Series;
            if (series == null)
                return ValidationResult.Success;

            if (series.Study != null)
                if (series.StudyInstanceUid != series.Study.StudyInstanceUid)
                    return new ValidationResult($"StudyInstanceUid and Study.StudyInstanceUid are not the same: {series.StudyInstanceUid}:{ series.Study.StudyInstanceUid}");

            return ValidationResult.Success;
        }
    }
}
