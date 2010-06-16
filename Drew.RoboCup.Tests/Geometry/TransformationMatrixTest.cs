/*
 * Created by Drew, 01/06/2010 03:47.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup.Geometry
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
            Assert.AreEqual(new Vector3(0,0,-10), TransformationMatrix.Identity.RotateX(Math.PI/2).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,-10,0), TransformationMatrix.Identity.RotateX(Math.PI).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(10,0,0), TransformationMatrix.Identity.RotateX(Math.PI/2).Transform(new Vector3(10,0,0)));
        }

        [Test] public void MultipleXRotations() {
            var m = TransformationMatrix.Identity.RotateX(Math.PI/2).RotateX(-Math.PI).RotateX(Math.PI);
            Assert.AreEqual(new Vector3(0,0,-10), m.Transform(new Vector3(0,10,0)));
        }

        [Test] public void RotateY() {
            Assert.AreEqual(new Vector3(-10,0,0), TransformationMatrix.Identity.RotateY(Math.PI/2).Transform(new Vector3(0,0,10)));
            Assert.AreEqual(new Vector3(-10,0,0), TransformationMatrix.Identity.RotateY(Math.PI).Transform(new Vector3(10,0,0)));
            Assert.AreEqual(new Vector3(0,10,0), TransformationMatrix.Identity.RotateY(Math.PI/2).Transform(new Vector3(0,10,0)));
        }

        [Test] public void MultipleYRotations() {
            var m = TransformationMatrix.Identity.RotateY(Math.PI/2).RotateY(-Math.PI).RotateY(Math.PI);
            Assert.AreEqual(new Vector3(-10,0,0), m.Transform(new Vector3(0,0,10)));
        }
        
        [Test] public void RotateZ() {
            Assert.AreEqual(new Vector3(10,0,0), TransformationMatrix.Identity.RotateZ(Math.PI/2).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,-10,0), TransformationMatrix.Identity.RotateZ(Math.PI).Transform(new Vector3(0,10,0)));
            Assert.AreEqual(new Vector3(0,0,10), TransformationMatrix.Identity.RotateZ(Math.PI/2).Transform(new Vector3(0,0,10)));
        }

        [Test] public void MultipleZRotations() {
            var m = TransformationMatrix.Identity.RotateZ(Math.PI/2).RotateZ(-Math.PI).RotateZ(Math.PI);
            Assert.AreEqual(new Vector3(10,0,0), m.Transform(new Vector3(0,10,0)));
        }
        
        [Test] public void TranslateThenRotate() {
            // translate origin 10 units down x-axis, then rotate around y by 90 degrees
//            var matrix = TransformationMatrix.Identity.Translate(10, 0, 0).RotateY(90);
            var matrix = TransformationMatrix.Identity.RotateY(90).Translate(10, 0, 0);
            Assert.AreEqual(new Vector3(0,0,10), matrix.Transform(Vector3.Origin));
        }
    }
}
