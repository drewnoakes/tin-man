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
// Created 05/06/2010 04:43

using System;

namespace TinMan
{
    /// <summary>
    /// General interface for any object that wishes to be notified of new perceptor state.
    /// </summary>
    public interface IStateObserver {
        /// <summary>
        /// Called with updated perceptor state.
        /// </summary>
        /// <param name="state">The updated perceptor state.</param>
        void Observe(PerceptorState state);
    }
}
