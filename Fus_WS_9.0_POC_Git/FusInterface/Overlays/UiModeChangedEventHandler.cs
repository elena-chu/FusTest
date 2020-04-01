using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public class UiModeChangedEventArgs : EventArgs
	{
		public UiModeChangedEventArgs(UiMode newMode)
		{
			NewMode = newMode;
		}
		public readonly UiMode NewMode;
	}

	public delegate void UiModeChangedEventHandler(object sender, UiModeChangedEventArgs args);
}
