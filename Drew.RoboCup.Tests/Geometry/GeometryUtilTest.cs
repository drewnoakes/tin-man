/*
 * Created by Drew, 01/06/2010 03:49.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup.Geometry
{
    [TestFixture]
    public class GeometryUtilTest {
        [Test] public void NormaliseDegrees() {
            Assert.AreEqual(0, GeometryUtil.NormaliseDegrees(0));
            Assert.AreEqual(0, GeometryUtil.NormaliseDegrees(360));
            Assert.AreEqual(0, GeometryUtil.NormaliseDegrees(720));
            Assert.AreEqual(0, GeometryUtil.NormaliseDegrees(-360));
            Assert.AreEqual(10, GeometryUtil.NormaliseDegrees(370));
            Assert.AreEqual(350, GeometryUtil.NormaliseDegrees(-10));
            Assert.AreEqual(359, GeometryUtil.NormaliseDegrees(359));
        }
        
        [Test] public void DegreesToRadians() {
            Assert.AreEqual(0, GeometryUtil.DegreesToRadians(0));
            Assert.AreEqual(Math.PI, GeometryUtil.DegreesToRadians(180));
            Assert.AreEqual(-Math.PI, GeometryUtil.DegreesToRadians(-180));
            Assert.AreEqual(Math.PI*2, GeometryUtil.DegreesToRadians(360));
        }
        
        [Test] public void RadiansToDegrees() {
            Assert.AreEqual(0, GeometryUtil.RadiansToDegrees(0));
            Assert.AreEqual(180, GeometryUtil.RadiansToDegrees(Math.PI));
            Assert.AreEqual(-180, GeometryUtil.RadiansToDegrees(-Math.PI));
            Assert.AreEqual(360, GeometryUtil.RadiansToDegrees(Math.PI*2));
        }
    }
}
