/*
 * Created by Drew, 07/06/2010 00:07.
 */

using System;
using NUnit.Framework;

namespace Drew.RoboCup.Geometry
{
    [TestFixture]
    public sealed class PolarTest
    {
        [Test] public void Constructor() {
            var p = new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3));
            Assert.AreEqual(1, p.Distance);
            Assert.AreEqual(Angle.FromRadians(2), p.Theta);
            Assert.AreEqual(Angle.FromRadians(3), p.Phi);
        }
        
        [Test] public void ToVector3() {
            var p = new Polar(1, 
                              Angle.FromRadians(Math.PI), 
                              Angle.FromRadians(Math.PI/2d));
            Assert.AreEqual(new Vector3(0,0,1), p.ToVector3());
            
            p = new Polar(1,
                          Angle.FromRadians(Math.PI), 
                          Angle.Zero);
            Assert.AreEqual(new Vector3(-1,0,0), p.ToVector3());
        }
        
        [Test] public void IsZero() {
            Assert.IsTrue(Polar.Zero.IsZero);
            Assert.IsTrue(new Polar(0, Angle.FromRadians(0), Angle.FromRadians(0)).IsZero);
            Assert.IsFalse(new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)).IsZero);
        }
        
        [Test] public void Equality() {
            Assert.AreEqual(new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)),
                            new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)));
            Assert.AreNotEqual(new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)),
                               new Polar(2, Angle.FromRadians(2), Angle.FromRadians(3)));
            Assert.AreNotEqual(new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)),
                               new Polar(1, Angle.FromRadians(3), Angle.FromRadians(3)));
            Assert.AreNotEqual(new Polar(1, Angle.FromRadians(2), Angle.FromRadians(3)),
                               new Polar(1, Angle.FromRadians(2), Angle.FromRadians(4)));
        }
    }
}
