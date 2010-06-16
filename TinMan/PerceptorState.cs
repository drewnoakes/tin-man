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
// Created 07/05/2010 03:15

using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TinMan
{
    /// <summary>
    /// Models the snapshot of perceptor state sent from the server.  Not all fields will
    /// necessarily be populated.
    /// </summary>
    public sealed class PerceptorState {
        /// <summary>
        /// Gets the simulation time at which this state applies.  Simulation time is distinct from
        /// <see cref="GameTime"/> in that it is always increasing, even when the game's
        /// <see cref="PlayMode"/> means a game is not in progress.  The majority of agent hinge
        /// movement should be timed via this value.
        /// </summary>
        public TimeSpan SimulationTime { get; private set; }
        /// <summary>
        /// Gets the length of time into the current game period.  If the <see cref="PlayMode"/>
        /// means that a game period is not currently in progress, then this value will be static.
        /// Note also that this value can jump backwards after the first game period.
        /// </summary>
        public TimeSpan GameTime { get; private set; }
        /// <summary>
        /// Gets the current state of the soccer game.
        /// </summary>
        public PlayMode PlayMode { get; private set; }
        /// <summary>
        /// Gets the side of the field upon which the agent's team is currently playing.
        /// </summary>
        public FieldSide TeamSide { get; private set; }
        /// <summary>
        /// Gets the uniform number assigned to this agent.  If no number has been assigned yet,
        /// this value may be <c>null</c>.
        /// </summary>
        public int? UniformNumber { get; private set; }
        
        public IEnumerable<GyroState> GyroStates { get; private set; }
        public IEnumerable<HingeState> HingeStates { get; private set; }
        public IEnumerable<UniversalJointState> UniversalJointStates { get; private set; }
        public IEnumerable<TouchState> TouchStates { get; private set; }
        public IEnumerable<ForceState> ForceStates { get; private set; }
        public IEnumerable<AccelerometerState> AccelerometerStates { get; private set; }
        
        // TODO move all these to a special VisionPerceptorState type/property as they are only populated every three cycles
        public IEnumerable<LandmarkPosition> LandmarkPositions { get; private set; }
        public Polar? BallPosition { get; private set; }
        public IEnumerable<PlayerPosition> TeamMatePositions { get; private set; }
        public IEnumerable<PlayerPosition> OppositionPositions { get; private set; }
        
        public double? AgentBattery { get; private set; }
        public double? AgentTemperature { get; private set; }
        
        public IEnumerable<HeardMessage> HeardMessages { get; private set; }

        // TODO observe the server and see whether some of these 'nullable' values are actually never null in practice
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is instantiated by the TinMan framework.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public PerceptorState(TimeSpan simulationTime, TimeSpan gameTime, PlayMode playMode, FieldSide teamSide,
                              int? playerId,
                              IEnumerable<GyroState> gyroRates, IEnumerable<HingeState> hingeJointStates, IEnumerable<UniversalJointState> universalJointStates,
                              IEnumerable<TouchState> touchStates, IEnumerable<ForceState> forceStates, IEnumerable<AccelerometerState> accelerometerStates,
                              IEnumerable<LandmarkPosition> landmarkPositions,
                              IEnumerable<PlayerPosition> teamMatePositions, IEnumerable<PlayerPosition> oppositionPositions,
                              Polar? ballPosition,
                              double? agentBattery, double? agentTemperature, IEnumerable<HeardMessage> heardMessages) {
            SimulationTime = simulationTime;
            GameTime = gameTime;
            PlayMode = playMode;
            TeamSide = teamSide;
            UniformNumber = playerId;
            GyroStates = gyroRates;
            HingeStates = hingeJointStates;
            UniversalJointStates = universalJointStates;
            TouchStates = touchStates;
            ForceStates = forceStates;
            AccelerometerStates = accelerometerStates;
            LandmarkPositions = landmarkPositions;
            TeamMatePositions = teamMatePositions;
            OppositionPositions = oppositionPositions;
            BallPosition = ballPosition;
            AgentBattery = agentBattery;
            AgentTemperature = agentTemperature;
            HeardMessages = heardMessages;
        }
        
        /// <summary>Looks up the current angle for the given hinge.</summary>
        /// <remarks>Note that this method is marked with internal visibility as agent code
        /// should not need to use it.  Instead, access <see cref="Hinge.Angle"/> directly
        /// and avoid the O(N) lookup cost.</remarks>
        internal bool TryGetHingeAngle(Hinge hinge, out Angle angle) {
            if (HingeStates!=null) {
                foreach (var hj in HingeStates) {
                    if (hj.Label==hinge.PerceptorLabel) {
                        angle = hj.Angle;
                        return true;
                    }
                }
            }
            angle = Angle.NaN;
            return false;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            
            sb.AppendFormat(  "SimulationTime = {0}", SimulationTime);
            sb.AppendFormat("\nGameTime = {0}", GameTime);
            sb.AppendFormat("\nPlayMode = {0}", PlayMode);
            if (AgentBattery.HasValue)
                sb.AppendFormat("\nAgentBattery = {0}", AgentBattery);
            if (AgentTemperature.HasValue)
                sb.AppendFormat("\nAgentTemperature = {0}", AgentTemperature);
            if (HingeStates != null) {
                foreach (var j in HingeStates)
                    sb.AppendFormat("\nHinge Joint '{0}' -> {1}", j.Label, j.Angle.Degrees);
            }
            if (UniversalJointStates != null) {
                foreach (var j in UniversalJointStates)
                    sb.AppendFormat("\nBall Joint '{0}' -> {1} / {2}", j.Label, j.Angle1.Degrees, j.Angle2.Degrees);
            }
            if (AccelerometerStates != null) {
                foreach (var a in AccelerometerStates)
                    sb.AppendFormat("\nAccelerometer '{0}' -> {1}", a.Label, a.AccelerationVector);
            }
            if (GyroStates != null) {
                foreach (var g in GyroStates)
                    sb.AppendFormat("\nGyro '{0}' -> {1}, {2}, {3}", g.Label, g.XOrientation, g.YOrientation, g.ZOrientation);
            }
            if (TouchStates != null) {
                foreach (var t in TouchStates)
                    sb.AppendFormat("\nTouch State '{0}' -> {1}", t.Label, t.IsTouching);
            }
            if (ForceStates != null) {
                foreach (var f in ForceStates)
                    sb.AppendFormat("\nForce State '{0}' -> pos {1}, force {2}", f.Label, f.PointOnBody, f.ForceVector);
            }
            if (LandmarkPositions != null) {
                foreach (var l in LandmarkPositions)
                    sb.AppendFormat("\n{0} -> pos {1}", l.Landmark, l.PolarPosition);
            }
            if (BallPosition != null) {
                sb.AppendFormat("\nBall -> '{0}'", BallPosition);
            }
            if (TeamMatePositions != null) {
                foreach (var p in TeamMatePositions)
                    sb.AppendFormat("\n{0}", p);
            }
            if (OppositionPositions != null) {
                foreach (var p in OppositionPositions)
                    sb.AppendFormat("\n{0}", p);
            }
            if (HeardMessages != null) {
                foreach (var m in HeardMessages)
                    sb.AppendFormat("\nMessage at {1} from {2} text '{3}'", m.HeardAtTime, m.IsFromSelf ? "self" : m.RelativeDirection.Degrees.ToString(), m.Text);
            }
            return sb.ToString();
        }
    }
    
    [DebuggerDisplay("Gyro {Label}={XOrientation},{YOrientation},{ZOrientation}")]
    public struct GyroState {
        // TODO work out what units these gyro rates are in and potentially change to Angle instances
        public string Label { get; private set; }
        public double XOrientation { get; private set; }
        public double YOrientation { get; private set; }
        public double ZOrientation { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public GyroState(string label, double xOrientation, double yOrientation, double zOrientation) : this() {
            Label = label;
            XOrientation = xOrientation;
            YOrientation = yOrientation;
            ZOrientation = zOrientation;
        }
        
        public override string ToString() {
            return string.Format("{0} X={1} Y={2} Z={3}", Label, XOrientation, YOrientation, ZOrientation);
        }
    }
    
    public struct AccelerometerState {
        public string Label { get; private set; }
        public Vector3 AccelerationVector { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public AccelerometerState(string label, Vector3 accelerationVector) : this() {
            Label = label;
            AccelerationVector = accelerationVector;
        }
        
        public override string ToString() {
            return string.Format("{0} {1}", Label, AccelerationVector);
        }
    }
    
    public struct TouchState {
        public string Label { get; private set; }
        public bool IsTouching { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public TouchState(string label, bool isTouching) : this() {
            Label = label;
            IsTouching = isTouching;
        }
        
        public override string ToString() {
            return string.Format("{0} {1}touching", Label, IsTouching ? "" : "not ");
        }
    }
    
    public struct ForceState {
        public string Label { get; private set; }
        public Vector3 PointOnBody { get; private set; }
        public Vector3 ForceVector { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public ForceState(string label, Vector3 pointOnBody, Vector3 forceVector) : this() {
            Label = label;
            PointOnBody = pointOnBody;
            ForceVector = forceVector;
        }
    }
    
    public struct PlayerPosition {
        public bool IsTeamMate { get; private set; }
        public int PlayerId { get; private set; }
        public IEnumerable<BodyPartPosition> PartPositions { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public PlayerPosition(bool isTeamMate, int playerId, IEnumerable<BodyPartPosition> partPositions) : this() {
            IsTeamMate = isTeamMate;
            PlayerId = playerId;
            PartPositions = partPositions;
        }
        
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(IsTeamMate ? "TeamMate " : "Opposition ");
            sb.Append(PlayerId);
            sb.Append(" -> ");
            bool first = true;
            foreach (var p in PartPositions) {
                if (!first) {
                    sb.Append(", ");
                    first = false;
                }
                sb.AppendFormat("{0} @ {1}", p.Label, p.PolarPosition);
            }
            return sb.ToString();
        }
    }
    
    public struct LandmarkPosition {
        public Landmark Landmark { get; private set; }
        public Polar PolarPosition { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public LandmarkPosition(Landmark landmark, Polar radialPosition) : this() {
            Landmark = landmark;
            PolarPosition = radialPosition;
        }
    }
    
    [DebuggerDisplay("HingeJoint {Label}={Angle}")]
    public struct HingeState {
        public string Label { get; private set; }
        public Angle Angle { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public HingeState(string label, Angle angle) : this() {
            Label = label;
            Angle = angle;
        }
        
        public override string ToString() {
            return string.Format("{0} {1}", Label, Angle);
        }
    }
        
    [DebuggerDisplay("UniversalJoint {Label}={Angle1},{Angle2}")]
    public struct UniversalJointState {
        public string Label { get; private set; }
        public Angle Angle1 { get; private set; }
        public Angle Angle2 { get; private set; }
        
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public UniversalJointState(string label, Angle angle1, Angle angle2) : this() {
            Label = label;
            Angle1 = angle1;
            Angle2 = angle2;
        }
    }

    /// <summary>
    /// Represents a heard message by an agent on the field.  Models both the contents of the message
    /// along with the time it was heard and from what direction.  Note that an agent may here
    /// their own message, so check <see cref="IsFromSelf"/>.
    /// </summary>
    public sealed class HeardMessage {
        /// <summary>
        /// Gets a value indicating whether the agent heard their own message.
        /// </summary>
        public bool IsFromSelf { get { return RelativeDirection.IsNaN; } }
        /// <summary>
        /// The time at which the message was heard.  This value is relative to <see cref="PerceptorState.GameTime"/>,
        /// <see cref="PerceptorState.SimulationTime"/>.
        /// </summary>
        public TimeSpan HeardAtTime { get; private set; }
        /// <summary>
        /// Gets the relative direction from which this message was heard.  Note that this direction is only in one axis.
        /// TODO determine and document whether this direction is relative to the agent's head, body or field
        /// </summary>
        public Angle RelativeDirection { get; private set; }
        /// <summary>
        /// Gets the message text.  See <see cref="Message"/> for more information about messages and their text.
        /// </summary>
        public string Text { get; private set; }
        
        /// <summary>
        /// Initialises a heard message.
        /// </summary>
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// To send a message, use <see cref="ISimulationContext.Say"/>.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public HeardMessage(TimeSpan time, Angle direction, Message message) {
            if (message==null)
                throw new ArgumentNullException("message");
            HeardAtTime = time;
            RelativeDirection = direction;
            Text = message.Text;
        }
        
        public override string ToString() {
            return string.Format("Message \"{0}\" at {1} from {2}",
                                Text, HeardAtTime,
                                IsFromSelf ? "self" : RelativeDirection.ToString());
        }
    }
    
    /// <summary>
    /// Describes the location of an agent's body part relative to an observing agent, as
    /// observed by the agent's vision perceptor.
    /// </summary>
    public struct BodyPartPosition {
        public string Label { get; private set; }
        public Polar PolarPosition { get; private set; }
        
        public BodyPartPosition(string label, Polar position) : this() {
            Label = label;
            PolarPosition = position;
        }
        
        public override string ToString() {
            return string.Format("{0} {1}", Label, PolarPosition);
        }
    }
    
    /// <summary>
    /// Enumeration of fixed landmarks around the field that may be observed by an agent's
    /// vision perceptor.
    /// </summary>
    public enum Landmark {
        /// <summary>The north-west flag.  Referred to as FL1 by the server.</summary>
        FlagLeftTop,
        /// <summary>The south-west flag.  Referred to as FL2 by the server.</summary>
        FlagLeftBottom,
        /// <summary>The north-east flag.  Referred to as FR1 by the server.</summary>
        FlagRightTop,
        /// <summary>The south-east flag.  Referred to as FR2 by the server.</summary>
        FlagRightBottom,
        
        /// <summary>The north-west goal.  Referred to as GL1 by the server.</summary>
        GoalLeftTop,
        /// <summary>The south-west goal.  Referred to as GL2 by the server.</summary>
        GoalLeftBottom,
        /// <summary>The north-east goal.  Referred to as GR1 by the server.</summary>
        GoalRightTop,
        /// <summary>The south-east goal.  Referred to as GR2 by the server.</summary>
        GoalRightBottom
    }
    
    /// <summary>
    /// Enumeration of field sides.
    /// </summary>
    public enum FieldSide {
        /// <summary>The side of the field is unknown.</summary>
        Unknown = 0,
        /// <summary>The left side of the field, having a yellow goal.</summary>
        Left,
        /// <summary>The right side of the field, having a blue goal.</summary>
        Right
    }
}
