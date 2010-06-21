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
// Created 21/06/2010 15:58

using System;
using System.Net.Sockets;

namespace TinMan
{
    /// <summary>
    /// Facilitates various superpower actions within the simulated universe such as killing agents,
    /// throwing the ball around or changing the game play mode.
    /// </summary>
    /// The tools provided by this class are not available for use in competitive matches.  Instead they
    /// are inteded for use as part of machine learning or supervisory roles.
    /// <remarks>
    public sealed class Wizard {
        /// <summary>The default port exposed by the server for monitors, trainers or referees.</summary>
        public const int DefaultTcpPort = 3200;
        private static readonly Log _log = Log.Create();

        // TODO what about the server command (getAck) ?
        
        private TcpClient _client;
        private NetworkStream _stream;
        
        /// <summary>Initialises a new wizard for localhost on the default port.</summary>
        public Wizard() {
            HostName = "localhost";
            PortNumber = DefaultTcpPort;
        }
        
        /// <summary>Gets the next message sent by the server.</summary>
        /// <param name="timeout">The timeout period, if after which elapses the method will return null.</param>
        /// <returns>The message, or <c>null</c> if the timeout period elapses.</returns>
        public string ReadMessage(TimeSpan timeout) {
            return NetworkUtil.ReadResponseString(_stream, timeout);
        }
        
        #region Connection management

        /// <summary>
        /// Creates a TCP collection to the server at <see cref="HostName"/> on port <see cref="PortNumber"/>.
        /// This method must be called before any actions are taken.
        /// </summary>
        public void Connect() {
            if (_client!=null)
                throw new InvalidOperationException("Already connected.");
            
            _log.Info("Connecting via TCP to {0}:{1}", HostName, PortNumber);
            
            try {
                _client = new TcpClient(HostName, PortNumber);
            } catch (SocketException ex) {
                _log.Error(ex, "Unable to connect to {0}:{1}", HostName, PortNumber);
                throw;
            }
            
            _log.Info("Connected.");
            
            _stream = _client.GetStream();
        }
        
