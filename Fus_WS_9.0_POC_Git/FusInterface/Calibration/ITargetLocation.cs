using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;
using Ws.Fus.Interfaces.Overlays;

namespace Ws.Fus.Interfaces.Calibration
{
	// Target Location C# Interface - Public because it’s used in other dlls
	//
	// Supports methods, read-write properties, read-only properties and events
	// by default all are public and virtual
	   
	    public interface ITargetLocation
	    {
		    //read write property 
		    PointRAS? TargetLocationRas { get; set; }

            // The event handler is TargetLocationChangedRasEventHandler (actually delegate)
            // The event is TargetLocationChangedRas
            event TargetLocationChangedRasEventHandler TargetLocationChangedRas;
            // The event handler is TargetAcPcEnabledChangedEventHandler (actually delegate)
            // The event is TargetAcPcEnabledChanged
            event TargetAcPcEnabledChangedEventHandler TargetAcPcEnabledChanged;
	    }
}
