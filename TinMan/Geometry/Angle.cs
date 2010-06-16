#region License
/* 
 * This file is part of TinMan.
 *
 * TinMan is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TinMan is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with TinMan.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 06/06/2010 23:28.

using System;

namespace TinMan
{
    /// <summary>
    /// Represents an angle as a double-precision floating point value.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Degrees} deg")]
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
            var radians = Radians;
            while (radians < 0)
                radians += Math.PI*2;
            if (radians >= Math.PI*2)
                radians = radians % (Math.PI*2);
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
        
        public override string ToString() {
            return string.Format("{0:0.##} degrees", Degrees);
        }
    }
}
