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
// Created 01/06/2010 03:47

using System;
using NUnit.Framework;

namespace TinMan
{
    [TestFixture]
    public sealed class TransformationMatrixTest {
        [Test] public void IdentityConversion() {
            Assert.AreEqual(Vector3.Origin, TransformationMatrix.Identity.Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(1,2,3), TransformationMatrix.Identity.Transform(new Vector3(1,2,3)));
        }
        
        [Test] public void Multiply() {
            Assert.AreEqual(TransformationMatrix.Identity, TransformationMatrix.Identity.Multiply(TransformationMatrix.Identity));
        }
        
        [Test] public void Translation() {
            Assert.AreEqual("[1 0 0 0]\n[0 1 0 0]\n[0 0 1 0]\n[10 10 10 1]", TransformationMatrix.Identity.Translate(10,10,10).ToString());
            
            Assert.AreEqual(new Vector3(10,10,10), TransformationMatrix.Identity.Translate(10,10,10).Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(10,10,10), TransformationMatrix.Identity.Translate(5,5,5).Transform(new Vector3(5,5,5)));
            Assert.AreEqual(new Vector3(11,13,15), TransformationMatrix.Identity.Translate(1,2,3).Transform(new Vector3(10,11,12)));
        }
        
        [Test] public void MultipleTranslations() {
            var m = TransformationMatrix.Identity.Translate(5,0,0).Translate(0,5,0).Translate(-5,0,5);
            Assert.AreEqual(new Vector3(0,5,5), m.Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(5,5,5), m.Transform(new Vector3(5,0,0)));
            Assert.AreEqual(new Vector3(5,10,10), m.Transform(new Vector3(5,5,5)));
        }
        
        [Test] public void ToStringTest() {
            Assert.AreEqual("[1 0 0 0]\n[0 1 0 0]\n[0 0 1 0]\n[0 0 0 1]", TransformationMatrix.Identity.ToString());
        }
        
        [Test] public void RotateX() {
            Assert.AreEqual(new Vector3(0,0,-10), TransformationMatrix.Identity.RotateX(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,-10,0), TransformationMatrix.Identity.RotateX(Angle.FromRadians(Math.PI)).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(10,0,0), TransformationMatrix.Identity.RotateX(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(10,0,0)));
        }

        [Test] public void MultipleXRotations() {
            var m = TransformationMatrix.Identity.RotateX(Angle.FromRadians(Math.PI/2)).RotateX(Angle.FromRadians(-Math.PI)).RotateX(Angle.FromRadians(Math.PI));
            Assert.AreEqual(new Vector3(0,0,-10), m.Transform(new Vector3(0,10,0)));
        }

        [Test] public void RotateY() {
            Assert.AreEqual(new Vector3(-10,0,0), TransformationMatrix.Identity.RotateY(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(0,0,10)));
            Assert.AreEqual(new Vector3(-10,0,0), TransformationMatrix.Identity.RotateY(Angle.FromRadians(Math.PI)).Transform(new Vector3(10,0,0)));
            Assert.AreEqual(new Vector3(0,10,0), TransformationMatrix.Identity.RotateY(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(0,10,0)));
        }

        [Test] public void MultipleYRotations() {
            var m = TransformationMatrix.Identity.RotateY(Angle.FromRadians(Math.PI/2)).RotateY(Angle.FromRadians(-Math.PI)).RotateY(Angle.FromRadians(Math.PI));
            Assert.AreEqual(new Vector3(-10,0,0), m.Transform(new Vector3(0,0,10)));
        }
        
        [Test] public void RotateZ() {
            Assert.AreEqual(new Vector3(10,0,0), TransformationMatrix.Identity.RotateZ(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,-10,0), TransformationMatrix.Identity.RotateZ(Angle.FromRadians(Math.PI)).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,0,10), TransformationMatrix.Identity.RotateZ(Angle.FromRadians(Math.PI/2)).Transform(new Vector3(0,0,10)));
        }

