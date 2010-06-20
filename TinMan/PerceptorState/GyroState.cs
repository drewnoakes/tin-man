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
    /// <summary>Represents the state of a simulated gyro.</summary>
	[DebuggerDisplay("Gyro {Label}={XOrientation},{YOrientation},{ZOrientation}")]
	public struct GyroState {
        /// <summary>Gets the label that identifies the gyro.</summary>
	    public string Label { get; private set; }
	    
	    // TODO work out what units these gyro rates are in and potentially change to Angle instances
	    
        /// <summary>Gets the rotation in the X axis reported by the gyro.</summary>
	    public double XOrientation { get; private set; }
        /// <summary>Gets the rotation in the Y axis reported by the gyro.</summary>
	    public double YOrientation { get; private set; }
        /// <summary>Gets the rotation in the Z axis reported by the gyro.</summary>
	    public double ZOrientation { get; private set; }
	    
	    /// <remarks>
	    /// Most users will not need to use this constructor as this type is only for inbound messages.
	    /// This constructor is public to allow for unit testing.
	    /// </remarks>
	    public GyroState(string label, double xOrientation, double yOrientation, double zOrientation) : this() {
	        Label = label;
	        XOrientation = xOrientation;
	        YOrientation = yOrientation;
	        ZOrientation = zOrientation;
	    }
	    
	    public override string ToString() {
	        return string.Format("{0} X={1} Y={2} Z={3}", Label, XOrientation, YOrientation, ZOrientation);
	    }
	}
}
