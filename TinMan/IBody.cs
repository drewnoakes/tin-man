/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Collections.Generic;

namespace Drew.RoboCup
{
	public interface IBody
	{
		HingeController GetHingeControllerForLabel(string label);
		IEnumerable<HingeController> AllHinges { get; }
	}
}
