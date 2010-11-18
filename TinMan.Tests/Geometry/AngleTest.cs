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
// Created 06/06/2010 23:45

using System;
using NUnit.Framework;

namespace TinMan
{
    [TestFixture]
    public sealed class AngleTest {
        [Test] public void NormaliseDegrees() {
            Assert.AreEqual(0, Angle.FromRadians(0).Normalise().Radians);
            Assert.AreEqual(0, Angle.FromRadians(2*Math.PI).Normalise().Radians);
            Assert.AreEqual(0, Angle.FromRadians(4*Math.PI).Normalise().Radians);
            Assert.AreEqual(0, Angle.FromRadians(-2*Math.PI).Normalise().Radians);
            Assert.AreEqual(1, Angle.FromRadians(2*Math.PI+1).Normalise().Radians);
            Assert.AreEqual(2*Math.PI-1, Angle.FromRadians(-1).Normalise().Radians);
            Assert.AreEqual(2, Angle.FromRadians(2).Normalise().Radians);
        }
        
        [Test] public void DegreesToRadians() {
            Assert.AreEqual(0, Angle.DegreesToRadians(0));
            Assert.AreEqual(Math.PI, Angle.DegreesToRadians(180));
            Assert.AreEqual(-Math.PI, Angle.DegreesToRadians(-180));
            Assert.AreEqual(Math.PI*2, Angle.DegreesToRadians(360));
        }
        
        [Test] public void RadiansToDegrees() {
            Assert.AreEqual(0, Angle.RadiansToDegrees(0));
            Assert.AreEqual(180, Angle.RadiansToDegrees(Math.PI));
            Assert.AreEqual(-180, Angle.RadiansToDegrees(-Math.PI));
            Assert.AreEqual(360, Angle.RadiansToDegrees(Math.PI*2));
        }
    }
}
