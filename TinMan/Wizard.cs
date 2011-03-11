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
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Net.Sockets;

namespace TinMan
{
    /// <summary>
    /// Facilitates various superpower actions within the simulated universe such as killing agents,
    /// throwing the ball around or changing the game play mode.
    /// </summary>
    /// <remarks>
    /// The tools provided by this class are not available for use in competitive matches.  Instead they
    /// are inteded for use as part of machine learning or supervisory roles.
    /// </remarks>
    public sealed class Wizard
    {
        /// <summary>The default port exposed by the server for monitors, trainers or referees.</summary>
        public const int DefaultTcpPort = 3200;

        /// <summary>The default host name, <c>localhost</c>.</summary>
        private const string DefaultHostName = "localhost";

        private static readonly Log _log = Log.Create();

        /// <summary>
        /// Fires whenever the ball position is updated.  The accompanying time is the game time, and will
        /// be <see cref="TimeSpan.Zero"/> the first time this event fires (as the data is unavailable on
        /// the first read).
        /// </summary>
        public event Action<TimeSpan, TransformationMatrix> BallTransformUpdated;

        /// <summary>
        /// Fires whenever an agent's position is updated.  The accompanying time is the game time, and will
        /// be <see cref="TimeSpan.Zero"/> the first time this event fires (as the data is unavailable on
        /// the first read).
        /// </summary>
        public event Action<TimeSpan, TransformationMatrix> AgentTransformUpdated;

        private NetworkStream _stream;
        private bool _isRunning;

        /// <summary>Initialises a new wizard for localhost on the default port.</summary>
        public Wizard()
        {
            HostName = DefaultHostName;
            PortNumber = DefaultTcpPort;
        }

