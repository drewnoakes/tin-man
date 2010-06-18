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
// Created 10/06/2010 03:01

using TinMan;

class MinimalAgent : AgentBase<NaoBody>
{
    public MinimalAgent()
      : base(new NaoBody()) {}

    public override void Think(ISimulationContext context, PerceptorState state)
    {
        // TODO kick goal
    }

//    static void Main()
//    {
//        // This call blocks while your agent runs
//        new AgentHost().Run(new SampleAgent());
//    }
}


