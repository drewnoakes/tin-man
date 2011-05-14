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

using System;

namespace TinMan
{
    /// <summary>
    /// Represents a vector in polar coordinates.  This type is immutable.
    /// </summary>
    public struct Polar
    {
        /// <summary>A constant polar value of zero.</summary>
        public static readonly Polar Zero;

        /// <summary>The distance from the origin.</summary>
        public double Distance { get; private set; }

        /// <summary>
        /// The angle in the horizontal plane. For an agent standing upright on the field, increasing values of this property represent
        /// polar points moving from left to right.  Zero degrees points straight ahead in that frame of reference.
        /// </summary>
        public Angle Theta { get; private set; }

        /// <summary>
        /// The latitudal angle. For an agent standing upright on the field, increasing values of this property represent polar points
        /// moving from below eyeline, upwards toward the sky.  Zero degrees means horizontal.  A negative angle implies the ray is
        /// angled downwards from its origin within that frame of reference.
        /// </summary>
        public Angle Phi { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="theta">Angle in the horizontal plane.  Zero points towards the opponent's goal.</param>
        /// <param name="phi">Lattitudinal angle.  Zero means horizontal, negative pointing downwards.</param>
        public Polar(double distance, Angle theta, Angle phi) : this()
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException("distance", distance, "Must be zero or greater.");

            // TODO have seen NaN values from the server... should these be allowed?

            Distance = distance;
            Theta = theta;
            Phi = phi;
        }

        /// <summary>
        /// Convert from polar coordinates to cartesian coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector3 ToVector3()
        {
            /*
             * Code from TickTack:
             *   f1l[0] = F1L.dist * cos(F1L.phi * M_PI/180.0) * cos(F1L.theta * M_PI/180.0);
             *   f1l[1] = F1L.dist * cos(F1L.phi * M_PI/180.0) * sin(F1L.theta * M_PI/180.0);
             *   f1l[2] = F1L.dist * sin(F1L.phi * M_PI/180.0);
             * Note that Apollo3D does this differently:
             *   float theta = gDegToRad(gNormalizeDeg(vs.theta + 90.0f)); // WHY DO THEY ADD 90 DEGREES HERE?
             *   float phi   = gDegToRad(gNormalizeDeg(vs.phi));
             *   vs.localPos[0] = distance * gCos(phi) * gCos(theta);
             *   vs.localPos[1] = distance * gCos(phi) * gSin(theta);
             *   vs.localPos[2] = distance * gSin(phi);
             * And LGB does this differently again:
             *   double dmark = cos(polar.phi()) * polar.dist();
             *   cartesian.x() = (sin(-polar.theta()) * dmark);   // WHY DO THEY NEGATE THETA HERE?
             *   cartesian.y() = (cos(-polar.theta()) * dmark);   // WHY DO THEY NEGATE THETA HERE?
             *   cartesian.z() = (sin(polar.phi()) * polar.dist());
             */

            double t = Distance*Phi.Cos;
            double x = t*Theta.Cos;
            double y = t*Theta.Sin;
            double z = Distance*Phi.Sin;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Gets a value indicating whether the distance and both angles are equal to zero.
        /// </summary>
        public bool IsZero
        {
            get { return Distance == 0 && Theta == Angle.Zero && Phi == Angle.Zero; }
        }

        public override string ToString()
        {
            return string.Format("<{0:0.00} θ={1:0.00}° φ={2:0.00}°>", Distance, Theta.Degrees, Phi.Degrees);
        }
    }
}