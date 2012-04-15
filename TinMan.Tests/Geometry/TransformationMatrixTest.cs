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

using NUnit.Framework;

namespace TinMan
{
    [TestFixture]
    public sealed class TransformationMatrixTest
    {
        // TODO adjust column widths in ToString to show v. small numbers with E-12 style presentation

        private static readonly TransformationMatrix _identity = TransformationMatrix.Identity;

        [Test]
        public void IdentityConversion()
        {
            Assert.AreEqual(Vector3.Origin, _identity.Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(1, 2, 3), _identity.Transform(new Vector3(1, 2, 3)));
        }

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(_identity, _identity.Multiply(_identity));
        }

        [Test]
        public void MultipleTranslations()
        {
            var m = _identity.Translate(5, 0, 0).Translate(0, 5, 0).Translate(-5, 0, 5);
            Assert.AreEqual(new Vector3(0, 5, 5), m.Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(5, 5, 5), m.Transform(new Vector3(5, 0, 0)));
            Assert.AreEqual(new Vector3(5, 10, 10), m.Transform(new Vector3(5, 5, 5)));
        }

        [Test]
        public void RotateX()
        {
            var xRot90 = _identity.RotateX(Angle.HalfPi);
            Assert.AreEqual(new Vector3(0, 0, 10), xRot90.Transform(new Vector3(0, 10, 0)));
            Assert.AreEqual(new Vector3(10, 0, 0), xRot90.Transform(new Vector3(10, 0, 0)));

            var xRot180 = _identity.RotateX(Angle.Pi);
            Assert.AreEqual(new Vector3(0, -10, 0), xRot180.Transform(new Vector3(0, 10, 0)));
            Assert.AreEqual(new Vector3(10, 0, 0), xRot180.Transform(new Vector3(10, 0, 0)));
        }

        [Test]
        public void MultipleXRotations()
        {
            // rotate 90 degrees
            var xRot90 = _identity.RotateX(Angle.HalfPi);
            var expected1 = new TransformationMatrix(1, 0, 0, 0,
                                                     0, 6.1232339E-17, -1, 0,
                                                     0, 1, 6.1232339E-17, 0,
                                                     0, 0, 0, 1);
            Assert.AreEqual(expected1, xRot90);

            // rotate back 90 degrees the other way
            var reverted = xRot90.RotateX(-Angle.HalfPi);
            Assert.AreEqual(_identity, reverted);

            // now rotate forward 180 degrees
            var m = reverted.RotateX(Angle.Pi);
            var expected3 = new TransformationMatrix(1, 0, 0, 0,
                                                     0, -1, -1.2246467E-16, 0,
                                                     0, 1.2246467E-16, -1, 0,
                                                     0, 0, 0, 1);
            Assert.AreEqual(expected3, m);

            // try applying this now
            Assert.AreEqual(new Vector3(0, -10, 0), m.Transform(new Vector3(0, 10, 0)));
            
            // do the same thing, in a slightly differet way
            var xRot90Plus90 = _identity.RotateX(Angle.HalfPi).RotateX(Angle.HalfPi);
            Assert.AreEqual(new Vector3(0, -10, 0), xRot90Plus90.Transform(new Vector3(0, 10, 0)));
            Assert.AreEqual(m, xRot90Plus90);
        }

        [Test]
        public void RotateY()
        {
            var yRot90 = _identity.RotateY(Angle.HalfPi);
            Assert.AreEqual(new Vector3(0, 0, -10), yRot90.Transform(new Vector3(10, 0, 0)));
            Assert.AreEqual(new Vector3(0, 10, 0), yRot90.Transform(new Vector3(0, 10, 0)));

            var yRot180 = _identity.RotateY(Angle.Pi);
            Assert.AreEqual(new Vector3(-10, 0, 0), yRot180.Transform(new Vector3(10, 0, 0)));
            Assert.AreEqual(new Vector3(0, 10, 0), yRot180.Transform(new Vector3(0, 10, 0)));
        }

