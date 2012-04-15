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
// Created 06/05/2010 23:29

using System;
using System.Linq;
using NUnit.Framework;
using TinMan.PerceptorParsing;

namespace TinMan
{
    [TestFixture]
    public sealed class PerceptorParserTest
    {
        private static Parser Parse(string s)
        {
            var parser = new Parser(new Scanner(new StringBuffer(s)));
            parser.Parse();
            return parser;
        }

        [Test]
        public void ShouldParseLongStringWithoutError()
        {
            var strings = new[]
                              {
                                  "(time (now 417.65))(GS (t 0.00) (pm BeforeKickOff))(GYR (n torso) (rt 0.00 0.00 0.00))(ACC (n torso) (a 0.00 0.00 9.81))(HJ (n hj1) (ax -0.00))(HJ (n hj2) (ax -0.00))(HJ (n raj1) (ax -0.00))(HJ (n raj2) (ax -0.00))(HJ (n raj3) (ax -0.00))(HJ (n raj4) (ax -0.00))(HJ (n laj1) (ax -0.00))(HJ (n laj2) (ax -0.00))(HJ (n laj3) (ax -0.00))(HJ (n laj4) (ax -0.00))(HJ (n rlj1) (ax -0.00))(HJ (n rlj2) (ax -0.00))(HJ (n rlj3) (ax -0.00))(HJ (n rlj4) (ax -0.00))(HJ (n rlj5) (ax -0.00))(HJ (n rlj6) (ax -0.00))(HJ (n llj1) (ax -0.00))(HJ (n llj2) (ax -0.00))(HJ (n llj3) (ax -0.00))(HJ (n llj4) (ax -0.00))(HJ (n llj5) (ax -0.00))(HJ (n llj6) (ax -0.00))",
                                  "(time (now 203.83))(GS (t 0.02) (pm KickOff_Left))(GYR (n torso) (rt 15.33 -21.37 -97.40))(ACC (n torso) (a 2.13 8.56 0.00))(HJ (n hj1) (ax -120.00))(HJ (n hj2) (ax -45.00))(HJ (n raj1) (ax -120.01))(HJ (n raj2) (ax -95.04))(HJ (n raj3) (ax -120.00))(HJ (n raj4) (ax 1.49))(HJ (n laj1) (ax -124.53))(HJ (n laj2) (ax -7.27))(HJ (n laj3) (ax -120.03))(HJ (n laj4) (ax -90.01))(HJ (n rlj1) (ax -95.88))(HJ (n rlj2) (ax -44.98))(HJ (n rlj3) (ax -24.97))(HJ (n rlj4) (ax -133.55))(HJ (n rlj5) (ax -44.97))(FRP (n rf) (c 0.06 0.08 -0.02) (f 15.81 6.81 24.96))(HJ (n rlj6) (ax -28.14))(HJ (n llj1) (ax -86.22))(HJ (n llj2) (ax -21.97))(HJ (n llj3) (ax -20.53))(HJ (n llj4) (ax -127.43))(HJ (n llj5) (ax -52.01))(FRP (n lf) (c -0.04 0.08 -0.01) (f 26.77 5.28 18.93))(HJ (n llj6) (ax -45.00))"
                              };

            foreach (var s in strings)
            {
                var parser = Parse(s);
                Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            }
        }

        [Test, SetCulture("de-DE")]
        public void CanParseForCultureWithCommaDecimalSeparator()
        {
            // In some cultures 1.234 is the English equivalent of 1,234.00.
            // This test sets the culture to de-DE to ensure that any parsing of float values
            // from the server (which always use periods for decimal separators) result in
            // the intended values.
            const string s = "(time (now 417.65))(GS (t 0.00) (pm BeforeKickOff))(GYR (n torso) (rt 0.00 0.00 0.00))(ACC (n torso) (a 0.00 0.00 9.81))(HJ (n hj1) (ax -0.00))(HJ (n hj2) (ax -0.00))(HJ (n raj1) (ax -0.00))(HJ (n raj2) (ax -0.00))(HJ (n raj3) (ax -0.00))(HJ (n raj4) (ax -0.00))(HJ (n laj1) (ax -0.00))(HJ (n laj2) (ax -0.00))(HJ (n laj3) (ax -0.00))(HJ (n laj4) (ax -0.00))(HJ (n rlj1) (ax -0.00))(HJ (n rlj2) (ax -0.00))(HJ (n rlj3) (ax -0.00))(HJ (n rlj4) (ax -0.00))(HJ (n rlj5) (ax -0.00))(HJ (n rlj6) (ax -0.00))(HJ (n llj1) (ax -0.00))(HJ (n llj2) (ax -0.00))(HJ (n llj3) (ax -0.00))(HJ (n llj4) (ax -0.00))(HJ (n llj5) (ax -0.00))(HJ (n llj6) (ax -0.00))"; //

            var parser = Parse(s);
            Assert.AreEqual(417.65, parser.State.SimulationTime.TotalSeconds, 0.001);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
        }

