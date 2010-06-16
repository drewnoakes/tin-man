/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Collections.Generic;

namespace TinMan
{
	public interface IBody {
        /// <summary>
        /// Gets a well-known path of the Ruby Scene Graph (RSG) file in the RCSS3D server package
        /// for the model to be loaded for this agent's body.
        /// </summary>
        string RsgPath { get; }

        /// <summary>
        /// Performs a lookup to find the hinge with specified effector label.
        /// </summary>
        /// <param name="effectorLabel"></param>
        /// <returns></returns>
		Hinge GetHingeForEffectorLabel(string effectorLabel);
		
		IEnumerable<Hinge> AllHinges { get; }
		
		Vector3 ConvertCameraPolarToLocalVector(Polar cameraView);
	}
}
