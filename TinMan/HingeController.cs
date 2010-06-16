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
// Created 16/05/2010 15:46

using System;
using System.Diagnostics;

namespace TinMan
{
    public interface IHingeController<T> {
        Hinge Hinge { get; }
        void MoveTo(T moveData);
        void Step(TimeSpan simulationTime);
    }
    
    /// <summary>
    /// Responsible for the control logic of a single <see cref="Hinge"/>.
    /// Designed to allow for maximum speed and control.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Maintains actual/target positions</item>
    ///   <item>Produces actions to achieve target</item>
    ///   <item>Avoids oscillations and oversteering</item>
    ///   <item>Locks a joint in position, as during simulation it can be nudged out of place</item>
    /// </list>
    /// </remarks>
    public sealed class HingeController : IHingeController<Angle> {
        /// <summary>The smallest difference between target and actual angles below which they are considered equal.  Avoids endless oscillation.</summary>
        private static readonly Angle EpsilonAngle = Angle.FromDegrees(1);
        
        private HingeControllerState _state = HingeControllerState.Resting;
        
        /// <summary>The last angle reported by the server.</summary>
        // TODO almost redundant, given Hinge.LastAngle
        public Angle LastAngle { get; private set; }
        /// <summary>The angle required by the client.</summary>
        public Angle TargetAngle { get; private set; }
        /// <summary>The hinge being controlled.</summary>
        public Hinge Hinge { get; private set; }
        
//        private int _correctionCount;

        public HingeController(Hinge hinge) {
            if (hinge==null)
                throw new ArgumentNullException("hinge");
            Hinge = hinge;
            LastAngle = Angle.NaN;
            TargetAngle = Angle.NaN;
        }
                
        public void Step(TimeSpan simulationTime) {
            // TODO some of these values could be properties of Hinge itself
            Angle moveLastCycle = LastAngle - Hinge.Angle;
            LastAngle = Hinge.Angle;
            
            if (TargetAngle.IsNaN)
                TargetAngle = LastAngle;
            
            switch (_state) {
                case HingeControllerState.Resting: {
                    var error = TargetAngle - LastAngle;
                    if (error.Abs > EpsilonAngle * 2) {
                        // We've supposedly finished moving, but are not in position anymore. Perhaps the joint was forced.
                        _state = HingeControllerState.Moving;
                        // TODO review whether we need 'correcting' state
//                        _state = HingeControllerState.Correcting;
//                        _correctionCount = 0;
                        Step(simulationTime);
                    }
                    break;
                }
//                case HingeControllerState.Correcting: {
//                    var error = TargetAngle - LastAngle;
//                    if (error.Abs < EpsilonAngle) {
//                        _state = HingeControllerState.Resting;
//                        Hinge.Speed = AngularSpeed.Zero;
//                        break;
//                    }
//                    // Apply a gentle correction, controlled by the passing of time rather than magnitude of error
//                    if (_correctionCount<2)
//                        Hinge.Speed = error/TimeSpan.FromMilliseconds(200);// * 0.2;
//                    else if (_correctionCount<4)
//                        Hinge.Speed = error/TimeSpan.FromMilliseconds(500);// * 0.1;
//                    else
//                        Hinge.Speed = error/TimeSpan.FromMilliseconds(800);// * 0.05;
//                    _correctionCount++;
//                    break;
//                }
                case HingeControllerState.MovePending: {
                    _state = HingeControllerState.Moving;
                    Step(simulationTime);
                    break;
                }
                case HingeControllerState.Moving: {
                    Angle targetFromLast = (TargetAngle - LastAngle);
                    // If we've already requested this angle, and the hinge didn't move (much) in the last cycle
                    if (targetFromLast.Abs <= EpsilonAngle && moveLastCycle.Abs <= EpsilonAngle) {
                        _state = HingeControllerState.Resting;
                        Hinge.Speed = AngularSpeed.Zero;
                        break;
                    }
                    // TODO this speed is not in degrees per second!!!
                    //var maxSpeed = AngularSpeed.FromDegreesPerSecond(180);
                    Hinge.Speed = (targetFromLast/TimeSpan.FromMilliseconds(5500));//.Limit(-maxSpeed, maxSpeed);
                    break;
                }
                default:
                    throw new Exception("Unexpected state: " + _state);
            }
        }
      
        public void MoveTo(Angle angle) {
            Hinge.ValidateAngle(angle);
            // If we're basically already at the requested target angle, return.
            if ((angle - TargetAngle).Abs < EpsilonAngle)
                return;
            TargetAngle = angle;
            _state = HingeControllerState.MovePending;
        }
    }
    
    public enum HingeControllerState {
        Resting,
        MovePending,
        Moving,
//        Correcting
    }
}
