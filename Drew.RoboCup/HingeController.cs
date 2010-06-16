/*
 * Created by Drew, 16/05/2010 15:46.
 */
using System;
using System.Diagnostics;

namespace Drew.RoboCup
{
    /// <summary>
    /// Responsible for the control logic of a single hinge joint.  Designed to allow for maximum power and control.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Maintains actual/target positions</item>
    ///   <item>Produces actions to achieve target</item>
    ///   <item>Avoids oscillations and oversteering</item>
    ///   <item>Models joint's angle limits</item>
    ///   <item>Locks a joint in position, as during simulation it can be nudged out of place</item>
    /// </list>
    /// </remarks>
    public sealed class HingeController {
        /// <summary>The smallest difference between target and actual angles below which they are considered equal.  Avoids endless oscillation.  In degrees.</summary>
        private const double EpsilonDegrees = 1;
        
        public double MinAngle { get; private set; }
        public double MaxAngle { get; private set; }
        public string PerceptorLabel { get; private set; }
        public string EffectorLabel { get; private set; }

        private HingeControllerState _state = HingeControllerState.Resting;
        
        /// <summary>The last angle reported by the server.</summary>
        public double LastAngle { get; private set; }
        /// <summary>The angle required by the client.</summary>
        public double TargetAngle { get; private set; }
        /// <summary>The last angle requested of the server.</summary>
        public double RequestedAngle { get; private set; }
        
        private int _correctionCount;

        public HingeController(string perceptorLabel, string effectorLabel, double minAngle, double maxAngle) {
            PerceptorLabel = perceptorLabel;
            EffectorLabel = effectorLabel;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            RequestedAngle = double.NaN;
            LastAngle = double.NaN;
            TargetAngle = double.NaN;
        }
                
        public IAction Step(PerceptorState state) {
            double currentAngle = state.GetHingeJointAngle(PerceptorLabel);
            double moveLastCycle = LastAngle - currentAngle;
            LastAngle = currentAngle;
            if (double.IsNaN(TargetAngle))
                TargetAngle = LastAngle;
            if (double.IsNaN(RequestedAngle))
                RequestedAngle = LastAngle;
            
            switch (_state) {
                case HingeControllerState.Resting: {
                    var error = TargetAngle - LastAngle;
                    if (Math.Abs(error) > EpsilonDegrees * 2) {
                        // We've supposedly finished moving, but are not in position anymore. Perhaps the joint was forced.
                        _state = HingeControllerState.Correcting;
                        _correctionCount = 0;
                        return Step(state);
                    }
                    return null;
                }
                case HingeControllerState.Correcting: {
                    var error = TargetAngle - LastAngle;
                    if (Math.Abs(error) < EpsilonDegrees) {
                        _state = HingeControllerState.Resting;
                        return null;
                    }
                    // Apply a gentle correction, controlled by the passing of time rather than magnitude of error
                    double move;
                    if (_correctionCount<2)
                        move = error * 0.2;
                    else if (_correctionCount<4)
                        move = error * 0.1;
                    else
                        move = error * 0.05;
                    _correctionCount++;
                    return new MoveHingeJointAction(EffectorLabel, move);
                }
                case HingeControllerState.MovePending: {
                    RequestedAngle = LastAngle;
                    _state = HingeControllerState.Moving;
                    return Step(state);
                }
                case HingeControllerState.Moving: {
                    double targetFromRequested = (TargetAngle - RequestedAngle);
                    // If we've already requested this angle, and the hinge didn't move (much) in the last cycle
                    if (Math.Abs(targetFromRequested) <= EpsilonDegrees && Math.Abs(moveLastCycle) <= EpsilonDegrees) {
                        _state = HingeControllerState.Resting;
                        return null;            
                    }
                    const double MaxDegreesPerCycle = 10;
                    double move = Math.Max(-MaxDegreesPerCycle, Math.Min(MaxDegreesPerCycle, targetFromRequested));
                    // Remember what we've requested so that next step we don't re-request the same
                    // range of motion and end up with double the move.
                    RequestedAngle = Math.Min(MaxAngle, Math.Max(MinAngle, move + RequestedAngle));
                    // TODO it looks like the value here is actually not a move angle, but rather a speed in radians per second
                    return new MoveHingeJointAction(EffectorLabel, move);
                }
                default:
                    throw new Exception("Unexpected state: " + _state);
            }
        }
      
        public void MoveTo(double angle) {
            ValidateAngle(angle);
            // If we're basically already at the requested target angle, return.
            if (Math.Abs(angle - TargetAngle) < EpsilonDegrees)
                return;
            TargetAngle = angle;
            _state = HingeControllerState.MovePending;
        }

        public void ValidateAngle(double angle) {
            if (!IsAngleValid(angle))
                throw new ArgumentOutOfRangeException("angle", string.Format("{0} is not a valid angle for hinge {1}.  The range is between {2} and {3}.", angle, EffectorLabel, MinAngle, MaxAngle));
        }
        
        public bool IsAngleValid(double angle) {
            return angle <= MaxAngle && angle >= MinAngle;
        }
        
        public double LimitAngle(double angle) {
            return Math.Min(Math.Max(angle, MinAngle), MaxAngle);
        }
    }
    
    public enum HingeControllerState {
        Resting,
        MovePending,
        Moving,
        Correcting
    }
}
