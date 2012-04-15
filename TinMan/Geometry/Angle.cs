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
using TinMan.Annotations;

// ReSharper disable MemberCanBeInternal

namespace TinMan
{
    /// <summary>
    /// Represents an immutable angle value as a double-precision floating point value.
    /// </summary>
    /// <remarks>
    /// The backing value holds the angle in radians, as most operations upon this type require radian values.
    /// Conversion to degrees is performed as necessary.  There may be some imprecision if creating <see cref="Angle"/>
    /// instances from degree values, and later converting back to degrees.
    /// </remarks>
    [DebuggerDisplay("{Degrees} deg")]
    public struct Angle : IEquatable<Angle>
    {
        private const double Epsilon = 0.0001;

        /// <summary>A constant angle of zero.</summary>
        public static readonly Angle Zero = new Angle(0);

        public static readonly Angle TwoPi = new Angle(Math.PI*2);
        public static readonly Angle HalfPi = new Angle(Math.PI/2);
        public static readonly Angle Pi = new Angle(Math.PI);

        /// <summary>
        /// Gets an angle whose value in degrees and radians is <see cref="double.NaN"/>.
        /// Returns <see cref="IsNaN"/> as <c>true</c>.
        /// </summary>
        public static readonly Angle NaN = new Angle(double.NaN);

        #region Static factory methods and private constructor

        /// <summary>Creates random angle between <see cref="Zero"/> and <see cref="TwoPi"/>.</summary>
        /// <param name="random"></param>
        /// <returns></returns>
        [Pure]
        public static Angle Random(Random random)
        {
            return new Angle(random.NextDouble()*2*Math.PI);
        }

        /// <summary>Creates an angle from a source value in radians.</summary>
        /// <param name="radians">The angular value, in radians.</param>
        /// <returns></returns>
        [Pure]
        public static Angle FromRadians(double radians)
        {
            return new Angle(radians);
        }

        /// <summary>Creates an angle from a source value in degrees.</summary>
        /// <param name="degrees">The angular value, in degrees.</param>
        /// <returns></returns>
        [Pure]
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(DegreesToRadians(degrees));
        }

        /// <summary>Returns the <see cref="Angle"/> whose sine is the specified number.</summary>
        /// <returns>
        /// An <see cref="Angle"/>, such that -π/2 ≤<see cref="Radians"/>≤π/2 -or- <see cref="Angle.NaN"/> 
        /// if <paramref name="d"/> &lt; -1 or <paramref name="d"/> &gt; 1.
        /// </returns>
        /// <remarks>Uses the <see cref="Math.Asin"/> method.</remarks>
        /// <param name="d">A number representing a sine, where -1 ≤<paramref name="d"/>≤ 1. </param>
        [Pure]
        public static Angle Asin(double d)
        {
            return FromRadians(Math.Asin(d));
        }

        /// <summary>Returns the <see cref="Angle"/> whose cosine is the specified number.</summary>
        /// <returns>
        /// An <see cref="Angle"/>, such that 0 ≤<see cref="Radians"/>≤π -or- <see cref="Angle.NaN"/> if
        /// <paramref name="d"/> &lt; -1 or <paramref name="d"/> &gt; 1.
        /// </returns>
        /// <remarks>Uses the <see cref="Math.Acos"/> method.</remarks>
        /// <param name="d">A number representing a cosine, where -1 ≤<paramref name="d"/>≤ 1. </param>
        [Pure]
        public static Angle Acos(double d)
        {
            return FromRadians(Math.Acos(d));
        }

        /// <summary>Returns the <see cref="Angle"/> whose tangent is the specified number.</summary>
        /// <returns>
        /// An <see cref="Angle"/>, such that -π/2≤<see cref="Radians"/>≤π/2 -or- <see cref="Angle.NaN"/> if 
        /// <paramref name="d"/> equals <see cref="Double.NaN"/>, -<see cref="Angle.HalfPi"/>
        /// if <paramref name="d"/> equals <see cref="Double.NegativeInfinity"/>, or <see cref="Angle.HalfPi"/>
        /// if <paramref name="d"/> equals <see cref="Double.PositiveInfinity"/>.
        /// </returns>
        /// <remarks>Uses the <see cref="Math.Atan"/> method.</remarks>
        /// <param name="d">A number representing a tangent. </param>
        [Pure]
        public static Angle Atan(double d)
        {
            return FromRadians(Math.Atan(d));
        }

        /// <summary>Returns the <see cref="Angle"/> whose tangent is the quotient of two specified numbers.</summary>
        /// <returns>
        /// An <see cref="Angle"/> such that -π≤<see cref="Radians"/>≤π, and <see cref="Tan"/> = <paramref name="y"/> / <paramref name="x"/>, 
        /// where (<paramref name="x"/>, <paramref name="y"/>) is a point in the Cartesian plane. 
        /// </returns>
        /// <remarks>Uses the <see cref="Math.Atan2"/> method.</remarks>
        /// <param name="y">The y coordinate of a point. </param><param name="x">The x coordinate of a point. </param>
        [Pure]
        public static Angle Atan2(double y, double x)
        {
            return FromRadians(Math.Atan2(y, x));
        }

