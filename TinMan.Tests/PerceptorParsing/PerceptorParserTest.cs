/*
 * Created by Drew, 06/05/2010 23:29.
 */
using System;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

namespace TinMan.PerceptorParsing
{
    [TestFixture]
    public sealed class PerceptorParserTest
	{
        private Parser Parse(string s) {
        	var parser = new Parser(new Scanner(new StringBuffer(s)));
		    parser.Parse();
		    return parser;
        }
        
        [Test] public void ShouldParseLongStringWithoutError() {
            var strings = new[] {
                "(time (now 417.65))(GS (t 0.00) (pm BeforeKickOff))(GYR (n torso) (rt 0.00 0.00 0.00))(ACC (n torso) (a 0.00 0.00 9.81))(HJ (n hj1) (ax -0.00))(HJ (n hj2) (ax -0.00))(HJ (n raj1) (ax -0.00))(HJ (n raj2) (ax -0.00))(HJ (n raj3) (ax -0.00))(HJ (n raj4) (ax -0.00))(HJ (n laj1) (ax -0.00))(HJ (n laj2) (ax -0.00))(HJ (n laj3) (ax -0.00))(HJ (n laj4) (ax -0.00))(HJ (n rlj1) (ax -0.00))(HJ (n rlj2) (ax -0.00))(HJ (n rlj3) (ax -0.00))(HJ (n rlj4) (ax -0.00))(HJ (n rlj5) (ax -0.00))(HJ (n rlj6) (ax -0.00))(HJ (n llj1) (ax -0.00))(HJ (n llj2) (ax -0.00))(HJ (n llj3) (ax -0.00))(HJ (n llj4) (ax -0.00))(HJ (n llj5) (ax -0.00))(HJ (n llj6) (ax -0.00))",
                "(time (now 203.83))(GS (t 0.02) (pm KickOff_Left))(GYR (n torso) (rt 15.33 -21.37 -97.40))(ACC (n torso) (a 2.13 8.56 0.00))(HJ (n hj1) (ax -120.00))(HJ (n hj2) (ax -45.00))(HJ (n raj1) (ax -120.01))(HJ (n raj2) (ax -95.04))(HJ (n raj3) (ax -120.00))(HJ (n raj4) (ax 1.49))(HJ (n laj1) (ax -124.53))(HJ (n laj2) (ax -7.27))(HJ (n laj3) (ax -120.03))(HJ (n laj4) (ax -90.01))(HJ (n rlj1) (ax -95.88))(HJ (n rlj2) (ax -44.98))(HJ (n rlj3) (ax -24.97))(HJ (n rlj4) (ax -133.55))(HJ (n rlj5) (ax -44.97))(FRP (n rf) (c 0.06 0.08 -0.02) (f 15.81 6.81 24.96))(HJ (n rlj6) (ax -28.14))(HJ (n llj1) (ax -86.22))(HJ (n llj2) (ax -21.97))(HJ (n llj3) (ax -20.53))(HJ (n llj4) (ax -127.43))(HJ (n llj5) (ax -52.01))(FRP (n lf) (c -0.04 0.08 -0.01) (f 26.77 5.28 18.93))(HJ (n llj6) (ax -45.00))"
            };
            
            foreach (var s in strings) {
                var parser = Parse(s);
    		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            }
        }
        
