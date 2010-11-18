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

using System.Collections.Generic;
using System.Text;

namespace TinMan
{
	/// <summary>Represents information about the location of a seen player.</summary>
	public struct PlayerPosition {
	    public bool IsTeamMate { get; private set; }
	    public int PlayerId { get; private set; }
	    public IEnumerable<BodyPartPosition> PartPositions { get; private set; }
	    
	    /// <remarks>
	    /// Most users will not need to use this constructor as this type is only for inbound messages.
	    /// This constructor is public to allow for unit testing.
	    /// </remarks>
	    public PlayerPosition(bool isTeamMate, int playerId, IEnumerable<BodyPartPosition> partPositions) : this() {
	        IsTeamMate = isTeamMate;
	        PlayerId = playerId;
	        PartPositions = partPositions;
	    }
	    
	    public override string ToString() {
	        var sb = new StringBuilder();
	        sb.Append(IsTeamMate ? "TeamMate " : "Opposition ");
	        sb.Append(PlayerId);
	        sb.Append(" -> ");
	        bool first = true;
	        foreach (var p in PartPositions) {
	            if (!first) {
	                sb.Append(", ");
	                first = false;
	            }
	            sb.AppendFormat("{0} @ {1}", p.Label, p.PolarPosition);
	        }
	        return sb.ToString();
	    }
	}

    /// <summary>
    /// Describes the location of an agent's body part relative to an observing agent, as
    /// observed by the agent's vision perceptor.
    /// </summary>
    public struct BodyPartPosition {
        /// <summary>Gets the label that identifies the body part.</summary>
        public string Label { get; private set; }
        public Polar PolarPosition { get; private set; }
        
        public BodyPartPosition(string label, Polar position) : this() {
            Label = label;
            PolarPosition = position;
        }
        
        public override string ToString() {
            return string.Format("{0} {1}", Label, PolarPosition);
        }
    }
}
