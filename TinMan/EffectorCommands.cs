/*
 * Created by Drew, 04/06/2010 02:58.
 */
using System;
using System.Text;

namespace TinMan
{
    // TODO implement MoveUniversalJointCommand, even though it's not used by Nao
    /// UniversalJoint Effector
    /// Format:  ({name} {ax1} {ax2})<br/>
    /// Message: (lae1 -2.3 1.2)

    /// <summary>
    /// Defines the interface that all effector commands sent to the server
    /// from an agent implement.
    /// </summary>
    public interface IEffectorCommand {
        void AppendSExpression(StringBuilder s);
    }
    
    /// <summary>
    /// Commands a HingeJointEffector to set the speed of rotation to a new value.
    /// </summary>
    /// <remarks>
    /// Format:  ({name} {ax})<br/>
    /// Example: (lae3 5.3)
    /// </remarks>
    internal sealed class HingeSpeedCommand : IEffectorCommand {
        private readonly Hinge _hinge;
        private readonly AngularSpeed _angularSpeed;
        public HingeSpeedCommand(Hinge hinge, AngularSpeed angularSpeed) {
            _hinge = hinge;
            _angularSpeed = angularSpeed;
        }
        
        public void AppendSExpression(StringBuilder s) {
            s.AppendFormat("({0} {1:0.######})", _hinge.EffectorLabel, _angularSpeed.DegreesPerSecond);
        }
    }
    
    /// <summary>
    /// Allows a player to position itself on the field before the game starts.
    /// </summary>
    /// <remarks>
    /// Format:  (beam {x} {y} {rot})<br/>
    /// Example: (beam 10.0 -10.0 0.0)
    /// </remarks>
    internal sealed class BeamCommand : IEffectorCommand {
        private readonly double _x;
        private readonly double _y;
        private readonly Angle _rotation;
        public BeamCommand(double x, double y, Angle rotation) {
            _x = x;
            _y = y;
            _rotation = rotation;
        }
        
        public void AppendSExpression(StringBuilder s) {
            s.AppendFormat("(beam {0:0.####} {1:0.####} {2:0.####})", _x, _y, _rotation.Degrees);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Format:  (scene {filename})<br/>
    /// Message: (scene rsg/agent/nao/nao.rsg)
    /// </remarks>
    internal sealed class SceneSpecificationCommand : IEffectorCommand {
        private readonly string _rsgPath;
        public SceneSpecificationCommand(string rsgPath) {
            _rsgPath = rsgPath;
        }
        
        public void AppendSExpression(StringBuilder s) {
            s.AppendFormat("(scene {0})", _rsgPath);
        }
    }
    
    /// <summary>
    /// This command must be sent once for player after <see cref="SceneSpecificationCommand"/>.
    /// It registers this agent as a member of the specified team with the specified uniform number.
    /// All players of a team must use the same team name and different numbers. Sending a uniform number
    /// of 0 assigns the next free number automatically.
    /// Note that the side on which a team starts to play depends on which team connected first.
    /// </summary>
    /// <remarks>
    /// Format:  (init (unum {playernumber})(teamname {yourteamname}))<br/>
    /// Example: (init (unum 1)(teamname FHO))
    /// </remarks>
    internal sealed class InitialisePlayerCommand : IEffectorCommand {
        private readonly int _uniformNumber;
        private readonly string _teamName;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// If an agent sends 0 as uniformNumber, the number is assigned automatically by the server to the next free number.
        /// </remarks>
        /// <param name="uniformNumber"></param>
        /// <param name="teamName"></param>
        public InitialisePlayerCommand(int uniformNumber, string teamName) {
            _uniformNumber = uniformNumber;
            _teamName = teamName;
        }
        
        public void AppendSExpression(StringBuilder s) {
            s.AppendFormat("(init (unum {0}) (teamname {1}))", _uniformNumber, _teamName);
        }
    }
    
    /// <summary>
    /// The say effector permits communication among agents by broadcasting messages.
    /// </summary>
    /// <remarks>
    /// Format:  (say {message})<br/>
    /// Example: (say ``helloworld'')
    /// </remarks>
    internal sealed class SayCommand : IEffectorCommand {
        /// <summary>
        /// Message may consist of 20 characters, which may be taken from the ASCII printing character
        /// subset [0x20; 0x7E] except the white space character ' ' and the normal brackets '(' and ')'.
        /// </summary>
        public static bool IsValidMessage(string message) {
            // TODO now that this type is internal, provide a public IsValidMessage method somewhere
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
        public SayCommand(string messageToSay) {
            if (!IsValidMessage(messageToSay))
                throw new ArgumentException("Message is invalid.", "messageToSay");
            _messageToSay = messageToSay;
        }
        public void AppendSExpression(StringBuilder s) {
            s.AppendFormat("(say {0})", _messageToSay);
        }
    }
}
