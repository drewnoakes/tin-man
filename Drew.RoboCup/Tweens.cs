/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    // TODO write unit tests for these
    public sealed class WaitMotion : IBodyManipulator {
        public bool IsFinished { get; private set; }
        public TimeSpan WaitTime { get; private set; }
        private TimeSpan _startTime = TimeSpan.Zero;
        public WaitMotion(TimeSpan waitTime) {
            WaitTime = waitTime;
        }
        public void Step(TimeSpan simulationTime)
        {
            if (_startTime==TimeSpan.Zero) {
                _startTime = simulationTime;
            } else {
                if (simulationTime - _startTime > WaitTime)
                    IsFinished = true;
            }
        }
    }
    
    public sealed class HingeMotionTween : IBodyManipulator {
        private readonly HingeController _hinge;
        private readonly Angle _finalAngle;
        private readonly TimeSpan _moveDuration;
        private readonly EasingFunction _easingFunction;
        private TimeSpan _moveStartTime = TimeSpan.MinValue;
        private Angle _moveStartAngle = Angle.NaN;
        
        public HingeMotionTween(HingeController hinge, Angle finalAngle, TimeSpan moveDuration, EaseType easeType) {
            if (moveDuration<=TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("moveDuration", "Must be greater than zero.");
            hinge.ValidateAngle(finalAngle);
            _hinge = hinge;
            _finalAngle = finalAngle;
            _moveDuration = moveDuration;
            _easingFunction = Easing.GetFunction(easeType);
        }
        
        public bool IsFinished { get; private set; }
        
        public void Step(TimeSpan simulationTime) {
            Debug.Assert(simulationTime >= _moveStartTime, "SimulationTime should be equal to or after _moveStartTime.");
            Debug.Assert(!IsFinished, "IsFinished should be false.");
            
            if (_moveStartTime==TimeSpan.MinValue) {
                _moveStartTime = simulationTime;
                _moveStartAngle = _hinge.TargetAngle;
            }
            
            var elapsed = simulationTime - _moveStartTime;
            
            if (elapsed >= _moveDuration) {
                // Ensure we set the correct target
                _hinge.MoveTo(_finalAngle);
                // This motion is finished
                IsFinished = true;
                return;
            }
                        
            var moveTotalAngle = _finalAngle - _moveStartAngle;
            var moveTimeRatio = elapsed.Ticks / (double)_moveDuration.Ticks;
            var expectedRadians = _easingFunction(elapsed.TotalMilliseconds,
                                                _moveStartAngle.Radians, 
                                                moveTotalAngle.Radians, 
                                                _moveDuration.TotalMilliseconds);
            
            _hinge.MoveTo(Angle.FromRadians(expectedRadians));
        }
    }
    
    public sealed class TweenQueue : IBodyManipulator, IEnumerable<IBodyManipulator> {
        private readonly Queue<IBodyManipulator> _queue = new Queue<IBodyManipulator>();
        public bool IsFinished { get; private set; }
        public string Name { get; set; }

        public void Add(IBodyManipulator tween) {
            _queue.Enqueue(tween);
            IsFinished = false;
        }

        public void Step(TimeSpan simulationTime) {
            // TODO test this
            IBodyManipulator tween = null;
            while (_queue.Count>0 && (tween=_queue.Peek()).IsFinished)
                _queue.Dequeue();
            
            if (_queue.Count==0) {
                IsFinished = true;
                return;
            }
            
            tween.Step(simulationTime);
        }
        
        IEnumerator<IBodyManipulator> IEnumerable<IBodyManipulator>.GetEnumerator() {
            return _queue.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((IEnumerable<IBodyManipulator>)this).GetEnumerator();
        }
    }
    
    public sealed class TweenGroup : IBodyManipulator, IEnumerable<IBodyManipulator> {
        private readonly LinkedList<IBodyManipulator> _list = new LinkedList<IBodyManipulator>();
        public bool IsFinished { get; private set; }
        
        public void Add(IBodyManipulator tween) {
            _list.AddLast(tween);
            IsFinished = false;
        }
        
        public void Clear() {
            _list.Clear();
            IsFinished = true;
        }
        
        public void Step(TimeSpan simulationTime) {
            var node = _list.First;
            while (node!=null) {
                if (node.Value.IsFinished) {
                    _list.Remove(node);
                } else {
                    node.Value.Step(simulationTime);
                }
                node = node.Next;
            }
            
            if (_list.First==null)
                IsFinished = true;
        }
    
        IEnumerator<IBodyManipulator> IEnumerable<IBodyManipulator>.GetEnumerator() {
            return _list.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((IEnumerable<IBodyManipulator>)this).GetEnumerator();
        }
    }

/*    
    /// <summary>
    /// Oscillates between two values consistently.
    /// </summary>
    public sealed class BouncingHingeMotionTween : IBodyManipulator {
        private readonly HingeController _hinge;
        private readonly double _fromAngle;
        private readonly double _toAngle;
        private readonly TimeSpan _period;
        private TimeSpan _startTime = TimeSpan.MinValue;

        public BouncingHingeMotionTween(HingeController hinge, double fromAngle, double toAngle, TimeSpan period) {
            if (period<=TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("period", "Must be greater than zero.");
            _hinge = hinge;
            _fromAngle = fromAngle;
            _toAngle = toAngle;
            _period = period;
        }
        
        public bool IsFinished { get; set; }
        
        public void Step(TimeSpan simulationTime) {
            Debug.Assert(simulationTime >= _startTime, "SimulationTime should be equal to or after StartTime.");
            
            if (_startTime==TimeSpan.MinValue)
                _startTime = simulationTime;
            
//            var hingeAngle = state.GetHingeJointAngle(_perceptorLabel);

            TimeSpan elapsed = simulationTime - _startTime;
            double cycles = elapsed.TotalSeconds / _period.TotalSeconds;
            
            var cyclesCompleted = (int)cycles;
            var isAngleIncreasing = cyclesCompleted%2==0; // TODO
            
            var progressRatio = cycles - cyclesCompleted;

            var totalArc = _toAngle - _fromAngle;
            
            double expectedPosition = 
                isAngleIncreasing 
                ? ( progressRatio    * totalArc) + _fromAngle
                : ((1-progressRatio) * totalArc) + _fromAngle;
            
//            if (( isAngleIncreasing && _hinge.Angle >= expectedPosition) ||
//                (!isAngleIncreasing && _hinge.Angle <= expectedPosition)) {
//                // for whatever reason, the hinge is ahead of where we expect it to be right now
//                // so just return null and wait until the next cycle
//
//                Console.WriteLine("{0} time={1:0000.00}, cycle={2:0000.00} cur={3:000.00}, target={4:000.00}, move=SKIP",
//                    _effectorLabel.PadRight(8), simulationTime.TotalSeconds, cycles, hingeAngle, expectedPosition);
//                return;
//            }
                        
//            double angleDelta = expectedPosition - _hinge.RequestedAngle;
//            
//            Console.WriteLine("{0} time={1:0000.00}, cycle={2:0000.00} cur={3:000.00}, target={4:000.00}, move={5:000.00}",
//                              _hinge.EffectorLabel.PadRight(8), simulationTime.TotalSeconds, cycles, hingeAngle, expectedPosition, angleDelta);
            
            _hinge.MoveTo(expectedPosition);
//            return string.Format("({0} {1:0.########})", _effectorLabel, angleDelta);
        }
    }
*/    
}