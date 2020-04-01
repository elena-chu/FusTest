using Ws.Dicom.Interfaces.Entities;
using Ws.Fus.Strips.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Strips.Interfaces.Services
{
    public interface IStripsManager
    {
        event EventHandler Changed;
        IEnumerable<IStrip> GetStrips();
        IEnumerable<IStrip> GetPlanningStrips();
        void AddSeries(IEnumerable<Series> newSeries);
    }
}
