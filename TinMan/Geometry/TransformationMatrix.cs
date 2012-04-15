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
// Created 31/05/2010 21:03

using System;
using System.Diagnostics;
using TinMan.Annotations;

// ReSharper disable MemberCanBeInternal

namespace TinMan
{
    /// <summary>
    /// Represents a 4x4 matrix used to transform <see cref="Vector3"/> instances.
    /// This type is immutable.
    /// </summary>
    public sealed class TransformationMatrix : IEquatable<TransformationMatrix>
    {
        private const double Epsilon = 0.0001;

        static TransformationMatrix()
        {
            Identity = new TransformationMatrix(1, 0, 0, 0,
                                                0, 1, 0, 0,
                                                0, 0, 1, 0,
                                                0, 0, 0, 1);
            NaN = new TransformationMatrix(double.NaN, double.NaN, double.NaN, double.NaN,
                                           double.NaN, double.NaN, double.NaN, double.NaN,
                                           double.NaN, double.NaN, double.NaN, double.NaN,
                                           double.NaN, double.NaN, double.NaN, double.NaN);
        }

        /// <summary>The identity matrix.</summary>
        /// <remarks>
        /// <pre>
        /// [1 0 0 0]
        /// [0 1 0 0]
        /// [0 0 1 0]
        /// [0 0 0 1]
        /// </pre>
        /// </remarks>
        [NotNull]
        public static TransformationMatrix Identity { get; private set; }

        /// <summary>
        /// A transformation matrix in which all 16 values are <see cref="double.NaN"/>.
        /// </summary>
        [NotNull]
        public static TransformationMatrix NaN { get; private set; }

        [NotNull, Pure]
        public static TransformationMatrix Translation(double x, double y, double z)
        {
            return new TransformationMatrix(1, 0, 0, x,
                                            0, 1, 0, y,
                                            0, 0, 1, z,
                                            0, 0, 0, 1);
        }
        
        /// <summary>
        /// Gets a transformation matrix to transform to the coordinate system specified by the provided
        /// axes.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public static TransformationMatrix GetTransformForCoordinateAxes(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            // TODO validate that the provided axes are in fact orthonormal otherwise the returned matrix is useless
            return new TransformationMatrix(xAxis.X, yAxis.X, zAxis.X, 0,
                                            xAxis.Y, yAxis.Y, zAxis.Y, 0,
                                            xAxis.Z, yAxis.Z, zAxis.Z, 0,
                                            0, 0, 0, 1);
        }

        private readonly double _m00;
        private readonly double _m01;
        private readonly double _m02;
        private readonly double _m03;
        private readonly double _m10;
        private readonly double _m11;
        private readonly double _m12;
        private readonly double _m13;
        private readonly double _m20;
        private readonly double _m21;
        private readonly double _m22;
        private readonly double _m23;
        private readonly double _m30;
        private readonly double _m31;
        private readonly double _m32;
        private readonly double _m33;

