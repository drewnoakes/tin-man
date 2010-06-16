/*
 * Created by Drew, 07/05/2010 02:50.
 */
using System;

namespace Drew.RoboCup
{
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
		    if (distance < 0)
		        throw new ArgumentOutOfRangeException("distance", distance, "Must be zero or greater.");
		    
		    Distance = distance;
		    Angle1 = angle1;
		    Angle2 = angle2;
		}
	    
		public Vector3 RadialToVector()
	        {
	            // f1l[0] = F1L[0] * cos(F1L[2] * M_PI/180.0) * cos(F1L[1] * M_PI/180.0);
	            // f1l[1] = F1L[0] * cos(F1L[2] * M_PI/180.0) * sin(F1L[1] * M_PI/180.0);
	            // f1l[2] = F1L[0] * sin(F1L[2] * M_PI/180.0);
	
	            /*
	             * Note that Apollo3D does this differently:
	             *   float theta = gDegToRad(gNormalizeDeg(vs.theta + 90.0f)); // WHY DO THEY ADD 90 DEGREES HERE?
	             *   float phi   = gDegToRad(gNormalizeDeg(vs.phi));
	             *   vs.localPos[0] = distance * gCos(phi) * gCos(theta);
	             *   vs.localPos[1] = distance * gCos(phi) * gSin(theta);
	             *   vs.localPos[2] = distance * gSin(phi);
	             */
	            
	            double angle1Radians = GeometryUtil.DegreesToRadians(Angle1);
	            double angle2Radians = GeometryUtil.DegreesToRadians(Angle2);
	            
	            double x = Distance * Math.Cos(angle2Radians) * Math.Cos(angle1Radians);
	            double y = Distance * Math.Cos(angle2Radians) * Math.Sin(angle1Radians);
	            double z = Distance * Math.Sin(angle2Radians);
	
	            return new Vector3(x, y, z);
	        }
		
		public bool IsEmpty {
		    get { return Distance==0 && Angle1==0 && Angle2==0; }
		}
	
		public override string ToString() {
		    return string.Format("<{0:0.00}m @ {1:0.00}°,{2:0.00}°>", Distance, Angle1, Angle2);
		}
	}
}
