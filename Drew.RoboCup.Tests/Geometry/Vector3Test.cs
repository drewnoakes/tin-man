/*
 * Created by Drew, 01/06/2010 03:54.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup
{
    [TestFixture]
    public sealed class Vector3Test {
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
        
        [Test] public void GetCrossProduct() {
            // basic unit vectors, perpendicular
            Assert.AreEqual(new Vector3(0,0,1),  Vector3.GetCrossProduct(new Vector3(1,0,0), new Vector3(0,1,0)));
            Assert.AreEqual(new Vector3(0,0,1),  Vector3.GetCrossProduct(new Vector3(-1,0,0), new Vector3(0,-1,0)));
            Assert.AreEqual(new Vector3(0,0,-1), Vector3.GetCrossProduct(new Vector3(-1,0,0), new Vector3(0,1,0)));
        
            // non-unit vectors, perpendicular
            Assert.AreEqual(new Vector3(0,0,6),  Vector3.GetCrossProduct(new Vector3(2,0,0), new Vector3(0,3,0)));
            
            // non-perpendicular vectors
            Assert.AreEqual(new Vector3(0,0,1),  Vector3.GetCrossProduct(new Vector3(1,1,0), new Vector3(0,1,0)));
            Assert.AreEqual(new Vector3(0,0,1),  Vector3.GetCrossProduct(new Vector3(1,1,0), new Vector3(1,2,0)));
        }
        
        [Test] public void GetCrossProductForParallelVectors() {
            Assert.AreEqual(Vector3.Origin, Vector3.GetCrossProduct(new Vector3(1,0,0), new Vector3(1,0,0)));
            Assert.AreEqual(Vector3.Origin, Vector3.GetCrossProduct(new Vector3(1,2,0), new Vector3(1,2,0)));
        }
        
        [Test] public void GetCrossProductWithZeroLengthVector() {
            Assert.AreEqual(Vector3.Origin, Vector3.GetCrossProduct(new Vector3(0,0,0), new Vector3(1,0,0)));
            Assert.AreEqual(Vector3.Origin, Vector3.GetCrossProduct(new Vector3(1,0,0), new Vector3(0,0,0)));
        }
    }
}
