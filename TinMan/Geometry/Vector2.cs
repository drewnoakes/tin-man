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
// Created 15/04/2012 18:40

using System;
using TinMan.Annotations;

namespace TinMan
{
    /// <summary>
    /// Represents a 2-dimensional vector using double-precision values for both dimensions.
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        private const double Epsilon = 0.0001;

        /// <summary>A constant <see cref="Vector2"/> of zero, equivalent to the origin or cartesian coordinates.</summary>
        public static readonly Vector2 Origin = new Vector2(0, 0);

        /// <summary>A constant <see cref="Vector2"/> with a NaN value in all dimensions.</summary>
        public static readonly Vector2 NaN = new Vector2(double.NaN, double.NaN);

        /// <summary>A constant <see cref="Vector2"/> with unit length that points along the <see cref="X"/> axis.</summary>
        public static readonly Vector2 UnitX = new Vector2(1, 0);

        /// <summary>A constant <see cref="Vector2"/> with unit length that points along the <see cref="Y"/> axis.</summary>
        public static readonly Vector2 UnitY = new Vector2(0, 1);

        #region Static utility methods

        public static double GetDotProduct(Vector2 a, Vector2 b)
        {
            // a . b = a1b1 + a2b2
            return a.X*b.X + a.Y*b.Y;
        }

        #endregion

        #region Properties

        /// <summary>Gets the X component of this 3D vector.</summary>
        public double X { get; private set; }

        /// <summary>Gets the Y component of this 3D vector.</summary>
        public double Y { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="X"/> and <see cref="Y"/> are both equal to zero (i.e. the origin).
        /// </summary>
        public bool IsZero
        {
            get
            {
                return Math.Abs(X - 0) < Epsilon
                    && Math.Abs(Y - 0) < Epsilon;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="X"/> and <see cref="Y"/> are both equal to <see cref="double.NaN"/>.
        /// </summary>
        public bool IsNaN
        {
            get { return double.IsNaN(X) && double.IsNaN(Y); }
        }

        /// <summary>Gets the length of this vector.</summary>
        /// <remarks>Note that this value is not cached, and so is calculated each time this property is read.</remarks>
        /// <returns></returns>
        public double Length
        {
            get { return Math.Sqrt(X*X + Y*Y); }
        }

        #endregion

        /// <summary>
        /// Initialises a new <see cref="Vector2"/> with the specified values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a vector that has the same direction as this one, but with a length of one (a unit vector).
        /// </summary>
        /// <returns></returns>
        public Vector2 Normalize()
        {
            var length = Length;

            // avoid DivideByZeroException (no Epsilon comparison needed)
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return length == 0
                       ? new Vector2() 
                       : new Vector2(X / length, Y / length);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        [Pure]
        public double Dot(Vector2 vector)
        {
            return GetDotProduct(this, vector);
        }

        /// <summary>
        /// Returns a copy of this <see cref="Vector2" /> with its <see cref="X"/> property set
        /// to <paramref name="newX"/>.
        /// </summary>
        /// <param name="newX">The <see cref="X"/> value to use.</param>
        /// <returns></returns>
        [Pure]
        public Vector2 WithX(double newX)
        {
            return new Vector2(newX, Y);
        }

        /// <summary>
        /// Returns a copy of this <see cref="Vector2" /> with its <see cref="Y"/> property set
        /// to <paramref name="newY"/>.
        /// </summary>
        /// <param name="newY">The <see cref="Y"/> value to use.</param>
        /// <returns></returns>
        [Pure]
        public Vector2 WithY(double newY)
        {
            return new Vector2(X, newY);
        }
        
        /// <summary>
        /// Returns a copy of this vector having absolute values of each component.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Vector2 Abs()
        {
            return new Vector2(Math.Abs(X), Math.Abs(Y));
        }

        #region Operator overloads

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(
                -a.X,
                -a.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.X - b.X,
                a.Y - b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.X + b.X,
                a.Y + b.Y);
        }

        public static Vector2 operator *(Vector2 v, double scale)
        {
            return new Vector2(
                v.X*scale,
                v.Y*scale);
        }

        public static Vector2 operator /(Vector2 v, double divisor)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (divisor == 0)
                throw new DivideByZeroException();
            // ReSharper restore CompareOfFloatsByEqualityOperator
            
            return new Vector2(
                v.X/divisor,
                v.Y/divisor);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !a.Equals(b);
        }

        #endregion

        #region ToString, Equality and Hashing

        public override string ToString()
        {
            return string.Format("<{0:0.00} {1:0.00}>", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2))
                return false;

            return Equals((Vector2)obj);
        }

        public bool Equals(Vector2 that)
        {
            if (double.IsNaN(X) &&
                double.IsNaN(Y) && 
                double.IsNaN(that.X) && 
                double.IsNaN(that.Y))
                return true;
            
            return Math.Abs(X - that.X) < Epsilon 
                && Math.Abs(Y - that.Y) < Epsilon;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var c = (int)X;
                c += (int)(Y*5);
                return c;
            }
        }

        #endregion
        
        /// <summary>
        /// Returns the angle between this vector and <paramref name="v"/>.
        /// The return value is constrained to the radian range [0,PI].
        /// </summary>
        /// <param name="v">Vector to measure the angle against.</param>
        /// <returns>The angle in the radian range [0,PI].</returns>
        [Pure]
        public Angle AngleTo(Vector2 v) 
        { 
            var dot = Dot(v) / (Length*v.Length);
            
            if (dot < -1) dot = -1;
            if (dot >  1) dot =  1;
            
            return Angle.FromRadians(Math.Acos(dot));
        } 
    }
}