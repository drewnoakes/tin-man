/*
 * Created by Drew, 07/05/2010 02:50.
 */
using System;

namespace Drew.RoboCup
{
	public static class GeometryUtil {
        /// <summary>
        /// Calculates the distance along a line that is closest to <paramref name="point"/>.
        /// </summary>
        /// <param name="origin">The starting point of the line.</param>
        /// <param name="direction">The direction of the line.</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double CalculateDistanceAlongLineThatIsClosestToPoint(Vector3 origin, Vector3 direction, Vector3 point) {
            // TODO if a dedicated Ray class is made, move this method to it
            Vector3 v = direction.Normalize();
            Vector3 s = v.Cross(new Vector3(0,0,1));
            double u 
                = ((s.X/s.Y) * (origin.Y - point.Y) + (point.X - origin.X))
                / (v.X - (s.X/s.Y)*v.Y);
            return (origin + v*u - point).GetLength();
        }
	}
}
