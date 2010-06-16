﻿/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Drew.RoboCup.Nao
{
	/// <remarks>
	/// The Nao humanoid robot manufactured by Aldebaran Robotics.
	/// Its biped architecture has 22 degrees of freedom and allows Nao to have great mobility.
	/// The Nao model replaced the <em>soccerbot</em> as the default model for RoboCup competitive simulated 3D soccer.
	/// </remarks>
	public sealed class NaoBody : IBody {
	    /// <summary>Approximate weight of the Nao robot is 4.5kg.</summary>
	    public const double WeightKilograms = 4.5;
	    /// <summary>Approximate height of the Nao robot is 57cm.</summary>
	    public const double Height = 0.57;
	    
	    /// <summary>Well-known path of the Ruby Scene Graph (RSG) file for the NAO model in the RCSS3D server package.</summary>
		public const string RsgFilePath = "rsg/agent/nao/nao.rsg";

		/// <summary>Neck joint.  Allows the head (and camera) to be panned left and right.  Zero degrees looks directly ahead.  Angles range -120 to 120 degrees.</summary>
		public HingeController HJ1 { get; private set; }
		/// <summary>Head joint.  Allows the head (and camera) to be tilted up and down. Zero degrees looks horizontally.  Angles range -45 to 45 degrees.</summary>
		public HingeController HJ2 { get; private set; }

		/// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
		public HingeController LAJ1 { get; private set; }
		/// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -1 to 95 degrees.</summary>
		public HingeController LAJ2 { get; private set; }
		/// <summary>Allows the arm to rotate along its length between the shoulder and the elbow.  Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
		public HingeController LAJ3 { get; private set; }
		/// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -90 to 1 degrees.</summary>
		public HingeController LAJ4 { get; private set; }

		/// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
		public HingeController RAJ1 { get; private set; }
		/// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -95 to 1 degrees.</summary>
		public HingeController RAJ2 { get; private set; }
		/// <summary>Allows the arm to rotate along its length between the shoulder and the elbow. Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
		public HingeController RAJ3 { get; private set; }
		/// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -1 to 90 degrees.</summary>
		public HingeController RAJ4 { get; private set; }

		/// <summary>
		/// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with 
		/// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
		/// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
		/// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
		/// </summary>
		public HingeController LLJ1 { get; private set; }
		/// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -25 to 45 degrees.</summary>
		public HingeController LLJ2 { get; private set; }
		/// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
		public HingeController LLJ3 { get; private set; }
		/// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
		public HingeController LLJ4 { get; private set; }
		/// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
		public HingeController LLJ5 { get; private set; }
		/// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -45 to 25 degrees.</summary>
		public HingeController LLJ6 { get; private set; }

		/// <summary>
		/// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with 
		/// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
		/// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
		/// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
		/// </summary>
		public HingeController RLJ1 { get; private set; }
		/// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -45 to 25 degrees.</summary>
		public HingeController RLJ2 { get; private set; }
		/// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
		public HingeController RLJ3 { get; private set; }
		/// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
		public HingeController RLJ4 { get; private set; }
		/// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
		public HingeController RLJ5 { get; private set; }
		/// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -25 to 45 degrees.</summary>
		public HingeController RLJ6 { get; private set; }

		public IEnumerable<HingeController> AllHinges { get; private set; }

		public NaoBody() {
		    HJ1 = new HingeController("hj1", "he1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
		    HJ2 = new HingeController("hj2", "he2", Angle.FromDegrees(-45),  Angle.FromDegrees(45));

		    LAJ1 = new HingeController("laj1", "lae1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
		    LAJ2 = new HingeController("laj2", "lae2", Angle.FromDegrees(-1),   Angle.FromDegrees(95));
			LAJ3 = new HingeController("laj3", "lae3", Angle.FromDegrees(-120), Angle.FromDegrees(120));
			LAJ4 = new HingeController("laj4", "lae4", Angle.FromDegrees(-90),  Angle.FromDegrees(1));

			RAJ1 = new HingeController("raj1", "rae1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
			RAJ2 = new HingeController("raj2", "rae2", Angle.FromDegrees(-95),  Angle.FromDegrees(1));
			RAJ3 = new HingeController("raj3", "rae3", Angle.FromDegrees(-120), Angle.FromDegrees(120));
			RAJ4 = new HingeController("raj4", "rae4", Angle.FromDegrees(-1),   Angle.FromDegrees(90));

			LLJ1 = new HingeController("llj1", "lle1", Angle.FromDegrees(-90),  Angle.FromDegrees(1));
			LLJ2 = new HingeController("llj2", "lle2", Angle.FromDegrees(-25),  Angle.FromDegrees(45));
			LLJ3 = new HingeController("llj3", "lle3", Angle.FromDegrees(-25),  Angle.FromDegrees(100));
			LLJ4 = new HingeController("llj4", "lle4", Angle.FromDegrees(-130), Angle.FromDegrees(1));
			LLJ5 = new HingeController("llj5", "lle5", Angle.FromDegrees(-45),  Angle.FromDegrees(75));
			LLJ6 = new HingeController("llj6", "lle6", Angle.FromDegrees(-45),  Angle.FromDegrees(25));

			RLJ1 = new HingeController("rlj1", "rle1", Angle.FromDegrees(-90),  Angle.FromDegrees(1));
			RLJ2 = new HingeController("rlj2", "rle2", Angle.FromDegrees(-45),  Angle.FromDegrees(25));
			RLJ3 = new HingeController("rlj3", "rle3", Angle.FromDegrees(-25),  Angle.FromDegrees(100));
			RLJ4 = new HingeController("rlj4", "rle4", Angle.FromDegrees(-130), Angle.FromDegrees(1));
			RLJ5 = new HingeController("rlj5", "rle5", Angle.FromDegrees(-45),  Angle.FromDegrees(75));
			RLJ6 = new HingeController("rlj6", "rle6", Angle.FromDegrees(-25),  Angle.FromDegrees(45));

			AllHinges = new[] {
				HJ1, HJ2,
				RAJ1, RAJ2, RAJ3, RAJ4,
				LAJ1, LAJ2, LAJ3, LAJ4,
				RLJ1, RLJ2, RLJ3, RLJ4, RLJ5, RLJ6,
				LLJ1, LLJ2, LLJ3, LLJ4, LLJ5, LLJ6
			};
		}

		public HingeController GetHingeControllerForLabel(string label) {
		    return AllHinges.SingleOrDefault(
		        h => string.Equals(h.EffectorLabel, label, StringComparison.OrdinalIgnoreCase));
		}
	}
}