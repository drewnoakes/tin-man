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
// Created 23/06/2010 15:29

using System;
using TinMan;

namespace TinManSamples.CSharp
{
    public sealed class WavingAgent : AgentBase<NaoBody>
    {
        public WavingAgent()
            : base(new NaoBody())
        {}

        public override void Think(PerceptorState state)
        {
            // We only do some work if a key has been pressed, otherwise fall through
            if (!Console.KeyAvailable)
                return;

            char c = Console.ReadKey(true).KeyChar;

            // If the pressed key was a digit
            if (c >= '0' && c <= '9')
            {
                // Multiply the number pressed by 10 to get the desired angle
                int angle = (c - '0')*10;

                // Move the left shoulder to that angle and hold it there.
                // This operation will occur automatically over subsequent cycles, and the
                // joint angle be regulated until a different instruction is provided.
                Body.LAJ1.MoveToWithGain(Angle.FromDegrees(angle), 1);
            }
            else if (c == '?')
            {
                // Set a 'control function' that returns a random speed between -50 and +50 for the joint each cycle.
                // Note how this control function applies across multiple cycles.  Therefore a single press of the '?'
                // key will create a sequence of erratic movements that will only stop when another digit key is pressed.
                var random = new Random();
                Body.LAJ1.SetControlFunction((hinge, context, perceptorState) => AngularSpeed.FromDegreesPerSecond(random.Next(100) - 50));
            }
        }
    }
}