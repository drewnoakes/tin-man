#region License

// This file is part of TinMan.
// 
// TinMan is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// TinMan is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with TinMan.  If not, see <http://www.gnu.org/licenses/>.

#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 10/06/2010 03:01

using System;
using TinMan;

namespace TinManSamples.CSharp
{
    // TODO what happens if we send negative while inertia moving positively, that is, can we apply a brake to slow down faster than we would from friction alone?
    internal sealed class HingeCharacterisationAgent : AgentBase<NaoBody>
    {
        private bool _beamToGoal = true;
        private int _stableCount;
        private int _moveCount;
        private AngularSpeed _impulseSpeed = AngularSpeed.FromDegreesPerSecond(5);
        private Angle _lastAngle;
        private TimeSpan _lastSimulationTime;

        public HingeCharacterisationAgent()
            : base(new NaoBody())
        {
            PrintUsage();
        }

        public override void Think(PerceptorState state)
        {
            if (Console.KeyAvailable)
                HandleUserInput(Console.ReadKey(true).KeyChar);

            if (_beamToGoal)
            {
                _beamToGoal = false;
                Context.Beam(-Measures.FieldXLength/2, 0, Angle.Zero);
            }

            var hinge = Body.HJ1;
            var currentAngle = hinge.Angle;
            // TODO calculate SpeedLastCycle (or at AngularDeltaLastCycle) on the Hinge itself
            var speedLastCycle = (currentAngle - _lastAngle)/(state.SimulationTime - _lastSimulationTime);
            if (_moveCount > 0)
            {
                hinge.DesiredSpeed = _impulseSpeed;
                _stableCount = 3;
                _moveCount--;
            }
            else
            {
                hinge.DesiredSpeed = AngularSpeed.Zero;
                if (speedLastCycle == AngularSpeed.Zero && _stableCount > 0)
                    _stableCount--;
            }
            _lastAngle = currentAngle;
            _lastSimulationTime = state.SimulationTime;

            if (_stableCount != 0)
            {
                Console.WriteLine("{0:###0.00###}, {1:0.00}, {2}, {3}",
                                  state.SimulationTime.TotalSeconds,
                                  currentAngle.Degrees,
                                  speedLastCycle.DegreesPerSecond,
                                  hinge.DesiredSpeed.DegreesPerSecond);
            }
        }

        private void HandleUserInput(char key)
        {
            switch (key)
            {
                case 'B':
                    _beamToGoal = true;
                    Console.WriteLine("Beaming to goal");
                    break;
                case '-':
                    _impulseSpeed = -_impulseSpeed;
                    Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
                    break;
                case '>':
                    _impulseSpeed += AngularSpeed.FromDegreesPerSecond(1);
                    Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
                    break;
                case '<':
                    _impulseSpeed -= AngularSpeed.FromDegreesPerSecond(1);
                    Console.WriteLine("Speed: {0}", _impulseSpeed.DegreesPerSecond);
                    break;
                default:
                    if (key >= '1' && key <= '9')
                    {
                        Console.WriteLine("Time,Angle,LastSpeed,SpeedRequest");
                        _moveCount = key - '0';
                        _lastAngle = Angle.NaN;
                    }
                    else
                    {
                        PrintUsage();
                    }
                    break;
            }
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine(@"This agent sets the HJ1 (left/right neck joint) speed for a number of cycles, and prints out the position and speed of the hinge for successive cycles, both during the impulse and until it comes to rest.  This process is controlled via the keyboard:
  B   beam to goal
  -   negate the impulse speed
  <   decrease the impulse speed by one deg/sec
  >   increase the impulse speed by one deg/sec
  1-9 apply the impulse for the selected number of cycles");
        }
    }
}