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
// Created 15/03/2011 12:01

using System;
using TinMan;

namespace TinManSamples.CSharp
{
    public sealed class PidAgent : AgentBase<NaoBody>
    {
        private PidHingeController _pidLaj1;

        public PidAgent()
            : base(new NaoBody())
        {}

        public override void OnInitialise()
        {
            _pidLaj1 = new PidHingeController(Body.LAJ1);
            PrintUsage();
            Console.Out.WriteLine(_pidLaj1);
        }

        public override void Think(PerceptorState state)
        {
            // We only do some work if a key has been pressed, otherwise fall through
            if (!Console.KeyAvailable)
                return;

            char c = Console.ReadKey(true).KeyChar;
            
            bool change = false;
            
            switch (c)
            {
                case 'P':
                    _pidLaj1.ProportionalGain += 1;
                    change = true;
                    break;
                case 'p':
                    _pidLaj1.ProportionalGain = Math.Max(0, _pidLaj1.ProportionalGain - 1);
                    change = true;
                    break;
                case 'I':
                    _pidLaj1.IntegralGain += 0.1;
                    change = true;
                    break;
                case 'i':
                    _pidLaj1.IntegralGain = Math.Max(0, _pidLaj1.IntegralGain - 0.1);
                    change = true;
                    break;
                case 'D':
                    _pidLaj1.DerivativeGain += 100;
                    change = true;
                    break;
                case 'd':
                    _pidLaj1.DerivativeGain = Math.Max(0, _pidLaj1.DerivativeGain - 100);
                    change = true;
                    break;
                case '[':
                    _pidLaj1.TargetAngle = Angle.FromDegrees(-90);
                    break;
                case ']':
                    _pidLaj1.TargetAngle = Angle.FromDegrees(90);
                    break;
                case 'Q':
                    StopSimulation();
                    break;
                default:
                    PrintUsage();
                    break;
            }

            if (change)
                Console.Out.WriteLine(_pidLaj1);
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine("P - increase Kp");
            Console.Out.WriteLine("p - decrease Kp");
            Console.Out.WriteLine("I - increase Ki");
            Console.Out.WriteLine("i - decrease Ki");
            Console.Out.WriteLine("D - increase Kd");
            Console.Out.WriteLine("d - decrease Kd");
            Console.Out.WriteLine("[ - move LAJ1 to -90 degrees");
            Console.Out.WriteLine("] - move LAJ1 to +90 degrees");
            Console.Out.WriteLine("Q - quit");
        }
    }
}