using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Fus.Interfaces.Coordinates;

namespace Ws.Fus.Interfaces.Calibration
{
	// TargetLocationChangedRasEventArgs class - 
	// Data passed in this class That inherits from EventArgs

	public class TargetLocationChangedRasEventArgs  : EventArgs
	{
		//Prefer making all data read only and set in the constructor
		public TargetLocationChangedRasEventArgs(PointRAS? targetLocationRas)
		{
			TargetLocationRas = targetLocationRas;
		}
		public readonly PointRAS? TargetLocationRas;
	}

	//The Event handler is delegate that returns void and takes “object sender” and the event args object
	public delegate void TargetLocationChangedRasEventHandler(object sender, TargetLocationChangedRasEventArgs ea);
}
