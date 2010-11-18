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
// Created 06/05/2010 15:08

using System;

namespace TinMan
{
    // TODO come up with a better name for north/south goals than 'top' and 'bottom' as we now use top/bottom for goals and flags
    // TODO field dimensions may be variable, depending upon simulator (true but this information is only available via the monitor port, afaik)
    
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
    public static class FieldGeometry {
        /// <summary>The radius of the ball.</summary>
        public const double BallRadiusMetres = 0.04;
        /// <summary>The mass of the ball in kilograms.</summary>
        public const double BallMassKilograms = 0.026;
        /// <summary>The size of the field across its narrower dimension.</summary>
        public const double FieldYLength = 12.0;
        /// <summary>The size of the field across its wider dimension, from goal to goal.</summary>
        public const double FieldXLength = 18.0;
        /// <summary>The height above ground level of the simulated area of the field.</summary>
        /// <remarks>Note that the agent may not be positioned more than 20 above ground level.</remarks>
        public const double FieldZHeight = 40.0;
        /// <summary>The width of the goal as a player looks at it.</summary>
        public const double GoalYLength = 2.1;
        /// <summary>The height that the goal extends above the ground.</summary>
        public const double GoalZLength = 0.8;
        /// <summary>The depth of the goal.  That is, the distance from the opening of the goal on the field's side to the back of the net.</summary>
        public const double GoalXLength = 0.6;
        /// <summary></summary>
        public const double PenaltyAreaXLength = 1.8;
        /// <summary></summary>
        public const double PenaltyAreaYLength = 3.9;
        /// <summary></summary>
        public const double FreeKickDistance = 1.3;
        /// <summary></summary>
        public const double FreeKickMoveDistance = 1.5;
        /// <summary></summary>
        public const double GoalKickDistance = 1.0;

        /// <summary>The location, in global coordinates, of the base of the top-left (north-west) flag.</summary>
        public static readonly Vector3 FlagLeftTopPosition;
        /// <summary>The location, in global coordinates, of the base of the bottom-left (south-west) flag.</summary>
        public static readonly Vector3 FlagLeftBottomPosition;
        /// <summary>The location, in global coordinates, of the base of the top-right (north-east) flag.</summary>
        public static readonly Vector3 FlagRightTopPosition;
        /// <summary>The location, in global coordinates, of the base of the bottom-right (south-east) flag.</summary>
        public static readonly Vector3 FlagRightBottomPosition;

        /// <summary>The location, in global coordinates, of the top of the top-left (north-west) goal post.</summary>
        public static readonly Vector3 GoalLeftTopPosition;
        /// <summary>The location, in global coordinates, of the top of the bottom-left (south-west) goal post.</summary>
        public static readonly Vector3 GoalLeftBottomPosition;
        /// <summary>The location, in global coordinates, of the top of the top-right (north-east) goal post.</summary>
        public static readonly Vector3 GoalRightTopPosition;
        /// <summary>The location, in global coordinates, of the top of the bottom-right (south-east) goal post.</summary>
        public static readonly Vector3 GoalRightBottomPosition;
        
        static FieldGeometry() {
            const double flagHeight = 0; // 0.375f;  // TODO verify that the spotted point of the flag is at ground level (Z==0)
            const double goalPostX = FieldXLength/2; // TODO verify that the flag is exactly on the corner of the field
            const double goalPostY = GoalYLength/2;  // TODO verify this -- the height of the point spotted on the goal is halfway up it?
        
            // Using global coordinate system.  (0,0) is the exact center of the field.
            
            FlagLeftTopPosition     = new Vector3(-FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagRightTopPosition    = new Vector3(+FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagLeftBottomPosition  = new Vector3(-FieldXLength/2, -FieldYLength/2, flagHeight);
            FlagRightBottomPosition = new Vector3(+FieldXLength/2, -FieldYLength/2, flagHeight);
            GoalLeftTopPosition     = new Vector3(-goalPostX, +GoalYLength/2, goalPostY);
            GoalRightTopPosition    = new Vector3(+goalPostX, +GoalYLength/2, goalPostY);
            GoalLeftBottomPosition  = new Vector3(-goalPostX, -GoalYLength/2, goalPostY);
            GoalRightBottomPosition = new Vector3(+goalPostX, -GoalYLength/2, goalPostY);
        }
        
        /// <summary>Gets the location of a landmark in global coordinates.</summary>
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
        
        /// <summary>
        /// Calculates whether the given point is within the field.  The Z vector component is not considered.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static bool IsInField(Vector3 vector) {
            return vector.X >= -FieldXLength/2 && vector.X <= FieldXLength/2
                && vector.Y > -FieldYLength/2 && vector.Y < FieldYLength/2;
        }
        
        private static readonly Random _random = new Random();
        
        public static Vector3 GetRandomPosition(FieldSide side) {
            double x1 = -FieldXLength/2;
            double x2 = +FieldXLength/2;
            if (side==FieldSide.Left)
                x2 = 0;
            else if (side==FieldSide.Right)
                x1 = 0;
            
            double z =  _random.NextDouble() * FieldZHeight;
            double y = (_random.NextDouble() * FieldYLength) - (FieldYLength/2);
            double x = (_random.NextDouble() * (x2 - x1)) + x1;
            
            return new Vector3(x, y, z);
        }
    }
}
