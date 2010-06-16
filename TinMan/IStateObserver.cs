/*
 * Created by Drew, 05/06/2010 04:43.
 */
using System;

namespace TinMan
{
	public interface IStateObserver {
	    /// <summary>
	    /// Processes a step in the simulation.  Any side effects of this step are made to effector controllers
	    /// such as <see cref="HingeController"/>.
	    /// </summary>
	    /// <param name="simulationTime"></param>
	    void Observe(PerceptorState state);
	}
}
