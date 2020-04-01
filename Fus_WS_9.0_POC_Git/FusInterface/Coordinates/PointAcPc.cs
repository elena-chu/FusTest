using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Coordinates
{
	public struct PointAcPc
	{
		public PointAcPc(double ml, double ap, double si)
		{
			ML = ml;
			AP = ap;
			SI = si;
		}

		public PointAcPc(PointAcPc src)
		{
			ML = src.ML;
			AP = src.AP;
			SI = src.SI;
		}

		private const double RasEpsilon = 0.001;

		public override bool Equals(object obj)
		{
			var otherValue = (PointAcPc)obj;
			return 
				Math.Abs(ML - otherValue.ML) < RasEpsilon &&
				Math.Abs(AP - otherValue.AP) < RasEpsilon &&
				Math.Abs(SI - otherValue.SI) < RasEpsilon;
		}

		// if you override Equals you must also override GetHashCode, otherwise if this is
		// used as a key in a hash table things wouldn't work correctly
		public override int GetHashCode()
		{
			return ML.GetHashCode() ^ AP.GetHashCode() ^ SI.GetHashCode();
		}

		public override string ToString()
		{
			return $"ML={ML},AP={AP},SI={SI}";
		}

		public readonly double ML;
		public readonly double AP;
		public readonly double SI;
	}
}
