/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRobot
    {
        // TODO separate out soccer from non-soccer items
        
        IEnumerable<string> GetInitialisationMessages();
        
        int PlayerId { get; }
        FieldSide TeamSide { get; }
        PlayMode PlayMode { get; }
        string TeamName { get; }
        
        // TODO rather than returning a string, how about a set of actions that the caller would serialise?
        string Step(PerceptorState state);
        
        void HandleUserInput(char key);
    }
}
