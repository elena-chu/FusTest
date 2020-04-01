using System;

namespace Ws.Fus.Interfaces.Overlays
{
	public class TargetAcPcEnabledChangedEventArgs : EventArgs
	{
		public TargetAcPcEnabledChangedEventArgs(bool isEnabled)
		{
			IsEnabled = isEnabled;
		}

		public readonly bool IsEnabled;
	}
    //The Event handler is delegate that returns void and takes “object sender” and the event args object
    public delegate void TargetAcPcEnabledChangedEventHandler(object sender, TargetAcPcEnabledChangedEventArgs ea);
}