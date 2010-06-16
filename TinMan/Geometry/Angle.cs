/*
 * Created by Drew, 06/06/2010 23:28.
 */
using System;

namespace TinMan
{
    /// <summary>
    /// Represents an angle as a double-precision floating point value.
    /// </summary>
    public struct Angle : IEquatable<Angle>
    {
        public static readonly Angle Zero = new Angle(0);
        public static readonly Angle NaN = new Angle(double.NaN);
        
        #region Static factory methods and private constructor

        public static Angle FromRadians(double radians) {
            return new Angle(radians);
        }
        
        public static Angle FromDegrees(double degrees) {
            return new Angle(DegreesToRadians(degrees));
        }
        
        private Angle(double radians) : this() {
            Radians = radians;
        }
        
        #endregion
        
        #region Static utility methods
        
	    public static double DegreesToRadians(double degrees) {
	        const double degToRadFactor = Math.PI/180;
	        return degrees * degToRadFactor;
	    }
	    
	    public static double RadiansToDegrees(double radians) {
	        const double radToDegFactor = 180/Math.PI;
	        return radians * radToDegFactor;
	    }
        
        #endregion

        #region Properties
        
        public double Radians { get; private set; }
        public double Degrees {
            get { return RadiansToDegrees(Radians); }
        }
        
        public double Cos { get { return Math.Cos(Radians); } }
        public double Sin { get { return Math.Sin(Radians); } }
        public double Tan { get { return Math.Tan(Radians); } }
        
        public bool IsNaN { get { return double.IsNaN(Radians); } }
        public Angle Abs { get { return new Angle(Math.Abs(Radians)); } }
        
        #endregion

        public Angle Normalise() {
	        // TODO can this be done this with mod division? check with unit test.
	        var radians = Radians;
	        while (radians < 0)
	            radians += Math.PI*2;
	        while (radians >= Math.PI*2)
	            radians -= Math.PI*2;
	        return Angle.FromRadians(radians);
	    }
        
        public Angle Limit(Angle lowerLimit, Angle upperLimit) {
            if (lowerLimit > upperLimit)
                throw new ArgumentException("The lower limit must be less than the upper limit.");
            if (this < lowerLimit)
                return lowerLimit;
            if (this > upperLimit)
                return upperLimit;
            return this;
        }

        #region Operators, Equality and Hashing
        
        public override bool Equals(object obj) {
            return (obj is Angle) ? Equals((Angle)obj) : false;
        }
        
        public bool Equals(Angle other) {
            return other.Radians == Radians;
        }
        
        public override int GetHashCode() {
            return Radians.GetHashCode();
        }
        
        public static Angle operator +(Angle a, Angle b) {
            return Angle.FromRadians(a.Radians + b.Radians);
        }
        
        public static Angle operator -(Angle a, Angle b) {
            return Angle.FromRadians(a.Radians - b.Radians);
        }
        
        public static Angle operator -(Angle a) {
            return Angle.FromRadians(-a.Radians);
        }
        
        public static Angle operator *(Angle a, double scale) {
            return Angle.FromRadians(a.Radians * scale);
        }

        public static Angle operator /(Angle a, double quotient) {
            return Angle.FromRadians(a.Radians / quotient);
        }

        public static AngularSpeed operator /(Angle a, TimeSpan time) {
            return AngularSpeed.FromRadiansPerSecond(a.Radians / time.TotalSeconds);
        }

        public static bool operator >(Angle left, Angle right) {
            return left.Radians > right.Radians;
        }

        public static bool operator <(Angle left, Angle right) {
            return left.Radians < right.Radians;
        }

        public static bool operator >=(Angle left, Angle right) {
            return left.Radians >= right.Radians;
        }

        public static bool operator <=(Angle left, Angle right) {
            return left.Radians <= right.Radians;
        }

        public static bool operator ==(Angle left, Angle right) {
            return left.Equals(right);
        }
        
        public static bool operator !=(Angle left, Angle right) {
            return !left.Equals(right);
        }
        #endregion
    }
}
