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

using System;
using TinMan;

class LoggingAgent : AgentBase<NaoBody>
{
    static LoggingAgent()
    {
        Log.InfoAction = delegate(string message) { /* Call alternative logging framework here */ };
        Log.VerboseAction = delegate(string message) { /* Call alternative logging framework here */ };
        Log.WarnAction = delegate(string message) { /* Call alternative logging framework here */ };
        Log.ErrorAction = delegate(string message, Exception exception) { /* Call alternative logging framework here */ };
    }
    
    public LoggingAgent() : base(new NaoBody())
    {
        Log.Info("Creating agent.");
    }

    public override void Think(ISimulationContext context, PerceptorState state)
    {
        Log.Verbose("Simulation time = {0}", state.SimulationTime);
    }

//    static void Main()
//    {
//        new AgentHost().Run(new LoggingAgent());
//    }
}
