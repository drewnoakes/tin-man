/*
 * Created by Drew, 16/05/2010 15:46.
 */
using System;
using System.Diagnostics;

namespace Drew.RoboCup
{
    /// <summary>
    /// Responsible for the control logic of a single hinge joint.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Maintains actual/target positions</item>
    ///   <item>Produces actions to achieve target</item>
    ///   <item>Avoids oscillations and oversteering</item>
    ///   <item>Models joint's angle limits</item>
    ///   <item>Reports when a requested move doesn't eventuate after a given time elapses</item>
    ///   <item>Locks a joint in position, as during simulation it can be nudged out of place</item>
    /// </list>
    /// </remarks>
    public sealed class HingeController     {
        public double MinAngle { get; private set; }
        public double MaxAngle { get; private set; }
        public string PerceptorLabel { get; private set; }
        public string EffectorLabel { get; private set; }

        /// <summary>The smallest difference between target and actual angles below which they are considered equal.  Avoids endless oscillation.  In degrees.</summary>
        private const double Epsilon = 0.2;
        
        /// <summary>The last angle reported by the server.</summary>
        private double _currentAngle = double.NaN;
        /// <summary>The angle required by the client.</summary>
        private double _targetAngle = double.NaN;
        /// <summary>The last angle requested of the server.</summary>
        public double RequestedAngle { get; private set; }

        public HingeController(string perceptorLabel, string effectorLabel, double minAngle, double maxAngle) {
            PerceptorLabel = perceptorLabel;
            EffectorLabel = effectorLabel;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            RequestedAngle = double.NaN;
        }
        
        private const int CountdownFrom = 15; // 300ms
        private int _countdown = CountdownFrom;
        
        public string Step(PerceptorState state) {
            double angle = state.GetHingeJointAngle(PerceptorLabel);
            
            // This seems to happen fairly often, though never by very much.  No point in flagging it though.
            // Debug.Assert(angle <= MaxAngle, "Latest angle is larger than the maximum angle.");
            // Debug.Assert(angle >= MinAngle, "Latest angle is smaller than the minimum angle.");
            
            if (double.IsNaN(_currentAngle)) {
                // First step seen
                RequestedAngle = angle;
                
                if (double.IsNaN(_targetAngle))
                    _targetAngle = angle;
            }
            
            _currentAngle = angle;
            
            if (Math.Abs(_currentAngle - _targetAngle) < Epsilon) {
                // We've reached our target
                //                Console.WriteLine("{0} now={1:000.00}, target={2:000.00}, requested={3:000.00}, move=NONE",
                //                                  _effectorLabel, _currentAngle, _targetAngle, RequestedAngle);
                return null;
            }
          
            // Use a countdown that, as it approaches zero, reduces the requested angle influence.
            // The countdown is reset whenever a new angle is set.
            
            // Still not at the target.  Calculate the discrepancy.
            double targetFromRequested = (_targetAngle - RequestedAngle); // * 0.1;
            // Sometimes the requested angle is not attained due to resistance forces/rounding errors/whatever.
            // Include a small calibration amount if we're far enough away.
            // TODO calibration should be based on time, not a threshold (5 deg)... need a way of knowing if our requested angle is not met in a reasonable period, even if target is constantly updating
            double calibration = (_targetAngle - _currentAngle); // * 0.02; // 20ms cycle means 0.2 ratio gives 100% in 100ms
//            if (Math.Abs(calibration) > 5)

            if (Math.Abs(calibration) <= Epsilon)
                return null;            

            double timeFactor = _countdown / (double)CountdownFrom;
            double move = (timeFactor * targetFromRequested * 0.2)
                       + ((1-timeFactor) * calibration * 0.025);
            
            // Remember what we've requested so that next step we don't re-request the same
            // range of motion and end up with double the move.
            RequestedAngle = Math.Min(MaxAngle, Math.Max(MinAngle, move + RequestedAngle));
            
//            Console.WriteLine("{0} now={1:000.00}, target={2:000.00}, requested={3:000.00}, calibration={4:0.00}, move={5:0.00}",
//                              _effectorLabel, _currentAngle, _targetAngle, RequestedAngle, calibration, move);

            if (_countdown > 0)
                _countdown--;

            return string.Format("({0} {1:0.######})", EffectorLabel, move);
        }
        
        public void MoveTo(double angle) {
            ValidateAngle(angle);
            _countdown = CountdownFrom;
            _targetAngle = angle;
        }

        public void ValidateAngle(double angle) {
            if (angle > MaxAngle)
                throw new ArgumentOutOfRangeException("angle", string.Format("Attempting to set hinge {0} angle to {1} but the maximum is {2}.", EffectorLabel, angle, MaxAngle));
            if (angle < MinAngle)
                throw new ArgumentOutOfRangeException("angle", string.Format("Attempting to set hinge {0} angle to {1} but the minimum is {2}.", EffectorLabel, angle, MinAngle));
        }
        
        public bool IsAngleValid(double angle) {
            return angle <= MaxAngle && angle >= MinAngle;
        }
    }
}
