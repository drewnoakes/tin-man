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
using System.Linq;

// ReSharper disable MemberCanBeInternal

namespace TinMan
{
    /// <summary>
    /// Represents a 4x4 matrix used to transform <see cref="Vector3"/> instances.
    /// This type is immutable.
    /// </summary>
    public sealed class TransformationMatrix
    {
        private const double Epsilon = 0.0001;

        static TransformationMatrix()
        {
            Identity = new TransformationMatrix(new double[]
                                                    {
                                                        1, 0, 0, 0,
                                                        0, 1, 0, 0,
                                                        0, 0, 1, 0,
                                                        0, 0, 0, 1
                                                    });
            NaN = new TransformationMatrix(new[]
                                               {
                                                   double.NaN, double.NaN, double.NaN, double.NaN,
                                                   double.NaN, double.NaN, double.NaN, double.NaN,
                                                   double.NaN, double.NaN, double.NaN, double.NaN,
                                                   double.NaN, double.NaN, double.NaN, double.NaN
                                               });
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
        public static TransformationMatrix Identity { get; private set; }

        /// <summary>
        /// A transformation matrix in which all 16 values are <see cref="double.NaN"/>.
        /// </summary>
        public static TransformationMatrix NaN { get; private set; }

        /// <summary>
        /// Gets a transformation matrix to transform to the coordinate system specified by the provided
        /// axes.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <returns></returns>
        public static TransformationMatrix GetTransformForCoordinateAxes(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return new TransformationMatrix(new[]
                                                {
                                                    xAxis.X, yAxis.X, zAxis.X, 0,
                                                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                                                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                                                    0, 0, 0, 1
                                                });
        }

        /// <summary>
        /// The array of 16 double values that backs this matrix.  This array is assigned in the constructor
        /// and MUST NOT be changed after that time in order to maintain the immutability of this type.
        /// </summary>
        private readonly double[] _values;

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
        /// <remarks>Note that the array passed into this method is not copied.  The caller must ensure
        /// that the array will not be modified after calling this constructor, otherwise the matrix will
        /// reflect that change.</remarks>
        /// <param name="values"></param>
        public TransformationMatrix(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 16)
                throw new ArgumentException("Array must contain 16 items for a 4x4 transformation matrix.");
            _values = values;
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// translated by the specified amount.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public TransformationMatrix Translate(double x, double y, double z)
        {
            return Multiply(new TransformationMatrix(new[]
                                                         {
                                                             1, 0, 0, 0,
                                                             0, 1, 0, 0,
                                                             0, 0, 1, 0,
                                                             x, y, z, 1
                                                         }));
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the X axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public TransformationMatrix RotateX(Angle angle)
        {
            double c = angle.Cos;
            double s = angle.Sin;
            return Multiply(new TransformationMatrix(new[]
                                                         {
                                                             1, 0, 0, 0,
                                                             0, c, -s, 0,
                                                             0, s, c, 0,
                                                             0, 0, 0, 1
                                                         }));
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the Y axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public TransformationMatrix RotateY(Angle angle)
        {
            double c = angle.Cos;
            double s = angle.Sin;
            return Multiply(new TransformationMatrix(new[]
                                                         {
                                                             c, 0, s, 0,
                                                             0, 1, 0, 0,
                                                             -s, 0, c, 0,
                                                             0, 0, 0, 1
                                                         }));
        }

        /// <summary>
        /// Returns a transformation matrix which is the equivalent of this instance, only
        /// rotated by the specified amount around the Z axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public TransformationMatrix RotateZ(Angle angle)
        {
            double c = angle.Cos;
            double s = angle.Sin;
            return Multiply(new TransformationMatrix(new[]
                                                         {
                                                             c, -s, 0, 0,
                                                             s, c, 0, 0,
                                                             0, 0, 1, 0,
                                                             0, 0, 0, 1
                                                         }));
        }

        /// <summary>
        /// Returns a transformation matrix which is the product of this instance with the specified
        /// matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public TransformationMatrix Multiply(TransformationMatrix matrix)
        {
            var newValues = new double[16];
            for (int i = 0; i < 16; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    newValues[i + j] = _values[i]*matrix._values[j]
                                     + _values[i + 1]*matrix._values[4 + j]
                                     + _values[i + 2]*matrix._values[8 + j]
                                     + _values[i + 3]*matrix._values[12 + j];
                }
            }
            return new TransformationMatrix(newValues);
        }

        /// <summary>
        /// Applies the transformations modelled within this matrix to the specified vector, returning a new, transformed vector.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vector3 Transform(Vector3 input)
        {
            double x = input.X*_values[0] + input.Y*_values[4] + input.Z*_values[8] + _values[12];
            double y = input.X*_values[1] + input.Y*_values[5] + input.Z*_values[9] + _values[13];
            double z = input.X*_values[2] + input.Y*_values[6] + input.Z*_values[10] + _values[14];
            double s = input.X*_values[3] + input.Y*_values[7] + input.Z*_values[11] + _values[15];
            if (Math.Abs(s - 0) < Epsilon)
                return Vector3.Origin;
            return new Vector3(x/s, y/s, z/s);
        }

        #region Untested utility methods

        // TODO write unit tests for these, esp when combinations exist

        /// <summary>Get the direction of the x-axis of this transformation.</summary>
        public Vector3 GetXAxis()
        {
            return new Vector3(_values[0], _values[1], _values[2]);
        }

        /// <summary>Get the direction of the y-axis of this transformation.</summary>
        public Vector3 GetYAxis()
        {
            return new Vector3(_values[4], _values[5], _values[6]);
        }

        /// <summary>Get the direction of the z-axis of this transformation.</summary>
        public Vector3 GetZAxis()
        {
            return new Vector3(_values[8], _values[9], _values[10]);
        }

        /// <summary>Get the translation part of this transformation.</summary>
        public Vector3 GetTranslation()
        {
            return new Vector3(_values[12], _values[13], _values[14]);
        }

        #endregion

        #region ToString, Equality and Hashing

        public override string ToString()
        {
            return string.Format("[{0} {1} {2} {3}]\n" +
                                 "[{4} {5} {6} {7}]\n" +
                                 "[{8} {9} {10} {11}]\n" +
                                 "[{12} {13} {14} {15}]", _values.Select(v => (object)v.ToString("0.##")).ToArray());
        }

        public override bool Equals(object obj)
        {
            var matrix = obj as TransformationMatrix;
            if (matrix == null)
                return false;
            for (int i = 0; i < 16; i++)
            {
                if (Math.Abs(matrix._values[i] - _values[i]) > Epsilon)
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int c = 0;
                for (int i = 0; i < _values.Length; i++)
                    c += (int)((i + 1)*_values[i]);
                return c;
            }
        }

        #endregion

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

        public double GetDeterminant()
        {
            double[] m = _values;
            return
                 m[12] * m[9]  * m[6]  * m[3]   -  m[8] * m[13] * m[6]  * m[3]   -
                 m[12] * m[5]  * m[10] * m[3]   +  m[4] * m[13] * m[10] * m[3]   +
                 m[8]  * m[5]  * m[14] * m[3]   -  m[4] * m[9]  * m[14] * m[3]   -
                 m[12] * m[9]  * m[2]  * m[7]   +  m[8] * m[13] * m[2]  * m[7]   +
                 m[12] * m[1]  * m[10] * m[7]   -  m[0] * m[13] * m[10] * m[7]   -
                 m[8]  * m[1]  * m[14] * m[7]   +  m[0] * m[9]  * m[14] * m[7]   +
                 m[12] * m[5]  * m[2]  * m[11]  -  m[4] * m[13] * m[2]  * m[11]  -
                 m[12] * m[1]  * m[6]  * m[11]  +  m[0] * m[13] * m[6]  * m[11]  +
                 m[4]  * m[1]  * m[14] * m[11]  -  m[0] * m[5]  * m[14] * m[11]  -
                 m[8]  * m[5]  * m[2]  * m[15]  +  m[4] * m[9]  * m[2]  * m[15]  +
                 m[8]  * m[1]  * m[6]  * m[15]  -  m[0] * m[9]  * m[6]  * m[15]  -
                 m[4]  * m[1]  * m[10] * m[15]  +  m[0] * m[5]  * m[10] * m[15];
        }
    }
}