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
// Created 01/06/2010 03:54

using System;
using NUnit.Framework;

namespace TinMan
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
            Assert.AreEqual(0, Vector3.Origin.Length);
            Assert.AreEqual(5, new Vector3(3,4,0).Length);
            Assert.AreEqual(10, new Vector3(10,0,0).Length);
            Assert.AreEqual(10, new Vector3(0,10,0).Length);
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
