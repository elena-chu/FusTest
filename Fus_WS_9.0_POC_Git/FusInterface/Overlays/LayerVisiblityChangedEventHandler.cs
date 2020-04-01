using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public class LayerVisiblityChangedEventArgs : EventArgs
	{
		public LayerVisiblityChangedEventArgs(UiMode layer/*, bool isVisible*/)
		{
			Layer = layer;
		}
		public readonly UiMode Layer;
	}

	public delegate void LayerVisiblityChangedEventHandler(object sender, LayerVisiblityChangedEventArgs args);
}
