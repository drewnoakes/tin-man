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
    public sealed class AngleTest
    {
        [Test]
        public void NormalisePositive()
        {
            Assert.AreEqual(0, Angle.FromDegrees(0).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(360).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(720).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(-360).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(1, Angle.FromDegrees(361).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(315, Angle.FromDegrees(315).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(359, Angle.FromDegrees(-1).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(1, Angle.FromDegrees(1).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(90, Angle.FromDegrees(90).NormalisePositive().Degrees, 0.00001);
            Assert.AreEqual(270, Angle.FromDegrees(-90).NormalisePositive().Degrees, 0.00001);
        }

        [Test]
        public void NormaliseBalanced()
        {
            Assert.AreEqual(0, Angle.FromDegrees(0).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(360).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(720).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(0, Angle.FromDegrees(-360).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(1, Angle.FromDegrees(361).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(-45, Angle.FromDegrees(315).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(-1, Angle.FromDegrees(-1).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(1, Angle.FromDegrees(1).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(90, Angle.FromDegrees(90).NormaliseBalanced().Degrees, 0.00001);
            Assert.AreEqual(-90, Angle.FromDegrees(-90).NormaliseBalanced().Degrees, 0.00001);
        }

        [Test]
        public void DegreesToRadians()
        {
            Assert.AreEqual(0, Angle.DegreesToRadians(0));
            Assert.AreEqual(Math.PI, Angle.DegreesToRadians(180));
            Assert.AreEqual(-Math.PI, Angle.DegreesToRadians(-180));
            Assert.AreEqual(Math.PI*2, Angle.DegreesToRadians(360));
        }

        [Test]
        public void RadiansToDegrees()
        {
            Assert.AreEqual(0, Angle.RadiansToDegrees(0));
            Assert.AreEqual(180, Angle.RadiansToDegrees(Math.PI));
            Assert.AreEqual(-180, Angle.RadiansToDegrees(-Math.PI));
            Assert.AreEqual(360, Angle.RadiansToDegrees(Math.PI*2));
        }

        [Test]
        public void ArithmeticOperators()
        {
            Assert.AreEqual(Angle.FromRadians(0), Angle.FromRadians(1) - Angle.FromRadians(1));
            Assert.AreEqual(Angle.FromRadians(2), Angle.FromRadians(1) + Angle.FromRadians(1));
            Assert.AreEqual(Angle.FromRadians(2), Angle.FromRadians(1) * 2);
            Assert.AreEqual(Angle.FromRadians(0.5), Angle.FromRadians(1) / 2);
            Assert.AreEqual(Angle.FromRadians(-1), -Angle.FromRadians(1));
        }

        [Test]
        public void ComparisonOperators()
        {
            // ReSharper disable EqualExpressionComparison

            Assert.IsTrue(Angle.FromRadians(1) > Angle.FromRadians(0));
            Assert.IsTrue(Angle.FromRadians(1) > Angle.FromRadians(-1));
            Assert.IsFalse(Angle.FromRadians(1) > Angle.FromRadians(2));

            Assert.IsTrue(Angle.FromRadians(1) >= Angle.FromRadians(1));
            Assert.IsTrue(Angle.FromRadians(2) >= Angle.FromRadians(1));
            Assert.IsFalse(Angle.FromRadians(1) >= Angle.FromRadians(2));

            Assert.IsTrue(Angle.FromRadians(0) < Angle.FromRadians(1));
            Assert.IsTrue(Angle.FromRadians(-1) < Angle.FromRadians(1));
            Assert.IsFalse(Angle.FromRadians(2) < Angle.FromRadians(1));

            Assert.IsTrue(Angle.FromRadians(1) <= Angle.FromRadians(1));
            Assert.IsTrue(Angle.FromRadians(1) <= Angle.FromRadians(2));
            Assert.IsFalse(Angle.FromRadians(2) <= Angle.FromRadians(1));

            Assert.IsTrue(Angle.FromRadians(1) == Angle.FromRadians(1));
            Assert.IsFalse(Angle.FromRadians(2) == Angle.FromRadians(1));

            Assert.IsFalse(Angle.FromRadians(1) != Angle.FromRadians(1));
            Assert.IsTrue(Angle.FromRadians(2) != Angle.FromRadians(1));

            // ReSharper restore EqualExpressionComparison
        }

        [Test]
        public void Equals()
        {
            Assert.AreEqual(Angle.NaN, Angle.NaN);
            Assert.AreEqual(Angle.Zero, Angle.Zero);
            Assert.AreEqual(Angle.FromDegrees(180), Angle.Pi);
            Assert.AreEqual(Angle.FromRadians(1), Angle.FromRadians(1.000001));
        }
    }
}