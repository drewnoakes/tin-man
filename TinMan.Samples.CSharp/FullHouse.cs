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
// Created 12/06/2010 06:35

using System.Threading;
using TinMan;

namespace TinManSamples.CSharp
{
    internal class HerdAgent : AgentBase<NaoBody>
    {
        public HerdAgent()
            : base(new NaoBody())
        {}

        public override void Think(PerceptorState state)
        {
            if (Context.TeamName == "Team1")
            {
                Body.LAJ1.DesiredSpeed = AngularSpeed.FromDegreesPerSecond(0.01);
                Body.RAJ1.DesiredSpeed = AngularSpeed.FromDegreesPerSecond(0.01);
            }
        }
    }

    internal static class FullHouse
    {
        public static void Run(int agentCountPerSide)
        {
            CreateTeam(agentCountPerSide, "Team1");
            CreateTeam(agentCountPerSide, "Team2");
        }

        private static void CreateTeam(int agentCount, string teamName)
        {
            for (int i = 0; i < agentCount; i++)
            {
                var host = new AgentHost { TeamName = teamName };

                // Run the host in a new thread
                new Thread(() => host.Run(new HerdAgent())).Start();

                // Give the server a chance to catch its breath
                Thread.Sleep(2000);
            }
        }
    }
}