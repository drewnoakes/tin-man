/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
    public abstract class AgentBase<TBody> : IAgent where TBody : IBody {
        
        public TBody Body { get; private set; }
        IBody IAgent.Body { get { return ((AgentBase<TBody>)this).Body; } }
        
        /// <summary>
        /// Initialises AgentBase with a body instance.
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="ArgumentNullException"><paramref name="body"/> is null.</exception>
        protected AgentBase(TBody body) {
            if (body==null)
                throw new ArgumentNullException("body");
            Body = body;
        }
        
        /// <summary>
        /// Gives the agent a chance to process the latest body state and
        /// perform any necessary actions.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        public abstract void Step(ISimulationContext context, PerceptorState state);
    }
    
    public interface IAgent {
        IBody Body { get; }
        
        /// <summary>
        /// Gives the agent a chance to process the latest body state and
        /// perform any necessary actions.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="state"></param>
        void Step(ISimulationContext context, PerceptorState state);
    }
}
