/*
 * Created by Drew, 01/06/2010 03:54.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup.Geometry
{
    [TestFixture]
    public class VectorTest {
        [Test] public void Origin() {
            Assert.AreEqual(0, Vector3.Origin.X);
            Assert.AreEqual(0, Vector3.Origin.Y);
            Assert.AreEqual(0, Vector3.Origin.Z);
        }
        
        [Test] public void Equality() {
            Assert.AreEqual(new Vector3(1,2,3), new Vector3(1,2,3));
            Assert.AreEqual(Vector3.Origin, Vector3.Origin);
        }
        
        [Test] public void AdditionOperator() {
            Assert.AreEqual(new Vector3(0,0,10), Vector3.Origin + new Vector3(0,0,10));
            Assert.AreEqual(new Vector3(10,10,20), new Vector3(0,0,10) + new Vector3(10,10,10));
            Assert.AreEqual(new Vector3(0,0,0), new Vector3(0,0,10) + new Vector3(0,0,-10));
        }
        
        [Test] public void SubtractionOperator() {
            Assert.AreEqual(new Vector3(10,10,0), new Vector3(10,10,10) - new Vector3(0,0,10));
            Assert.AreEqual(new Vector3(0,0,10), Vector3.Origin - new Vector3(0,0,-10));
            Assert.AreEqual(new Vector3(0,0,0), new Vector3(0,0,10) - new Vector3(0,0,10));
        }
        
        [Test] public void Normalise() {
            Assert.AreEqual(0, Vector3.Origin.X);
            Assert.AreEqual(0, Vector3.Origin.Y);
            Assert.AreEqual(0, Vector3.Origin.Z);
        }
        
        [Test] public void GetLength() {
            Assert.AreEqual(0, Vector3.Origin.GetLength());
            Assert.AreEqual(5, new Vector3(3,4,0).GetLength());
            Assert.AreEqual(10, new Vector3(10,0,0).GetLength());
            Assert.AreEqual(10, new Vector3(0,10,0).GetLength());
        }
    }
}
