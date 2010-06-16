/*
 * Created by Drew, 10/05/2010 12:43.
 */
using System;

namespace TinMan
{
    // TODO lose this interface
	public interface IUserInteractiveAgent {
	    void HandleUserInput(char key, ISimulationContext context);
	}
}
