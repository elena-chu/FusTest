using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
    public interface IACPC
	{
		float? GetACPCLength();
        string GetCurrentAtlasOriginFactorString();

        event EventHandler ACPCLengthChanged;
        event EventHandler ACPCOriginUpdated;
    }
}
