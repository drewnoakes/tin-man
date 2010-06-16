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
	///  (0,0,0)
	///        *-----------------+-----------------+
	///        |                 |                 |
	///        |                 |                 |
	///        |                 |                 |
	///       -* (0,4.95,0)      |                 +-     ---
	///        |                 |                 |        |
	///   Goal |                 O                 | Goal  2.1m
	///        |                 |                 |        |
	///       -+ (0,7.05,0)      |                 +-     ---
	///        |                 |                 |
	///        |                 |                 |
	///        |                 |                 |
	///        +-----------------+-----------------+
	///                                            (18,12,0)
	/// </pre>
	/// </remarks>
	public static class FieldGeometry
	{
	    private const double FieldHeight = 12.0;
	    private const double FieldWidth = 18.0;
	    private const double GoalWidth = 2.1;
	    private const double GoalHeight = 0.8;
	    
	    public static readonly Vector3 FlagLeftTopBasePosition = new Vector3(0, 0, 0);
	    public static readonly Vector3 FlagLeftBottomBasePosition = new Vector3(0, FieldHeight, 0);
	    public static readonly Vector3 FlagRightTopBasePosition = new Vector3(FieldWidth, 0, 0);
	    public static readonly Vector3 FlagRightBottomBasePosition = new Vector3(FieldWidth, FieldHeight, 0);
	    
	    public static readonly Vector3 GoalLeftTopBasePosition = new Vector3(0, FieldHeight/2 - GoalWidth/2, 0);
	    public static readonly Vector3 GoalLeftBottomBasePosition = new Vector3(0, FieldHeight/2 + GoalWidth/2, 0);
	    public static readonly Vector3 GoalRightTopBasePosition = new Vector3(FieldWidth, FieldHeight/2 - GoalWidth/2, 0);
	    public static readonly Vector3 GoalRightBottomBasePosition = new Vector3(FieldWidth, FieldHeight/2 + GoalWidth/2, 0);
	
	    public static readonly Vector3 GoalLeftTopTopPosition = new Vector3(0, FieldHeight/2 - GoalWidth/2, GoalHeight);
	    public static readonly Vector3 GoalLeftBottomTopPosition = new Vector3(0, FieldHeight/2 + GoalWidth/2, GoalHeight);
	    public static readonly Vector3 GoalRightTopTopPosition = new Vector3(FieldWidth, FieldHeight/2 - GoalWidth/2, GoalHeight);
	    public static readonly Vector3 GoalRightBottomTopPosition = new Vector3(FieldWidth, FieldHeight/2 + GoalWidth/2, GoalHeight);
	    
	    public static Vector3 GetLandmarkVector(Landmark landmark) {
	        switch (landmark) {
	            case Landmark.FlagLeftTop:      return FlagLeftTopBasePosition;
	            case Landmark.FlagLeftBottom:   return FlagLeftBottomBasePosition;
	            case Landmark.FlagRightTop:     return FlagRightTopBasePosition;
	            case Landmark.FlagRightBottom:  return FlagRightBottomBasePosition;
	            case Landmark.GoalLeftTop:      return GoalLeftTopTopPosition;
	            case Landmark.GoalLeftBottom:   return GoalLeftBottomTopPosition;
	            case Landmark.GoalRightTop:     return GoalRightTopTopPosition;
	            case Landmark.GoalRightBottom:  return GoalRightBottomTopPosition;
	            default: throw new ArgumentException("Unexpected Landmark enum value: " + landmark);
	        }
	    }
	    
	    public static bool IsInField(Vector3 vector) {
	        return vector.X > 0 && vector.X < FieldWidth
	            && vector.Y > 0 && vector.Y < FieldHeight;
	    }
	}
}