        private Angle(double radians)
            : this()
        {
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
        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI/180;
            return degrees*degToRadFactor;
        }

        /// <summary>
        /// Converts a number of radians into a number of degrees.  Generally speaking,
        /// the use of the <see cref="Angle"/> type obviates the need for these methods.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180/Math.PI;
            return radians*radToDegFactor;
        }

        #endregion

        #region Properties

        /// <summary>Gets the angle as a double value in radians per second.</summary>
        public double Radians { get; private set; }

        /// <summary>Gets the angle as a double value in degrees per second.</summary>
        public double Degrees
        {
            get { return RadiansToDegrees(Radians); }
        }

        /// <summary>Gets the cosine of this angle.</summary>
        public double Cos
        {
            get { return Math.Cos(Radians); }
        }

        /// <summary>Gets the sine of this angle.</summary>
        public double Sin
        {
            get { return Math.Sin(Radians); }
        }

        /// <summary>Gets the tangent of this angle.</summary>
        public double Tan
        {
            get { return Math.Tan(Radians); }
        }

        /// <summary>
        /// Gets a value indicating whether this angle's value is <see cref="double.NaN"/>
        /// in both radians and degrees.
        /// </summary>
        public bool IsNaN
        {
            get { return double.IsNaN(Radians); }
        }

        /// <summary>
        /// Gets the absolute value.  If this angle is negative, it returns the value
        /// multiplied by negative one.
        /// </summary>
        public Angle Abs
        {
            get { return new Angle(Math.Abs(Radians)); }
        }

        #endregion

        /// <summary>
        /// Returns an equivalent angle within the range of [0,360) degrees.
        /// </summary>
        /// <returns></returns>
        public Angle NormalisePositive()
        {
            var radians = Radians;
            while (radians < 0)
                radians += Math.PI*2;
            if (radians >= Math.PI*2)
                radians = radians%(Math.PI*2);
            return FromRadians(radians);
        }

        /// <summary>
        /// Returns an equivalent angle within the range of [-180,180) degrees.
        /// </summary>
        /// <returns></returns>
        public Angle NormaliseBalanced()
        {
            var radians = Radians;
            while (radians < -Math.PI)
                radians += Math.PI*2;
            while (radians >= Math.PI)
                radians -= 2*Math.PI;
            return FromRadians(radians);
        }

        /// <summary>
        /// Returns the angle nearest to this that is within the range from <paramref name="lowerLimit"/>
        /// and <paramref name="upperLimit"/>.  The returned value is clamped within the specified limits.
        /// </summary>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        public Angle Limit(Angle lowerLimit, Angle upperLimit)
        {
            if (lowerLimit > upperLimit)
                throw new ArgumentException("The lower limit must be less than the upper limit.");
            if (this < lowerLimit)
                return lowerLimit;
            if (this > upperLimit)
                return upperLimit;
            return this;
        }

        #region Operators, Equality and Hashing

        public override bool Equals(object obj)
        {
            return obj is Angle && Equals((Angle) obj);
        }

        public bool Equals(Angle other)
        {
            return (double.IsNaN(Radians) && double.IsNaN(other.Radians))
                   || Math.Abs(other.Radians - Radians) < Epsilon;
        }

        public override int GetHashCode()
        {
            return Radians.GetHashCode();
        }

        public static Angle operator +(Angle a, Angle b)
        {
            return FromRadians(a.Radians + b.Radians);
        }

        public static Angle operator -(Angle a, Angle b)
        {
            return FromRadians(a.Radians - b.Radians);
        }

        public static Angle operator -(Angle a)
        {
            return FromRadians(-a.Radians);
        }

        public static Angle operator *(Angle a, double scale)
        {
            return FromRadians(a.Radians*scale);
        }

        public static Angle operator /(Angle a, double quotient)
        {
            return FromRadians(a.Radians/quotient);
        }

        public static AngularSpeed operator /(Angle a, TimeSpan time)
        {
            return AngularSpeed.FromRadiansPerSecond(a.Radians/time.TotalSeconds);
        }

        public static bool operator >(Angle left, Angle right)
        {
            return left.Radians > right.Radians;
        }

        public static bool operator <(Angle left, Angle right)
        {
            return left.Radians < right.Radians;
        }

        public static bool operator >=(Angle left, Angle right)
        {
            return left.Radians >= right.Radians;
        }

        public static bool operator <=(Angle left, Angle right)
        {
            return left.Radians <= right.Radians;
        }

        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0:0.##}°", Degrees);
        }
    }
}