/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    public interface IRobot {
        // TODO separate out soccer from non-soccer items
        
        IEnumerable<IAction> GetInitialisationActions();
        
        int PlayerId { get; }
        FieldSide TeamSide { get; }
        PlayMode PlayMode { get; }
        string TeamName { get; }
        
        IEnumerable<IAction> Step(PerceptorState state);
    }
    
    public interface IInteractiveRobot {
        void HandleUserInput(char key);
    }
}
