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
// Created 03/04/2011 20:45

namespace TinMan
{
    /// <summary>Represents information about the orientation of observed field lines.</summary>
    public struct VisibleLine
    {
    	// TODO determine if there is something special about end1 vs end2 and document that here
    	
        /// <summary>Gets an end of the observed line.</summary>
        public Polar End1 { get; private set; }

        /// <summary>Gets an end of the observed line.</summary>
        public Polar End2 { get; private set; }

        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public VisibleLine(Polar end1, Polar end2) : this()
        {
            End1 = end1;
            End2 = end2;
        }
    }
}