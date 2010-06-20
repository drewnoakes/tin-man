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
}