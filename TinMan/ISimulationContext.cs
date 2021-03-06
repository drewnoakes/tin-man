﻿#region License

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
// Created 12/06/2010 01:04

namespace TinMan
{
    /// <summary>
    /// Provides a set of actions and data to an agent.
    /// </summary>
    public interface ISimulationContext
    {
        /// <summary>Gets the name assigned to this team.</summary>
        string TeamName { get; }

        /// <summary>Gets and sets an optional wizard for this simulation.  May be <c>null</c>.</summary>
        Wizard Wizard { get; set; }

        /// <summary>Gets the side of the playing field that this agent's team has been assigned to.</summary>
        FieldSide TeamSide { get; }

        /// <summary>
        /// Gets the uniform number assigned to this agent.  If no number has been assigned yet,
        /// this value may be <c>null</c>.
        /// </summary>
        int? UniformNumber { get; }

        /// <summary>Gets a collection of measurements from the field.</summary>
        Measures Measures { get; }

        /// <summary>
        /// Causes the agent to speak a message out loud such that nearby agents can hear it.
        /// This is the only method of inter-agent communication allowed in RoboCup.
        /// </summary>
        /// <param name="messageToSay">The string message to say across the field to other
        /// agents.  This string must be valid according to <see cref="Message.IsValid"/>.</param>
        void Say(string messageToSay);

        /// <summary>
        /// Beams the agent to a given location on the field.  The agent's orientation is also specified.
        /// Values are in field coordinates, such that (0,0) is the centre of the field.
        /// </summary>
        /// <param name="x">The target x-position of the agent.</param>
        /// <param name="y">The target y-position of the agent.</param>
        /// <param name="rotation">Defines the rotation angle of the player. Zero degrees points to positive x axis (to
        /// the right of the field), 90 degrees to positive y axis (to the top of the field).</param>
        void Beam(double x, double y, Angle rotation);
    }
}