        [Test]
        public void ShouldParseSimulationTime()
        {
            var parser = Parse("(time (now 417.65))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(TimeSpan.FromSeconds(417.65), parser.State.SimulationTime);
        }

        [Test]
        public void ShouldParsePlayMode()
        {
            var parser = Parse("(GS (t 0.00) (pm KickOff_Left))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(PlayMode.KickOffLeft, parser.State.PlayMode);
        }

        [Test]
        public void ShouldParseGameTime()
        {
            var parser = Parse("(GS (t 3.14) (pm PlayOn))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(TimeSpan.FromSeconds(3.14), parser.State.GameTime);
        }

        [Test]
        public void ShouldParseGyro()
        {
            var parser = Parse("(GYR (n torso) (rt 1.23 2.34 3.45))");
            Assert.IsNotNull(parser.State.GyroStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.GyroStates.Single();
            Assert.AreEqual("torso", state.Label);
            Assert.AreEqual(1.23, state.XOrientation, 0.0001);
            Assert.AreEqual(2.34, state.YOrientation, 0.0001);
            Assert.AreEqual(3.45, state.ZOrientation, 0.0001);
        }

        [Test]
        public void ShouldParseAccelerometer()
        {
            var parser = Parse("(ACC (n torso) (a 0.00 -0.05 8.83))");
            Assert.IsNotNull(parser.State.AccelerometerStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.AccelerometerStates.Single();
            Assert.AreEqual("torso", state.Label);
            Assert.AreEqual(0.00, state.AccelerationVector.X, 0.0001);
            Assert.AreEqual(-0.05, state.AccelerationVector.Y, 0.0001);
            Assert.AreEqual(8.83, state.AccelerationVector.Z, 0.0001);
        }

        [Test]
        public void ShouldParseHingeJoint()
        {
            var parser = Parse("(HJ (n hj1) (ax 1.5))");
            Assert.IsNotNull(parser.State.HingeStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.HingeStates.Single();
            Assert.AreEqual("hj1", state.Label);
            Assert.AreEqual(Angle.FromDegrees(1.5), state.Angle);
        }

        [Test]
        public void ShouldParseUniversalJoint()
        {
            var parser = Parse("(UJ (n laj1) (ax1 -1.50) (ax2 2.00))");
            Assert.IsNotNull(parser.State.UniversalJointStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.UniversalJointStates.Single();
            Assert.AreEqual("laj1", state.Label);
            Assert.AreEqual(Angle.FromDegrees(-1.5), state.Angle1);
            Assert.AreEqual(Angle.FromDegrees(2.00), state.Angle2);
        }

        [Test]
        public void ShouldParseTouch()
        {
            var parser = Parse("(TCH n bumper val 1)");
            Assert.IsNotNull(parser.State.TouchStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.TouchStates.Single();
            Assert.AreEqual("bumper", state.Label);
            Assert.IsTrue(state.IsTouching);
        }

        [Test]
        public void ShouldParseForce()
        {
            var parser = Parse("(FRP (n lf) (c -0.14 0.08 -0.05) (f 1.12 -0.26 13.07))");
            Assert.IsNotNull(parser.State.ForceStates);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.ForceStates.Single();
            Assert.AreEqual("lf", state.Label);
            Assert.AreEqual(-0.14, state.PointOnBody.X, 0.0001);
            Assert.AreEqual(0.08, state.PointOnBody.Y, 0.0001);
            Assert.AreEqual(-0.05, state.PointOnBody.Z, 0.0001);
            Assert.AreEqual(1.12, state.ForceVector.X, 0.0001);
            Assert.AreEqual(-0.26, state.ForceVector.Y, 0.0001);
            Assert.AreEqual(13.07, state.ForceVector.Z, 0.0001);
        }

        [Test]
        public void ShouldParseSeenFlag()
        {
            var parser = Parse("(See (F2L (pol 11.52 52.50 -8.10)))");
            Assert.IsNotNull(parser.State.LandmarkPositions);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.LandmarkPositions.Single();
            Assert.AreEqual(Landmark.FlagLeftBottom, state.Landmark);
            Assert.AreEqual(11.52, state.PolarPosition.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.PolarPosition.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.PolarPosition.Phi);
        }

        [Test]
        public void ShouldParseSeenGoal()
        {
            var parser = Parse("(See (G2R (pol 11.52 52.50 -8.10)))");
            Assert.IsNotNull(parser.State.LandmarkPositions);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.LandmarkPositions.Single();
            Assert.AreEqual(Landmark.GoalRightBottom, state.Landmark);
            Assert.AreEqual(11.52, state.PolarPosition.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.PolarPosition.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.PolarPosition.Phi);
        }

        [Test]
        public void ShouldParseSeenBall()
        {
            var parser = Parse("(See (B (pol 11.52 52.50 -8.10)))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.IsNotNull(parser.State.BallPosition);
            var state = parser.State.BallPosition.Value;
            Assert.AreEqual(11.52, state.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.Phi);
        }

        [Test]
        public void ShouldParseMyPosInSeeMessage()
        {
            var parser = Parse("(See (mypos -8.20 4.00 0.54))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.IsNotNull(parser.State.AgentPosition);
            var state = parser.State.AgentPosition.Value;
            Assert.AreEqual(new Vector3(-8.2, 4, 0.54), state);
        }

        [Test]
        public void ShouldParseSeenPlayer()
        {
            // Have seen both of these in wireshark...
            // (See (P (team NaoRobot) (id 1) (rlowerarm (pol 0.19 -34.82 -20.73)) (llowerarm (pol 0.19 33.50 -21.28)))
            // (See (P (team NaoRobot) (id 1) (head (pol 9.04 -57.66 -28.25)) 
            //                                (rlowerarm (pol 8.93 -57.84 -29.70)) 
            //                                (llowerarm (pol 9.07 -59.44 -28.47))
            //                                (rfoot (pol 9.01 -59.41 -30.45))))
            var parser = Parse("(See (P (team NaoRobot) (id 2) (head (pol 9.04 -57.66-28.25)) (rlowerarm (pol 8.93 -57.84 -29.70)) (llowerarm (pol 9.07 -59.44 -28.47)) (rfoot (pol 9.01 -59.41 -30.45))))");
            Assert.IsNotNull(parser.State.OppositionPositions);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.OppositionPositions.Single();
            Assert.IsFalse(state.IsTeamMate);
            Assert.AreEqual(2, state.PlayerId);
            Assert.AreEqual(4, state.PartPositions.Count());
            var components = state.PartPositions.ToList();
            Assert.AreEqual("head", components[0].Label);
            Assert.AreEqual(9.04, components[0].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-57.66, components[0].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-28.25, components[0].PolarPosition.Phi.Degrees, 0.0001);
            Assert.AreEqual("rlowerarm", components[1].Label);
            Assert.AreEqual(8.93, components[1].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-57.84, components[1].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-29.70, components[1].PolarPosition.Phi.Degrees, 0.0001);
            Assert.AreEqual("llowerarm", components[2].Label);
            Assert.AreEqual(9.07, components[2].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-59.44, components[2].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-28.47, components[2].PolarPosition.Phi.Degrees, 0.0001);
            Assert.AreEqual("rfoot", components[3].Label);
            Assert.AreEqual(9.01, components[3].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-59.41, components[3].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-30.45, components[3].PolarPosition.Phi.Degrees, 0.0001);
        }

        [Test]
        public void ShouldParseSeenPlayer2()
        {
            // In this example the opponent's team name contains a colon, which caused a problem in version 0.5.5
            var parser = Parse("(time (now 121.63))(GS (t 56.92) (pm PlayOn))(hear 56.92 126.43 BPSn1d4S2d5)(GYR (n torso) (rt 11.32 -0.06 -0.00))(ACC (n torso) (a 0.02 -2.55 9.13))(HJ (n hj1) (ax -2.06))(HJ (n hj2) (ax 0.00))" +
                               "(See (G1L (pol 12.50 26.94 1.94)) (G2L (pol 11.41 35.80 2.21)) (F1L (pol 16.74 10.45 -0.82)) (B (pol 8.96 -20.21 -2.25)) " +
                               "(P (team Nexus3:D) (id 9) (head (pol 8.83 -24.68 0.31)) (rlowerarm (pol 9.00 -23.94 -0.69)) (llowerarm (pol 8.70 -23.93 -0.71)) (rfoot (pol 8.91 -24.07 -2.45)) (lfoot (pol 8.81 -24.02 -2.49))) " +
                               "(P (team Nexus3:D) (id 6) (head (pol 7.69 -53.52 0.17)) (rlowerarm (pol 7.80 -52.88 -1.35)) (llowerarm (pol 7.56 -53.94 -1.39)) (rfoot (pol 7.76 -53.52 -3.11)) (lfoot (pol 7.66 -53.59 -3.25))) " +
                               "(P (team KarachiKoalas) (id 5) (head (pol 13.22 13.22 0.95)) (rlowerarm (pol 13.04 12.83 0.48)) (llowerarm (pol 13.22 12.61 0.76)) (rfoot (pol 13.16 13.14 -1.45)) (lfoot (pol 13.25 12.82 -1.44))) " +
                               "(P (team Nexus3:D) (id 7) (head (pol 5.11 -56.81 -0.06)) (rlowerarm (pol 4.99 -57.59 -2.20)) (llowerarm (pol 5.26 -56.18 -2.12)) (rfoot (pol 5.07 -56.82 -4.95)) (lfoot (pol 5.18 -56.70 -5.16))) " +
                               "(P (team KarachiKoalas) (id 3) (head (pol 15.20 7.54 0.84)) (rlowerarm (pol 15.01 7.59 0.32)) (llowerarm (pol 15.18 6.82 0.45)) (rfoot (pol 15.07 7.08 -1.38)) (lfoot (pol 15.17 6.99 -1.11))) " +
                               "(P (team KarachiKoalas) (id 7) (head (pol 10.60 9.44 0.46)) (rlowerarm (pol 10.47 10.02 -0.21)) (llowerarm (pol 10.65 8.97 -0.03)) (rfoot (pol 10.62 9.85 -2.02)) (lfoot (pol 10.68 9.36 -1.93))) " +
                               "(P (team KarachiKoalas) (id 2) (head (pol 8.52 59.22 0.14)) (rlowerarm (pol 8.33 58.94 -0.65)) (llowerarm (pol 8.58 58.01 -0.48)) (rfoot (pol 8.43 58.68 -3.33)) (lfoot (pol 8.56 58.44 -3.06))) " +
                               "(P (team KarachiKoalas) (id 8) (head (pol 9.90 8.89 0.67)) (rlowerarm (pol 9.73 8.74 -0.59)) (llowerarm (pol 9.93 7.72 -0.37)) (rfoot (pol 9.90 8.91 -2.37)) (lfoot (pol 9.94 8.22 -2.27))) " +
                               "(P (team KarachiKoalas) (id 9) (head (pol 9.41 12.15 0.46)) (rlowerarm (pol 9.29 12.73 -0.18)) (llowerarm (pol 9.50 11.59 -0.15)) (rfoot (pol 9.45 12.79 -2.30)) (lfoot (pol 9.53 12.42 -1.88))) " +
                               "(P (team Nexus3:D) (id 8) (head (pol 6.95 -40.63 0.05)) (rlowerarm (pol 7.10 -40.59 -1.66)) (llowerarm (pol 6.81 -40.33 -1.51)) (rfoot (pol 7.01 -40.33 -3.62)) (lfoot (pol 6.91 -40.76 -3.53))) " +
                               "(P (team KarachiKoalas) (id 4) (head (pol 14.19 9.97 0.67)) (rlowerarm (pol 14.03 9.76 0.42)) (llowerarm (pol 14.33 9.35 0.46)) (rfoot (pol 14.07 9.47 -1.15)) (lfoot (pol 14.18 9.76 -1.25))) " +
                               "(P (team KarachiKoalas) (id 1) (head (pol 9.29 59.79 0.28)) (llowerarm (pol 9.17 59.35 0.04)) (lfoot (pol 9.29 59.52 -2.84))) " +
                               "(P (team KarachiKoalas) (id 6) (rlowerarm (pol 0.19 -33.84 -21.11)) (llowerarm (pol 0.19 36.34 -21.55))) " +
                               "(L (pol 0.93 -59.86 -35.02) (pol 13.35 -28.61 -1.48)) (L (pol 10.10 59.79 -2.85) (pol 16.74 10.37 -0.89)) (L (pol 15.95 -60.03 -1.41) (pol 16.73 10.28 -0.96)) (L (pol 11.74 18.30 -1.69) (pol 9.39 35.34 -2.53)) (L (pol 11.72 18.08 -1.86) (pol 13.06 23.77 -1.56)) (L (pol 9.39 35.36 -2.52) (pol 11.00 39.50 -2.29)) (L (pol 6.74 -45.90 -3.77) (pol 7.65 -40.93 -3.25)) (L (pol 7.66 -40.79 -3.13) (pol 8.13 -33.68 -3.00)) (L (pol 8.12 -33.77 -3.06) (pol 8.06 -25.88 -3.03)) (L (pol 8.07 -25.72 -2.90) (pol 7.48 -18.89 -3.31)) (L (pol 7.49 -18.65 -3.11) (pol 6.51 -14.48 -3.83)) (L (pol 6.50 -14.61 -3.94) (pol 5.41 -15.62 -4.78)) (L (pol 5.41 -15.54 -4.71) (pol 4.66 -25.07 -5.79)) (L (pol 4.67 -24.76 -5.53) (pol 4.76 -38.61 -5.71)) (L (pol 4.76 -38.47 -5.60) (pol 5.64 -46.05 -4.60)) (L (pol 5.64 -46.08 -4.63) (pol 6.74 -45.93 -3.80))" +
                               ")" +
                               "(HJ (n raj1) (ax 0.94))(HJ (n raj2) (ax -0.79))(HJ (n raj3) (ax -0.58))(HJ (n raj4) (ax -0.03))(HJ (n laj1) (ax 0.93))(HJ (n laj2) (ax 0.79))(HJ (n laj3) (ax 0.57))(HJ (n laj4) (ax -0.05))(HJ (n rlj1) (ax -0.17))(HJ (n rlj2) (ax 0.28))(HJ (n rlj3) (ax -0.00))(HJ (n rlj4) (ax -0.02))(HJ (n rlj5) (ax 0.12))(FRP (n rf) (c 0.03 0.08 -0.02) (f 0.06 -4.59 22.50))(HJ (n rlj6) (ax -0.35))(HJ (n llj1) (ax 0.37))(HJ (n llj2) (ax -0.27))(HJ (n llj3) (ax -0.00))(HJ (n llj4) (ax 0.40))(HJ (n llj5) (ax 0.12))(FRP (n lf) (c -0.03 0.08 -0.02) (f 0.17 -4.60 20.18))(HJ (n llj6) (ax -0.34))");
            Assert.IsNotNull(parser.State.OppositionPositions);
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.OppositionPositions.First();
            Assert.IsFalse(state.IsTeamMate);
            Assert.AreEqual(9, state.PlayerId);
            Assert.AreEqual(5, state.PartPositions.Count());
//            var components = state.PartPositions.ToList();
//            Assert.AreEqual("head", components[0].Label);
//            Assert.AreEqual(9.04,   components[0].PolarPosition.Distance, 0.0001);
//            Assert.AreEqual(-57.66, components[0].PolarPosition.Theta.Degrees, 0.0001);
//            Assert.AreEqual(-28.25, components[0].PolarPosition.Phi.Degrees, 0.0001);
//            Assert.AreEqual("rlowerarm", components[1].Label);
//            Assert.AreEqual(8.93,   components[1].PolarPosition.Distance, 0.0001);
//            Assert.AreEqual(-57.84, components[1].PolarPosition.Theta.Degrees, 0.0001);
//            Assert.AreEqual(-29.70, components[1].PolarPosition.Phi.Degrees, 0.0001);
//            Assert.AreEqual("llowerarm", components[2].Label);
//            Assert.AreEqual(9.07,   components[2].PolarPosition.Distance, 0.0001);
//            Assert.AreEqual(-59.44, components[2].PolarPosition.Theta.Degrees, 0.0001);
//            Assert.AreEqual(-28.47, components[2].PolarPosition.Phi.Degrees, 0.0001);
//            Assert.AreEqual("rfoot", components[3].Label);
//            Assert.AreEqual(9.01,   components[3].PolarPosition.Distance, 0.0001);
//            Assert.AreEqual(-59.41, components[3].PolarPosition.Theta.Degrees, 0.0001);
//            Assert.AreEqual(-30.45, components[3].PolarPosition.Phi.Degrees, 0.0001);
        }

        [Test]
        public void ShouldParseAgentState()
        {
            var parser = Parse("(AgentState (temp 4) (battery 75))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(4, parser.State.AgentTemperature);
            Assert.AreEqual(75, parser.State.AgentBattery);
        }

        [Test]
        public void ShouldParseMessageFromSelf()
        {
            // (hear 0.00 self ComeOn!)
            var parser = Parse("(hear 12.3 self helloworld)");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var message = parser.State.HeardMessages.Single();
            Assert.IsTrue(message.IsFromSelf);
            Assert.AreEqual("helloworld", message.Text);
            Assert.AreEqual(TimeSpan.FromSeconds(12.3), message.HeardAtTime);
        }

        [Test]
        public void ShouldParseMessageFromDirection()
        {
            var parser = Parse("(hear 12.3 12.34 overhere)");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var message = parser.State.HeardMessages.Single();
            Assert.IsFalse(message.IsFromSelf);
            Assert.AreEqual("overhere", message.Text);
            Assert.AreEqual(Angle.FromDegrees(12.34), message.RelativeDirection);
            Assert.AreEqual(TimeSpan.FromSeconds(12.3), message.HeardAtTime);
        }

        [Test]
        public void ShouldParseMessageWithMiscCharacters()
        {
            var parser = Parse("(hear 6.04 -2.16 ]0.uDDI0jOB6wfC6Fucr)");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var message = parser.State.HeardMessages.Single();
            Assert.IsFalse(message.IsFromSelf);
            Assert.AreEqual("]0.uDDI0jOB6wfC6Fucr", message.Text);
            Assert.AreEqual(Angle.FromDegrees(-2.16), message.RelativeDirection);
            Assert.AreEqual(TimeSpan.FromSeconds(6.04), message.HeardAtTime);
        }

        [Test]
        public void ShouldParseMessageWithMiscCharacters2()
        {
            var parser = Parse("(time (now 61.80))(GS (t 0.00) (pm BeforeKickOff))(hear 0.00 -1.16 L0,SD1HBH)(GYR (n torso) (rt -0.00 -0.00 0.00))(ACC (n torso) (a -0.00 -0.00 0.01))(HJ (n hj1) (ax -0.00))(HJ (n hj2) (ax -19.45))(HJ (n raj1) (ax -27.82))(HJ (n raj2) (ax -44.97))(HJ (n raj3) (ax 0.00))(HJ (n raj4) (ax 67.91))(HJ (n laj1) (ax -27.82))(HJ (n laj2) (ax 44.97))(HJ (n laj3) (ax -0.00))(HJ (n laj4) (ax -67.91))(HJ (n rlj1) (ax -10.59))(HJ (n rlj2) (ax -1.34))(HJ (n rlj3) (ax 47.88))(HJ (n rlj4) (ax -89.63))(HJ (n rlj5) (ax 45.54))(HJ (n rlj6) (ax 0.00))(HJ (n llj1) (ax -10.58))(HJ (n llj2) (ax 1.34))(HJ (n llj3) (ax 47.88))(HJ (n llj4) (ax -89.63))(HJ (n llj5) (ax 45.54))(HJ (n llj6) (ax 0.00))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var message = parser.State.HeardMessages.Single();
            Assert.IsFalse(message.IsFromSelf);
            Assert.AreEqual("L0,SD1HBH", message.Text);
            Assert.AreEqual(TimeSpan.FromSeconds(0), message.HeardAtTime);
            Assert.AreEqual(Angle.FromDegrees(-1.16), message.RelativeDirection);
        }

        [Test]
        public void ShouldParseMessageWithMiscCharacters3()
        {
            var parser = Parse("(hear 56.98 76.22 ??!?h?y?????jH?????_)");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var message = parser.State.HeardMessages.Single();
            Assert.IsFalse(message.IsFromSelf);
            Assert.AreEqual("??!?h?y?????jH?????_", message.Text);
            Assert.AreEqual(TimeSpan.FromSeconds(56.98), message.HeardAtTime);
            Assert.AreEqual(Angle.FromDegrees(76.22), message.RelativeDirection);
        }

        [Test]
        public void ShouldParseMessageWithMiscCharacters4()
        {
            var parser = Parse("(time (now 307.99))(GS (t 249.58) (pm PlayOn))(hear 249.58 self BPSn0d1Sn0d1)(hear 249.58 -1.45 l221yEJrw^NuB_VVEA|[)(GYR (n torso) (rt 59.64 16.77 -15.97))(ACC (n torso) (a 1.01 -2.14 4.08))(HJ (n hj1) (ax 0.00))(HJ (n hj2) (ax -0.00))(See (G1L (pol 13.43 33.03 19.78)) (G2L (pol 13.39 42.38 17.73)) (F1L (pol 15.24 8.49 20.00)) (B (pol 2.92 30.52 9.91)) (P (team Strive3D) (id 2) (head (pol 3.13 33.72 18.18)) (rlowerarm (pol 3.16 34.87 13.89)) (llowerarm (pol 3.15 30.89 15.07)) (rfoot (pol 3.19 32.68 9.56)) (lfoot (pol 3.24 30.94 10.59))) (P (team KarachiKoalas) (id 2) (rlowerarm (pol 0.21 -55.46 -35.88)) (llowerarm (pol 0.21 55.44 -35.63))) (P (team Strive3D) (id 1) (head (pol 12.76 37.53 17.56)) (rlowerarm (pol 12.78 38.09 16.71)) (llowerarm (pol 12.74 36.65 16.61)) (rfoot (pol 12.79 37.24 15.52)) (lfoot (pol 12.80 36.80 15.56))) (mypos -2.87 0.32 0.50) (L (pol 3.19 59.99 1.41) (pol 7.88 -32.92 15.79)) (L (pol 14.44 59.94 8.82) (pol 15.23 8.42 19.95)) (L (pol 7.37 -59.98 8.11) (pol 15.25 8.60 20.10)) (L (pol 11.80 27.07 17.40) (pol 11.69 46.22 12.81)) (L (pol 11.79 26.93 17.29) (pol 13.57 28.57 17.39)) (L (pol 11.69 46.21 12.80) (pol 13.48 45.32 13.48)) (L (pol 1.22 18.22 -3.37) (pol 2.03 -7.35 8.02)) (L (pol 2.03 -7.29 8.07) (pol 3.11 -4.78 12.93)) (L (pol 3.12 -4.52 13.14) (pol 4.01 6.11 14.83)) (L (pol 4.01 6.37 15.05) (pol 4.56 19.54 14.48)) (L (pol 4.56 19.57 14.51) (pol 4.70 33.33 12.13)) (L (pol 4.71 33.56 12.32) (pol 4.41 46.60 8.35)) (L (pol 4.42 46.80 8.51) (pol 3.73 58.23 3.54)) (L (pol 3.73 57.89 3.26) (pol 3.41 59.85 1.91)) (L (pol 1.68 60.31 -7.23) (pol 1.67 59.70 -7.74)) (L (pol 1.67 59.95 -7.53) (pol 1.22 18.40 -3.22)))(HJ (n raj1) (ax -27.87))(HJ (n raj2) (ax -45.12))(HJ (n raj3) (ax 0.05))(HJ (n raj4) (ax 68.03))(HJ (n laj1) (ax -27.90))(HJ (n laj2) (ax 45.14))(HJ (n laj3) (ax -0.05))(HJ (n laj4) (ax -68.05))(HJ (n rlj1) (ax 0.79))(HJ (n rlj2) (ax -0.85))(HJ (n rlj3) (ax 44.75))(HJ (n rlj4) (ax -59.37))(HJ (n rlj5) (ax 33.51))(HJ (n rlj6) (ax 0.85))(HJ (n llj1) (ax 1.51))(HJ (n llj2) (ax 0.85))(HJ (n llj3) (ax 24.94))(HJ (n llj4) (ax -43.76))(HJ (n llj5) (ax 36.91))(FRP (n lf) (c 0.03 0.08 -0.01) (f -0.58 -1.04 39.60))(HJ (n llj6) (ax -0.85))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var messages = parser.State.HeardMessages.ToArray();
            Assert.AreEqual(2, messages.Length);
            Assert.IsTrue(messages[0].IsFromSelf);
            Assert.AreEqual("BPSn0d1Sn0d1", messages[0].Text);
            Assert.AreEqual(TimeSpan.FromSeconds(249.58), messages[0].HeardAtTime);
            Assert.AreEqual(Angle.NaN, messages[0].RelativeDirection);
            Assert.IsFalse(messages[1].IsFromSelf);
            Assert.AreEqual("l221yEJrw^NuB_VVEA|[", messages[1].Text);
            Assert.AreEqual(TimeSpan.FromSeconds(249.58), messages[1].HeardAtTime);
            Assert.AreEqual(Angle.FromDegrees(-1.45), messages[1].RelativeDirection);
        }

        [Test]
        public void ShouldParseSingleDigitDouble()
        {
            var parser = Parse("(AgentState (temp 0) (battery 1))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(0, parser.State.AgentTemperature);
            Assert.AreEqual(1, parser.State.AgentBattery);
        }

        [Test]
        public void ShouldParseTeamSide()
        {
            var parser = Parse("(GS (unum 1) (team left) (t 0.00) (pm BeforeKickOff))");
            Assert.IsNotNull(parser.State.TeamSide);
            Assert.AreEqual(FieldSide.Left, parser.State.TeamSide);
        }

        [Test]
        public void ShouldParsePlayerId()
        {
            var parser = Parse("(GS (unum 1) (team left) (t 0.00) (pm BeforeKickOff))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.IsNotNull(parser.State.UniformNumber);
            Assert.AreEqual(1, parser.State.UniformNumber);
        }

        [Test]
        public void ShouldFailOnUnknownExpression()
        {
            var parser = Parse("gibberish");
            Assert.IsTrue(parser.errors.HasError);
        }

        [Test]
        public void ShouldParseVisibleLines()
        {
            var parser = Parse("(See " +
                               "(G1L (pol 13.43 33.03 19.78)) (G2L (pol 13.39 42.38 17.73)) (F1L (pol 15.24 8.49 20.00)) " +
                               "(B (pol 2.92 30.52 9.91)) " +
                               "(P (team Strive3D) (id 2) (head (pol 3.13 33.72 18.18)) (rlowerarm (pol 3.16 34.87 13.89)) (llowerarm (pol 3.15 30.89 15.07)) (rfoot (pol 3.19 32.68 9.56)) (lfoot (pol 3.24 30.94 10.59))) (P (team KarachiKoalas) (id 2) (rlowerarm (pol 0.21 -55.46 -35.88)) (llowerarm (pol 0.21 55.44 -35.63))) " +
                               "(P (team Strive3D) (id 1) (head (pol 12.76 37.53 17.56)) (rlowerarm (pol 12.78 38.09 16.71)) (llowerarm (pol 12.74 36.65 16.61)) (rfoot (pol 12.79 37.24 15.52)) (lfoot (pol 12.80 36.80 15.56))) (mypos -2.87 0.32 0.50) " +
                               "(L (pol 3.19 59.99 1.41) (pol 7.88 -32.92 15.79)) " +
                               "(L (pol 14.44 59.94 8.82) (pol 15.23 8.42 19.95)) " +
                               "(L (pol 7.37 -59.98 8.11) (pol 15.25 8.60 20.10)) " +
                               "(L (pol 11.80 27.07 17.40) (pol 11.69 46.22 12.81)) " +
                               "(L (pol 11.79 26.93 17.29) (pol 13.57 28.57 17.39)) " +
                               "(L (pol 11.69 46.21 12.80) (pol 13.48 45.32 13.48)) " +
                               "(L (pol 1.22 18.22 -3.37) (pol 2.03 -7.35 8.02)) " +
                               "(L (pol 2.03 -7.29 8.07) (pol 3.11 -4.78 12.93)) " +
                               "(L (pol 3.12 -4.52 13.14) (pol 4.01 6.11 14.83)) " +
                               "(L (pol 4.01 6.37 15.05) (pol 4.56 19.54 14.48)) " +
                               "(L (pol 4.56 19.57 14.51) (pol 4.70 33.33 12.13)) " +
                               "(L (pol 4.71 33.56 12.32) (pol 4.41 46.60 8.35)) " +
                               "(L (pol 4.42 46.80 8.51) (pol 3.73 58.23 3.54)) " +
                               "(L (pol 3.73 57.89 3.26) (pol 3.41 59.85 1.91)) " +
                               "(L (pol 1.68 60.31 -7.23) (pol 1.67 59.70 -7.74)) " +
                               "(L (pol 1.67 59.95 -7.53) (pol 1.22 18.40 -3.22)))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var lines = parser.State.VisibleLines;
            Assert.IsNotNull(lines);
            Assert.AreEqual(16,     lines.Count());

            Assert.AreEqual(3.19,   lines.Skip(0).First().End1.Distance, 0.0001);
            Assert.AreEqual(59.99,  lines.Skip(0).First().End1.Theta.Degrees, 0.0001);
            Assert.AreEqual(1.41,   lines.Skip(0).First().End1.Phi.Degrees, 0.0001);
            Assert.AreEqual(7.88,   lines.Skip(0).First().End2.Distance, 0.0001);
            Assert.AreEqual(-32.92, lines.Skip(0).First().End2.Theta.Degrees, 0.0001);
            Assert.AreEqual(15.79,  lines.Skip(0).First().End2.Phi.Degrees, 0.0001);
        }

        [Test]
        public void ShouldParseNaNDoubles()
        {
            var parser = Parse("(See (L (pol 4.06 -11.31 12.96) (pol nan nan nan)))");
            Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var lines = parser.State.VisibleLines;
            Assert.IsNotNull(lines);
            Assert.AreEqual(1, lines.Count());

            Assert.AreEqual(4.06,   lines.Skip(0).First().End1.Distance, 0.0001);
            Assert.AreEqual(-11.31, lines.Skip(0).First().End1.Theta.Degrees, 0.0001);
            Assert.AreEqual(12.96,  lines.Skip(0).First().End1.Phi.Degrees, 0.0001);

            Assert.IsTrue(double.IsNaN(lines.Skip(0).First().End2.Distance));
            Assert.IsTrue(double.IsNaN(lines.Skip(0).First().End2.Theta.Degrees));
            Assert.IsTrue(double.IsNaN(lines.Skip(0).First().End2.Phi.Degrees));
        }
    }
}