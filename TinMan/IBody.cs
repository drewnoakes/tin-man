#region License
/* 
 * This file is part of TinMan.
 *
 * TinMan is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TinMan is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with TinMan.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 06/05/2010 15:08

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
