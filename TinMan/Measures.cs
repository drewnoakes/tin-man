﻿#region License
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
using System.Configuration;

namespace TinMan
{
    // TODO come up with a better name for north/south goals than 'top' and 'bottom' as we now use top/bottom for goals and flags
    
    /// <summary>
    /// Holds information about the dimensions and geometry of the soccer field upon which the
    /// robots are playing.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>The dimensions of the soccer field are 21 by 14 meters.</item>
    ///     <item>The center spot has a radius of 4.5 meters.</item>
    ///     <item>Each goal is 2.1 by 0.6 meter with a height of 0.8 meters.</item>
    ///     <item>The soccer field is surrounded by a border of 10 meters in each direction.</item>
    ///     <item>Space outside this border area is not reachable by an agent.</item>
    ///     <item>The soccer ball has a radius of 0.04 meters and a mass of 26 grams.</item>
    /// </list>
    /// <pre>
    ///        |--------------- 21 m --------------|
    ///  
    ///  (-11.5,-7)
    ///        *-----------------+-----------------+            ---
    ///        |                 |                 |              |
    ///        |                 |                 |              |
    ///        |                 |                 |              |
    ///       -* (-11.5,-1.05)   |                 +-     ---     |
    ///        |                 |                 |        |     |
    ///   Goal |                 O (0,0)           | Goal  2.1m  14m
    ///        |                 |                 |        |     |
    ///       -+ (-11.5,1.05)    |                 +-     ---     |
    ///        |                 |                 |              |
    ///        |                 |                 |              |
    ///        |                 |                 |              |
    ///        +-----------------+-----------------+            ---
    ///                                            (11.5,7)
    /// </pre>
    /// </remarks>
    public sealed class Measures
    {
        private static readonly Log _log = Log.Create();
        private readonly Random _random = new Random();

        /// <summary>The radius of the ball in metres.</summary>
        public double BallRadiusMetres { get; private set; }
        /// <summary>The mass of the ball in kilograms.</summary>
        public double BallMassKilograms { get; private set; }
        /// <summary>The size of the field across its narrower dimension.</summary>
        public double FieldYLength { get; private set; }
        /// <summary>The size of the field across its wider dimension, from goal to goal.</summary>
        public double FieldXLength { get; private set; }
        /// <summary>The x-coordinate value at the left edge of the field.</summary>
        public double FieldXLeft { get; private set; }
        /// <summary>The x-coordinate value at the right edge of the field.</summary>
        public double FieldXRight { get; private set; }
        /// <summary>The x-coordinate value at the left edge of the field.</summary>
        public double FieldYTop { get; private set; }
        /// <summary>The x-coordinate value at the right edge of the field.</summary>
        public double FieldYBottom { get; private set; }
        /// <summary>The height above ground level of the simulated area of the field.</summary>
        /// <remarks>Note that the agent may not be positioned more than 20 above ground level.</remarks>
        public double FieldZHeight { get; private set; }
        /// <summary>The width of the goal as a player looks at it.</summary>
        public double GoalYLength { get; private set; }
        /// <summary>The height that the goal extends above the ground.</summary>
        public double GoalZLength { get; private set; }
        /// <summary>The depth of the goal.  That is, the distance from the opening of the goal on the field's side to the back of the net.</summary>
        public double GoalXLength { get; private set; }
        /// <summary></summary>
        public double PenaltyAreaXLength { get; private set; }
        /// <summary></summary>
        public double PenaltyAreaYLength { get; private set; }
        /// <summary></summary>
        public double FreeKickDistance { get; private set; }
        /// <summary></summary>
        public double FreeKickMoveDistance { get; private set; }
        /// <summary></summary>
        public double GoalKickDistance { get; private set; }

        /// <summary>The location, in global coordinates, of the base of the top-left (north-west) flag.</summary>
        public Vector3 FlagLeftTopPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the base of the bottom-left (south-west) flag.</summary>
        public Vector3 FlagLeftBottomPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the base of the top-right (north-east) flag.</summary>
        public Vector3 FlagRightTopPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the base of the bottom-right (south-east) flag.</summary>
        public Vector3 FlagRightBottomPosition { get; private set; }

