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
// Created 07/05/2010 02:50

namespace TinMan
{
    /// <summary>
    /// A collection of static geometry-related utility methods.
    /// </summary>
    public static class GeometryUtil
    {
        /// <summary>
        /// Calculates the distance along a line that is closest to <paramref name="point"/>.
        /// </summary>
        /// <param name="origin">The starting point of the line.</param>
        /// <param name="direction">The direction of the line.</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double CalculateDistanceAlongLineThatIsClosestToPoint(Vector3 origin, Vector3 direction, Vector3 point)
        {
            // TODO if a dedicated Ray class is made, move this method to it
            Vector3 v = direction.Normalize();
            Vector3 s = v.Cross(new Vector3(0, 0, 1));
            double u
                = ((s.X/s.Y)*(origin.Y - point.Y) + (point.X - origin.X))
                  /(v.X - (s.X/s.Y)*v.Y);
            return (origin + v*u - point).Length;
        }
    }
}