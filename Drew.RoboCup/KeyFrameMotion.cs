/*
 * Created by Drew, 17/05/2010 11:15.
 */
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;

namespace Drew.RoboCup
{
/*
    public sealed class KeyFrameSequence : IBodyManipulator {
        private readonly List<KeyFrame> _frames = new List<KeyFrame>();
        private TimeSpan _currentFrameUntilTime;
        private int _currentFrameIndex;
        private KeyFrame _currentFrame;
        
        public bool Repeat { get; set; }
        public string Name { get; set; }
        public bool IsFinished { get; private set; }
        
        public KeyFrameSequence() {
            Repeat = false;
            Reset();
        }
        
        public KeyFrame AddFrame(TimeSpan duration) {
            var frame = new KeyFrame(duration);
            _frames.Add(frame);
            return frame;
        }
        
        public void Step(TimeSpan simulationTime) {
            if (_currentFrameUntilTime==TimeSpan.Zero) {
                // load the first frame
                _currentFrame = NextFrame();
                if (_currentFrame==null)
                    return;
                _currentFrameUntilTime = simulationTime + _currentFrame.Duration;
                _currentFrame.Play();
            }
            
            // the current frame is still active
            if (simulationTime < _currentFrameUntilTime)
                return;
            
            // NOTE I originally had some code here that would skip a frame if the simulation time passed beyond
            // it, but this may through movements out and it might also skip the last frame (although that could be
            // fixed with the right algorithm)
            _currentFrame = NextFrame();
            if (_currentFrame==null) {
                IsFinished = true;
            } else {
                _currentFrameUntilTime += _currentFrame.Duration;
                _currentFrame.Play();
            }
        }
        
        public void Reset() {
            IsFinished = false;
            _currentFrameUntilTime = TimeSpan.Zero;
            _currentFrameIndex = -1;
        }
        
        private KeyFrame NextFrame() {
            if (_frames.Count==0)
                return null;
            _currentFrameIndex++;
            if (_frames.Count<=_currentFrameIndex) {
                if (!Repeat)
                    return null;
                _currentFrameIndex = 0;
            }
            return _frames[_currentFrameIndex];
        }
    }
    
    public sealed class KeyFrame {
        private readonly List<HingeMovement> _moves = new List<HingeMovement>();
        public TimeSpan Duration { get; private set; }
        public KeyFrame(TimeSpan duration) {
            Duration = duration;
        }        
        public void Move(HingeController hinge, double angle, TimeSpan duration) {
            hinge.ValidateAngle(angle);
            _moves.Add(new HingeMovement { Hinge = hinge, TargetAngle = angle, Duration = duration });
        }
        internal void Play() {
            foreach (var move in _moves)
                move.Hinge.MoveTo(move.TargetAngle, move.Duration);
        }
        private struct HingeMovement {
            public HingeController Hinge { get; set; }
            public double TargetAngle { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
*/    
    public sealed class KeyFrameMotionSequenceParser {
        /*
         * <sequence name="Crouch">
         * 	<frame durationSeconds="1">
         * 	   <hinge label="raj1" angle="12.3" />
         * 	</frame>
         * </sequence>
         */
        public List<TweenQueue> Parse(XmlNode doc, IBody body) {
            var sequences = new List<TweenQueue>();
            foreach (XmlNode sequenceNode in doc.SelectNodes(@"sequence")) {
//                var sequence = new KeyFrameSequence();
                var sequence = new TweenQueue();
                sequences.Add(sequence);
                var sequenceName = sequenceNode.GetAttributeValue("name");
                sequence.Name = sequenceName;
                foreach (XmlNode frameNode in sequenceNode.SelectNodes("frame")) {
                    var frameDuration = TimeSpan.FromSeconds(double.Parse(frameNode.GetAttributeValue("durationSeconds")));
//                    var frame = sequence.AddFrame(frameDuration);
                    var frame = new TweenGroup();
                    sequence.Add(frame);
                    frame.Add(new WaitMotion(frameDuration));
                    foreach (XmlNode hingeNode in frameNode.SelectNodes("hinge")) {
                        var label = hingeNode.GetAttributeValue("label");
                        var angle = double.Parse(hingeNode.GetAttributeValue("angle"));
                        var hinge = body.GetHingeControllerForLabel(label);
                        if (hinge==null) {
                            Console.WriteLine("{0}: No hinge with label '{1}' was found.  Skipping.", sequenceName, label);
                            continue;
                        }
                        if (!hinge.IsAngleValid(angle)) {
                            Console.WriteLine("{0}: Hinge '{1}' cannot be set to {2}.  Min={3}, Max={4}.", sequenceName, label, angle, hinge.MinAngle, hinge.MaxAngle);
                            continue;
                        }
                        var durationMillis = int.Parse(hingeNode.GetAttributeValueOrDefault("durationMillis", "1000"));
                        string easeTypeName = hingeNode.GetAttributeValueOrDefault("easeType", null);
                        EaseType easeType = EaseType.CubicEaseInOut;
                        if (easeTypeName!=null) {
                            var matches = Enum.GetValues(typeof(EaseType)).Cast<EaseType>().Where(s => string.Equals(s.ToString(), easeTypeName, StringComparison.OrdinalIgnoreCase));
                            if (matches.Any())
                                easeType = matches.Single();
                            else
                                Console.WriteLine("{0}: Unrecognised 'easeType' value: {1}", sequenceName, easeTypeName);
                        }
                        frame.Add(new HingeMotionTween(hinge, angle, TimeSpan.FromMilliseconds(durationMillis), easeType));
                    }
                }
            }
            return sequences;
        }
    }
    

}
