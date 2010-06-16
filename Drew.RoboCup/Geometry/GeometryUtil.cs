/*
 * Created by Drew, 07/05/2010 02:50.
 */
using System;

namespace Drew.RoboCup
{
	public static class GeometryUtil {
	    public static double NormaliseDegrees(double degrees) {
	        // TODO can this be done this with mod division?
	        while (degrees < 0)
	            degrees += 360;
	        while (degrees >= 360)
	            degrees -= 360;
	        return degrees;
	    }
	    
	    public static double DegreesToRadians(double degrees) {
	        const double degToRadFactor = Math.PI/180;
	        return degrees * degToRadFactor;
	    }
	    
	    public static double RadiansToDegrees(double radians) {
	        const double radToDegFactor = 180/Math.PI;
	        return radians * radToDegFactor;
	    }
	}
}
