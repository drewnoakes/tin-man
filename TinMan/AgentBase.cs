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
    public abstract class AgentBase<TBody> : IAgent where TBody : class, IBody
    {
        public event Action ThinkCompleted;
        public event Action ShuttingDown;

        private ISimulationContext _context;

        #region Properties

        /// <summary>Gets the agent's body.</summary>
        /// <remarks>Will not be <c>null</c>.</remarks>
        public TBody Body { get; private set; }

        /// <summary>Gets the agent's body.</summary>
        /// <remarks>
        /// Exposes the body as an <see cref="IBody"/> as required by the base interface.  This property
        /// is an explicit interface implementation, meaning it's hidden on an instance unless it's declared
        /// as the interface.  The alternative property provides strongly typed access which is more convenient.
        /// </remarks>
        IBody IAgent.Body
        {
            get { return Body; }
        }

        /// <summary>
        /// Gets and sets the simulation context for this agent.  The setter is intended only for use by the TinMan
        /// framework, and is populated by <see cref="AgentHost.Run" />.  This value is unavailable before the first
        /// call to <see cref="IAgent.Think"/>.  Attempting to access it before that time will result in an
        /// <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// This is an explicit interface implementation so that the setter is hidden from subclasses in order to
        /// avoid confusion.
        /// </remarks>
        ISimulationContext IAgent.Context
        {
            get
            {
                if (_context == null)
                    throw new InvalidOperationException("The Context property cannot be accessed before the first call to Think.");
                return _context;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (_context != null)
                    throw new InvalidOperationException("Context has already been set.");
                _context = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISimulationContext"/> for this agent.  This value is unavailable before the first call to
        /// <see cref="IAgent.Think"/>.  Attempting to access it before that time will result in an
        /// <see cref="InvalidOperationException"/>.
        /// </summary>
        protected ISimulationContext Context
        {
            get { return ((IAgent)this).Context; }
        }

        /// <summary>Gets whether the agent should remain connected to the server and processing state.</summary>
        public bool IsAlive { get; private set; }

        /// <summary>Gets a logger used by the agent.</summary>
        protected Log Log { get; private set; }

        #endregion

        /// <summary>
        /// Initialises AgentBase with a body instance.
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> is null.</exception>
        protected AgentBase(TBody body)
        {
            if (body == null)
                throw new ArgumentNullException("body");
            Body = body;
            Log = Log.Create();
            IsAlive = true;
        }

        /// <summary>
        /// Performs any initialisation required by the agent prior to the first call to <see cref="IAgent.Think"/>.
        /// Called by the TinMan framework.  You are not required to override this method.
        /// </summary>
        public virtual void OnInitialise()
        {}

        #region Think

        void IAgent.Think(PerceptorState state)
        {
            Think(state);

            var evt = ThinkCompleted;
            if (evt != null)
                evt();
        }

        /// <summary>
        /// Gives the agent a chance to process the latest body state and perform any necessary actions.
        /// </summary>
        /// <param name="state">The latest snapshot of the agent's state.</param>
        public abstract void Think(PerceptorState state);

        #endregion

        #region Shutdown sequence

        void IAgent.OnShuttingDown()
        {
            OnShutDown();

            var evt = ShuttingDown;
            if (evt != null)
                evt();
        }

        /// <summary>
        /// Performs any final action required by the agent as the run loop exits.
        /// Called by the TinMan framework.  You are not required to override this method.
        /// </summary>
        protected virtual void OnShutDown() { }

        #endregion

        /// <summary>
        /// Requests that the <see cref="AgentHost"/> exit the run loop at the completion of this cycle.
        /// This action cannot be undone.
        /// </summary>
        protected void StopSimulation()
        {
            Log.Info("Agent requested that the simulation stops.");
            IsAlive = false;
        }
    }
}