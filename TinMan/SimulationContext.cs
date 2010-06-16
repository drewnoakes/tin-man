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
    public interface ISimulationContext {
        string TeamName { get; }
        void Say(string messageToSay);
        void Beam(double x, double y, Angle rotation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Threadsafe.
    /// </remarks>
    internal sealed class SimulationContext : ISimulationContext {
        private readonly object _lock = new object();
        private readonly Client _client;
        private BeamCommand _beamCommand;
        private SayCommand _sayCommand;
        
        public SimulationContext(Client client) {
            _client = client;
        }
        
        public string TeamName {
            get { return _client.TeamName; }
        }
        
        public void Say(string messageToSay) {
            lock (_lock)
                _sayCommand = new SayCommand(messageToSay);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation">Defines the rotation angle of the player. Zero degrees points to positive x axis (to
        /// the right of the field), 90 degrees to positive y axis (to the top of the field).</param>
        public void Beam(double x, double y, Angle rotation) {
            lock (_lock)
                _beamCommand = new BeamCommand(x, y, rotation);
        }

        internal void FlushCommands(List<IEffectorCommand> commands) {
            lock (_lock) {
                if (_sayCommand!=null) {
                    commands.Add(_sayCommand);
                    _sayCommand = null;
                }
                if (_beamCommand!=null) {
                    commands.Add(_beamCommand);
                    _beamCommand = null;
                }
            }
        }
    }
}
