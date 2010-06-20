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
// Created 07/05/2010 03:15

namespace TinMan
{
	/// <summary>
	/// Enumeration of fixed landmarks around the field that may be observed by an agent's
	/// vision perceptor.
	/// </summary>
	public enum Landmark {
	    /// <summary>The north-west flag.  Referred to as FL1 by the server.</summary>
	    FlagLeftTop,
	    /// <summary>The south-west flag.  Referred to as FL2 by the server.</summary>
	    FlagLeftBottom,
	    /// <summary>The north-east flag.  Referred to as FR1 by the server.</summary>
	    FlagRightTop,
	    /// <summary>The south-east flag.  Referred to as FR2 by the server.</summary>
	    FlagRightBottom,
	    
	    /// <summary>The north-west goal.  Referred to as GL1 by the server.</summary>
	    GoalLeftTop,
	    /// <summary>The south-west goal.  Referred to as GL2 by the server.</summary>
	    GoalLeftBottom,
	    /// <summary>The north-east goal.  Referred to as GR1 by the server.</summary>
	    GoalRightTop,
	    /// <summary>The south-east goal.  Referred to as GR2 by the server.</summary>
	    GoalRightBottom
	}
}
