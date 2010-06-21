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
// Created 07/05/2010 02:18

using System;
using System.Collections.Generic;

namespace TinMan
{
    /// <summary>
    /// Enumeration of all possible modes that a simulated game of soccer can be in.
    /// </summary>
    public enum PlayMode {
        
        // IMPORTANT -- DO NOT REORDER THESE ITEMS
        
        /// <summary>
        /// This value is specific to TinMan and won't be returned by the server.
        /// TinMan uses it to indicate that no value has been received from the server.
        /// </summary>
        Unknown = -1,
        /// <summary>Before the match.</summary>
        BeforeKickOff = 0,
        /// <summary>Kick off for the left team.</summary>
        KickOffLeft = 1,
        /// <summary>Kick off for the right team.</summary>
        KickOffRight = 2,
        /// <summary>Regular game play.</summary>
        PlayOn,
        /// <summary></summary>
        KickInLeft,
        /// <summary></summary>
        KickInRight,
        /// <summary>Corner kick left team.</summary>
        CornerKickLeft,
        /// <summary>Corner kick right team.</summary>
        CornerKickRight,
        /// <summary>Goal kick for left team.</summary>
        GoalKickLeft,
        /// <summary>Goal kick for right team.</summary>
        GoalKickRIght,
        /// <summary>Offside for the left team.</summary>
        OffsideLeft,
        /// <summary>Offside for the right team.</summary>
        OffsideRight,
        /// <summary></summary>
        GameOver,
        /// <summary></summary>
        GoalLeft,
        /// <summary></summary>
        GoalRight,
        /// <summary>Free kick for the left team.</summary>
        FreeKickLeft,
        /// <summary>Free kick for the right team.</summary>
        FreeKickRight,
        /// <summary>No play mode exists.</summary>
        None
    }
    
    /// <summary>
    /// A collection of utility methods for converting server play modes (strings) to TinMan ones (enums).
    /// Most users of TinMan won't need to use this type, as TinMan only uses the <see cref="PlayMode"/> enum
    /// in its APIs.
    /// </summary>
    public static class PlayModeUtil {
        private static readonly Dictionary<string, PlayMode> _playModeByStringCode;
        private static readonly Dictionary<PlayMode, string> _stringCodeByPlayMode;
        
        static PlayModeUtil() {
            _playModeByStringCode = new Dictionary<string, PlayMode> {
                { "BeforeKickOff", PlayMode.BeforeKickOff },
                { "KickOff_Left", PlayMode.KickOffLeft },
                { "KickOff_Right", PlayMode.KickOffRight },
                { "PlayOn", PlayMode.PlayOn },
                { "KickIn_Left", PlayMode.KickInLeft },
                { "KickIn_Right", PlayMode.KickInRight },
                { "corner_kick_left", PlayMode.CornerKickLeft },
                { "corner_kick_right", PlayMode.CornerKickRight },
                { "goal_kick_left", PlayMode.GoalKickLeft },
                { "goal_kick_right", PlayMode.GoalKickRIght },
                { "offside_left", PlayMode.OffsideLeft },
                { "offside_right", PlayMode.OffsideRight },
                { "GameOver", PlayMode.GameOver },
                { "Goal_Left", PlayMode.GoalLeft },
                { "Goal_Right", PlayMode.GoalRight },
                { "free_kick_left", PlayMode.FreeKickLeft },
                { "free_kick_right", PlayMode.FreeKickRight },
                { "unknown", PlayMode.None }
            };
            
            _stringCodeByPlayMode = new Dictionary<PlayMode, string>();
            foreach (var pair in _playModeByStringCode)
                _stringCodeByPlayMode[pair.Value] = pair.Key;
        }

        /// <summary>Gets the enum value for the specified server play mode string.</summary>
        /// <param name="modeStr"></param>
        /// <param name="playMode"></param>
        /// <returns></returns>
        public static bool TryParse(string modeStr, out PlayMode playMode) {
            return _playModeByStringCode.TryGetValue(modeStr, out playMode);
        }
        
        /// <summary>Gets the string used by the server for the specified play mode enum value.</summary>
        /// <param name="playMode"></param>
        /// <returns></returns>
        public static string GetServerString(this PlayMode playMode) {
            string str;
            if (!_stringCodeByPlayMode.TryGetValue(playMode, out str))
                throw new ArgumentException("Unexpected PlayMode enum value: " + playMode);
            return str;
        }
    }
}
