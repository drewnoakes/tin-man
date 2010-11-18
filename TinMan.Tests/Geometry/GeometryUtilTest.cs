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
// Created 01/06/2010 03:49

using NUnit.Framework;

namespace TinMan
{
    [TestFixture]
    public sealed class GeometryUtilTest {
        [Test] public void CalculateDistanceAlongLineThatIsClosestToPoint() {
            Assert.AreEqual(1, GeometryUtil.CalculateDistanceAlongLineThatIsClosestToPoint(
                Vector3.Origin, new Vector3(1,0,0), new Vector3(1,1,0)));
        }
    }
}
