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
        /// <summary>The smallest difference between target and actual angles below which they are considered equal.  Avoids endless oscillation.</summary>
        private static readonly Angle EpsilonAngle = Angle.FromDegrees(1);
        
        public Angle MinAngle { get; private set; }
        public Angle MaxAngle { get; private set; }
        public string PerceptorLabel { get; private set; }
        public string EffectorLabel { get; private set; }

        private HingeControllerState _state = HingeControllerState.Resting;
        
        /// <summary>The last angle reported by the server.</summary>
        public Angle LastAngle { get; private set; }
        /// <summary>The angle required by the client.</summary>
        public Angle TargetAngle { get; private set; }
        /// <summary>The last angle requested of the server.</summary>
        public Angle RequestedAngle { get; private set; }
        
        private int _correctionCount;

        public HingeController(string perceptorLabel, string effectorLabel, Angle minAngle, Angle maxAngle) {
            PerceptorLabel = perceptorLabel;
            EffectorLabel = effectorLabel;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            RequestedAngle = Angle.NaN;
            LastAngle = Angle.NaN;
            TargetAngle = Angle.NaN;
        }
                
        public IAction Step(PerceptorState state) {
            Angle currentAngle = state.GetHingeJointAngle(PerceptorLabel);
            Angle moveLastCycle = LastAngle - currentAngle;
            LastAngle = currentAngle;
            if (TargetAngle.IsNaN)
                TargetAngle = LastAngle;
            if (RequestedAngle.IsNaN)
                RequestedAngle = LastAngle;
            
            switch (_state) {
                case HingeControllerState.Resting: {
                    var error = TargetAngle - LastAngle;
                    if (error.Abs > EpsilonAngle * 2) {
                        // We've supposedly finished moving, but are not in position anymore. Perhaps the joint was forced.
                        _state = HingeControllerState.Correcting;
                        _correctionCount = 0;
                        return Step(state);
                    }
                    return null;
                }
                case HingeControllerState.Correcting: {
                    var error = TargetAngle - LastAngle;
                    if (error.Abs < EpsilonAngle) {
                        _state = HingeControllerState.Resting;
                        return null;
                    }
                    // Apply a gentle correction, controlled by the passing of time rather than magnitude of error
                    Angle move;
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
                    Angle targetFromRequested = (TargetAngle - RequestedAngle);
                    // If we've already requested this angle, and the hinge didn't move (much) in the last cycle
                    if (targetFromRequested.Abs <= EpsilonAngle && moveLastCycle.Abs <= EpsilonAngle) {
                        _state = HingeControllerState.Resting;
                        return null;            
                    }
                    Angle MaxDegreesPerCycle = Angle.FromDegrees(10);
//                    double move = Math.Max(-MaxDegreesPerCycle, Angle.Min(MaxDegreesPerCycle, targetFromRequested));
                    Angle move = targetFromRequested.Limit(-MaxDegreesPerCycle, MaxDegreesPerCycle);
                    // Remember what we've requested so that next step we don't re-request the same
                    // range of motion and end up with double the move.
                    RequestedAngle = (move + RequestedAngle).Limit(MinAngle, MaxAngle);
                    // TODO it looks like the value here is actually not a move angle, but rather a speed in radians per second
                    return new MoveHingeJointAction(EffectorLabel, move);
                }
                default:
                    throw new Exception("Unexpected state: " + _state);
            }
        }
      
        public void MoveTo(Angle angle) {
            ValidateAngle(angle);
            // If we're basically already at the requested target angle, return.
            if ((angle - TargetAngle).Abs < EpsilonAngle)
                return;
            TargetAngle = angle;
            _state = HingeControllerState.MovePending;
        }

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
    }
    
    public enum HingeControllerState {
        Resting,
        MovePending,
        Moving,
        Correcting
    }
}
