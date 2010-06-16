/*
 * Created by Drew, 12/06/2010 03:01.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinMan
{
    public interface IBodyController<TController,TControllerInput>
        where TController : IHingeController<TControllerInput> {
        TController GetHingeControllerByEffectorLabel(string effectorLabel);
        void Step(TimeSpan simulationTime);
    }
    
    public sealed class NaoBodyController<TController,TControllerInput> 
        : IBodyController<TController,TControllerInput>
            where TController : IHingeController<TControllerInput> {
        
	    /// <summary>Neck joint.  Allows the head (and camera) to be panned left and right.  Zero degrees looks directly ahead.  Angles range -120 to 120 degrees.</summary>
	    public TController HJ1 { get; private set; }
	    /// <summary>Head joint.  Allows the head (and camera) to be tilted up and down. Zero degrees looks horizontally.  Angles range -45 to 45 degrees.</summary>
	    public TController HJ2 { get; private set; }
	
	    /// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
	    public TController LAJ1 { get; private set; }
	    /// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -1 to 95 degrees.</summary>
	    public TController LAJ2 { get; private set; }
	    /// <summary>Allows the arm to rotate along its length between the shoulder and the elbow.  Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
	    public TController LAJ3 { get; private set; }
	    /// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -90 to 1 degrees.</summary>
	    public TController LAJ4 { get; private set; }
	
	    /// <summary>Shoulder.  Allows the entire arm to rotate within a circle extending in front of, above, behind and below the shoulder.  Zero degrees points straight ahead.  Angles range -120 to 120 degrees.  Positive values raise the arm upwards above the head.</summary>
	    public TController RAJ1 { get; private set; }
	    /// <summary>Shoulder.  Allows the entire arm to extend such that the hand of a straighenend arm moves from the hip outwards away from the body.  Zero degrees keeps the arm within the circle governed by arm joint one.  Angles range -95 to 1 degrees.</summary>
	    public TController RAJ2 { get; private set; }
	    /// <summary>Allows the arm to rotate along its length between the shoulder and the elbow. Angles range -120 to 120 degrees. At zero degrees the palm faces down if the arm is extended in front of the body.  Positive values rotate the hand clockwise (as viewed by the robot).</summary>
	    public TController RAJ3 { get; private set; }
	    /// <summary>Bends the arm at the elbow.  At zero degrees the arm is straight.  Angles range -1 to 90 degrees.</summary>
	    public TController RAJ4 { get; private set; }
	
	    /// <summary>
	    /// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with
	    /// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
	    /// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
	    /// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
	    /// </summary>
	    public TController LLJ1 { get; private set; }
	    /// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -25 to 45 degrees.</summary>
	    public TController LLJ2 { get; private set; }
	    /// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
	    public TController LLJ3 { get; private set; }
	    /// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
	    public TController LLJ4 { get; private set; }
	    /// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
	    public TController LLJ5 { get; private set; }
	    /// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -45 to 25 degrees.</summary>
	    public TController LLJ6 { get; private set; }
	
	    /// <summary>
	    /// Hip joint, allowing an extended leg to swing out to the side, such that it is parallel to the ground with
	    /// the toes pointing upwards.  Unlike other joints, the first left and right leg joints are oriented at 45
	    /// degrees to the coordinate system, and the yaw pitch of both joints is locked such that they're both equal
	    /// at all times. At zero degrees, the legs are parallel in a standing posture.  Angles range -90 to 1 degree.
	    /// </summary>
	    public TController RLJ1 { get; private set; }
	    /// <summary>Tilts the leg such that the hip is pushed sideways when standing.  At zero degrees, the leg's line is parallel to the torso's z axis, if the prior joint is also zero.  Angles range -45 to 25 degrees.</summary>
	    public TController RLJ2 { get; private set; }
	    /// <summary>Bends the leg from the hip such that the knee lifts forward towards the chest.  At zero degrees, the leg is extended straight down.  Angles range -25 to 100 degrees.</summary>
	    public TController RLJ3 { get; private set; }
	    /// <summary>Bends the leg at the knee such that the calf approaches the back of the thigh.  At zero degrees, the leg is straight.  Angles range -130 to 1 degrees.</summary>
	    public TController RLJ4 { get; private set; }
	    /// <summary>Bends the ankle such that toes lift towards the knee (positive angles) or point away down the length of the leg (negative values).  At zero degrees, the line running the length of the sole of the foot is perpendicular to the calf.  Angles range -45 to 75 degrees.</summary>
	    public TController RLJ5 { get; private set; }
	    /// <summary>Rolls the foot in and out (supine/pronate).  At zero degrees, the line running the width of the sole of the foot is perpendicular to the calf.  Angles range -25 to 45 degrees.</summary>
	    public TController RLJ6 { get; private set; }
	    
	    private readonly IEnumerable<TController> AllHingeControllers;
	
	    public NaoBodyController(NaoBody body, Func<Hinge,TController> controllerConstructor) {
	        HJ1 = controllerConstructor(body.HJ1);
	        HJ2 = controllerConstructor(body.HJ2);
	
	        LAJ1 = controllerConstructor(body.LAJ1);
	        LAJ2 = controllerConstructor(body.LAJ2);
	        LAJ3 = controllerConstructor(body.LAJ3);
	        LAJ4 = controllerConstructor(body.LAJ4);
	
	        RAJ1 = controllerConstructor(body.RAJ1);
	        RAJ2 = controllerConstructor(body.RAJ2);
	        RAJ3 = controllerConstructor(body.RAJ3);
	        RAJ4 = controllerConstructor(body.RAJ4);
	
	        LLJ1 = controllerConstructor(body.LLJ1);
	        LLJ2 = controllerConstructor(body.LLJ2);
	        LLJ3 = controllerConstructor(body.LLJ3);
	        LLJ4 = controllerConstructor(body.LLJ4);
	        LLJ5 = controllerConstructor(body.LLJ5);
	        LLJ6 = controllerConstructor(body.LLJ6);
	
	        RLJ1 = controllerConstructor(body.RLJ1);
	        RLJ2 = controllerConstructor(body.RLJ2);
	        RLJ3 = controllerConstructor(body.RLJ3);
	        RLJ4 = controllerConstructor(body.RLJ4);
	        RLJ5 = controllerConstructor(body.RLJ5);
	        RLJ6 = controllerConstructor(body.RLJ6);
	        
	        AllHingeControllers = new[] {
				HJ1, HJ2,
				RAJ1, RAJ2, RAJ3, RAJ4,
				LAJ1, LAJ2, LAJ3, LAJ4,
				RLJ1, RLJ2, RLJ3, RLJ4, RLJ5, RLJ6,
				LLJ1, LLJ2, LLJ3, LLJ4, LLJ5, LLJ6
			};
	    }
	    
	    public void Step(TimeSpan simulationTime) {
	        foreach (var controller in AllHingeControllers)
	            controller.Step(simulationTime);
	    }
	    
	    public TController GetHingeControllerByEffectorLabel(string effectorLabel) {
		    return AllHingeControllers.SingleOrDefault(
		        c => string.Equals(c.Hinge.EffectorLabel, effectorLabel, StringComparison.OrdinalIgnoreCase));
	    }
	}
}
