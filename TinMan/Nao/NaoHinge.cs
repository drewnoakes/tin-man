/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinMan
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// The Nao humanoid robot manufactured by Aldebaran Robotics.
	/// Its biped architecture has 22 degrees of freedom and allows Nao to have great mobility.
	/// The Nao model replaced the <em>soccerbot</em> as the default model for RoboCup competitive simulated 3D soccer.
	/// </remarks>
	public static class NaoHinge {
	    /// <summary>Neck joint.  Allows the head (and camera) to be panned left and right.  Zero degrees looks directly ahead.  Angles range -120 to 120 degrees.</summary>
	    public static Hinge HJ1 { get; private set; }
	    /// <summary>Head joint.  Allows the head (and camera) to be tilted up and down. Zero degrees looks horizontally.  Angles range -45 to 45 degrees.</summary>
	    public static Hinge HJ2 { get; private set; }
	
	    /// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
	    public static Hinge LAJ1 { get; private set; }
	    /// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -1 to 95 degrees.</summary>
	    public static Hinge LAJ2 { get; private set; }
	    /// <summary>Allows the arm to rotate along its length between the shoulder and the elbow.  Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
	    public static Hinge LAJ3 { get; private set; }
	    /// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -90 to 1 degrees.</summary>
	    public static Hinge LAJ4 { get; private set; }
	
	    /// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
	    public static Hinge RAJ1 { get; private set; }
	    /// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -95 to 1 degrees.</summary>
	    public static Hinge RAJ2 { get; private set; }
	    /// <summary>Allows the arm to rotate along its length between the shoulder and the elbow. Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
	    public static Hinge RAJ3 { get; private set; }
	    /// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -1 to 90 degrees.</summary>
	    public static Hinge RAJ4 { get; private set; }
	
	    /// <summary>
	    /// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with
	    /// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
	    /// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
	    /// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
	    /// </summary>
	    public static Hinge LLJ1 { get; private set; }
	    /// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -25 to 45 degrees.</summary>
	    public static Hinge LLJ2 { get; private set; }
	    /// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
	    public static Hinge LLJ3 { get; private set; }
	    /// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
	    public static Hinge LLJ4 { get; private set; }
	    /// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
	    public static Hinge LLJ5 { get; private set; }
	    /// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -45 to 25 degrees.</summary>
	    public static Hinge LLJ6 { get; private set; }
	
	    /// <summary>
	    /// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with
	    /// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
	    /// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
	    /// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
	    /// </summary>
	    public static Hinge RLJ1 { get; private set; }
	    /// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -45 to 25 degrees.</summary>
	    public static Hinge RLJ2 { get; private set; }
	    /// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
	    public static Hinge RLJ3 { get; private set; }
	    /// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
	    public static Hinge RLJ4 { get; private set; }
	    /// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
	    public static Hinge RLJ5 { get; private set; }
	    /// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -25 to 45 degrees.</summary>
	    public static Hinge RLJ6 { get; private set; }
	
	    static NaoHinge() {
	        HJ1 = new Hinge("hj1", "he1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
	        HJ2 = new Hinge("hj2", "he2", Angle.FromDegrees(-45),  Angle.FromDegrees(45));
	
	        LAJ1 = new Hinge("laj1", "lae1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
	        LAJ2 = new Hinge("laj2", "lae2", Angle.FromDegrees(-1),   Angle.FromDegrees(95));
	        LAJ3 = new Hinge("laj3", "lae3", Angle.FromDegrees(-120), Angle.FromDegrees(120));
	        LAJ4 = new Hinge("laj4", "lae4", Angle.FromDegrees(-90),  Angle.FromDegrees(1));
	
	        RAJ1 = new Hinge("raj1", "rae1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
	        RAJ2 = new Hinge("raj2", "rae2", Angle.FromDegrees(-95),  Angle.FromDegrees(1));
	        RAJ3 = new Hinge("raj3", "rae3", Angle.FromDegrees(-120), Angle.FromDegrees(120));
	        RAJ4 = new Hinge("raj4", "rae4", Angle.FromDegrees(-1),   Angle.FromDegrees(90));
	
	        LLJ1 = new Hinge("llj1", "lle1", Angle.FromDegrees(-90),  Angle.FromDegrees(1));
	        LLJ2 = new Hinge("llj2", "lle2", Angle.FromDegrees(-25),  Angle.FromDegrees(45));
	        LLJ3 = new Hinge("llj3", "lle3", Angle.FromDegrees(-25),  Angle.FromDegrees(100));
	        LLJ4 = new Hinge("llj4", "lle4", Angle.FromDegrees(-130), Angle.FromDegrees(1));
	        LLJ5 = new Hinge("llj5", "lle5", Angle.FromDegrees(-45),  Angle.FromDegrees(75));
	        LLJ6 = new Hinge("llj6", "lle6", Angle.FromDegrees(-45),  Angle.FromDegrees(25));
	
	        RLJ1 = new Hinge("rlj1", "rle1", Angle.FromDegrees(-90),  Angle.FromDegrees(1));
	        RLJ2 = new Hinge("rlj2", "rle2", Angle.FromDegrees(-45),  Angle.FromDegrees(25));
	        RLJ3 = new Hinge("rlj3", "rle3", Angle.FromDegrees(-25),  Angle.FromDegrees(100));
	        RLJ4 = new Hinge("rlj4", "rle4", Angle.FromDegrees(-130), Angle.FromDegrees(1));
	        RLJ5 = new Hinge("rlj5", "rle5", Angle.FromDegrees(-45),  Angle.FromDegrees(75));
	        RLJ6 = new Hinge("rlj6", "rle6", Angle.FromDegrees(-25),  Angle.FromDegrees(45));
	    }
	}
}
