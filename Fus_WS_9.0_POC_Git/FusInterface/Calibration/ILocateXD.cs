using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;

namespace Ws.Fus.Interfaces.Calibration
{
    public class XDLocChangedEventArgs : EventArgs
    {
        public PointRAS? XDLoc { get; }
        public XDLocChangedEventArgs(PointRAS? xdLoc)
        {
            XDLoc = xdLoc;
        }
    }
    public interface ILocateXD
    {
        bool CanLocateXD { get; }
        PointRAS? XDLoc { get; }

        event EventHandler CanLocateXDChanged;
        event EventHandler<XDLocChangedEventArgs> XDLocChanged;
        void LocateXD();
    }
}
