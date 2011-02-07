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

using System.Collections.Generic;

namespace TinMan
{
    /// <summary>
    /// Interface for all agent bodies used in TinMan.  Two models that ship with SimSpark are already
    /// implemented in TimMan, namely <see cref="NaoBody"/> and <see cref="SoccerbotBody"/>.
    /// </summary>
    public interface IBody
    {
        /// <summary>
        /// Gets a well-known path of the Ruby Scene Graph (RSG) file in the RCSS3D server package
        /// for the model to be loaded for this agent's body.
        /// </summary>
        string RsgPath { get; }

        /// <summary>
        /// Performs a lookup to find the <see cref="Hinge"/> with specified effector label.
        /// Returns <c>null</c> if no hinge is found.
        /// </summary>
        /// <param name="effectorLabel">The label of the hinge's effector.</param>
        /// <returns>The <see cref="Hinge"/> if found, otherwise <c>null</c>.</returns>
        Hinge GetHingeForEffectorLabel(string effectorLabel);

        /// <summary>Gets all <see cref="Hinge"/> instances in the agent's body.</summary>
        IEnumerable<Hinge> AllHinges { get; }

/*
        /// <summary>
        /// Performs a lookup to find the universal joint with specified effector label.  Returns <c>null</c> if
        /// no universal joint is found.
        /// </summary>
        /// <param name="effectorLabel"></param>
        /// <returns></returns>
        UniversalJoint GetUniversalJointForEffectorLabel(string effectorLabel);

        /// <summary>Gets all universal joints in the agent's body.</summary>
        IEnumerable<UniversalJoint> AllUniversalJoints { get; }
*/

        /// <summary>
        /// Converts a polar coordinate from the agent's view perspective (as is reported by vision
        /// perceptors) into a vector in the agent's local coordinates.
        /// </summary>
        /// <param name="cameraView"></param>
        /// <returns></returns>
        Vector3 ConvertCameraPolarToLocalVector(Polar cameraView);
    }
}