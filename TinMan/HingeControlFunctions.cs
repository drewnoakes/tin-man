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
// Created 15/06/2010 22:17

using System;

namespace TinMan
{
	/// <summary>
	/// A collection of extension methods designed to control hinge joints.
	/// </summary>
	/// <remarks>
	/// While these methods could have been added directly to <see cref="Hinge"/>, the idea is that
	/// all hinge control functionality provided with the TinMan framework operates via the same
	/// extensibility mechanism that user code would as well.
	/// </remarks>
	public static class HingeControlFunctions {
	    /// <summary>
	    /// Starts the process of moving a hinge joint to a specified angular position and holding it there.
	    /// The of <paramref name="gain"/> influences the amount of time it will take to achieve
	    /// <paramref name="desiredAngle"/>.  Smaller gains create slower movements.  Note that high levels
	    /// of gain will create unstable oscillations that will never settle.
	    /// </summary>
	    /// <remarks>
	    /// Once this method is called, the joint will be controlled in all subsequent simulation cycles until
	    /// either the hinge's <see cref="Hinge.DesiredSpeed"/> is set, or <see cref="Hinge.ClearControlFunction"/>
	    /// is called.
	    /// </remarks>
	    /// <param name="hinge"></param>
	    /// <param name="desiredAngle"></param>
	    /// <param name="gain"></param>
	    /// <exception cref="ArgumentNullException"><paramref name="hinge"/> is <c>null</c>.</exception>
	    public static void MoveToWithGain(this Hinge hinge, Angle desiredAngle, double gain) {
	        if (hinge==null)
	            throw new ArgumentNullException("hinge");
	        
	        // Set a control function for this hinge.  Any existing control function will be replaced.
	        hinge.SetControlFunction(delegate(Hinge h, ISimulationContext c) {
	             // Speed for this cycle is a factor of the gain and the current angular distance
	             var angleDiff = desiredAngle - h.Angle;
	             // If we're sufficiently close to the desired angle, stop moving
	             if (angleDiff.Abs.Degrees < 1)
	                 return AngularSpeed.Zero;
	             // Still moving, so calculate the desired speed for the next simulation cycle
	             double speed = angleDiff.Degrees * gain;
	             return AngularSpeed.FromDegreesPerSecond(speed);
	        });
	    }
	}
}