        /// <summary>
        /// Initialises a new transformation matrix from an array of 16 double values.
        /// Note that the array is specified in column-major order:
        /// <code>
        /// [00, 01, 02, 03,
        ///  04, 05, 06, 07,
        ///  08, 09, 10, 11,
        ///  12, 13, 14, 15]
        /// </code>
        /// </summary>
        /// <remarks>Note that the array passed into this method is copied so may be
        /// modified safely once this method returns without effecting this
        /// transformation matrix.</remarks>
        /// <param name="values"></param>
        public TransformationMatrix([NotNull] double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 16)
                throw new ArgumentException("Array must contain 16 items for a 4x4 transformation matrix.");
            _m00 = values[0];
            _m01 = values[1];
            _m02 = values[2];
            _m03 = values[3];
            _m10 = values[4];
            _m11 = values[5];
            _m12 = values[6];
            _m13 = values[7];
            _m20 = values[8];
            _m21 = values[9];
            _m22 = values[10];
            _m23 = values[11];
            _m30 = values[12];
            _m31 = values[13];
            _m32 = values[14];
            _m33 = values[15];
        }

        public TransformationMatrix(double m00, double m01, double m02, double m03,
                                    double m10, double m11, double m12, double m13,
                                    double m20, double m21, double m22, double m23,
                                    double m30, double m31, double m32, double m33)
        {
            _m00 = m00; _m01 = m01; _m02 = m02; _m03 = m03;
            _m10 = m10; _m11 = m11; _m12 = m12; _m13 = m13;
            _m20 = m20; _m21 = m21; _m22 = m22; _m23 = m23;
            _m30 = m30; _m31 = m31; _m32 = m32; _m33 = m33;
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// translated by the specified amount.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public TransformationMatrix Translate(double x, double y, double z)
        {
            return Translation(x, y, z).Multiply(this);
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the X axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public TransformationMatrix RotateX(Angle angle)
        {
            var c = angle.Cos;
            var s = angle.Sin;
            return new TransformationMatrix(1, 0,  0, 0,
                                            0, c, -s, 0,
                                            0, s,  c, 0,
                                            0, 0,  0, 1).Multiply(this);
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the Y axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public TransformationMatrix RotateY(Angle angle)
        {
            var c = angle.Cos;
            var s = angle.Sin;
            return new TransformationMatrix( c, 0, s, 0,
                                             0, 1, 0, 0,
                                            -s, 0, c, 0,
                                             0, 0, 0, 1).Multiply(this);
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the Z axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public TransformationMatrix RotateZ(Angle angle)
        {
            var c = angle.Cos;
            var s = angle.Sin;
            return new TransformationMatrix(c, -s, 0, 0,
                                            s,  c, 0, 0,
                                            0,  0, 1, 0,
                                            0,  0, 0, 1).Multiply(this);
        }

        /// <summary>
        /// Returns a transformation matrix which is the product of this instance with the
        /// specified matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [NotNull, Pure]
        public TransformationMatrix Multiply([NotNull] TransformationMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new TransformationMatrix(
            	_m00*matrix._m00 + _m01*matrix._m10 + _m02*matrix._m20 + _m03*matrix._m30,
    	        _m00*matrix._m01 + _m01*matrix._m11 + _m02*matrix._m21 + _m03*matrix._m31,
    	        _m00*matrix._m02 + _m01*matrix._m12 + _m02*matrix._m22 + _m03*matrix._m32,
    	        _m00*matrix._m03 + _m01*matrix._m13 + _m02*matrix._m23 + _m03*matrix._m33,
    	
		        _m10*matrix._m00 + _m11*matrix._m10 + _m12*matrix._m20 + _m13*matrix._m30,
    	        _m10*matrix._m01 + _m11*matrix._m11 + _m12*matrix._m21 + _m13*matrix._m31,
    	        _m10*matrix._m02 + _m11*matrix._m12 + _m12*matrix._m22 + _m13*matrix._m32,
    	        _m10*matrix._m03 + _m11*matrix._m13 + _m12*matrix._m23 + _m13*matrix._m33,
    	
		        _m20*matrix._m00 + _m21*matrix._m10 + _m22*matrix._m20 + _m23*matrix._m30,
    	        _m20*matrix._m01 + _m21*matrix._m11 + _m22*matrix._m21 + _m23*matrix._m31,
    	        _m20*matrix._m02 + _m21*matrix._m12 + _m22*matrix._m22 + _m23*matrix._m32,
    	        _m20*matrix._m03 + _m21*matrix._m13 + _m22*matrix._m23 + _m23*matrix._m33,
    	
		        _m30*matrix._m00 + _m31*matrix._m10 + _m32*matrix._m20 + _m33*matrix._m30,
    	        _m30*matrix._m01 + _m31*matrix._m11 + _m32*matrix._m21 + _m33*matrix._m31,
    	        _m30*matrix._m02 + _m31*matrix._m12 + _m32*matrix._m22 + _m33*matrix._m32,
    	        _m30*matrix._m03 + _m31*matrix._m13 + _m32*matrix._m23 + _m33*matrix._m33);
        }

        /// <summary>
        /// Applies the transformations modelled within this matrix to the specified vector, returning a new, transformed vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Pure]
        public Vector3 Transform(Vector3 input)
        {
            var x = _m00*input.X + _m01*input.Y + _m02*input.Z + _m03;
            var y = _m10*input.X + _m11*input.Y + _m12*input.Z + _m13;
            var z = _m20*input.X + _m21*input.Y + _m22*input.Z + _m23;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            Debug.Assert(_m30 == 0 && _m31 == 0 && _m32 == 0 && _m33 == 1);
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return new Vector3(x, y, z);

//            double x = input.X*_m00 + input.Y*_m10 + input.Z*_m20 + _m30;
//            double y = input.X*_m01 + input.Y*_m11 + input.Z*_m21 + _m31;
//            double z = input.X*_m02 + input.Y*_m12 + input.Z*_m22 + _m32;
//            double s = input.X*_m03 + input.Y*_m13 + input.Z*_m23 + _m33;
//            if (Math.Abs(s) < Epsilon)
//                return Vector3.Origin;
//            return new Vector3(x/s, y/s, z/s);
        }

        #region Untested utility methods

        // TODO write unit tests for these, esp when combinations exist

        /// <summary>Get the direction of the x-axis of this transformation.</summary>
        [Pure]
        public Vector3 GetXAxis()
        {
            return new Vector3(_m00, _m01, _m02);
        }

        /// <summary>Get the direction of the y-axis of this transformation.</summary>
        [Pure]
        public Vector3 GetYAxis()
        {
            return new Vector3(_m10, _m11, _m12);
        }

        /// <summary>Get the direction of the z-axis of this transformation.</summary>
        [Pure]
        public Vector3 GetZAxis()
        {
            return new Vector3(_m20, _m21, _m22);
        }

        #endregion

        /// <summary>Get the translation part of this transformation.</summary>
        [Pure]
        public Vector3 GetTranslation()
        {
            return new Vector3(_m03, _m13, _m23);
        }

        #region ToString, Equality and Hashing

        public override string ToString()
        {
            return string.Format("[{0,4:0.##} {1,4:0.##} {2,4:0.##} {3,4:0.##}]\n" +
                                 "[{4,4:0.##} {5,4:0.##} {6,4:0.##} {7,4:0.##}]\n" +
                                 "[{8,4:0.##} {9,4:0.##} {10,4:0.##} {11,4:0.##}]\n" +
                                 "[{12,4:0.##} {13,4:0.##} {14,4:0.##} {15,4:0.##}]", 
                                _m00,_m01,_m02,_m03,
                                _m10,_m11,_m12,_m13,
                                _m20,_m21,_m22,_m23,
                                _m30,_m31,_m32,_m33);
        }

        public override bool Equals(object obj)
        {
            var matrix = obj as TransformationMatrix;
            
            if (matrix == null)
                return false;
    		
            return Equals(matrix);
        }
        
        public bool Equals(TransformationMatrix matrix)
        {
            return Math.Abs(matrix._m00 - _m00) <= Epsilon
                && Math.Abs(matrix._m01 - _m01) <= Epsilon
                && Math.Abs(matrix._m02 - _m02) <= Epsilon
                && Math.Abs(matrix._m03 - _m03) <= Epsilon
                && Math.Abs(matrix._m10 - _m10) <= Epsilon
                && Math.Abs(matrix._m11 - _m11) <= Epsilon
                && Math.Abs(matrix._m12 - _m12) <= Epsilon
                && Math.Abs(matrix._m13 - _m13) <= Epsilon
                && Math.Abs(matrix._m20 - _m20) <= Epsilon
                && Math.Abs(matrix._m21 - _m21) <= Epsilon
                && Math.Abs(matrix._m22 - _m22) <= Epsilon
                && Math.Abs(matrix._m23 - _m23) <= Epsilon
                && Math.Abs(matrix._m30 - _m30) <= Epsilon
                && Math.Abs(matrix._m31 - _m31) <= Epsilon
                && Math.Abs(matrix._m32 - _m32) <= Epsilon
                && Math.Abs(matrix._m33 - _m33) <= Epsilon;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int c = (int)_m00;
                c += (int)(2*_m01);
                c += (int)(3*_m02);
                c += (int)(4*_m03);
                c += (int)(5*_m10);
                c += (int)(6*_m11);
                c += (int)(7*_m12);
                c += (int)(8*_m13);
                c += (int)(9*_m20);
                c += (int)(10*_m21);
                c += (int)(11*_m22);
                c += (int)(12*_m23);
                c += (int)(13*_m30);
                c += (int)(14*_m31);
                c += (int)(15*_m32);
                c += (int)(16*_m33);
                return c;
            }
        }

        #endregion

/*
        /// <summary>Calculates the inversion of this transformation matrix.</summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The determinant is zero.</exception>
        public TransformationMatrix Invert()
        {
            TransformationMatrix inversion;
            if (!TryInvert(out inversion))
                throw new InvalidOperationException("Cannot invert a TransformationMatrix with a zero determinant.");
            return inversion;
        }

        /// <summary>Attempts to calculate the inversion of this transformation matrix.</summary>
        /// <param name="inversion"></param>
        /// <returns><c>true</c> if the inversion succeeeded, <c>false</c> if the inversion was not possible because the determinant was zero.</returns>
        public bool TryInvert(out TransformationMatrix inversion)
        {
            // TODO investigate potential perf improvement (http://www.devmaster.net/wiki/Transformation_matrices#Inversion) Transformation matrices involving only rotation and translation (ie. no scale) have convenient properties making them very easy to invert.
            double det = GetDeterminant();
            if (Math.Abs(det - 0) < Epsilon)
            {
                inversion = null;
                return false;
            }
            double[] m = _values;
            inversion = new TransformationMatrix(new[]
            {
                (m[9] *m[14]*m[7]  - m[13]*m[10]*m[7]  + m[13]*m[6] *m[11] - m[5] *m[14]*m[11] - m[9] *m[6] *m[15] + m[5] *m[10]*m[15]) / det,
                (m[13]*m[10]*m[3]  - m[9] *m[14]*m[3]  - m[13]*m[2] *m[11] + m[1] *m[14]*m[11] + m[9] *m[2] *m[15] - m[1] *m[10]*m[15]) / det,
                (m[5] *m[14]*m[3]  - m[13]*m[6] *m[3]  + m[13]*m[2] *m[7]  - m[1] *m[14]*m[7]  - m[5] *m[2] *m[15] + m[1] *m[6] *m[15]) / det,
                (m[9] *m[6] *m[3]  - m[5] *m[10]*m[3]  - m[9] *m[2] *m[7]  + m[1] *m[10]*m[7]  + m[5] *m[2] *m[11] - m[1] *m[6] *m[11]) / det,
                (m[12]*m[10]*m[7]  - m[8] *m[14]*m[7]  - m[12]*m[6] *m[11] + m[4] *m[14]*m[11] + m[8] *m[6] *m[15] - m[4] *m[10]*m[15]) / det,
                (m[8] *m[14]*m[3]  - m[12]*m[10]*m[3]  + m[12]*m[2] *m[11] - m[0] *m[14]*m[11] - m[8] *m[2] *m[15] + m[0] *m[10]*m[15]) / det,
                (m[12]*m[6] *m[3]  - m[4] *m[14]*m[3]  - m[12]*m[2] *m[7]  + m[0] *m[14]*m[7]  + m[4] *m[2] *m[15] - m[0] *m[6] *m[15]) / det,
                (m[4] *m[10]*m[3]  - m[8] *m[6] *m[3]  + m[8] *m[2] *m[7]  - m[0] *m[10]*m[7]  - m[4] *m[2] *m[11] + m[0] *m[6] *m[11]) / det,
                (m[8] *m[13]*m[7]  - m[12]*m[9] *m[7]  + m[12]*m[5] *m[11] - m[4] *m[13]*m[11] - m[8] *m[5] *m[15] + m[4] *m[9] *m[15]) / det,
                (m[12]*m[9] *m[3]  - m[8] *m[13]*m[3]  - m[12]*m[1] *m[11] + m[0] *m[13]*m[11] + m[8] *m[1] *m[15] - m[0] *m[9] *m[15]) / det,
                (m[4] *m[13]*m[3]  - m[12]*m[5] *m[3]  + m[12]*m[1] *m[7]  - m[0] *m[13]*m[7]  - m[4] *m[1] *m[15] + m[0] *m[5] *m[15]) / det,
                (m[8] *m[5] *m[3]  - m[4] *m[9] *m[3]  - m[8] *m[1] *m[7]  + m[0] *m[9] *m[7]  + m[4] *m[1] *m[11] - m[0] *m[5] *m[11]) / det,
                (m[12]*m[9] *m[6]  - m[8] *m[13]*m[6]  - m[12]*m[5] *m[10] + m[4] *m[13]*m[10] + m[8] *m[5] *m[14] - m[4] *m[9] *m[14]) / det,
                (m[8] *m[13]*m[2]  - m[12]*m[9] *m[2]  + m[12]*m[1] *m[10] - m[0] *m[13]*m[10] - m[8] *m[1] *m[14] + m[0] *m[9] *m[14]) / det,
                (m[12]*m[5] *m[2]  - m[4] *m[13]*m[2]  - m[12]*m[1] *m[6]  + m[0] *m[13]*m[6]  + m[4] *m[1] *m[14] - m[0] *m[5] *m[14]) / det,
                (m[4] *m[9] *m[2]  - m[8] *m[5] *m[2]  + m[8] *m[1] *m[6]  - m[0] *m[9] *m[6]  - m[4] *m[1] *m[10] + m[0] *m[5] *m[10]) / det
            });

            return true;
        }
*/

        /// <summary>
        /// Computes the determinant of this matrix.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public double GetDeterminant()
        {
    	    return _m00*(_m11*_m22*_m33 + _m12*_m23*_m31 + _m13*_m21*_m32 - _m13*_m22*_m31 -_m11*_m23*_m32 - _m12*_m21*_m33)
    		     - _m01*(_m10*_m22*_m33 + _m12*_m23*_m30 + _m13*_m20*_m32 - _m13*_m22*_m30 -_m10*_m23*_m32 - _m12*_m20*_m33)
    		     + _m02*(_m10*_m21*_m33 + _m11*_m23*_m30 + _m13*_m20*_m31 - _m13*_m21*_m30 -_m10*_m23*_m31 - _m11*_m20*_m33)
    		     - _m03*(_m10*_m21*_m32 + _m11*_m22*_m30 + _m12*_m20*_m31 - _m12*_m21*_m30 -_m10*_m22*_m31 - _m11*_m20*_m32);
        }
    }
}