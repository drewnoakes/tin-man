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

// ReSharper disable MemberCanBeInternal

namespace TinMan
{
	/// <summary>
	/// Represents a 3-dimensional vector using double-precision values for all three dimensions.
	/// </summary>
	public struct Vector3 : IEquatable<Vector3>
	{
		private const double Epsilon = 0.0001;

		/// <summary>A constant Vector3 of zero, equivalent to the origin or cartesian coordinates.</summary>
		public static readonly Vector3 Origin = new Vector3(0, 0, 0);

		/// <summary>A constant Vector3 with a NaN value in all dimensions.</summary>
		public static readonly Vector3 NaN = new Vector3(double.NaN, double.NaN, double.NaN);

		/// <summary>A constant Vector3 with unit length that points along the X axis.</summary>
		public static readonly Vector3 UnitX = new Vector3(1, 0, 0);

		/// <summary>A constant Vector3 with unit length that points along the Y axis.</summary>
		public static readonly Vector3 UnitY = new Vector3(0, 1, 0);

		/// <summary>A constant Vector3 with unit length that points along the Z axis.</summary>
		public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);

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
		public static Vector3 GetCrossProduct(Vector3 a, Vector3 b)
		{
			// a × b = (a2b3 − a3b2, a3b1 − a1b3, a1b2 − a2b1)
			return new Vector3(
				a.Y*b.Z - a.Z*b.Y,
				a.Z*b.X - a.X*b.Z,
				a.X*b.Y - a.Y*b.X
			);
		}

		public static double GetDotProduct(Vector3 a, Vector3 b)
		{
			// a . b = a1b1 + a2b2 + a3b3
			return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
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
		/// Gets a value indicating whether X, Y and Z are all equal to zero (i.e. the origin).
		/// </summary>
		public bool IsZero
		{
			get
			{
				return Math.Abs(X - 0) < Epsilon
					&& Math.Abs(Y - 0) < Epsilon
					&& Math.Abs(Z - 0) < Epsilon;
			}
		}

		/// <summary>
		/// Gets a value indicating whether X, Y and Z are all equal to <see cref="double.NaN"/>.
		/// </summary>
		public bool IsNaN
		{
			get { return double.IsNaN(X) && double.IsNaN(Y) && double.IsNaN(Z); }
		}

		/// <summary>Gets the length of this vector.</summary>
		/// <remarks>Note that this value is not cached, and so is calculated each time this property is read.</remarks>
		/// <returns></returns>
		public double Length
		{
			get { return Math.Sqrt(X*X + Y*Y + Z*Z); }
		}

		#endregion

		/// <summary>
		/// Initialises a new 3D vector with the specified values.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3(double x, double y, double z) : this()
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Returns a vector that has the same direction as this one, but with a length of one (a unit vector).
		/// </summary>
		/// <returns></returns>
		public Vector3 Normalize()
		{
			double length = Length;

			// avoid DivideByZeroException (no Epsilon comparison needed)
			if (length == 0)
				return new Vector3();

			return new Vector3(X/length, Y/length, Z/length);
		}

		/// <summary>
		/// Gets the 3D vector that is the result of crossing this vector with the specified one.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public Vector3 Cross(Vector3 vector)
		{
			return GetCrossProduct(this, vector);
		}

		public double Dot(Vector3 vector)
		{
			return GetDotProduct(this, vector);
		}

		/// <summary>
		/// Returns a copy of this <see cref="Vector3" /> with its <see cref="X"/> property set
		/// to <paramref name="newX"/>.
		/// </summary>
		/// <param name="newX">The <see cref="X"/> value to use.</param>
		/// <returns></returns>
		public Vector3 WithX(double newX)
		{
			return new Vector3(newX, Y, Z);
		}

		/// <summary>
		/// Returns a copy of this <see cref="Vector3" /> with its <see cref="Y"/> property set
		/// to <paramref name="newY"/>.
		/// </summary>
		/// <param name="newY">The <see cref="Y"/> value to use.</param>
		/// <returns></returns>
		public Vector3 WithY(double newY)
		{
			return new Vector3(X, newY, Z);
		}

		/// <summary>
		/// Returns a copy of this <see cref="Vector3" /> with its <see cref="Z"/> property set
		/// to <paramref name="newZ"/>.
		/// </summary>
		/// <param name="newZ">The <see cref="Z"/> value to use.</param>
		/// <returns></returns>
		public Vector3 WithZ(double newZ)
		{
			return new Vector3(X, Y, newZ);
		}
		
		/// <summary>
		/// Returns a copy of this vector having absolute values of each component.
		/// </summary>
		/// <returns></returns>
		public Vector3 Abs()
		{
			return new Vector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
		}

		#region Operator overloads

		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(
				-a.X,
				-a.Y,
				-a.Z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X - b.X,
				a.Y - b.Y,
				a.Z - b.Z);
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X + b.X,
				a.Y + b.Y,
				a.Z + b.Z);
		}

		public static Vector3 operator *(Vector3 v, double scale)
		{
			return new Vector3(
				v.X*scale,
				v.Y*scale,
				v.Z*scale);
		}

		public static Vector3 operator /(Vector3 v, double divisor)
		{
			if (divisor == 0)
				throw new DivideByZeroException();
			
			return new Vector3(
				v.X/divisor,
				v.Y/divisor,
				v.Z/divisor);
		}

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return !a.Equals(b);
		}

		#endregion

		#region ToString, Equality and Hashing

		public override string ToString()
		{
			return string.Format("<{0:0.00} {1:0.00} {2:0.00}>", X, Y, Z);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector3))
				return false;

			return Equals((Vector3)obj);
		}


		public bool Equals(Vector3 that)
		{
			if (double.IsNaN(X) &&
			    double.IsNaN(Y) && 
			    double.IsNaN(Z) && 
			    double.IsNaN(that.X) && 
			    double.IsNaN(that.Y) && 
			    double.IsNaN(that.Z))
				return true;
			
			return Math.Abs(X - that.X) < Epsilon 
				&& Math.Abs(Y - that.Y) < Epsilon 
				&& Math.Abs(Z - that.Z) < Epsilon;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var c = (int)X;
				c += (int)(Y*5);
				c += (int)(Z*13);
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
		public Angle AngleTo(Vector3 v) 
		{ 
			double dot = this.Dot(v) / (Length*v.Length);
			
			if (dot < -1) dot = -1;
			if (dot >  1) dot =  1;
			
			return Angle.FromRadians(Math.Acos(dot));
		} 
	}
}