        [Test] public void ShouldParseSimulationTime() {
        	var parser = Parse("(time (now 417.65))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    Assert.AreEqual(TimeSpan.FromSeconds(417.65), parser.State.SimulationTime);
        }
        
        [Test] public void ShouldParsePlayMode() {
        	var parser = Parse("(GS (t 0.00) (pm KickOff_Left))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.AreEqual(PlayMode.KickOffLeft, parser.State.PlayMode);
        }
        
        [Test] public void ShouldParseGameTime() {
        	var parser = Parse("(GS (t 3.14) (pm PlayOn))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    Assert.AreEqual(TimeSpan.FromSeconds(3.14), parser.State.GameTime);
        }
        
        [Test] public void ShouldParseGyro() {
        	var parser = Parse("(GYR (n torso) (rt 1.23 2.34 3.45))");
        	Assert.IsNotNull(parser.State.GyroStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.GyroStates.Single();
            Assert.AreEqual("torso", state.Label);
            Assert.AreEqual(1.23, state.XOrientation, 0.0001);
            Assert.AreEqual(2.34, state.YOrientation, 0.0001);
            Assert.AreEqual(3.45, state.ZOrientation, 0.0001);
        }
        
        [Test] public void ShouldParseAccelerometer() {
        	var parser = Parse("(ACC (n torso) (a 0.00 -0.05 8.83))");
        	Assert.IsNotNull(parser.State.AccelerometerStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.AccelerometerStates.Single();
            Assert.AreEqual("torso", state.Label);
            Assert.AreEqual(0.00,  state.AccelerationVector.X, 0.0001);
            Assert.AreEqual(-0.05, state.AccelerationVector.Y, 0.0001);
            Assert.AreEqual(8.83,  state.AccelerationVector.Z, 0.0001);
        }
        
        [Test] public void ShouldParseHingeJoint() {
        	var parser = Parse("(HJ (n hj1) (ax 1.5))");
        	Assert.IsNotNull(parser.State.HingeStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.HingeStates.Single();
            Assert.AreEqual("hj1", state.Label);
            Assert.AreEqual(Angle.FromDegrees(1.5), state.Angle);
        }
        
        [Test] public void ShouldParseUniversalJoint() {
        	var parser = Parse("(UJ (n laj1) (ax1 -1.50) (ax2 2.00))");
        	Assert.IsNotNull(parser.State.UniversalJointStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.UniversalJointStates.Single();
            Assert.AreEqual("laj1", state.Label);
            Assert.AreEqual(Angle.FromDegrees(-1.5), state.Angle1);
            Assert.AreEqual(Angle.FromDegrees(2.00),  state.Angle2);
        }
 
        [Test] public void ShouldParseTouch() {
        	var parser = Parse("(TCH n bumper val 1)");
        	Assert.IsNotNull(parser.State.TouchStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.TouchStates.Single();
            Assert.AreEqual("bumper", state.Label);
            Assert.IsTrue(state.IsTouching);
        }
 
        [Test] public void ShouldParseForce() {
        	var parser = Parse("(FRP (n lf) (c -0.14 0.08 -0.05) (f 1.12 -0.26 13.07))");
        	Assert.IsNotNull(parser.State.ForceStates);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.ForceStates.Single();
            Assert.AreEqual("lf",  state.Label);
            Assert.AreEqual(-0.14, state.PointOnBody.X, 0.0001);
            Assert.AreEqual(0.08,  state.PointOnBody.Y, 0.0001);
            Assert.AreEqual(-0.05, state.PointOnBody.Z, 0.0001);
            Assert.AreEqual(1.12,  state.ForceVector.X, 0.0001);
            Assert.AreEqual(-0.26, state.ForceVector.Y, 0.0001);
            Assert.AreEqual(13.07, state.ForceVector.Z, 0.0001);
        }
        
        [Test] public void ShouldParseSeenFlag() {
        	var parser = Parse("(See (F2L (pol 11.52 52.50 -8.10)))");
        	Assert.IsNotNull(parser.State.LandmarkPositions);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.LandmarkPositions.Single();
            Assert.AreEqual(Landmark.FlagLeftBottom,  state.Landmark);
            Assert.AreEqual(11.52, state.PolarPosition.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.PolarPosition.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.PolarPosition.Phi);
        }        
        
        [Test] public void ShouldParseSeenGoal() {
        	var parser = Parse("(See (G2R (pol 11.52 52.50 -8.10)))");
        	Assert.IsNotNull(parser.State.LandmarkPositions);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.LandmarkPositions.Single();
            Assert.AreEqual(Landmark.GoalRightBottom, state.Landmark);
            Assert.AreEqual(11.52, state.PolarPosition.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.PolarPosition.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.PolarPosition.Phi);
        }
        
        [Test] public void ShouldParseSeenBall() {
        	var parser = Parse("(See (B (pol 11.52 52.50 -8.10)))");
        	Assert.IsNotNull(parser.State.BallPosition);
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            var state = parser.State.BallPosition.Value;
            Assert.AreEqual(11.52, state.Distance, 0.0001);
            Assert.AreEqual(Angle.FromDegrees(52.50), state.Theta);
            Assert.AreEqual(Angle.FromDegrees(-8.10), state.Phi);
        }
        
        [Test] public void ShouldParseSeenPlayer() {
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
            Assert.AreEqual(9.04,   components[0].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-57.66, components[0].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-28.25, components[0].PolarPosition.Phi.Degrees, 0.0001);
		    Assert.AreEqual("rlowerarm", components[1].Label);
            Assert.AreEqual(8.93,   components[1].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-57.84, components[1].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-29.70, components[1].PolarPosition.Phi.Degrees, 0.0001);
		    Assert.AreEqual("llowerarm", components[2].Label);
            Assert.AreEqual(9.07,   components[2].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-59.44, components[2].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-28.47, components[2].PolarPosition.Phi.Degrees, 0.0001);
		    Assert.AreEqual("rfoot", components[3].Label);
            Assert.AreEqual(9.01,   components[3].PolarPosition.Distance, 0.0001);
            Assert.AreEqual(-59.41, components[3].PolarPosition.Theta.Degrees, 0.0001);
            Assert.AreEqual(-30.45, components[3].PolarPosition.Phi.Degrees, 0.0001);
        }
        
        [Test] public void ShouldParseAgentState() {
        	var parser = Parse("(AgentState (temp 4) (battery 75))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    Assert.AreEqual(4, parser.State.AgentTemperature);
		    Assert.AreEqual(75, parser.State.AgentBattery);
        }
        
        [Test] public void ShouldParseMessageFromSelf() {
            // (hear 0.00 self ComeOn!)
        	var parser = Parse("(hear 12.3 self ''helloworld'')");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    var message = parser.State.Messages.Single();
		    Assert.IsTrue(message.IsFromSelf);
		    Assert.AreEqual("helloworld", message.MessageText);
		    Assert.AreEqual(TimeSpan.FromSeconds(12.3), message.HeardAtTime);
        }
        
        [Test] public void ShouldParseMessageFromDirection() {
        	var parser = Parse("(hear 12.3 12.34 ''overhere'')");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    var message = parser.State.Messages.Single();
		    Assert.IsFalse(message.IsFromSelf);
		    Assert.AreEqual("overhere", message.MessageText);
		    Assert.AreEqual(Angle.FromDegrees(12.34), message.RelativeDirection);
		    Assert.AreEqual(TimeSpan.FromSeconds(12.3), message.HeardAtTime);
        }
        
        [Test] public void ShouldParseSingleDigitDouble() {
        	var parser = Parse("(AgentState (temp 0) (battery 1))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
		    Assert.AreEqual(0, parser.State.AgentTemperature);
		    Assert.AreEqual(1, parser.State.AgentBattery);
        }
        
        [Test] public void ShouldParseTeamSide() {
            var parser = Parse("(GS (unum 1) (team left) (t 0.00) (pm BeforeKickOff))");
            Assert.IsNotNull(parser.State.TeamSide);
            Assert.AreEqual(FieldSide.Left, parser.State.TeamSide);
        }
        
        [Test] public void ShouldParsePlayerId() {
            var parser = Parse("(GS (unum 1) (team left) (t 0.00) (pm BeforeKickOff))");
		    Assert.IsFalse(parser.errors.HasError, parser.errors.ErrorMessages);
            Assert.IsNotNull(parser.State.PlayerId);
            Assert.AreEqual(1, parser.State.PlayerId);
        }

        [Test] public void ShouldFailOnUnknownExpression() {
        	var parser = Parse("gibberish");
		    Assert.IsTrue(parser.errors.HasError);
        }
	}
}