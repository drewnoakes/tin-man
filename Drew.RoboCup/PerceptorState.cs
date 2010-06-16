/*
 * Created by Drew, 07/05/2010 03:15.
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Drew.RoboCup
{
    /// <summary>
    /// Models the snapshot of perceptor state sent from the server.  Not all fields will
    /// necessarily be populated.
    /// </summary>
	public sealed class PerceptorState {
		public TimeSpan? GameTime { get; private set; }
		public TimeSpan? SimulationTime { get; private set; }
		public PlayMode? PlayMode { get; private set; }
		public FieldSide? TeamSide { get; private set; }
		public int? PlayerId { get; private set; }
		
		public IEnumerable<GyroState> GyroStates { get; private set; }		
		public IEnumerable<HingeJointState> HingeJointStates { get; private set; }
		public IEnumerable<UniversalJointState> UniversalJointStates { get; private set; }
		public IEnumerable<TouchState> TouchStates { get; private set; }
		public IEnumerable<ForceState> ForceStates { get; private set; }
		public IEnumerable<AccelerometerState> AccelerometerStates { get; private set; }
		
		public IEnumerable<LandmarkPosition> LandmarkPositions { get; private set; }
		public Polar? BallPosition { get; private set; }
		public IEnumerable<PlayerPosition> TeamMatePositions { get; private set; }
		public IEnumerable<PlayerPosition> OppositionPositions { get; private set; }
		
		public double? AgentBattery { get; private set; }
		public double? AgentTemperature { get; private set; }
		
		public IEnumerable<Message> Messages { get; private set; }

		// TODO observe the server and see whether some of these 'nullable' values are actually never null in practice
		
		public PerceptorState(TimeSpan? simulationTime, TimeSpan? gameTime, PlayMode? playMode, FieldSide? teamSide, int? playerId,
		                      IEnumerable<GyroState> gyroRates, IEnumerable<HingeJointState> hingeJointStates, IEnumerable<UniversalJointState> universalJointStates,
							  IEnumerable<TouchState> touchStates, IEnumerable<ForceState> forceStates, IEnumerable<AccelerometerState> accelerometerStates,
							  IEnumerable<LandmarkPosition> landmarkPositions,
							  IEnumerable<PlayerPosition> teamMatePositions, IEnumerable<PlayerPosition> oppositionPositions,
							  Polar? ballPosition,
							  double? agentBattery, double? agentTemperature, IEnumerable<Message> heardMessages) {
			SimulationTime = simulationTime;
			GameTime = gameTime;
			PlayMode = playMode;
			TeamSide = teamSide;
			PlayerId = playerId;
			GyroStates = gyroRates;
			HingeJointStates = hingeJointStates;
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
			Messages = heardMessages;
		}
		
		public Angle GetHingeJointAngle(string hingePerceptorLabel) {
            Debug.Assert(HingeJointStates!=null, "HingeJointStates should not be null.");
            foreach (var hj in HingeJointStates) {
		        if (hj.Label==hingePerceptorLabel)
		            return hj.Angle;
		    }
		    throw new ArgumentException(string.Format("No hinge state exists with label '{0}'.", hingePerceptorLabel));
		}

	    public override string ToString() {
	        var sb = new StringBuilder();
	        
            sb.AppendFormat(  "SimulationTime = {0}", SimulationTime);
            sb.AppendFormat("\nGameTime = {0}", GameTime);
            sb.AppendFormat("\nPlayMode = {0}", PlayMode);
            sb.AppendFormat("\nAgentBattery = {0}", AgentBattery);
            sb.AppendFormat("\nAgentTemperature = {0}", AgentTemperature);
            if (HingeJointStates != null) {
                foreach (var j in HingeJointStates)
                    sb.AppendFormat("\nHinge Joint '{0}' -> {1}", j.Label, j.Angle);
            }
            if (UniversalJointStates != null) {
                foreach (var j in UniversalJointStates)
                    sb.AppendFormat("\nBall Joint '{0}' -> {1} / {2}", j.Label, j.Angle1, j.Angle2);
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
            if (Messages != null) {
                foreach (var m in Messages)
                    sb.AppendFormat("\nMessage at {1} from {2} text '{3}'", m.HeardAtTime, m.IsFromSelf ? "self" : m.RelativeDirection.ToString(), m.MessageText);
            }
            return sb.ToString();
        }
    }
	
    [DebuggerDisplay("Gyro {Label}={XOrientation},{YOrientation},{ZOrientation}")]
	public struct GyroState {
	    public string Label { get; private set; }
	    public double XOrientation { get; private set; }
	    public double YOrientation { get; private set; }
	    public double ZOrientation { get; private set; }
	    
	    public GyroState(string label, double xOrientation, double yOrientation, double zOrientation) : this() {
	        Label = label;
	        XOrientation = xOrientation;
	        YOrientation = yOrientation;
	        ZOrientation = zOrientation;
	    }
	}
	
	public struct AccelerometerState {
		public string Label { get; private set; }
		public Vector3 AccelerationVector { get; private set; }
		
		public AccelerometerState(string label, Vector3 accelerationVector) : this() {
		    Label = label;
		    AccelerationVector = accelerationVector;
		}
	}
	
	public struct TouchState {
		public string Label { get; private set; }
		public bool IsTouching { get; private set; }
		
		public TouchState(string label, bool isTouching) : this() {
		    Label = label;
		    IsTouching = isTouching;
		}
	}
	
	public struct ForceState {
		public string Label { get; private set; }
		public Vector3 PointOnBody { get; private set; }
		public Vector3 ForceVector { get; private set; }
		
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
		
		public LandmarkPosition(Landmark landmark, Polar radialPosition) : this() {
			Landmark = landmark;
			PolarPosition = radialPosition;
		}
	}
	
    [DebuggerDisplay("HingeJoint {Label}={Angle}")]
    public struct HingeJointState {
		public string Label { get; private set; }
		public Angle Angle { get; private set; }
		
		public HingeJointState(string label, Angle angle) : this() {
			Label = label;
			Angle = angle;
		}
	}
		
    [DebuggerDisplay("UniversalJoint {Label}={Angle1},{Angle2}")]
	public struct UniversalJointState {
		public string Label { get; private set; }
		public Angle Angle1 { get; private set; }
		public Angle Angle2 { get; private set; }
		
		public UniversalJointState(string label, Angle angle1, Angle angle2) : this() {
			Label = label;
			Angle1 = angle1;
			Angle2 = angle2;
		}
	}

	public struct Message {
	    public bool IsFromSelf { get { return RelativeDirection.IsNaN; } }
	    public TimeSpan HeardAtTime { get; private set; }
	    public Angle RelativeDirection { get; private set; }
	    public string MessageText { get; private set; }
	    
	    public Message(TimeSpan time, Angle direction, string message) : this() {
	        HeardAtTime = time;
	        RelativeDirection = direction;
	        MessageText = message;
	    }
	}
	
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
	
	public enum Landmark {
		FlagLeftTop,
		FlagLeftBottom,
		FlagRightTop,
		FlagRightBottom,
		GoalLeftTop,
		GoalLeftBottom,
		GoalRightTop,
		GoalRightBottom
	}
	
	public enum FieldSide {
	    Unknown = 0,
	    Left,
	    Right
	}
}