        [Test] public void MultipleZRotations() {
            var m = TransformationMatrix.Identity.RotateZ(Angle.FromRadians(Math.PI/2)).RotateZ(Angle.FromRadians(-Math.PI)).RotateZ(Angle.FromRadians(Math.PI));
            Assert.AreEqual(new Vector3(10,0,0), m.Transform(new Vector3(0,10,0)));
        }
        
        [Test] public void TranslateThenRotate() {
            // translate origin 10 units down x-axis, then rotate around y by 90 degrees
//            var matrix = TransformationMatrix.Identity.Translate(10, 0, 0).RotateY(90);
            var matrix = TransformationMatrix.Identity.Translate(10, 0, 0).RotateY(Angle.FromDegrees(90));
            Assert.AreEqual(new Vector3(0,0,10), matrix.Transform(Vector3.Origin));
        }
        
        [Test] public void GetDeterminant() {
            Assert.AreEqual(1, TransformationMatrix.Identity.GetDeterminant());
            Assert.AreEqual(0, new TransformationMatrix(new double[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}).GetDeterminant());
            Assert.AreEqual(0, new TransformationMatrix(new double[] {1,1,1,1,2,2,2,2,3,3,3,3,4,4,4,4}).GetDeterminant());
            Assert.AreEqual(-6, new TransformationMatrix(new double[] {1,1,1,1,2,1,2,2,3,3,1,3,4,4,4,1}).GetDeterminant());
            Assert.AreEqual(-3, new TransformationMatrix(new double[] {0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0}).GetDeterminant());
        }
        
        [Test] public void Invert() {
            // Inverting the identity has no effect
            Assert.AreEqual(TransformationMatrix.Identity, TransformationMatrix.Identity.Invert());
            // Some random numeric examples
            Assert.AreEqual(
                new TransformationMatrix(new[] {-5/3d,0,1/3d,0,-4/3d,1,-1/3d,0,4/3d,0,-1/6d,0,4/3d,0,-5/3d,1}),
                new TransformationMatrix(new double[] {1,0,2,0,4,1,6,0,8,0,10,0,12,0,14,1}).Invert());
            Assert.AreEqual(
                new TransformationMatrix(new[] {0.01,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0.01}),
                new TransformationMatrix(new double[] {100,0,0,0,0,1,0,0,0,0,1,0,0,0,0,100}).Invert());
            // inversions undo translations and rotations
            Assert.AreEqual(
                TransformationMatrix.Identity.Translate(-10,-20,-30),
                TransformationMatrix.Identity.Translate(10,20,30).Invert());
            Assert.AreEqual(
                TransformationMatrix.Identity.RotateX(Angle.FromRadians(-Math.PI/2)),
                TransformationMatrix.Identity.RotateX(Angle.FromRadians(Math.PI/2)).Invert());
            Assert.AreEqual(
                TransformationMatrix.Identity.RotateY(Angle.FromRadians(-Math.PI/4)),
                TransformationMatrix.Identity.RotateY(Angle.FromRadians(Math.PI/4)).Invert());
            Assert.AreEqual(
                TransformationMatrix.Identity.RotateZ(Angle.FromRadians(-Math.PI/6)),
                TransformationMatrix.Identity.RotateZ(Angle.FromRadians(Math.PI/6)).Invert());
        }
        
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void InvertWhenDeterminantIsZero() {
            new TransformationMatrix(new double[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}).Invert();
        }
        
        [Test] public void TryInversion() {
            TransformationMatrix inversion;
            Assert.IsFalse(new TransformationMatrix(new double[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16}).TryInvert(out inversion));
            Assert.IsNull(inversion);
            Assert.IsTrue(TransformationMatrix.Identity.TryInvert(out inversion));
            Assert.AreEqual(TransformationMatrix.Identity, inversion);
        }
    }
}
