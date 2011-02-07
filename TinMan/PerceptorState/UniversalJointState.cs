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

using System.Diagnostics;

namespace TinMan
{
    /// <summary>Represents the state of a simulated universal joint.</summary>
    [DebuggerDisplay("UniversalJoint {Label}={Angle1},{Angle2}")]
    public struct UniversalJointState
    {
        /// <summary>Gets the label that identifies the universal joint.</summary>
        public string Label { get; private set; }
        /// <summary>Gets the first angle of the universal joint.</summary>
        public Angle Angle1 { get; private set; }
        /// <summary>Gets the second angle of the universal joint.</summary>
        public Angle Angle2 { get; private set; }

        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public UniversalJointState(string label, Angle angle1, Angle angle2) : this()
        {
            Label = label;
            Angle1 = angle1;
            Angle2 = angle2;
        }
    }
}