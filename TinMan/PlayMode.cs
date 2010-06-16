/*
 * Created by Drew, 07/05/2010 02:18.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
	public enum PlayMode
	{
	    Unknown = -1,
	    // the order of the first 3 play modes should not be changed.
	    BeforeKickOff = 0,      /*!< before_kick_off:   before the match  */
	    KickOffLeft = 1,       /*!< kick_off_left:     kick off for the left team  */
	    KickOffRight = 2,      /*!< kick_off_right:    kick off for the right team */
	    PlayOn,                 /*!< play_on:           regular game play */
	    KickInLeft,
	    KickInRight,
	    CornerKickLeft,       /*!< corner_kick_l:     corner kick left team   */
	    CornerKickRight,      /*!< corner_kick_r:     corner kick right team  */
	    GoalKickLeft,         /*!< goal_kick_l:       goal kick for left team */
	    GoalKickRIght,        /*!< goal_kick_r:       goal kick for right team*/
	    OffsideLeft,           /*!< offside_l:         offside for left team   */
	    OffsideRight,          /*!< offside_r:         offside for right team  */
	    GameOver,
	    GoalLeft,
	    GoalRight,
	    FreeKickLeft,         /*!< free_kick_l:       free kick for left team */
	    FreeKickRight,        /*!< free_kick_r:       free kick for right team*/
	    None                    /*!< no play mode, this must be the last entry */
	}
	
	public static class PlayModeUtil
	{
			private static readonly Dictionary<string, PlayMode> _playModeByStringCode = new Dictionary<string, PlayMode>
			{
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

		public static bool TryParse(string modeStr, out PlayMode playMode)
		{
			return _playModeByStringCode.TryGetValue(modeStr, out playMode);
		}
	}
}
