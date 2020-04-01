using Ws.Dicom.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Fo.Entities
{
    class FileSysStudyImp : Study
    {
        public LinkedList<string> SeriesDirs { get; set; } = new LinkedList<string>();
    }
}
