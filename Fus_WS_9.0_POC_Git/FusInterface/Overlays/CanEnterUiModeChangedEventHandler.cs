using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public class CanEnterUiModeChangedEventArgs : EventArgs
	{

        public CanEnterUiModeChangedEventArgs(UiMode mode, bool canEnter)
        {
            Mode = mode;
            CanEnter = canEnter;
        }

        public CanEnterUiModeChangedEventArgs(UiMode mode, bool canEnter, string subUiMode)
            : this(mode, canEnter)
		{
            SubUiMode = subUiMode;
        }

        public readonly UiMode Mode;
		public readonly bool CanEnter;
        public readonly string SubUiMode = null;

	}

	public delegate void CanEnterUiModeChangedEventHandler(object sender, CanEnterUiModeChangedEventArgs args);
}
