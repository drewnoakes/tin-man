/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Drew.RoboCup
{
	public interface IBodyManipulator {
	    bool IsFinished { get; }
	    
	    /// <summary>
	    /// Processes a step in the simulation.  Any side effects of this step are made to effector controllers
	    /// such as <see cref="HingeController"/>.
	    /// </summary>
	    /// <param name="simulationTime"></param>
	    void Step(TimeSpan simulationTime);
	}
}