        /// <summary>The location, in global coordinates, of the top of the top-left (north-west) goal post.</summary>
        public Vector3 GoalLeftTopPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the top of the bottom-left (south-west) goal post.</summary>
        public Vector3 GoalLeftBottomPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the top of the top-right (north-east) goal post.</summary>
        public Vector3 GoalRightTopPosition { get; private set; }
        /// <summary>The location, in global coordinates, of the top of the bottom-right (south-east) goal post.</summary>
        public Vector3 GoalRightBottomPosition { get; private set; }
        
        public Measures()
        {
            BallRadiusMetres = GetValue("BallRadiusMetres", 0.04);
            BallMassKilograms = GetValue("BallMassKilograms", 0.026);
            FieldYLength = GetValue("FieldYLength", 14);
            FieldXLength = GetValue("FieldXLength", 21);
            FieldZHeight = GetValue("FieldZHeight", 40);
            GoalYLength = GetValue("GoalYLength", 2.1);
            GoalZLength = GetValue("GoalZLength", 0.8);
            GoalXLength = GetValue("GoalXLength", 0.6);
            PenaltyAreaXLength = GetValue("PenaltyAreaXLength", 1.8);
            PenaltyAreaYLength = GetValue("PenaltyAreaYLength", 3.9);
            FreeKickDistance = GetValue("FreeKickDistance", 1.3);
            FreeKickMoveDistance = GetValue("FreeKickMoveDistance", 1.5);
            GoalKickDistance = GetValue("GoalKickDistance", 1.0);
            
            // update derived properties
            
            FieldXLeft = -FieldXLength / 2;
            FieldXRight = FieldXLength / 2;
            FieldYTop    =  FieldYLength / 2;
            FieldYBottom = -FieldYLength / 2;
            
            const double flagHeight = 0;
            double goalPostX = FieldXLength/2;
        
            // Using global coordinate system.  (0,0) is the exact center of the field.
            FlagLeftTopPosition     = new Vector3(-FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagRightTopPosition    = new Vector3(+FieldXLength/2, +FieldYLength/2, flagHeight);
            FlagLeftBottomPosition  = new Vector3(-FieldXLength/2, -FieldYLength/2, flagHeight);
            FlagRightBottomPosition = new Vector3(+FieldXLength/2, -FieldYLength/2, flagHeight);
            GoalLeftTopPosition     = new Vector3(-goalPostX, +GoalYLength/2, GoalZLength);
            GoalRightTopPosition    = new Vector3(+goalPostX, +GoalYLength/2, GoalZLength);
            GoalLeftBottomPosition  = new Vector3(-goalPostX, -GoalYLength/2, GoalZLength);
            GoalRightBottomPosition = new Vector3(+goalPostX, -GoalYLength/2, GoalZLength);
        }
        
        private static double GetValue(string keySuffix, double defaultValue)
        {
            var key = "TinMan.Measures." + keySuffix;
            var valueString = ConfigurationManager.AppSettings[key];
            if (valueString == null)
                return defaultValue;
            double parsedValue;
            if (!double.TryParse(valueString, out parsedValue))
            {
                _log.Warn("Unable to parse config key {0} as a double.  Using default value of {1} instead.", valueString, defaultValue);
                return defaultValue;
            }
            return parsedValue;
        }
        
        /// <summary>Gets the location of a landmark in global coordinates.</summary>
        /// <param name="landmark"></param>
        /// <returns></returns>
        public Vector3 GetLandmarkPointGlobal(Landmark landmark) 
        {
            switch (landmark) 
            {
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
        public bool IsInField(Vector3 vector)
        {
            return vector.X >= -FieldXLength/2 && vector.X <= FieldXLength/2
                && vector.Y >  -FieldYLength/2 && vector.Y <  FieldYLength/2;
        }
        
        public Vector3 GetRandomPosition(FieldSide side)
        {
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