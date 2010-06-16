/*
 * Created by Drew, 07/05/2010 02:50.
 */
using System;

namespace Drew.RoboCup
{
	public struct Vector3
	{
	    public double X { get; private set; }
	    public double Y { get; private set; }
	    public double Z { get; private set; }
		
	    public Vector3(double x, double y, double z) : this()
		{
			X = x;
			Y = y;
			Z = z;
		}
	    
	    public static Vector3 operator -(Vector3 a, Vector3 b) {
            return new Vector3(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
	    }

	    public static Vector3 operator +(Vector3 a, Vector3 b) {
            return new Vector3(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z);
	    }

	    public Vector3 Normalize()
	    {
            // norm2 = sqrt(xax[0] * xax[0] + xax[1] * xax[1] + xax[2] * xax[2]);
            // for(i=0; i<3; i++) xax[i] /= norm2;
            var length = GetLength();
            
            // avoid DivideByZeroException
            if (length==0)
                return new Vector3();
            
            return new Vector3(X/length, Y/length, Z/length);
	    }
	    
	    public double GetLength()
	    {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
	    }
	
		public override string ToString()
		{
		    return string.Format("<{0:0.00},{1:0.00},{2:0.00}>", X, Y, Z);
		}
	}
	
	public struct RadialPosition
	{
	    public static readonly RadialPosition Zero = new RadialPosition();
	    
	    /// <summary>The distance between the player and the object.</summary>
		public double Distance { get; private set; }
		/// <summary>The angle in the horizontal plane. Zero degree always points to the opponent goal.</summary>
		public double Angle1 { get; private set; }
		/// <summary>The latitudal angle. Here zero degree means horizontal.  A negative angle implies the ray is angled downwards from its origin.</summary>
		public double Angle2 { get; private set; }
		
		public RadialPosition(double distance, double angle1, double angle2) : this()
		{
		    Distance = distance;
		    Angle1 = angle1;
		    Angle2 = angle2;
		}
	    
		public Vector3 RadialToVector()
        {
            // f1l[0] = F1L[0] * cos(F1L[2] * M_PI/180.0) * cos(F1L[1] * M_PI/180.0);
            // f1l[1] = F1L[0] * cos(F1L[2] * M_PI/180.0) * sin(F1L[1] * M_PI/180.0);
            // f1l[2] = F1L[0] * sin(F1L[2] * M_PI/180.0);

            const double D2R = Math.PI / 180.0;
            
            double x = Distance * Math.Cos(Angle2 * D2R) * Math.Cos(Angle1 * D2R);
            double y = Distance * Math.Cos(Angle2 * D2R) * Math.Sin(Angle1 * D2R);
            double z = Distance * Math.Sin(Angle2 * D2R);

            return new Vector3(x, y, z);
        }
		
		public bool IsEmpty {
		    get { return Distance==0 && Angle1==0 && Angle2==0; }
		}

		public override string ToString()
		{
		    return string.Format("<{0:0.00}m @ {1:0.00}°,{2:0.00}°>", Distance, Angle1, Angle2);
		}
	}
}
