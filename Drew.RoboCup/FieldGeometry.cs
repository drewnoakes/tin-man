/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    // TODO come up with a better name for north/south goals than 'top' and 'bottom' as we now use top/bottom for goals and flags
    
	/// <summary>
	/// Holds information about the dimensions and geometry of the soccer field upon which the
	/// robots are playing.
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
	///     <item>The dimensions of the soccer field are 18 by 12 meters.</item>
	///     <item>The center spot has a radius of 4.5 meters.</item>
	///     <item>Each goal is 2.1 by 0.6 meter with a height of 0.8 meters.</item>
	///     <item>The soccer field is surrounded by a border of 10 meters in each direction.</item>
	///     <item>Space outside this border area is not reachable by an agent.</item>
	///     <item>The soccer ball has a radius of 0.04 meter and a mass of 26 grams.</item>
	/// </list>
	/// <pre>
	///        |--------------- 18 m --------------|
	///  
	///  (-9,-6)
	///        *-----------------+-----------------+            ---
	///        |                 |                 |              |
	///        |                 |                 |              |
	///        |                 |                 |              |
	///       -* (-9,-1.05)      |                 +-     ---     |
	///        |                 |                 |        |     |
	///   Goal |                 O (0,0)           | Goal  2.1m  12m
	///        |                 |                 |        |     |
	///       -+ (-9,1.05)       |                 +-     ---     |
	///        |                 |                 |              |
	///        |                 |                 |              |
	///        |                 |                 |              |
	///        +-----------------+-----------------+            ---
	///                                            (9,6)
	/// </pre>
	/// </remarks>
	public static class FieldGeometry
	{
	    // TODO field dimensions may be variable, depending upon simulator
	    private const double FieldYLength = 12.0;
	    private const double FieldXLength = 18.0;
	    private const double GoalWidth = 2.1;
	    private const double GoalHeight = 0.8;
	    
	    public static readonly Vector3 FlagLeftTopPosition;
	    public static readonly Vector3 FlagLeftBottomPosition;
	    public static readonly Vector3 FlagRightTopPosition;
	    public static readonly Vector3 FlagRightBottomPosition;
	    	
	    public static readonly Vector3 GoalLeftTopPosition;
	    public static readonly Vector3 GoalLeftBottomPosition;
	    public static readonly Vector3 GoalRightTopPosition;
	    public static readonly Vector3 GoalRightBottomPosition;
	    
	    static FieldGeometry() {
            const double flagHeight = 0; // 0.375f;     // TODO verify that the spotted point of the flag is at ground level (Z==0)
            const double goalFlagX = FieldXLength/2;    // TODO verify that the flag is exactly on the corner of the field
            const double goalFlagHeight = GoalHeight/2; // TODO verify this -- the height of the point spotted on the goal is halfway up it?
        
            // Using global coordinate system.  (0,0) is the exact center of the field.
            
            FlagLeftTopPosition     = new Vector3(-FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagRightTopPosition    = new Vector3(+FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagLeftBottomPosition  = new Vector3(-FieldXLength/2, -FieldYLength/2, flagHeight);
            FlagRightBottomPosition = new Vector3(+FieldXLength/2, -FieldYLength/2, flagHeight);
            GoalLeftTopPosition     = new Vector3(-goalFlagX, +GoalWidth/2, goalFlagHeight);
            GoalRightTopPosition    = new Vector3(+goalFlagX, +GoalWidth/2, goalFlagHeight);
            GoalLeftBottomPosition  = new Vector3(-goalFlagX, -GoalWidth/2, goalFlagHeight);
            GoalRightBottomPosition = new Vector3(+goalFlagX, -GoalWidth/2, goalFlagHeight);
	    }
	    
	    /// <summary>
	    /// Gets the location of a landmark in global coordinates.
	    /// </summary>
	    /// <param name="landmark"></param>
	    /// <returns></returns>
	    public static Vector3 GetLandmarkPointGlobal(Landmark landmark) {
	        switch (landmark) {
	            case Landmark.FlagLeftTop:      return FlagLeftTopPosition;
	            case Landmark.FlagLeftBottom:   return FlagLeftBottomPosition;
	            case Landmark.FlagRightTop:     return FlagRightTopPosition;
	            case Landmark.FlagRightBottom:  return FlagRightBottomPosition;
	            case Landmark.GoalLeftTop:      return GoalLeftTopPosition;
	            case Landmark.GoalLeftBottom:   return GoalLeftBottomPosition;
	            case Landmark.GoalRightTop:     return GoalRightTopPosition;
	            case Landmark.GoalRightBottom:  return GoalRightBottomPosition;
	            default: throw new ArgumentException("Unexpected Landmark enum value: " + landmark);
	        }
	    }
	    
	    public static bool IsInField(Vector3 vector) {
	        return vector.X >= -FieldXLength/2 && vector.X <= FieldXLength/2
	            && vector.Y > -FieldYLength/2 && vector.Y < FieldYLength/2;
	    }
	}
}
