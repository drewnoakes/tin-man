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
// Created 23/06/2010 15:29

using System;

using TinMan;

namespace TinMan.Samples.CSharp
{
    public sealed class WavingAgent : AgentBase<NaoBody>
    {
        public WavingAgent()
            : base(new NaoBody()) {}
        
        public override void Think(ISimulationContext context, PerceptorState state)
        {
            // We only do some work if a key has been pressed, otherwise fall through
            if (Console.KeyAvailable)
            {
                char c = Console.ReadKey(true).KeyChar;
                
                // If the pressed key was a digit
                if (c >= '0' && c <= '9')
                {
                    // Multiply the number pressed by 10 to get the desired angle
                    int angle = (c - '0') * 10;
                    
                    // Move the left shoulder to that angle
                    Body.LAJ1.MoveToWithGain(Angle.FromDegrees(angle), 1);
                }
                if (c=='?')
                {
                    var random = new Random();
                    Body.LAJ1.SetControlFunction((hinge, ctx) => {
                        return AngularSpeed.FromDegreesPerSecond(random.Next(100) - 50);
                    });
                }
            }
        }

        static void Main()
        {
            // This call blocks while your agent runs
            new AgentHost(){HostName = "yoda"}.Run(new WavingAgent());
        }
    }
}
