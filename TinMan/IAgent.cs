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
// Created 10/05/2010 12:43

using System;

namespace TinMan
{
    /// <summary>
    /// Interface for all TinMan agents.  Most users should derive directly from <see cref="AgentBase{T}"/>
    /// for simplicity.  If you must derive from another class, or wish to mock your agent, you can use
    /// this interface.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Raised when <see cref="Think"/> finishes.  Add on components may use this event to hook into
        /// the agent's process cycle.
        /// </summary>
        event Action ThinkCompleted;

        /// <summary>
        /// Raised when the agent is shutting down due to a call to <see cref="OnShuttingDown"/>.  Add on components
        /// may use this event to hook into the agent's process cycle.
        /// </summary>
        event Action ShuttingDown;

        /// <summary>Gets the agent's body.</summary>
        /// <remarks>Must not be <c>null</c>.</remarks>
        IBody Body { get; }

        /// <summary>Gets whether the agent should remain connected to the server and processing state.</summary>
        bool IsAlive { get; }

        /// <summary>
        /// Gets and sets the simulation context for this agent.  The setter is intended only for use by the TinMan
        /// framework.  This value is unavailable before the first call to <see cref="OnInitialise"/>.  Attempting to
        /// access it before that time will result in an exception.
        /// </summary>
        ISimulationContext Context { get; set; }

        /// <summary>
        /// Performs any initialisation required by the agent prior to the first call to <see cref="Think"/>.
        /// Called by the TinMan framework.
        /// </summary>
        void OnInitialise();

        /// <summary>
        /// Gives the agent a chance to process the latest body state and perform any necessary actions.
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>Implementations should raise <see cref="ThinkCompleted"/> after thinking so that external components
        /// that track the agent's life cycle are notified.</remarks>
        void Think(PerceptorState state);

        /// <summary>
        /// Called when the agent is about to shut down.  At this point, no further messages will be sent to the server.
        /// </summary>
        /// <remarks>Implementations should raise <see cref="ShuttingDown"/> so that external components that track the
        /// agent's life cycle are notified.</remarks>
        void OnShuttingDown();
    }
}