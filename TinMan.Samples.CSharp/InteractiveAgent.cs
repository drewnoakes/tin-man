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
// Created 14/06/2010 06:08

using System;
using TinMan;

namespace TinManSamples.CSharp
{
    /// <summary>
    /// A sample TinMan agent that is controllable via keyboard commands when run in a console.
    /// </summary>
    public sealed class InteractiveAgent : AgentBase<NaoBody>
    {
        public InteractiveAgent()
            : base(new NaoBody())
        {
            PrintUsage();
        }

        public override void Think(PerceptorState state)
        {
            if (Console.KeyAvailable)
                HandleUserInput(Console.ReadKey(true));
        }

        private void HandleUserInput(ConsoleKeyInfo keyInfo)
        {
            switch (char.ToLower(keyInfo.KeyChar))
            {
                case 'q':
                {
                    StopSimulation();
                    break;
                }
                case '1':
                {
                    const double gain = 4;
                    Body.HJ1.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.HJ2.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.LAJ1.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.LAJ2.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.LAJ3.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.RAJ1.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.RAJ2.MoveToWithGain(Angle.FromDegrees(0), gain);
                    Body.RAJ3.MoveToWithGain(Angle.FromDegrees(0), gain);
                    break;
                }
                case '2':
                {
                    const double gain = 0.5;
                    Body.HJ1.MoveToWithGain(Angle.FromDegrees(100), gain);
                    Body.HJ2.MoveToWithGain(Angle.FromDegrees(40), gain);
                    Body.LAJ1.MoveToWithGain(Angle.FromDegrees(90), gain);
                    Body.LAJ2.MoveToWithGain(Angle.FromDegrees(90), gain);
                    Body.LAJ3.MoveToWithGain(Angle.FromDegrees(90), gain);
                    Body.RAJ1.MoveToWithGain(Angle.FromDegrees(-90), gain);
                    Body.RAJ2.MoveToWithGain(Angle.FromDegrees(90), gain);
                    Body.RAJ3.MoveToWithGain(Angle.FromDegrees(90), gain);
                    break;
                }
                default:
                {
                    PrintUsage();
                    break;
                }
            }
        }

        private static void PrintUsage()
        {
            Console.Out.WriteLine(@"This agent moves between several predefined poses.  Different levels of gain are also demonstrated.  The process is controlled via the keyboard:
  1-2 select pose
  q   quit");
        }
    }
}