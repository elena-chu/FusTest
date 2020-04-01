using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Coordinates
{
	public interface ITransformation
	{
		PointAcPc? TransformRASToACPC(PointRAS locRas);
		PointRAS? TransformACPCToRAS(PointAcPc locAcPc);
	}
}
