using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Dicom.Persistency.Interfaces.Services
{
    public interface ISearchServiceFactory
    {
        Task GetAvailableServersAsync(IProgress<string> progress);

        bool IsFileSysService(string serverName);

        ISearchService CreateService(string serverName);
        IFileSysSearchService CreateFileSysSearchService(string searchDir);
    }
}
