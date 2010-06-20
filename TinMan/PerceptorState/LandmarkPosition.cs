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
	/// <summary>Represents information about the location of a seen landmark, such as a corner flag or a goal post.</summary>
	public struct LandmarkPosition {
	    public Landmark Landmark { get; private set; }
	    public Polar PolarPosition { get; private set; }
	    
	    /// <remarks>
	    /// Most users will not need to use this constructor as this type is only for inbound messages.
	    /// This constructor is public to allow for unit testing.
	    /// </remarks>
	    public LandmarkPosition(Landmark landmark, Polar radialPosition) : this() {
	        Landmark = landmark;
	        PolarPosition = radialPosition;
	    }
	}
}
