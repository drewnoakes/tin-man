/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
    public interface IAgent {
        /// <summary>
        /// Gets a string that indicates the path to the Ruby Scene Graph (RSG) file on the server
        /// that the simulator should load for this agent.  A common choice is <see cref="Nao.RsgFilePath"/>.
        /// </summary>
        string RsgPath { get; }
        
        /// <summary>
        /// Updates the agent with the latest perceptor state, returning zero or more commands
        /// for the agent's effectors.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        IEnumerable<IEffectorCommand> Step(PerceptorState state);
    }
    
    public interface IUserInteractiveAgent {
        void HandleUserInput(char key);
    }
}
