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
	/// <summary>Represents the state of a simulated force resistance perceptor.</summary>
	public struct ForceState {
	    /// <summary>Gets the label that identifies the force resistance perceptor.</summary>
	    public string Label { get; private set; }
	    public Vector3 PointOnBody { get; private set; }
	    public Vector3 ForceVector { get; private set; }
	    
	    /// <remarks>
	    /// Most users will not need to use this constructor as this type is only for inbound messages.
	    /// This constructor is public to allow for unit testing.
	    /// </remarks>
	    public ForceState(string label, Vector3 pointOnBody, Vector3 forceVector) : this() {
	        Label = label;
	        PointOnBody = pointOnBody;
	        ForceVector = forceVector;
	    }
	    
	    public override string ToString() {
	        return string.Format("Point={0}, Force={1}", PointOnBody, ForceVector);
	    }
	}
}