        /// <summary>
        /// Opens a TCP connection to the server at <see cref="HostName"/> on port <see cref="PortNumber"/>
        /// and begins processing server messages.
        /// <para/>
        /// This method must be called before any actions are taken.
        /// </summary>
        public void Run()
        {
            _log.Info("Connecting via TCP to {0}:{1}", HostName, PortNumber);

            TcpClient client;
            try
            {
                client = new TcpClient(HostName, PortNumber);
            }
            catch (SocketException ex)
            {
                _log.Error(ex, "Unable to connect to {0}:{1}", HostName, PortNumber);
                throw;
            }

            _log.Info("Connected.");
            _isRunning = true;

            using (client)
            using (_stream = client.GetStream())
            {
                while (_isRunning)
                {
                    int length = NetworkUtil.ReadInt32(_stream);
                    var sexp = new SExpressionReader(_stream, length);

                    var ballEvent = BallTransformUpdated;
                    if (ballEvent != null)
                    {
                        var gameTime = TimeSpan.Zero;
                        // Parse game time
                        if (sexp.In(2))
                        {
                            if (sexp.Take() == "time")
                            {
                                double secs;
                                string timeVal = sexp.Take();
                                if (double.TryParse(timeVal, out secs))
                                    gameTime = TimeSpan.FromSeconds(secs);
                            }
                            sexp.Out(2);

                            // Parse ball location
                            if (sexp.Skip(1) && sexp.In(1) && sexp.Skip(14) && sexp.In(1) && sexp.Skip(1) && sexp.In(1) && sexp.Skip(1))
                            {
                                TransformationMatrix transform;
                                if (TryReadTransformationMatrix(sexp, out transform))
                                    ballEvent(gameTime, transform);

                                // Parse agent location
                                var agentEvent = AgentTransformUpdated;
                                if (agentEvent != null && sexp.Out(2))
                                {
                                    // Loop through all agents
                                    bool done = false;
                                    while (!done)
                                    {
                                        if (sexp.In(3))
                                        {
                                            if (sexp.Take() == "SLT" && TryReadTransformationMatrix(sexp, out transform))
                                                agentEvent(gameTime, transform);
                                            if (!sexp.Out(3))
                                                done = true;
                                        }
                                        else
                                        {
                                            done = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    sexp.SkipToEnd();
                }
            }
        }

        private static bool TryReadTransformationMatrix(SExpressionReader sexp, out TransformationMatrix transform)
        {
            var values = new double[16];
            for (int i = 0; i < 16; i++)
            {
                string s = sexp.Take();
                if (s == null)
                {
                    transform = null;
                    return false;
                }
                double d;
                if (!double.TryParse(s, out d))
                {
                    transform = null;
                    return false;
                }
                values[i] = d;
            }
            transform = new TransformationMatrix(values);
            return true;
        }

        /// <summary>
        /// Causes the wizard to disconnect from the server and the <see cref="Run"/> method to exit.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

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

        private static string GetSideString(FieldSide teamSide)
        {
            // TODO is it ever valid to accept 'Unknown'?  I doubt it.
            switch (teamSide)
            {
                case FieldSide.Left:
                    return "Left";
                case FieldSide.Right:
                    return "Right";
                case FieldSide.Unknown:
                    return "None";
                default:
                    throw new ArgumentException("Unexpected value for FieldSide enum: " + teamSide);
            }
        }

        private static string GetVectorString(Vector3 vector)
        {
            return vector.X + " " + vector.Y + " " + vector.Z;
        }

        #endregion

        #region Commands

        /// <summary>Moves the specified agent to a given field position.</summary>
        /// <remarks>Note that most agent coordinate frames have their origins located within their torsos.  Therefore,
        /// the vector specified by <paramref name="newPosition"/> must contain a positive offset in the z-axis, otherwise
        /// the agent will be moved so that its feet are beneath the field.
        /// <para/>
        /// For <see cref="NaoBody" />, a sensible z-offset is 0.45.
        /// </remarks>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        /// <param name="newPosition">The position in field coordinates to which the origin of the agent's frame will be moved.
        /// Note that the agent may be positioned above or below the field surface, so be careful to use a sensible Z value.</param>
        public void SetAgentPosition(int uniformNumber, FieldSide teamSide, Vector3 newPosition)
        {
            SendCommand("(agent (unum {0}) (team {1}) (pos {2}))", uniformNumber, GetSideString(teamSide), GetVectorString(newPosition));
        }

        /// <summary>Moves the specified agent to a given field position, facing in the given direction.</summary>
        /// <remarks>Note that most agent coordinate frames have their origins located within their torsos.  Therefore,
        /// the vector specified by <paramref name="newPosition"/> must contain a positive offset in the z-axis, otherwise
        /// the agent will be moved so that its feet are beneath the field.
        /// <para/>
        /// For <see cref="NaoBody" />, a sensible z-offset is 0.45.
        /// </remarks>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        /// <param name="newPosition">The position in field coordinates to which the origin of the agent's frame will be moved.
        /// Note that the agent may be positioned above or below the field surface, so be careful to use a sensible Z value.</param>
        /// <param name="newDirection"></param>
        public void SetAgentPositionAndDirection(int uniformNumber, FieldSide teamSide, Vector3 newPosition, Angle newDirection)
        {
            // TODO are 'degrees' correct?  is this absolute?
            SendCommand("(agent (unum {0}) (team {1}) (move {2} {3}))",
                        uniformNumber,
                        GetSideString(teamSide),
                        GetVectorString(newPosition),
                        newDirection.Degrees);
        }

        /// <summary>Overrides the battery level for the specified agent.</summary>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        /// <param name="batteryLevel"></param>
        public void SetBatteryLevel(int uniformNumber, FieldSide teamSide, double batteryLevel)
        {
            // TODO what valid range can the battery level have?
            SendCommand("(agent (unum {0}) (team {1}) (battery {2}))", uniformNumber, GetSideString(teamSide), batteryLevel);
        }

        /// <summary>Overrides the temperature for the specified agent.</summary>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        /// <param name="temperature"></param>
        public void SetTemperature(int uniformNumber, FieldSide teamSide, double temperature)
        {
            // TODO what are valid temperature ranges?
            SendCommand("(agent (unum {0}) (team {1}) (temperature {2}))", uniformNumber, GetSideString(teamSide), temperature);
        }

        /// <summary>Repositions the ball at the specified position.
        /// Set <paramref name="newPosition"/>'s <see cref="Vector3.Z"/> value to
        /// <see cref="FieldGeometry.BallRadiusMetres"/> to position the ball on the ground.</summary>
        /// <param name="newPosition"></param>
        public void SetBallPosition(Vector3 newPosition)
        {
            SendCommand("(ball (pos {0}))", GetVectorString(newPosition));
        }

        /// <summary>Repositions the ball and provides it with a particular velocity.
        /// Set <paramref name="newPosition"/>'s <see cref="Vector3.Z"/> value to
        /// <see cref="FieldGeometry.BallRadiusMetres"/> to position the ball on the ground.</summary>
        /// <param name="newPosition"></param>
        /// <param name="newVelocity"></param>
        public void SetBallPositionAndVelocity(Vector3 newPosition, Vector3 newVelocity)
        {
            SendCommand("(ball (pos {0}) (vel {1}))", GetVectorString(newPosition), GetVectorString(newVelocity));
        }

        /// <summary>Sets the velocity of the ball, without altering its position.</summary>
        /// <param name="newVelocity"></param>
        public void SetBallVelocity(Vector3 newVelocity)
        {
            SendCommand("(ball (vel {0}))", GetVectorString(newVelocity));
        }

        /// <summary>Sets the play mode of the game in progress.</summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(PlayMode playMode)
        {
            SendCommand("(playMode {0})", playMode.GetServerString());
        }

        /// <summary>
        /// Drops the ball at its current position and move all players away by the free kick radius.
        /// If the ball is off the field, it is brought back within bounds.
        /// </summary>
        public void DropBall()
        {
            SendCommand("(dropBall)");
        }

        /// <summary>Kicks off the game for either side.</summary>
        /// <param name="team"></param>
        public void KickOff(FieldSide team)
        {
            // TODO is this any different to calling SetPlayMode with the corresponding enum values?
            SendCommand("(kickOff {0})", GetSideString(team));
        }

        /// <summary>
        /// Selects the specified agent.  Only some of the wizard's operations apply to the selected agent.
        /// </summary>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        public void SelectAgent(int uniformNumber, FieldSide teamSide)
        {
            SendCommand("(select (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }

        /// <summary>Removes the specified agent from the simulation.</summary>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        public void KillAgent(int uniformNumber, FieldSide teamSide)
        {
            SendCommand("(agent (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }

        /// <summary>Removes the agent previously selected via <see cref="SelectAgent"/> from the simulation.</summary>
        public void KillSelectedAgent()
        {
            SendCommand("(kill)");
        }

        /// <summary>Repositions the specified agent according to the server's rules.</summary>
        /// <param name="uniformNumber">The uniform number of the target agent.</param>
        /// <param name="teamSide">The side of the field which the target agent's team is defending.</param>
        public void RepositionAgent(int uniformNumber, FieldSide teamSide)
        {
            SendCommand("(repos (unum {0}) (team {1}))", uniformNumber, GetSideString(teamSide));
        }

        /// <summary>Repositions the agent previously selected via <see cref="SelectAgent"/> according
        /// to the server's rules.</summary>
        public void RepositionSelectedAgent()
        {
            SendCommand("(repos)");
        }

        private void SendCommand(string format, params object[] items)
        {
            NetworkUtil.WriteStringWith32BitLengthPrefix(_stream, string.Format(format, items));
        }

        #endregion
    }
}