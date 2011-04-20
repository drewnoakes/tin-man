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
            Assert.AreEqual(Vector3.NaN, Vector3.NaN);
            Assert.AreEqual(new Vector3(1,2,3), new Vector3(1.000001,2.000001,3.00000001));
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
        
        [Test] public void Negation() {
            Assert.AreEqual(new Vector3(-10,-10,-10), -new Vector3(10,10,10));
        }
        
        [Test] public void MultiplicationOperator() {
            Assert.AreEqual(new Vector3(10,10,10), new Vector3(10,10,10) * 1);
            Assert.AreEqual(Vector3.Origin, new Vector3(10,10,10) * 0);
            Assert.AreEqual(Vector3.Origin, Vector3.Origin * 1);
            Assert.AreEqual(Vector3.Origin, Vector3.Origin * 10);
            Assert.AreEqual(Vector3.Origin, Vector3.Origin * 0);
            Assert.AreEqual(new Vector3(0,0,10), new Vector3(0,0,5) * 2);
            Assert.AreEqual(new Vector3(2,2,2), new Vector3(1,1,1) * 2);
            Assert.AreEqual(Vector3.NaN, new Vector3(1,1,1) * double.NaN);
        }
        
        [Test] public void DivisionOperator() {
            Assert.AreEqual(new Vector3(10,10,10), new Vector3(10,10,10) / 1);
            Assert.AreEqual(Vector3.Origin, Vector3.Origin / 1);
            Assert.AreEqual(Vector3.Origin, Vector3.Origin / 10);
            Assert.AreEqual(new Vector3(0,0,5), new Vector3(0,0,10) / 2);
            Assert.AreEqual(new Vector3(1,1,1), new Vector3(2,2,2) / 2);
            Assert.AreEqual(Vector3.NaN, new Vector3(1,1,1) / double.NaN);
        }
        
    	[Test, ExpectedException(typeof(DivideByZeroException))]
    	public void DivisionOperator_DivideByZero() {
    		(new Vector3(10,10,10) / 0).ToString();
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
            Assert.AreEqual(new Vector3(0,0,1),  new Vector3(1,0,0).Cross(new Vector3(0,1,0)));
            Assert.AreEqual(new Vector3(0,0,1),  new Vector3(-1,0,0).Cross(new Vector3(0,-1,0)));
            Assert.AreEqual(new Vector3(0,0,-1), new Vector3(-1,0,0).Cross(new Vector3(0,1,0)));
        
            // non-unit vectors, perpendicular
            Assert.AreEqual(new Vector3(0,0,6),  new Vector3(2,0,0).Cross(new Vector3(0,3,0)));
            
            // non-perpendicular vectors
            Assert.AreEqual(new Vector3(0,0,1),  new Vector3(1,1,0).Cross(new Vector3(0,1,0)));
            Assert.AreEqual(new Vector3(0,0,1),  new Vector3(1,1,0).Cross(new Vector3(1,2,0)));
        }
        
        [Test] public void GetCrossProductForParallelVectors() {
            Assert.AreEqual(Vector3.Origin, new Vector3(1,0,0).Cross(new Vector3(1,0,0)));
            Assert.AreEqual(Vector3.Origin, new Vector3(1,2,0).Cross(new Vector3(1,2,0)));
        }
        
        [Test] public void GetCrossProductWithZeroLengthVector() {
            Assert.AreEqual(Vector3.Origin, new Vector3(0,0,0).Cross(new Vector3(1,0,0)));
            Assert.AreEqual(Vector3.Origin, new Vector3(1,0,0).Cross(new Vector3(0,0,0)));
        }
        
        [Test] public void Abs() {
    		Assert.AreEqual(Vector3.Origin, Vector3.Origin.Abs());
    		Assert.AreEqual(Vector3.NaN, Vector3.NaN.Abs());
    		Assert.AreEqual(new Vector3(1,2,3), new Vector3(-1,-2,-3).Abs());
        }
        
        [Test] public void AngleTo() {
    		Assert.AreEqual(Angle.NaN, Vector3.Origin.AngleTo(Vector3.Origin));
    		Assert.AreEqual(Angle.Zero, Vector3.UnitX.AngleTo(Vector3.UnitX));
    		Assert.AreEqual(Angle.FromDegrees(90), Vector3.UnitX.AngleTo(Vector3.UnitY));
    		Assert.AreEqual(Angle.FromDegrees(90), Vector3.UnitX.AngleTo(Vector3.UnitZ));
    		Assert.AreEqual(Angle.FromDegrees(90), Vector3.UnitY.AngleTo(Vector3.UnitZ));
    		
    		Assert.AreEqual(Angle.FromDegrees(180), Vector3.UnitX.AngleTo(-Vector3.UnitX));
        }
    }
}
