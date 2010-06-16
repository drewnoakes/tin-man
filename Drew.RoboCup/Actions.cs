/*
 * Created by Drew, 04/06/2010 02:58.
 */
using System;
using System.Text;

namespace Drew.RoboCup
{
    // TODO implement SayAction and MoveUniversalJointAction
    
    public interface IAction {
        void AppendCommand(StringBuilder s);
    }
    
    public sealed class MoveHingeJointAction : IAction {
        private readonly string _effectorLabel;
        private readonly double _radiansPerSecond;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectorLabel">The label of the hinge joint to move.</param>
        /// <param name="radiansPerSecond">The angular velocity at which to move the hinge, in radians per second.</param>
        public MoveHingeJointAction(string effectorLabel, double radiansPerSecond) {
            _effectorLabel = effectorLabel;
            _radiansPerSecond = radiansPerSecond;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("({0} {1:0.######})", _effectorLabel, _radiansPerSecond);
        }
    }
    
    public sealed class BeamAction : IAction {
        private readonly Vector3 _beamToLocation;
        public BeamAction(Vector3 beamToLocation) {
            _beamToLocation = beamToLocation;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(beam {0:0.####} {1:0.####} {2:0.####})", _beamToLocation.X, _beamToLocation.Y, _beamToLocation.Z);
        }
    }
    
    public sealed class SceneSpecificationAction : IAction {
        private readonly string _rsgPath;
        public SceneSpecificationAction(string rsgPath) {
            _rsgPath = rsgPath;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(scene {0})", _rsgPath);
        }
    }
    
    public sealed class InitialisePlayerAction : IAction {
        private readonly int _uniformNumber;
        private readonly string _teamName;
        public InitialisePlayerAction(int uniformNumber, string teamName) {
            _uniformNumber = uniformNumber;
            _teamName = teamName;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(init (unum {0}) (teamname {1}))", _uniformNumber, _teamName);
        }
    }
    
    /// <summary>
    /// The say effector permits communication among agents by broadcasting messages.
    /// </summary>
    public sealed class SayAction : IAction {
        /// <summary>
        /// Message may consist of 20 characters, which may be taken from the ASCII printing character
        /// subset [0x20; 0x7E] except the white space character ' ' and the normal brackets '(' and ')'.
        /// </summary>
        public static bool IsValidMessage(string message) {
            if (message==null)
                return false;
            if (message.Length==0 || message.Length>20)
                return false;
            if (message.IndexOfAny(new[] {' ', '(', ')'})!=-1)
                return false;
            foreach (var c in message) {
                if (c < 0x20 || c > 0x7E)
                    return false;
            }
            return true;
        }
        private readonly string _messageToSay;
        public SayAction(string messageToSay) {
            if (!IsValidMessage(messageToSay))
                throw new ArgumentException("Message is invalid.", "messageToSay");
            _messageToSay = messageToSay;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(say {0})", _messageToSay);
        }
    }
}
