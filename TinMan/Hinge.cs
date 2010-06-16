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
	    internal bool IsSpeedChanged { get; private set; }
	    
	    private AngularSpeed _speed = AngularSpeed.NaN;
	    public AngularSpeed Speed {
	        get { return _speed; }
	        set {
	            if (_speed==value)
	                return;
	            _speed = value;
	            IsSpeedChanged = true;
	        }
	    }

	    /// <summary>
	    /// Gets the current anglular position of this hinge.  This value is updated for each
	    /// simulation cycle before any agent code is executed.
	    /// </summary>
	    public Angle Angle { get; internal set; }
	    
	    public Hinge(string perceptorLabel, string effectorLabel, Angle minAngle, Angle maxAngle) {
	        PerceptorLabel = perceptorLabel;
	        EffectorLabel = effectorLabel;
	        MinAngle = minAngle;
	        MaxAngle = maxAngle;
	    }
	    
	    internal HingeSpeedCommand GetCommand() {
	        if (!IsSpeedChanged)
	            throw new InvalidOperationException("The speed value for this hinge was not changed.  Check IsSpeedChanged before calling this method.");
	        IsSpeedChanged = false;
	        return new HingeSpeedCommand(this, Speed);
	    }
	    
	    #region Angular limits
	
	    public void ValidateAngle(Angle angle) {
	        if (angle.IsNaN)
	            throw new ArgumentOutOfRangeException("angle", "NaN is invalid as an angle.");
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
