/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    public interface IRobot {
        string RsgPath { get; }
        string TeamName { get; }
        int UniformNumber { get; }

        FieldSide TeamSide { get; }
        PlayMode PlayMode { get; }
        
        IEnumerable<IAction> Step(PerceptorState state);
    }
    
    public interface IInteractiveRobot {
        void HandleUserInput(char key);
    }
}
