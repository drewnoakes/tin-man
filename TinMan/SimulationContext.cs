/*
 * Created by Drew, 12/06/2010 01:04.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
    public interface ISimulationContext {
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
        private BeamCommand _beamCommand;
        private SayCommand _sayCommand;
        
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
