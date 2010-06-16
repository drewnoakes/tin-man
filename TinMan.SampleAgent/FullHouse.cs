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
// Created 12/06/2010 06:35

using System.Threading;

using TinMan;

class HerdAgent : AgentBase<NaoBody>
{
  public HerdAgent()
    : base(new NaoBody()) {}

  public override void Step(ISimulationContext context, PerceptorState state)
  {
    if (context.TeamName=="Team1") {
      Body.LAJ1.Speed = AngularSpeed.FromDegreesPerSecond(0.01);
      Body.RAJ1.Speed = AngularSpeed.FromDegreesPerSecond(0.01);
    }
  }
}

class FullHouse
{
  static void Main()
  {
    CreateTeam(1, "Team1");
    CreateTeam(1, "Team2");
  }
  
  public static void CreateTeam(int agentCount, string teamName) {
    for (int i=0; i<agentCount; i++) {
      new Thread(() => new Client{TeamName=teamName}.Run(new HerdAgent())).Start();
      // give the server a chance to catch its breath
      Thread.Sleep(2000);
    }
  }
}
