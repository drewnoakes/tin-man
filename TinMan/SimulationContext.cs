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
// Created 12/06/2010 01:04

using System;
using System.Collections.Generic;

namespace TinMan
{
    /// <summary>Default implementation of <see cref="ISimulationContext"/>.</summary>
    /// <remarks>Threadsafe.</remarks>
    internal sealed class SimulationContext : ISimulationContext
    {
        private readonly object _lock = new object();
        private readonly AgentHost _client;
        private BeamCommand _beamCommand;
        private SayCommand _sayCommand;

        public SimulationContext(AgentHost client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            _client = client;
        }

        /// <summary>Gets the name assigned to this team.</summary>
        public string TeamName
        {
            get { return _client.TeamName; }
        }

        /// <summary>Gets the side of the playing field that this agent's team has been assigned to.</summary>
        public FieldSide TeamSide { get; internal set; }

        /// <summary>Gets the wizard of this simulation.  May be <c>null</c>.</summary>
        public Wizard Wizard { get; set; }

        /// <summary>
        /// Causes the agent to speak a message out loud such that nearby agents can hear it.
        /// This is the only method of inter-agent communication allowed in RoboCup.
        /// </summary>
        /// <param name="messageString"></param>
        public void Say(string messageString)
        {
            lock (_lock)
                _sayCommand = new SayCommand(new Message(messageString));
        }

        /// <summary>
        /// Beams the agent to a given location on the field.  The agent's orientation is also specified.
        /// Values are in field coordinates, such that (0,0) is the centre of the field.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation">Defines the rotation angle of the player. Zero degrees points to positive x axis (to
        /// the right of the field), 90 degrees to positive y axis (to the top of the field).</param>
        public void Beam(double x, double y, Angle rotation)
        {
            lock (_lock)
                _beamCommand = new BeamCommand(x, y, rotation);
        }

        internal void FlushCommands(List<IEffectorCommand> commands)
        {
            lock (_lock)
            {
                if (_sayCommand != null)
                {
                    commands.Add(_sayCommand);
                    _sayCommand = null;
                }
                if (_beamCommand != null)
                {
                    commands.Add(_beamCommand);
                    _beamCommand = null;
                }
            }
        }
    }
}