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
using System.Diagnostics;

namespace TinMan
{
    /// <summary>
    /// Represents an angle as a double-precision floating point value.
    /// This type is immutable.
    /// </summary>
    [DebuggerDisplay("{Degrees} deg")]
    public struct Angle : IEquatable<Angle> {
        /// <summary>A constant angle of zero.</summary>
        public static readonly Angle Zero = new Angle(0);
        /// <summary>
        /// Gets an angle whose value in degrees and radians is <see cref="double.NaN"/>.
        /// Returns <see cref="IsNaN"/> as <c>true</c>.
        /// </summary>
        public static readonly Angle NaN = new Angle(double.NaN);
        
        #region Static factory methods and private constructor

        /// <summary>Creates an angle from a source value in radians.</summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Angle FromRadians(double radians) {
            return new Angle(radians);
        }
        
        /// <summary>Creates an angle from a source value in degrees.</summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Angle FromDegrees(double degrees) {
            return new Angle(DegreesToRadians(degrees));
        }
        
        private Angle(double radians) : this() {
            Radians = radians;
        }
        
        #endregion
        
        #region Static utility methods
        
        /// <summary>
        /// Converts a number of degrees into a number of radians.  Generally speaking,
        /// the use of the <see cref="Angle"/> type obviates the need for these methods.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees) {
            const double degToRadFactor = Math.PI/180;
            return degrees * degToRadFactor;
        }
        
        /// <summary>
        /// Converts a number of radians into a number of degrees.  Generally speaking,
        /// the use of the <see cref="Angle"/> type obviates the need for these methods.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians) {
            const double radToDegFactor = 180/Math.PI;
            return radians * radToDegFactor;
        }
        
        #endregion

        #region Properties
        
        /// <summary>Gets the angle as a double value in radians per second.</summary>
        public double Radians { get; private set; }
        /// <summary>Gets the angle as a double value in degrees per second.</summary>
        public double Degrees {
            get { return RadiansToDegrees(Radians); }
        }
        
        /// <summary>Gets the cosine of this angle.</summary>
        public double Cos { get { return Math.Cos(Radians); } }
        /// <summary>Gets the sine of this angle.</summary>
        public double Sin { get { return Math.Sin(Radians); } }
        /// <summary>Gets the tangent of this angle.</summary>
        public double Tan { get { return Math.Tan(Radians); } }
        
        /// <summary>
        /// Gets a value indicating whether this angle's value is <see cref="double.NaN"/>
        /// in both radians and degrees.
        /// </summary>
        public bool IsNaN { get { return double.IsNaN(Radians); } }
        /// <summary>
        /// Gets the absolute value.  If this angle is negative, it returns the value
        /// multiplied by negative one.
        /// </summary>
        public Angle Abs { get { return new Angle(Math.Abs(Radians)); } }
        
        #endregion

        /// <summary>
        /// Returns an equivalent angle within the range of [0,360) degrees.
        /// </summary>
        /// <returns></returns>
        public Angle Normalise() {
            var radians = Radians;
            while (radians < 0)
                radians += Math.PI*2;
            if (radians >= Math.PI*2)
                radians = radians % (Math.PI*2);
            return FromRadians(radians);
        }
        
        /// <summary>
        /// Returns the angle nearest to this that is within the range from <paramref name="lowerLimit"/>
        /// and <paramref name="upperLimit"/>.  The returned value is clamped within the specified limits.
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
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
            return FromRadians(a.Radians + b.Radians);
        }
        
        public static Angle operator -(Angle a, Angle b) {
            return FromRadians(a.Radians - b.Radians);
        }
        
        public static Angle operator -(Angle a) {
            return FromRadians(-a.Radians);
        }
        
        public static Angle operator *(Angle a, double scale) {
            return FromRadians(a.Radians * scale);
        }

        public static Angle operator /(Angle a, double quotient) {
            return FromRadians(a.Radians / quotient);
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
