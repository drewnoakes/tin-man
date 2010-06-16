/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
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
    
    public interface IUserInteractiveAgent {
        void HandleUserInput(char key, ISimulationContext context);
    }
}
