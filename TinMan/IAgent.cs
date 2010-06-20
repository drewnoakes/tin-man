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

namespace TinMan
{
	/// <summary>
	/// Interface for all TinMan agents.  Most users should derive directly from <see cref="AgentBase{T}"/>
	/// for simplicity.  If you must derive from another class, or wish to mock your agent, you can use
	/// this interface.
	/// </summary>
	public interface IAgent {
	    /// <summary>Gets the agent's body.</summary>
	    /// <remarks>Must not be <c>null</c>.</remarks>
	    IBody Body { get; }
	    
	    /// <summary>Gets whether the agent should remain connected to the server and processing state.</summary>
	    bool IsAlive { get; }
	    
	    /// <summary>
	    /// Gives the agent a chance to process the latest body state and perform any necessary actions.
	    /// </summary>
	    /// <param name="context"></param>
	    /// <param name="state"></param>
	    void Think(ISimulationContext context, PerceptorState state);
	}
}
