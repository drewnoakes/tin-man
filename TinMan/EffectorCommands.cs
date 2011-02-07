#region License

/* 
 * This file is part of TinMan.
 *
 * TinMan is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TinMan is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with TinMan.  If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 04/06/2010 02:58

using System;
using System.Text;

namespace TinMan
{
    /// <summary>
    /// Defines the interface that all effector commands sent to the server from an agent implement.
    /// </summary>
    public interface IEffectorCommand
    {
        /// <summary>
        /// Appends the an SExpression that represents this effector command to <paramref name="s" />.
        /// </summary>
        /// <param name="s">The string builder in which output messages are accumulating, and to which
        /// this effector command's SExpression must be appended.</param>
        void AppendSExpression(StringBuilder s);
    }

    /// <summary>
    /// Commands a HingeJointEffector to set the speed of rotation to a new value.
    /// </summary>
    /// <remarks>
    /// Format:  ({name} {ax})<br/>
    /// Example: (lae3 5.3)
    /// </remarks>
    internal sealed class HingeSpeedCommand : IEffectorCommand
    {
        private readonly Hinge _hinge;
        private readonly AngularSpeed _angularSpeed;

        public HingeSpeedCommand(Hinge hinge, AngularSpeed angularSpeed)
        {
            _hinge = hinge;
            _angularSpeed = angularSpeed;
        }

        public void AppendSExpression(StringBuilder s)
        {
            // Note that the simulator expects the argument for the HingeJointEffector to
            // be in degrees per simulation cycle.
            Angle anglePerCycle = _angularSpeed*AgentHost.CyclePeriod;
            s.AppendFormat("({0} {1:0.######})", _hinge.EffectorLabel, anglePerCycle.Degrees);
        }
    }

/*
    /// <summary>
    /// Commands a UniversalJointEffector to set the speeds of rotation to a new value.
    /// </summary>
    /// <remarks>
    /// Format:  ({name} {ax1} {ax2})<br/>
    /// Message: (lae1 -2.3 1.2)
    /// </remarks>
    internal sealed class UniversalJointSpeedCommand : IEffectorCommand {
        private readonly UniversalJoint _universalJoint;
        private readonly AngularSpeed _angularSpeed1;
        private readonly AngularSpeed _angularSpeed2;
        public UniversalJointSpeedCommand(UniversalJoint universalJoint, AngularSpeed angularSpeed1, AngularSpeed angularSpeed2) {
            _universalJoint = universalJoint;
            _angularSpeed1 = angularSpeed1;
            _angularSpeed2 = angularSpeed2;
        }
        
        public void AppendSExpression(StringBuilder s) {
            // Note that the simulator expects the argument for the HingeJointEffector to
            // be in degrees per simulation cycle.
            var anglePerCycle1 = _angularSpeed1 * AgentHost.CyclePeriod;
            var anglePerCycle2 = _angularSpeed2 * AgentHost.CyclePeriod;
            s.AppendFormat("({0} {1:0.######} {2:0.######})", _universalJoint.EffectorLabel, anglePerCycle1.Degrees, anglePerCycle2.Degrees);
        }
    }
*/

    /// <summary>
    /// Allows a player to position itself on the field before the game starts.
    /// </summary>
    /// <remarks>
    /// Format:  (beam {x} {y} {rot})<br/>
    /// Example: (beam 10.0 -10.0 0.0)
    /// </remarks>
    internal sealed class BeamCommand : IEffectorCommand
    {
        private readonly double _x;
        private readonly double _y;
        private readonly Angle _rotation;

        public BeamCommand(double x, double y, Angle rotation)
        {
            _x = x;
            _y = y;
            _rotation = rotation;
        }

        public void AppendSExpression(StringBuilder s)
        {
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
    internal sealed class SceneSpecificationCommand : IEffectorCommand
    {
        private readonly string _rsgPath;

        public SceneSpecificationCommand(string rsgPath)
        {
            _rsgPath = rsgPath;
        }

        public void AppendSExpression(StringBuilder s)
        {
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
    internal sealed class InitialisePlayerCommand : IEffectorCommand
    {
        private readonly int _uniformNumber;
        private readonly string _teamName;

        /// <summary>
        /// </summary>
        /// <remarks>
        /// If an agent sends 0 as uniformNumber, the number is assigned automatically by the server to the next free number.
        /// </remarks>
        /// <param name="uniformNumber"></param>
        /// <param name="teamName"></param>
        public InitialisePlayerCommand(int uniformNumber, string teamName)
        {
            _uniformNumber = uniformNumber;
            _teamName = teamName;
        }

        public void AppendSExpression(StringBuilder s)
        {
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
    internal sealed class SayCommand : IEffectorCommand
    {
        private readonly Message _message;

        public SayCommand(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            _message = message;
        }

        public void AppendSExpression(StringBuilder s)
        {
            s.AppendFormat("(say {0})", _message.Text);
        }
    }

    /// <summary>
    /// Represents a validated message according to the rules specified by the server on what strings
    /// are allowed to be sent between agents on the field.
    /// </summary>
    /// <remarks>
    /// This type is immutable.
    /// </remarks>
    public sealed class Message
    {
        // TODO represent the validation via a regular expression and allow it to be specified in config so that a recompile isn't necessary in case it changes
        /// <summary>
        /// Message may consist of 20 characters, which may be taken from the ASCII printing character
        /// subset [0x20; 0x7E] except the white space character ' ' and the normal brackets '(' and ')'.
        /// </summary>
        public static bool IsValid(string messageString)
        {
            if (messageString == null)
                return false;
            if (messageString.Length == 0 || messageString.Length > 20)
                return false;
            if (messageString.IndexOfAny(new[] { ' ', '(', ')' }) != -1)
                return false;
            foreach (var c in messageString)
            {
                if (c < 0x20 || c > 0x7E)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="Message" /> with the value of <paramref name="messageString" />.
        /// </summary>
        /// <param name="messageString">The string with which to initialise this <see cref="Message" />.</param>
        /// <exception cref="ArgumentException">If <paramref name="messageString" /> is an invalid string
        /// according to <see cref="IsValid"/>.</exception>
        public Message(string messageString)
        {
            if (!IsValid(messageString))
                throw new ArgumentException("Invalid string.", "messageString");
            Text = messageString;
        }

        /// <summary>
        /// Gets the string representation of this message.
        /// </summary>
        public string Text { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}