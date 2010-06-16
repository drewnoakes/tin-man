/*
 * Created by Drew, 31/05/2010 21:03.
 */
using System;
using System.Linq;

namespace Drew.RoboCup
{
	/// <summary>
	/// Represents a 4x4 matrix used to transform <see cref="Vector3"/> instances.
	/// </summary>
	public sealed class TransformationMatrix {
	    static TransformationMatrix() {
	        Identity = new TransformationMatrix(new double[] {
	                                                1, 0, 0, 0,
	                                                0, 1, 0, 0,
	                                                0, 0, 1, 0,
	                                                0, 0, 0, 1
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
	    
	    private double[] _values;
	    
	    private TransformationMatrix(double[] values) {
	        if (values==null)
	            throw new ArgumentNullException("values");
	        if (values.Length!=16)
	            throw new ArgumentException("Array must contain 16 items for a 4x4 transformation matrix.");
	        _values = values;
	    }
	    
	    public TransformationMatrix Translate(double x, double y, double z) {
	        return Multiply(new TransformationMatrix(new double[] {
	                                                     1, 0, 0, 0,
	                                                     0, 1, 0, 0,
	                                                     0, 0, 1, 0,
	                                                     x, y, z, 1
	                                                 }));
	    }
	    
	    public TransformationMatrix RotateX(double angleRadians) {
	        double c = Math.Cos(angleRadians);
	        double s = Math.Sin(angleRadians);
	        return Multiply(new TransformationMatrix(new double[] {
	                                                     1, 0, 0, 0,
	                                                     0, c,-s, 0,
	                                                     0, s, c, 0,
	                                                     0, 0, 0, 1
	                                                 }));
	    }
	    
	    public TransformationMatrix RotateY(double angleRadians) {
	        double c = Math.Cos(angleRadians);
	        double s = Math.Sin(angleRadians);
	        return Multiply(new TransformationMatrix(new double[] {
	                                                     c, 0, s, 0,
	                                                     0, 1, 0, 0,
	                                                    -s, 0, c, 0,
	                                                     0, 0, 0, 1
	                                                 }));
	    }
	    
	    public TransformationMatrix RotateZ(double angleRadians) {
	        double c = Math.Cos(angleRadians);
	        double s = Math.Sin(angleRadians);
	        return Multiply(new TransformationMatrix(new double[] {
	                                                     c,-s, 0, 0,
	                                                     s, c, 0, 0,
	                                                     0, 0, 1, 0,
	                                                     0, 0, 0, 1
	                                                 }));
	    }
	    
	    public TransformationMatrix Multiply(TransformationMatrix matrix) {
            var newValues = new double[16];
            for (int i = 0; i < 16; i += 4) {
                for (int j = 0; j < 4; j++) {
                    newValues[i + j] = matrix._values[i]     * _values[j]
                                     + matrix._values[i + 1] * _values[4 + j]
                                     + matrix._values[i + 2] * _values[8 + j]
                                     + matrix._values[i + 3] * _values[12 + j];
                }
            }
	        return new TransformationMatrix(newValues);
	    }
	    
	    public Vector3 Transform(Vector3 input) {
	        double x = input.X*_values[0] + input.Y*_values[4] + input.Z*_values[8]  + _values[12];
	        double y = input.X*_values[1] + input.Y*_values[5] + input.Z*_values[9]  + _values[13];
	        double z = input.X*_values[2] + input.Y*_values[6] + input.Z*_values[10] + _values[14];
	        double s = input.X*_values[3] + input.Y*_values[7] + input.Z*_values[11] + _values[15];
	        if (s==0)
	            return Vector3.Origin;
            return new Vector3(x/s, y/s, z/s);
	    }
	    
        public override string ToString() {
	        
	        return string.Format("[{0} {1} {2} {3}]\n" +
	                             "[{4} {5} {6} {7}]\n" +
	                             "[{8} {9} {10} {11}]\n" +
	                             "[{12} {13} {14} {15}]", _values.Select(v => (object)v.ToString("0.##")).ToArray());
        }
	    
        public override bool Equals(object obj) {
	        TransformationMatrix matrix = obj as TransformationMatrix;
	        if (matrix==null)
	            return false;
	        for (int i=0; i<16; i++) {
	            if (matrix._values[i]!=_values[i])
	                return false;
	        }
            return true;
        }
	    
        public override int GetHashCode() {
	        unchecked {
                int c = 0;
                for (int i=0; i<_values.Length; i++)
                    c += (int)((i+1) * _values[i]);
                return c;
	        }
        }
	}
}
