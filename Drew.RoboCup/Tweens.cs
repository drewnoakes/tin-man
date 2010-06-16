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
    // TODO allow for other types of non-linear tweening to smooth motion, for example
    // TODO write unit tests for these
    
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
    
    public sealed class HingeMotionTween : IBodyManipulator {
        public event Action Finished;
        private readonly HingeController _hinge;
        private readonly double _fromAngle;
        private readonly double _toAngle;
        private readonly TimeSpan _duration;
        private TimeSpan _startTime = TimeSpan.MinValue;
        
        public HingeMotionTween(HingeController hinge, double fromAngle, double toAngle, TimeSpan duration) {
            if (duration<=TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "Must be greater than zero.");
            _hinge = hinge;
            _fromAngle = fromAngle;
            _toAngle = toAngle;
            _duration = duration;
        }
        
        public bool IsFinished { get; private set; }
        
        public void Step(TimeSpan simulationTime) {
            Debug.Assert(simulationTime >= _startTime, "SimulationTime should be equal to or after StartTime.");
            
            if (_startTime==TimeSpan.MinValue)
                _startTime = simulationTime;
            
//            var hingeAngle = state.GetHingeJointAngle(_perceptorLabel);
            
            var isAngleIncreasing = _toAngle > _fromAngle;
            
            if (( isAngleIncreasing && _hinge.RequestedAngle >= _toAngle) ||
                (!isAngleIncreasing && _hinge.RequestedAngle <= _toAngle)) {
                IsFinished = true;
                if (Finished!=null)
                    Finished();
                return;
            }
            
            if (simulationTime > _startTime + _duration)
                Console.WriteLine("HingeMotionTween has run beyond requested duration.");
            
            var elapsed = simulationTime - _startTime;
            var progressRatio = Math.Min(1, elapsed.TotalSeconds / _duration.TotalSeconds);
            var totalArc = _toAngle - _fromAngle;
            var expectedPosition = (progressRatio * totalArc) + _fromAngle;
            
//            if (( isAngleIncreasing && hingeAngle >= expectedPosition) ||
//                (!isAngleIncreasing && hingeAngle <= expectedPosition)) {
//                // TODO this might not work with HingeController...
//                // for whatever reason, the hinge is ahead of where we expect it to be right now
//                // so just return null and wait until the next cycle
//                return;
//            }
            
            _hinge.MoveTo(expectedPosition);
//            return string.Format("({0} {1:0.########})", _effectorLabel, expectedPosition - hingeAngle);
        }
    }
    
    public sealed class TweenQueue : IBodyManipulator, IEnumerable<IBodyManipulator> {
        private readonly Queue<IBodyManipulator> _queue = new Queue<IBodyManipulator>();
        public bool IsFinished { get; private set; }

        public void Add(IBodyManipulator tween) {
            _queue.Enqueue(tween);
            IsFinished = false;
        }

        public void Step(TimeSpan simulationTime) {
            // TODO test this
            var sb = new System.Text.StringBuilder();

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
}