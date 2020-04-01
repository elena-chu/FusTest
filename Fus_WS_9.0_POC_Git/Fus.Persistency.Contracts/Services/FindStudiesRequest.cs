using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public class FindStudiesRequest
    {

        public event EventHandler<Study> StudyFound;

        public string Description { get; set; }

        public string Family { get; set; }

        public string FirstName { get; set; }

        public DateTime? From { get; set; }

        public string PatientId { get; set; }

        public DateTime? To { get; set; }

        internal virtual void RaiseStudyFound(Study study)
        {
            try
            {
                StudyFound?.Invoke(this, study);
            }
            catch
            { /* ignore exception */ }
        }
    }
}