        [Test]
        public void MultipleYRotations()
        {
            Assert.AreEqual(_identity.RotateY(Angle.HalfPi), _identity.RotateY(Angle.HalfPi).RotateY(-Angle.Pi).RotateY(Angle.Pi));
        }

        [Test]
        public void RotateZ()
        {
            var zRot90 = _identity.RotateZ(Angle.HalfPi);
            Assert.AreEqual(new Vector3(0, 10, 0), zRot90.Transform(new Vector3(10, 0, 0)));
            Assert.AreEqual(new Vector3(-10, 0, 0), zRot90.Transform(new Vector3(0, 10, 0)));
            Assert.AreEqual(new Vector3(0, 0, 10), zRot90.Transform(new Vector3(0, 0, 10)));
            
            var zRot180 = _identity.RotateZ(Angle.Pi);
            Assert.AreEqual(new Vector3(-10, 0, 0), zRot180.Transform(new Vector3(10, 0, 0)));
            Assert.AreEqual(new Vector3(0, -10, 0), zRot180.Transform(new Vector3(0, 10, 0)));
        }

        [Test]
        public void MultipleZRotations()
        {
            Assert.AreEqual(_identity.RotateZ(Angle.HalfPi), _identity.RotateZ(Angle.HalfPi).RotateZ(-Angle.Pi).RotateZ(Angle.Pi));
        }

        [Test]
        public void Translate()
        {
            Assert.AreEqual(new TransformationMatrix(1, 0, 0, 10,
                                                     0, 1, 0, 10,
                                                     0, 0, 1, 10,
                                                     0, 0, 0, 1),
                            _identity.Translate(10, 10, 10));

            Assert.AreEqual(new Vector3(10, 10, 10), _identity.Translate(10, 10, 10).Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(10, 10, 10), _identity.Translate(5, 5, 5).Transform(new Vector3(5, 5, 5)));
            Assert.AreEqual(new Vector3(11, 13, 15), _identity.Translate(1, 2, 3).Transform(new Vector3(10, 11, 12)));

            Assert.AreEqual(new Vector3(10, 10, 10), _identity.Translate(10, 10, 10).GetTranslation());
            Assert.AreEqual(new Vector3(1, 2, 3), _identity.Translate(1, 2, 3).GetTranslation());
        }

        [Test]
        public void TranslateThenRotate()
        {
            // translate origin 10 units along the x-axis, then rotate around y by 90 degrees
            var matrix = _identity.Translate(10, 0, 0).RotateY(Angle.HalfPi);
            Assert.AreEqual(new Vector3(0, 0, -10), matrix.GetTranslation());
            Assert.AreEqual(new Vector3(0, 0, -10), matrix.Transform(Vector3.Origin));
            Assert.AreEqual(new Vector3(1, 1, -11), matrix.Transform(new Vector3(1,1,1)));
        }

        [Test]
        public void GetDeterminant()
        {
            Assert.AreEqual(1, _identity.GetDeterminant());
            Assert.AreEqual(0, new TransformationMatrix(new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}).GetDeterminant());
            Assert.AreEqual(0, new TransformationMatrix(new double[] {1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4}).GetDeterminant());
            Assert.AreEqual(-6, new TransformationMatrix(new double[] {1, 1, 1, 1, 2, 1, 2, 2, 3, 3, 1, 3, 4, 4, 4, 1}).GetDeterminant());
            Assert.AreEqual(-3, new TransformationMatrix(new double[] {0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0}).GetDeterminant());
        }

/*        
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
                TransformationMatrix.Identity.RotateX(Angle.HalfPi).Invert());
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
*/

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("[   1    0    0    0]\n[   0    1    0    0]\n[   0    0    1    0]\n[   0    0    0    1]", _identity.ToString());
        }
    }
}