using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ws.Fus.Interfaces.Coordinates
{
	public struct PointRAS
	{
        private const double RasEpsilon = 0.0001;

        public double AP { get; }

        public double RL { get; }

        public double SI { get; }

        public PointRAS(double rl, double ap, double si)
        {
			RL = rl;
			AP = ap;
			SI = si;
		}

		public PointRAS(PointRAS src)
		{
			RL = src.RL;
			AP = src.AP;
			SI = src.SI;
		}

        public PointRAS(Point3D p3d) : this(p3d.X, p3d.Y, p3d.Z)
        {
        }

        public static implicit operator Point3D(PointRAS ras) => new Point3D(ras.RL, ras.AP, ras.SI);
        public static implicit operator PointRAS(Point3D p3d) => new PointRAS(p3d);

        public static PointRAS operator -(PointRAS p1, PointRAS p2)
        {
            PointRAS newPoint = new PointRAS(p1.RL - p2.RL, p1.AP - p2.AP, p1.SI - p2.SI);
            return newPoint;
        }

        public static bool operator !=(PointRAS p1, PointRAS p2)
        {
            return !(p1 == p2);
        }

        public static PointRAS operator +(PointRAS p1, PointRAS p2)
        {
            PointRAS newPoint = new PointRAS(p1.RL + p2.RL, p1.AP + p2.AP, p1.SI + p2.SI);
            return newPoint;
        }

        public static bool operator ==(PointRAS p1, PointRAS p2)
        {
            return p1.Equals(p2);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((PointRAS)obj);
        }

        public bool Equals(PointRAS other)
        {
            return Equivalent(other, 0.0);
        }

        public bool Equivalent(PointRAS other, double epsilon)
        {
            return
                Math.Abs(RL - other.RL) <= epsilon &&
                Math.Abs(AP - other.AP) <= epsilon &&
                Math.Abs(SI - other.SI) <= epsilon;
        }

        public bool Equivalent(PointRAS other)
        {
            return Equivalent(other, RasEpsilon);
        }
        // if you override Equals you must also override GetHashCode, otherwise if this is
        // used as a key in a hash table things wouldn't work correctly
        public override int GetHashCode()
		{
			return RL.GetHashCode() ^ AP.GetHashCode() ^ SI.GetHashCode();
		}

		public override string ToString()
		{
			return $"RL={RL},AP={AP},SI={SI}";
		}
	}

 
}
