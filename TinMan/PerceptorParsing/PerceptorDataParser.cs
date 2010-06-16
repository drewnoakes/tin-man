/*
 * Created by Drew, 07/05/2010 02:52.
 */
using System;

namespace Drew.RoboCup
{
	/*
	 * GENERAL PERCEPTORS
	 * 
	 * GyroRate Perceptor
	 *    Format:  (GYR (n <name>) (rt <x> <y> <z>))
	 *    Example: (GYR (n torso) (rt 0.01 0.07 0.46))
	 *    These angles describe orientation of the body with respect to the global coordinate system.
	 * HingeJoint Perceptor
	 *    Format:  (HJ (n <name>) (ax <ax>))
	 *    Example: (HJ (n laj3) (ax -1.02))
	 *    Zero degrees means straightly aligned
	 * UniversalJoint Perceptor
	 *    Format:  (UJ (n <name>) (ax1 <ax1>) (ax2 <ax2>))
	 *    Example: (UJ (n laj1 2) (ax1 -1.32) (ax2 2.00))
	 *    Zero degrees means straightly aligned
	 * Touch Perceptor
	 *    Format:  (TCH n <name> val 0|1)
	 *    Example: (TCH n bumper val 1)
	 * ForceResistance Perceptor
	 *    Format:  (FRP (n <name>) (c <px> <py> <pz>) (f <fx> <fy> <fz>))
	 *    Example: (FRP (n lf) (c -0.14 0.08 -0.05) (f 1.12 -0.26 13.07))
	 *    Currently only available in feet (lf/rf).  Point on body of force, with force itself.
	 * Accelerometer
	 *    Format:  (ACC (n <name>) (a <x> <y> <z>))
	 *    Example: (ACC (n torso) (a 0.00 0.00 9.81))
	 * 
	 * SOCCER PERCEPTORS
	 * 
	 * Vision Perceptor
	 *    Format:  (See (<name> (pol <distance> <angle1> <angle2>))
	 *                  (P (team <teamname>) (id <playerID>) (pol <distance> <angle1> <angle2>))
	 *             )
	 *    Example: (See (F1L (pol 19.11 111.69 -9.57))
	 *                  (F2L (pol 16.41 -115.88 -11.15))
	 *                  (F1R (pol 46.53 22.04 -3.92))
	 *                  (F2R (pol 45.49 -18.74 -4.00))
	 *                  (G1L (pol 9.88 139.29 -21.07))
	 *                  (G2L (pol 8.40 -156.91 -25.00))
	 *                  (G1R (pol 43.56 7.84 -4.68))
	 *                  (G2R (pol 43.25 -4.10 -4.71))
	 *                  (B (pol 18.34 4.66 -9.90))
	 *                  (P (team RoboLog) (id 1) (pol 37.50 16.15 -0.00))
	 *             )
	 * GameState Perceptor
	 *    Format:  (GS (t <time>) (pm <playmode>))
	 *    Example: (GS (t 0.00) (pm BeforeKickOff))
	 *    Simulation time, starting at zero for kickoff of either half.
	 *    First time this message arrives, it includes information about ball weight and field size.
	 *    Possible playmodes (from ./plugin/soccer/soccertypes.h):
	 *        "BeforeKickOff", "KickOff Left", "KickOff Right", "PlayOn", "KickIn Left", "KickIn Right",
	 *        "corner kick left", "corner kick right", "goal kick left", "goal kick right", "offside left",
	 *        "offside right", "GameOver", "Goal Left", "Goal Right", "free kick left", "free kick right", "unknown"
	 * AgentState Perceptor
	 *    Format:  (AgentState (temp <degree>) (battery <percentile>))
	 *    Example: (AgentState (temp 48) (battery 75))
	 * Hear Perceptor
	 * 	  Format:  (hear <time> 'self'|<direction> <message>)
	 *    Example: (hear 12.3 self ``helloworld'')
	 *    Message may consist of characters from the ASCII printing
	 *    character subset [0x20; 0x7E], among which the alphanumerical symbols and mathematical
	 *    operators can be found for example. Three characters from this range are, however, excluded:
	 *    the white space character and the normal brackets ( and ).
	 *    Messages are restricted to a maximal length (currently 20 bytes).
	 *    Messages shouted from beyond a maximal distance (currently 50:0 meters) cannot be heard.
	 *    Each player has the maximal capacity of one heard message by a specific team every two sensor cycles (thus every 0:4 seconds per team).
	 */
	public sealed class PerceptorDataParser
	{
		/*
        (time (now 417.67))
        (GS (t 0.00) (pm BeforeKickOff))
        (GYR (n torso) (rt 0.00 -0.00 0.00))
        (ACC (n torso) (a 0.00 0.00 8.83))
        (HJ (n hj1) (ax 0.00))
        (HJ (n hj2) (ax -0.00))
        (See (F1L (pol 11.52 52.16 -8.16))
             (F1R (pol 11.51 -52.23 -8.25))
             (B (pol 1.89 -0.00 -57.87))
             (P (team RoboLog) (id 1) (pol 37.50 16.15 -0.00))
        )
        (HJ (n raj1) (ax 0.00))
        (HJ (n raj2) (ax 0.00))
        (HJ (n raj3) (ax -0.00))
        (HJ (n raj4) (ax 0.00))
        (HJ (n laj1) (ax 0.00))
        (HJ (n laj2) (ax -0.00))
        (HJ (n laj3) (ax -0.00))
        (HJ (n laj4) (ax -0.00))
        (HJ (n rlj1) (ax 0.00))
        (HJ (n rlj2) (ax 0.00))
        (HJ (n rlj3) (ax -0.00))
        (HJ (n rlj4) (ax -0.00))
        (HJ (n rlj5) (ax -0.00))
        (HJ (n rlj6) (ax 0.00))
        (HJ (n llj1) (ax 0.00))
        (HJ (n llj2) (ax 0.00))
        (HJ (n llj3) (ax -0.00))
        (HJ (n llj4) (ax -0.00))
        (HJ (n llj5) (ax -0.00))
        (HJ (n llj6) (ax -0.00))
		 */
		public PerceptorState Parse(string data)
		{
//			var jointNames = new[] {
//		        "he1", "he2",
//				"lae1", "lae2", "lae3", "lae4",
//	    	    "lle1", "lle2", "lle3", "lle4", "lle5", "lle6",
//				"rae1", "rae2", "rae3", "rae4",
//	    	    "rle1", "rle2", "rle3", "rle4", "rle5", "rle6"
//	    	};
		    
		    int index = 0;
		    
		    while (index < data.Length)
		    {
		        while (index < data.Length && data[index]!='(')
		            index++;
		        
		        var startIndex = index + 1;
		        var length = 0;
		        while (data[index]!=' ' && data[index]!=')' && data[index]!='(')
		        {
		            index++;
		            length++;
		        }
		        index++;
		        var label = data.Substring(startIndex, length);
		        
		        switch (label)
		        {
		            case "time":
		                break;
		            case "GS":
		                break;
		            case "HJ":
		                break;
		            default:
		                Console.Error.WriteLine("Ignoring unexpected label in data '{0}' from server.", label);
		                break;
		        }
		        
		        index++;
		    }
			
			// Assumes always the first item and with the same formatting...
			var secondsStr = data.Substring(11, data.IndexOf(')', 11) - 11);
			TimeSpan time = TimeSpan.FromSeconds(double.Parse(secondsStr));

			PlayMode playMode = PlayMode.GameOver;
			//var gyroRates = new List<

			return new PerceptorState(time, playMode, null, null, null, null, null, null, null, null, null, null, RadialPosition.Zero, 0, 0, null);
		}
	}
}
