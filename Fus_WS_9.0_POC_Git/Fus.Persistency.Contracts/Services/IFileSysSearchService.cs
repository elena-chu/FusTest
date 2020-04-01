using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public interface IFileSysSearchService : ISearchService
    {
        string SearchDir { get; }
    }
}