        /// <summary>Disconenects from the server.</summary>
        public void Disconnect() {
            if (_client==null)
                throw new InvalidOperationException("Not connected.");
            _stream.Dispose();
            _client.Close();
            _stream = null;
            _client = null;
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// The name of the host running the server to connect to.  By default this is <tt>localhost</tt>.
        /// </summary>
        public string HostName { get; set; }
        
        /// <summary>
        /// The TCP port number that the server is listening on.  By default this is 3200.
        /// </summary>
        public int PortNumber { get; set; }

        #endregion
        
        #region Private utility methods

        private static string GetSideString(FieldSide teamSide) {
            // TODO is it ever valid to accept 'Unknown'?  I doubt it.
            switch (teamSide) {
                case FieldSide.Left: return "Left";
                case FieldSide.Right: return "Right";
                case FieldSide.Unknown: return "None";
                default: throw new ArgumentException("Unexpected value for FieldSide enum: " + teamSide);
            }
        }
        
        private static string GetVectorString(Vector3 vector) {
            return vector.X + " " + vector.Y + " " + vector.Z;
        }
        
        #endregion
        
        #region Commands
        
        /// <summary>Moves the specified agent to a given field position.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        /// <param name="newPosition"></param>
        public void SetAgentPosition(int uniformNumber, FieldSide teamSide, Vector3 newPosition) {
            SendCommand("(agent (unum {0}) (team {1}) (pos {2}))", uniformNumber, GetSideString(teamSide), GetVectorString(newPosition));
        }
        
        /// <summary>Moves the specified agent to a given field position, facing in the given direction.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        /// <param name="newPosition"></param>
        /// <param name="newDirection"></param>
        public void SetAgentPositionAndDirection(int uniformNumber, FieldSide teamSide, Vector3 newPosition, Angle newDirection) {
            // TODO are 'degrees' correct?  is this absolute?
            SendCommand("(agent (unum {0}) (team {1}) (move {2} {3}))", 
                        uniformNumber, 
                        GetSideString(teamSide), 
                        GetVectorString(newPosition),
                        newDirection.Degrees);
        }
        
        /// <summary>Overrides the battery level for the specified agent.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        /// <param name="batteryLevel"></param>
        public void SetBatteryLevel(int uniformNumber, FieldSide teamSide, double batteryLevel) {
            // TODO what valid range can the battery level have?
            SendCommand("(agent (unum {0}) (team {1}) (battery {2}))", uniformNumber, GetSideString(teamSide), batteryLevel);
        }
        
        /// <summary>Overrides the temperature for the specified agent.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        /// <param name="temperature"></param>
        public void SetTemperature(int uniformNumber, FieldSide teamSide, double temperature) {
            // TODO what are valid temperature ranges?
            SendCommand("(agent (unum {0}) (team {1}) (temperature {2}))", uniformNumber, GetSideString(teamSide), temperature);
        }
        
        /// <summary>Repositions the ball at the specified position.</summary>
        /// <param name="newPosition"></param>
        public void SetBallPosition(Vector3 newPosition) {
            SendCommand("(ball (pos {0}))", GetVectorString(newPosition));
        }

        /// <summary>Repositions the ball and provides it with a particular velocity.</summary>
        /// <param name="newPosition"></param>
        /// <param name="newVelocity"></param>
        public void SetBallPositionAndVelocity(Vector3 newPosition, Vector3 newVelocity) {
            SendCommand("(ball (pos {0}) (vel {1}))", GetVectorString(newPosition), GetVectorString(newVelocity));
        }
        
        /// <summary>Sets the velocity of the ball, without altering its position.</summary>
        /// <param name="newVelocity"></param>
        public void SetBallVelocity(Vector3 newVelocity) {
            SendCommand("(ball (vel {0}))", GetVectorString(newVelocity));
        }
        
        /// <summary>Sets the play mode of the game in progress.</summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(PlayMode playMode) {
            SendCommand("(playMode {0})", playMode.GetServerString());
        }
        
        /// <summary>
        /// Performs a drop ball in accordance with the server's rules.  This command can be used
        /// when the ball leaves the field, for example.
        /// </summary>
        public void DropBall() {
            SendCommand("(dropBall)");
        }
        
        /// <summary>Kicks off the game for either side.</summary>
        /// <param name="team"></param>
        public void KickOff(FieldSide team) {
            // TODO is this any different to calling SetPlayMode with the corresponding enum values?
            SendCommand("(kickOff {0})", GetSideString(team));
        }
        
        /// <summary>
        /// Selects the specified agent.  Only some of the wizard's operations apply to the selected agent.
        /// </summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        public void SelectAgent(int uniformNumber, FieldSide teamSide) {
            SendCommand("(select (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }
        
        /// <summary>Removes the specified agent from the simulation.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        public void KillAgent(int uniformNumber, FieldSide teamSide) {
            SendCommand("(agent (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }
        
        /// <summary>Removes the agent previously selected via <see cref="SelectAgent"/> from the simulation.</summary>
        public void KillSelectedAgent() {
            SendCommand("(kill)");
        }
        
        /// <summary>Repositions the specified agent according to the server's rules.</summary>
        /// <param name="uniformNumber"></param>
        /// <param name="teamSide"></param>
        public void RepositionAgent(int uniformNumber, FieldSide teamSide) {
            SendCommand("(repos (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }
        
        /// <summary>Repositions the agent previously selected via <see cref="SelectAgent"/> according
        /// to the server's rules.</summary>
        public void RepositionSelectedAgent() {
            SendCommand("(repos)");
        }
        
        private void SendCommand(string format, params object[] items) {
            string str = string.Format(format, items);
            NetworkUtil.WriteStringWith32BitLengthPrefix(_stream, str);
        }
        
        #endregion
    }
}
