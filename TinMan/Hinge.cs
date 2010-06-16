/*
 * Created by Drew, 16/05/2010 15:46.
 */
using System;
using System.Diagnostics;

namespace TinMan
{
	/// <summary>
	/// Represents a single hinge joint within an agent's body.
	/// </summary>
	public sealed class Hinge {
	    /// <summary>Gets the minimum angle at which this hinge may be positioned.</summary>
	    public Angle MinAngle { get; private set; }
	    /// <summary>Gets the maximum angle at which this hinge may be positioned.</summary>
	    public Angle MaxAngle { get; private set; }
	    /// <summary></summary>
	    public string PerceptorLabel { get; private set; }
	    /// <summary></summary>
	    public string EffectorLabel { get; private set; }
	
	    public Hinge(string perceptorLabel, string effectorLabel, Angle minAngle, Angle maxAngle) {
	        PerceptorLabel = perceptorLabel;
	        EffectorLabel = effectorLabel;
	        MinAngle = minAngle;
	        MaxAngle = maxAngle;
	    }
	    
	    #region Angular limits
	
	    public void ValidateAngle(Angle angle) {
	        if (!IsAngleValid(angle))
	            throw new ArgumentOutOfRangeException("angle", string.Format("{0} is not a valid angle for hinge {1}.  The range is between {2} and {3}.", angle, EffectorLabel, MinAngle, MaxAngle));
	    }
	    
	    public bool IsAngleValid(Angle angle) {
	        return angle <= MaxAngle && angle >= MinAngle;
	    }
	    
	    public Angle LimitAngle(Angle angle) {
	        return angle.Limit(MinAngle, MaxAngle);
	    }
	    
	    #endregion
	}
}
