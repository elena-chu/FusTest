using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;

namespace Ws.Fus.Interfaces.MovementDetection
{
    public interface IMovementDetection
    {
 
        bool CanStartRefScan { get; }
        bool CanDetect { get; }
        PointRAS? MDVector { get; }
   
        event EventHandler CanStartRefScanChanged;
        event EventHandler CanDetectChanged;
        event EventHandler<MDVectorChangedEventArgs> MDVectorChanged;

        void StartRefScan();
        void Detect();
    }
}
