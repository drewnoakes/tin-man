/*
 * Created by Drew, 16/05/2010 15:46.
 */
using System;
using System.Diagnostics;

namespace TinMan
{
    public interface IHingeController {
        Hinge Hinge { get; }
        void MoveTo(Angle angle);
        IEffectorCommand Step(PerceptorState state);
    }
    
    /// <summary>
    /// Responsible for the control logic of a single <see cref="Hinge"/>.
    /// Designed to allow for maximum speed and control.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Maintains actual/target positions</item>
    ///   <item>Produces actions to achieve target</item>
    ///   <item>Avoids oscillations and oversteering</item>
    ///   <item>Locks a joint in position, as during simulation it can be nudged out of place</item>
    /// </list>
    /// </remarks>
    public sealed class HingeController : IHingeController {
        /// <summary>The smallest difference between target and actual angles below which they are considered equal.  Avoids endless oscillation.</summary>
        private static readonly Angle EpsilonAngle = Angle.FromDegrees(1);
        
        private HingeControllerState _state = HingeControllerState.Resting;
        
        /// <summary>The last angle reported by the server.</summary>
        public Angle LastAngle { get; private set; }
        /// <summary>The angle required by the client.</summary>
        public Angle TargetAngle { get; private set; }
//        /// <summary>The last angle requested of the server.</summary>
//        public Angle RequestedAngle { get; private set; }
        /// <summary>The hinge being controlled.</summary>
        public Hinge Hinge { get; private set; }
        
        private int _correctionCount;

        public HingeController(Hinge hinge) {
            if (hinge==null)
                throw new ArgumentNullException("hinge");
            Hinge = hinge;
//            RequestedAngle = Angle.NaN;
            LastAngle = Angle.NaN;
            TargetAngle = Angle.NaN;
        }
                
        public IEffectorCommand Step(PerceptorState state) {
            Angle currentAngle = state.GetHingeAngle(Hinge);
            Angle moveLastCycle = LastAngle - currentAngle;
            LastAngle = currentAngle;
            if (TargetAngle.IsNaN)
                TargetAngle = LastAngle;
//            if (RequestedAngle.IsNaN)
//                RequestedAngle = LastAngle;
            
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
                    AngularSpeed speed;
                    if (_correctionCount<2)
                        speed = error/TimeSpan.FromMilliseconds(200);// * 0.2;
                    else if (_correctionCount<4)
                        speed = error/TimeSpan.FromMilliseconds(500);// * 0.1;
                    else
                        speed = error/TimeSpan.FromMilliseconds(800);// * 0.05;
                    _correctionCount++;
                    return new MoveHingeCommand(Hinge, speed);
                }
                case HingeControllerState.MovePending: {
//                    RequestedAngle = LastAngle;
                    _state = HingeControllerState.Moving;
                    return Step(state);
                }
                case HingeControllerState.Moving: {
                    Angle targetFromLast = (TargetAngle - LastAngle);
                    // If we've already requested this angle, and the hinge didn't move (much) in the last cycle
                    if (targetFromLast.Abs <= EpsilonAngle && moveLastCycle.Abs <= EpsilonAngle) {
                        _state = HingeControllerState.Resting;
                        return null;            
                    }
                    var maxSpeed = AngularSpeed.FromDegreesPerSecond(180);
                    var moveSpeed = (targetFromLast/TimeSpan.FromMilliseconds(500)).Limit(-maxSpeed, maxSpeed);
                    // Remember what we've requested so that next step we don't re-request the same
                    // range of motion and end up with double the move.
//                    RequestedAngle = Hinge.LimitAngle(move + RequestedAngle);
                    // TODO it looks like the value here is actually not a move angle, but rather a speed in radians per second
                    return new MoveHingeCommand(Hinge, moveSpeed);
                }
                default:
                    throw new Exception("Unexpected state: " + _state);
            }
        }
      
        public void MoveTo(Angle angle) {
            Hinge.ValidateAngle(angle);
            // If we're basically already at the requested target angle, return.
            if ((angle - TargetAngle).Abs < EpsilonAngle)
                return;
            TargetAngle = angle;
            _state = HingeControllerState.MovePending;
        }
    }
    
    public enum HingeControllerState {
        Resting,
        MovePending,
        Moving,
        Correcting
    }
}
