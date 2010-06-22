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
// Created 21/06/2010 17:29

using System;
using System.Collections.Generic;
using System.Threading;

using TinMan;

namespace TinMan.Samples.CSharp
{
    class WizardExample {
        public WizardExample() {
            const string HostName = "yoda";
            
            var wizard = new Wizard();
            wizard.HostName = HostName;
            wizard.BallTransformUpdated += (time,transform) => Console.WriteLine("Ball position at {0}: {1}", time, transform.GetTranslation());
            wizard.AgentTransformUpdated += (time,transform) => Console.WriteLine("Agent position at {0}: {1}", time, transform.GetTranslation());
            
            var agentHosts = new List<AgentHost>();
            
            Action addAgent = () => {
                var agent = new MinimalAgent();
                var agentHost = new AgentHost() { UniformNumber = agentHosts.Count+1, HostName = HostName };
                new Thread(() => agentHost.Run(agent)).Start();
                agentHosts.Add(agentHost);
            };
            
            // Add a wizard
            new Thread(() => wizard.Run()).Start();
            
            bool quit = false;
            while (!quit) {
                if (Console.KeyAvailable) {
                    switch (Console.ReadKey(true).KeyChar) {
                        case '+':
                            Console.WriteLine("Adding agent");
                            addAgent();
                            break;
                        case 'd':
                            wizard.DropBall();
                            Console.WriteLine("Dropping ball");
                            break;
                        case 'b':
                            wizard.SetBallPosition(new Vector3(3,5,1));
                            Console.WriteLine("Setting ball position");
                            break;
                        case 'v':
                            wizard.SetBallVelocity(new Vector3(1,-1,10));
                            Console.WriteLine("Setting ball velocity");
                            break;
                        case 'k':
                            wizard.KickOff(FieldSide.Left);
                            Console.WriteLine("Kick off (left)");
                            break;
                        case 'K':
                            wizard.KickOff(FieldSide.Right);
                            Console.WriteLine("Kick off (right)");
                            break;
                        case 'x':
                            wizard.KillAgent(1, FieldSide.Left);
                            Console.WriteLine("Killing agent #1 on left side");
                            break;
                        case 'a':
                            var pos = FieldGeometry.GetRandomPosition(FieldSide.Left).WithZ(0);
                            int num = new Random().Next(agentHosts.Count)+1;
                            Console.WriteLine("Moving agent {0} to {1}", num, pos);
                            wizard.SetAgentPosition(num, FieldSide.Left, pos);
                            break;
                        case 'q':
                            quit = true;
                            break;
                        default:
                            Console.WriteLine("Unknown input.");
                            break;
                    }
                }
                // Sleep a little each cycle as otherwise this would be a very hot loop
                System.Threading.Thread.Sleep(50);
            }
            
            foreach (var agentHost in agentHosts)
                agentHost.Stop();
            
            wizard.Stop();
        }
        
        static void Main() {
            new WizardExample();
        }
    }
}