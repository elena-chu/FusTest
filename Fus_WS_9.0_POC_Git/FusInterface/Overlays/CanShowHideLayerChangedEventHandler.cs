using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public class CanShowHideLayerChangedEventArgs : EventArgs
	{
		public CanShowHideLayerChangedEventArgs(UiMode layer, bool canShowHide)
		{
			Layer = layer;
			CanShowHide = canShowHide;
		}
		public readonly UiMode Layer;
		public readonly bool CanShowHide;
	}

	public delegate void CanShowHideLayerChangedEventHandler(object sender, CanShowHideLayerChangedEventArgs args);
}
