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
// Created 10/05/2010 12:43

using System;

namespace TinMan
{
    /// <summary>
    /// A base class for agents in the TinMan framework.  Use instances of subclasses of this type in
    /// conjunction with an <see cref="AgentHost"/> to execute your agent within the simulation.
    /// </summary>
    public abstract class AgentBase<TBody> : IAgent where TBody : IBody {
        /// <summary>Gets the agent's body.</summary>
        /// <remarks>Will not be <c>null</c>.</remarks>
        public TBody Body { get; private set; }
        
        /// <summary>Gets the agent's body.</summary>
        /// <remarks>
        /// Exposes the body as an <see cref="IBody"/> as required by the base interface.  This property
        /// is an explicit interface implementation, meaning it's hidden on an instance unless it's declared
        /// as the interface.  The alternative property provides strongly typed access which is more convenient.
        /// </remarks>
        IBody IAgent.Body { get { return Body; } }
        
        /// <summary>Gets whether the agent should remain connected to the server and processing state.</summary>
        public bool IsAlive { get; private set; }
        
        /// <summary>Gets a logger used by the agent.</summary>
        protected Log Log { get; private set; }
        
        /// <summary>
        /// Initialises AgentBase with a body instance.
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> is null.</exception>
        protected AgentBase(TBody body) {
            if (body==null)
                throw new ArgumentNullException("body");
            Body = body;
            Log = Log.Create();
        }
        
        /// <summary>
        /// Gives the agent a chance to process the latest body state and
        /// perform any necessary actions.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        public abstract void Think(ISimulationContext context, PerceptorState state);
        
        /// <summary>
        /// Causes the <see cref="AgentHost"/> to stop hosting this agent.  This action cannot be
        /// undone.
        /// </summary>
        protected void StopSimulation() {
            Log.Info("Agent requested that the simulation stops.");
            IsAlive = false;
        }
    }
}
