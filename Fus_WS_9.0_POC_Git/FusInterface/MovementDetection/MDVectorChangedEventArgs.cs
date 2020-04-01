using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;

namespace Ws.Fus.Interfaces.MovementDetection
{
    public class MDVectorChangedEventArgs : EventArgs
    {
        public PointRAS? MDVector { get; }
        public MDVectorChangedEventArgs(PointRAS? movement)
        {
            MDVector = movement;
        }
    }
}
