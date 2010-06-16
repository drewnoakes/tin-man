/*
 * Created by Drew, 10/05/2010 16:04.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drew.RoboCup.Nao
{
    /// <summary>
    /// A Nao-model specific representation of the state of the robot in the simulation.
    /// </summary>
    /// <remarks>
    /// The Nao humanoid robot manufactured by Aldebaran Robotics. Its height is about 57cm and
    /// its weight is around 4.5Kg. Its biped architecture with 22 degrees of freedom allows Nao to
    /// have great mobility.
    /// </remarks>
    public sealed class NaoStateSnapshot
    {
        public double? Battery { get; private set; }
        public double? Temperature { get; private set; }
        public Vector3? TorsoAcceleration { get; private set; }
        public RadialPosition? BallPosition { get; private set; }
        public RadialPosition? FlagLeftBottomPosition { get; private set; }
        public RadialPosition? FlagLeftTopPosition { get; private set; }
        public RadialPosition? FlagRightBottomPosition { get; private set; }
        public RadialPosition? FlagRightTopPosition { get; private set; }
        public int LandmarkPositionCount { get; private set; }
        public RadialPosition? GoalLeftBottomPosition { get; private set; }
        public RadialPosition? GoalLeftTopPosition { get; private set; }
        public RadialPosition? GoalRightBottomPosition { get; private set; }
        public RadialPosition? GoalRightTopPosition { get; private set; }
        public ForceState? LeftFootForce { get; private set; }
        public ForceState? RightFootForce { get; private set; }
        public GyroState? TorsoGyro { get; private set; }
        public TimeSpan? SimulationTime { get; private set; }
        public TimeSpan? GameTime { get; private set; }
        public PlayMode? PlayMode { get; private set; }
        public bool? IsLeftFootTouching { get; private set; }
        public bool? IsRightFootTouching { get; private set; }
        public IEnumerable<PlayerPosition> TeamMatePositions { get; private set; }
        public IEnumerable<PlayerPosition> OppositionPositions { get; private set; }
        public IEnumerable<Message> Messages { get; private set; }
        
        public NaoStateSnapshot(PerceptorState state)
        {
            Battery = state.AgentBattery;
            Temperature = state.AgentTemperature;
            BallPosition = state.BallPosition;
            GameTime = state.GameTime;
            SimulationTime = state.SimulationTime;
            PlayMode = state.PlayMode;
            
            if (state.AccelerometerStates!=null)
                TorsoAcceleration = state.AccelerometerStates.Single().AccelerationVector;

            if (state.LandmarkPositions!=null) {
                int landmarkCount = 0;
                foreach (var f in state.LandmarkPositions) {
                    switch (f.Landmark) {
                        case Landmark.FlagLeftBottom: FlagLeftBottomPosition = f.RadialPosition; break;
                        case Landmark.FlagLeftTop: FlagLeftTopPosition = f.RadialPosition; break;
                        case Landmark.FlagRightBottom: FlagRightBottomPosition = f.RadialPosition; break;
                        case Landmark.FlagRightTop: FlagRightTopPosition = f.RadialPosition; break;
                        case Landmark.GoalLeftBottom: GoalLeftBottomPosition = f.RadialPosition; break;
                        case Landmark.GoalLeftTop: GoalLeftTopPosition = f.RadialPosition; break;
                        case Landmark.GoalRightBottom: GoalRightBottomPosition = f.RadialPosition; break;
                        case Landmark.GoalRightTop: GoalRightTopPosition = f.RadialPosition; break;
                        default:
                            throw new Exception("Unexpected FlagType enum value: " + f.Landmark);
                    }
                    landmarkCount++;
                }
                LandmarkPositionCount = landmarkCount;
            }

            if (state.ForceStates!=null) {
                foreach (var f in state.ForceStates) {
                    if (f.Label == "lf")
                        LeftFootForce = f;
                    else if (f.Label == "lf")
                        RightFootForce = f;
                    else
                        throw new Exception("Unexpected force label: " + f.Label);
                }
            }

            if (state.AccelerometerStates!=null)
                TorsoAcceleration = state.AccelerometerStates.Single().AccelerationVector;

            if (state.GyroStates!=null)
                TorsoGyro = state.GyroStates.Single();
            
            if (state.TouchStates!=null) {
                foreach (var t in state.TouchStates) {
                    if (t.Label=="lf")
                        IsLeftFootTouching = true;
                    else if (t.Label=="lf")
                        IsRightFootTouching = true;
                    else 
                        throw new Exception("Unexpected TouchState Label: " + t.Label);
                }
            }
            
            Messages = state.Messages;
            TeamMatePositions = state.TeamMatePositions;
            OppositionPositions = state.OppositionPositions;

            if (state.HingeJointStates!=null) {
                foreach (var hj in state.HingeJointStates) {
                    switch (hj.Label) {
                        default: break;
//                        case "hj1":  HeadAngle1 = hj.Angle; break; 
//                		case "hj2":  Blah = hj.Angle; break;
//                		case "laj1": Blah = hj.Angle; break;
//                		case "laj2": Blah = hj.Angle; break;
//                		case "laj3": Blah = hj.Angle; break;
//                		case "laj4": Blah = hj.Angle; break;
//                		case "llj1": Blah = hj.Angle; break;
//                		case "llj2": Blah = hj.Angle; break;
//                		case "llj3": Blah = hj.Angle; break;
//                		case "llj4": Blah = hj.Angle; break;
//                		case "llj5": Blah = hj.Angle; break;
//                		case "llj6": Blah = hj.Angle; break;
//                		case "raj1": Blah = hj.Angle; break;
//                		case "raj2": Blah = hj.Angle; break;
//                		case "raj3": Blah = hj.Angle; break;
//                		case "raj4": Blah = hj.Angle; break;
//                		case "rlj1": Blah = hj.Angle; break;
//                		case "rlj2": Blah = hj.Angle; break;
//                		case "rlj3": Blah = hj.Angle; break;
//                		case "rlj4": Blah = hj.Angle; break;
//                		case "rlj5": Blah = hj.Angle; break;
//                		case "rlj6": Blah = hj.Angle; break;
                    }
                }
            }

            // NOTE we ignore "state.UniversalJointStates" as the Nao robot doesn't have any
        }
    }
}
