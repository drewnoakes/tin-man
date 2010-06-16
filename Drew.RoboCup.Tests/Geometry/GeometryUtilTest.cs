/*
 * Created by Drew, 01/06/2010 03:49.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup
{
    [TestFixture]
    public sealed class GeometryUtilTest {
        [Test] public void CalculateDistanceAlongLineThatIsClosestToPoint() {
            Assert.AreEqual(1, GeometryUtil.CalculateDistanceAlongLineThatIsClosestToPoint(
                Vector3.Origin, new Vector3(1,0,0), new Vector3(1,1,0)));
        }
    }
}
