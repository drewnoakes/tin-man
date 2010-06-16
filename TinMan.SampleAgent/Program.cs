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
// Created 10/06/2010 03:01

using System;
using System.Collections.Generic;

namespace TinMan.SampleBot
{
    internal sealed class SampleAgent : AgentBase<NaoBody>, IUserInteractiveAgent {
        private bool _beamToGoal = true;
        private int _stableCount = 0;
        private int _moveCount = 0;
        private AngularSpeed _impulseSpeed = AngularSpeed.FromDegreesPerSecond(5) ;
        private Angle _lastAngle;
        private TimeSpan _lastSimulationTime;

        public SampleAgent()
            : base(new NaoBody()) {}
        
        public override void Step(ISimulationContext context, PerceptorState state) {
            // TODO what happens if we send 0
            // TODO what happens if we send negative while inertia moving positive

            if (_beamToGoal) {
                _beamToGoal = false;
                context.Beam(-FieldGeometry.FieldXLength/2, 0, Angle.Zero);
            }

            var hinge = Body.HJ1;
            Angle currentAngle = hinge.Angle;
            AngularSpeed speedLastCycle = (currentAngle - _lastAngle) / (state.SimulationTime - _lastSimulationTime);
            if (_moveCount>0) {
                hinge.Speed = _impulseSpeed;
                _stableCount = 3;
                _moveCount--;
            } else {
                hinge.Speed = AngularSpeed.Zero;
                if (speedLastCycle==AngularSpeed.Zero && _stableCount>0)
                    _stableCount--;
            }
            _lastAngle = currentAngle;
            _lastSimulationTime = state.SimulationTime;
            
            if (_stableCount!=0) {
                Console.WriteLine("{0:###0.00###}, {1:0.00}, {2}, {3}", 
                                  state.SimulationTime.TotalSeconds,
                                  currentAngle.Degrees, 
                                  speedLastCycle.DegreesPerSecond, 
                                  hinge.Speed.DegreesPerSecond);
            }
        }
        
        public void HandleUserInput(char key, ISimulationContext context) {
            if (key=='B') {
                _beamToGoal = true;
                Console.WriteLine("Beaming to goal");
            }
            if (key=='-') {
                _impulseSpeed = -_impulseSpeed;
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key=='>') {
                _impulseSpeed += AngularSpeed.FromDegreesPerSecond(1);
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key=='<') {
                _impulseSpeed -= AngularSpeed.FromDegreesPerSecond(1);
                Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
            }
            if (key>='1' && key<='9') {
                Console.WriteLine("Time,Angle,LastSpeed,SpeedRequest");
                _moveCount = key - '0';
                _lastAngle = Angle.NaN;
            }
        }
    }
    
//    internal static class Program {
//        public static void Main() {
//            new Client().Run(new SampleAgent());
//        }
//    }
}