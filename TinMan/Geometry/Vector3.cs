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
// Created 07/05/2010 02:50

using System;

namespace TinMan
{
    /// <summary>
    /// Represents a 3-dimensional vector using double-precision values for all three dimensions.
    /// </summary>
    public struct Vector3 {
        /// <summary>A constant Vector3 of zero, equivalent to the origin or cartesian coordinates.</summary>
        public static readonly Vector3 Origin = new Vector3(0, 0, 0);
        
        #region Static utility methods

        /// <summary>
        /// Returns a vector which is perpendicular to both <paramref name="a"/> and <paramref name="b" /> and the plane containing them.
        /// If either of these vectors are zero, or they are parallel, then their cross product is zero.
        /// The magnitude of the product equals the area of a parallelogram with the vectors for sides.
        /// Cross products are anticommutative, meaning <tt>A x B == -(B x A)</tt>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 GetCrossProduct(Vector3 a, Vector3 b) {
            // a × b = (a2b3 − a3b2, a3b1 − a1b3, a1b2 − a2b1)
            return new Vector3(
                        a.Y * b.Z - a.Z * b.Y,
                        a.Z * b.X - a.X * b.Z,
                        a.X * b.Y - a.Y * b.X
                        );
        }
    
        #endregion
        
        #region Properties
        
        /// <summary>Gets the X component of this 3D vector.</summary>
        public double X { get; private set; }
        /// <summary>Gets the Y component of this 3D vector.</summary>
        public double Y { get; private set; }
        /// <summary>Gets the Z component of this 3D vector.</summary>
        public double Z { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether X, Y and Z are all equal to zero.
        /// </summary>
        public bool IsZero {
            get { return X==0 && Y==0 && Z==0; }
        }
        
        /// <summary>Gets the length of this vector.</summary>
        /// <remarks>Note that this value is not cached, and so is calculated each time this property is read.</remarks>
        /// <returns></returns>
        public double Length {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        #endregion
        
        /// <summary>
        /// Initialises a new 3D vector with the specified values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(double x, double y, double z) : this() {
            X = x;
            Y = y;
            Z = z;
        }
        
        /// <summary>
        /// Returns a vector that has the same direction as this one, but with a length of one (a unit vector).
        /// </summary>
        /// <returns></returns>
        public Vector3 Normalize() {
            var length = Length;
            
            // avoid DivideByZeroException
            if (length==0)
                return new Vector3();
            
            return new Vector3(X/length, Y/length, Z/length);
        }
        
        /// <summary>
        /// Gets the 3D vector that is the result of crossing this vector with the specified one.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Cross(Vector3 vector) {
            return GetCrossProduct(this, vector);
        }
        
        #region Operator overloads
        
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

        public static Vector3 operator *(Vector3 v, double scale) {
                return new Vector3(
                    v.X * scale,
                    v.Y * scale,
                    v.Z * scale);
        }

        public static bool operator ==(Vector3 a, Vector3 b) {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b) {
            return !a.Equals(b);
        }

        #endregion
        
        #region ToString, Equality and Hashing
    
        public override string ToString() {
            return string.Format("<{0:0.00},{1:0.00},{2:0.00}>", X, Y, Z);
        }
        
        public override bool Equals(object obj) {
            if (!(obj is Vector3))
                return false;
            Vector3 that = (Vector3)obj;
            return Math.Abs(this.X - that.X) < 0.00001 &&
                   Math.Abs(this.Y - that.Y) < 0.00001 &&
                   Math.Abs(this.Z - that.Z) < 0.00001;
        }
        
        public override int GetHashCode() {
            unchecked {
                int c = (int)X;
                c += (int)(Y * 5);
                c += (int)(Z * 13);
                return c;
            }
        }
        
        #endregion
    }
}
