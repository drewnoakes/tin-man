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
// Created 12/02/2011 21:13

using System;
using System.Threading;
using TinMan;

namespace TinManSamples.CSharp
{
    /// <summary>
    /// This class provides a quick lanching place for the samples included in this project.
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            Console.Out.WriteLine(@"Sample Agent Launcher
---------------------
Choose one of the following:

  1 WavingAgent
  2 HingeCharacterisationAgent
  3 FullHouse
  4 InteractiveAgent
  5 MinimalAgent
  6 SoccerbotAgent
  7 RoboVizDemoAgent
  8 SocialAgent
  9 PidAgent
  0 WizardExample
  S DebugShapeTransformationAgent
");
            switch (char.ToLower(Console.ReadKey(true).KeyChar))
            {
                case '1':
                    new AgentHost().Run(new WavingAgent());
                    break;
                case '2':
                    new AgentHost().Run(new HingeCharacterisationAgent());
                    break;
                case '3':
                    FullHouse.Run(2);
                    break;
                case '4':
                    new AgentHost().Run(new InteractiveAgent());
                    break;
                case '5':
                    new AgentHost().Run(new MinimalAgent());
                    break;
                case '6':
                    new AgentHost().Run(new SoccerbotAgent());
                    break;
                case '7':
                    new AgentHost().Run(new RoboVizDemoAgent());
                    break;
                case '8':
                    for (var i = 0; i < 2; i++)
                    {
                        new Thread(() => new AgentHost().Run(new SocialAgent())).Start();
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                    break;
                case '9':
                    new AgentHost().Run(new PidAgent());
                    break;
                case '0':
                    new WizardExample();
                    break;
                case 's':
                    new AgentHost().Run(new DebugShapeTransformationAgent());
                    break;
            }
        }
    }
}