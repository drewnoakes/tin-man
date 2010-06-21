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

using TinMan;

namespace TinMan.Samples.CSharp
{
    class WizardExample {
        public WizardExample() {
            var wizard = new Wizard();
            wizard.Connect();
            
            bool quit = false;
            
            while (!quit) {
                if (Console.KeyAvailable) {
                    switch (Console.ReadKey(true).KeyChar) {
                        case 'd':
                            wizard.DropBall();
                            Console.WriteLine("\nDropping ball");
                            break;
                        case 'b':
                            wizard.SetBallPosition(new Vector3(3,5,1));
                            Console.WriteLine("\nSetting ball position");
                            break;
                        case 'v':
                            wizard.SetBallVelocity(new Vector3(1,-1,10));
                            Console.WriteLine("\nSetting ball velocity");
                            break;
                        case 'k':
                            wizard.KickOff(FieldSide.Left);
                            Console.WriteLine("\nKick off (left)");
                            break;
                        case 'K':
                            wizard.KickOff(FieldSide.Right);
                            Console.WriteLine("\nKick off (right)");
                            break;
                        case 'x':
                            wizard.KillAgent(1, FieldSide.Left);
                            Console.WriteLine("\nKilling agent #1 on left side");
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
                System.Threading.Thread.Sleep(100);
            }
            
            wizard.Disconnect();
        }
        
//        static void Main() {
//            new WizardExample();
//        }
    }
}