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
// Created 06/05/2010 14:07

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using TinMan.PerceptorParsing;

namespace TinMan
{
    /// <summary>
    /// Hosts an implementation of <see cref="IAgent"/>, handling all communication with the server and
    /// managing the lifecycle of the TinMan framework with respect to the simulation.
    /// </summary>
    public sealed class AgentHost
    {
        /// <summary>
        /// The default TCP port number upon upon the <c>rcssserver3d</c> process accepts agent
        /// connections.  If the server is hosted on a different port, use the <see cref="PortNumber"/>
        /// property to specify the necessary port number.
        /// </summary>
        public const int DefaultTcpPort = 3100;

        /// <summary>
        /// The default network host name upon which the <c>rcssserver3d</c> process accepts is running.
        /// By default this is <c>localhost</c>.  If the server is hosted on a remote machine, use the
        /// <see cref="HostName"/> property to specify the necessary host name.
        /// </summary>
        public const string DefaultHostName = "localhost";

        /// <summary>
        /// The period of time between simulation steps.  Agents are given the chance to think
        /// and update their effectors in steps of this period.
        /// </summary>
        public const double CyclePeriodSeconds = 0.02;

        /// <summary>
        /// The period of time between simulation steps.  Agents are given the chance to think
        /// and update their effectors in steps of this period.
        /// </summary>
        public static readonly TimeSpan CyclePeriod = TimeSpan.FromSeconds(CyclePeriodSeconds);

        private static readonly Log _log = Log.Create();
        private readonly SimulationContext _context;
        private bool _stopRequested;
        private string _teamName;
        private string _hostName;
        private int _portNumber;
        private int _desiredUniformNumber;
        private bool _hasRun;

        /// <summary>
        /// Creates a new client.  <see cref="HostName"/> is set to <tt>localhost</tt> and
        /// <see cref="TeamName"/> to <tt>TinManBots</tt>.  Change these explicitly after
        /// construction, but before calling <see cref="Run"/>.
        /// </summary>
        public AgentHost()
        {
            HostName = DefaultHostName;
            PortNumber = DefaultTcpPort;
            TeamName = "TinManBots";

            _context = new SimulationContext(this);
            Context = _context;
        }

        #region Properties

        /// <summary>Gets and sets the name of the team to which this agent belongs.</summary>
        /// <exception cref="InvalidOperationException"><see cref="Run"/> has already been called on this agent host.</exception>
        /// <exception cref="ArgumentNullException"><param name="value"/> is null.</exception>
        public string TeamName
        {
            get { return _teamName; }
            set
            {
                if (_hasRun)
                    throw new InvalidOperationException("TeamName cannot be set after AgentHost.Run has been called.");
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!Regex.IsMatch(value, "^[A-Za-z0-9_-]+$"))
                    throw new ArgumentException("Team name must contain only alpha-numeric characters.", "value");
                _teamName = value;
            }
        }

        /// <summary>
        /// Gets and sets the desired uniform number for this player.  A value of zero tells the server to
        /// assign the next available number automatically.  This is the default.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Run"/> has already been called on this agent host.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><param name="value"/> is negative.</exception>
        public int DesiredUniformNumber
        {
            get { return _desiredUniformNumber; }
            set
            {
                if (_hasRun)
                    throw new InvalidOperationException("DesiredUniformNumber cannot be set after AgentHost.Run has been called.");
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "The desired uniform number must be zero (no preference) or a positive integer.");
                 _desiredUniformNumber = value;
            }
        }

        /// <summary>
        /// The name of the host running the server to connect to.  By default this is <tt>localhost</tt>.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Run"/> has already been called on this agent host.</exception>
        /// <exception cref="ArgumentNullException"><param name="value"/> is null.</exception>
        public string HostName
        {
            get { return _hostName; }
            set 
            {
                if (_hasRun)
                    throw new InvalidOperationException("HostName cannot be set after AgentHost.Run has been called.");
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value.Trim().Length==0)
                    throw new ArgumentException("HostName cannot be blank.", "value");
                _hostName = value;
            }
        }

        /// <summary>
        /// The TCP port number that the server is listening on.  By default this is 3100.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Run"/> has already been called on this agent host.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><param name="value"/> is zero or less.</exception>
        public int PortNumber
        {
            get { return _portNumber; }
            set 
            {
                if (_hasRun)
                    throw new InvalidOperationException("PortNumber cannot be set after AgentHost.Run has been called.");
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", value, "PortNumber must be greater than zero.");
                _portNumber = value;
            }
        }

        /// <summary>
        /// Gets the simulation context used by this host.  The context provides appropriately
        /// scoped access to resources provided by the TinMan framework for use by an agent.
        /// </summary>
        public ISimulationContext Context { get; private set; }

        #endregion

        /// <summary>
        /// Connects to the RoboCup3D server and runs a simulation with <paramref name="agent"/>.
        /// This call blocks until either <see cref="Stop"/> is called, or <see cref="IAgent.IsAlive"/>
        /// becomes false (usually because an agent calls <see cref="AgentBase{TBody}.StopSimulation"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <exception cref="ArgumentNullException"><paramref name="agent"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Run"/> has already been called on this agent host.</exception>
        public void Run(IAgent agent)
        {
            if (agent == null)
                throw new ArgumentNullException("agent");
            if (_hasRun)
                throw new InvalidOperationException("Run can only be called once, and has already been called.");

            _log.Info("Connecting via TCP to {0}:{1}", HostName, PortNumber);

            // Try to make a TCP connection.
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

            // We delay setting this flag until now so that network errors during socket connection may
            // be retried using the same AgentHost.
            _hasRun = true;
            
            _log.Info("Initialising agent");

            agent.Context = Context;
            agent.OnInitialise();
            
            using (client)
            using (var stream = client.GetStream())
            {
                _log.Info("Sending initialisation messages");

                // Initialise with server.  We must first send the scene command to specify which robot we'll be using.
                // NOTE We read between sends at startup, even though no reponse will be received.  If we don't then we appear in middle, white.
                // TODO maybe just a pause is enough (rather than a read)
                // TODO at startup, don't log warnings about listen timeouts until after the first message has been received from the server
                SendCommands(stream, new[] { new SceneSpecificationCommand(agent.Body.RsgPath) });
                NetworkUtil.ReadResponseString(stream, TimeSpan.FromSeconds(0.5));
                
                // Specify which player on which team.
                SendCommands(stream, new[] { new InitialisePlayerCommand(DesiredUniformNumber, TeamName) });
                NetworkUtil.ReadResponseString(stream, TimeSpan.FromSeconds(0.5));

                var commands = new List<IEffectorCommand>();
                while (!_stopRequested && agent.IsAlive)
                {
                    // It seems like a good idea to pass the stream to Coco/R rather than loading the whole
                    // string into memory first, however because Coco/R requires the ability to seek within
                    // the stream, it would internally load the stream into a buffer anyway.  To avoid this
                    // memory churn, Coco/R should be replaced.
                    var data = NetworkUtil.ReadResponseString(stream, TimeSpan.FromSeconds(0.1));

                    // If we don't receive anything, loop around and wait until a message arrives
                    if (data == null)
                        continue;

                    // Parse message
                    var parser = new Parser(new Scanner(new StringBuffer(data))) { TeamName = TeamName };
                    parser.Parse();
                    var perceptorState = parser.State;
                    var errors = parser.errors;

                    if (errors.HasError)
                        _log.Error("PARSE ERROR: {0}\nDATA: {1}", errors.ErrorMessages, data);

                    // Update the body's hinges with current angular positions
                    foreach (var hinge in agent.Body.AllHinges)
                    {
                        Angle angle;
                        if (perceptorState.TryGetHingeAngle(hinge, out angle))
                            hinge.Angle = angle;
                    }

                    // Certain values are only seen once (at startup) so we copy them from the perceptor state to the context and make the permanently available there
                    if (perceptorState.TeamSide != FieldSide.Unknown)
                        _context.TeamSide = perceptorState.TeamSide;
                    if (perceptorState.PlayMode != PlayMode.Unknown && perceptorState.PlayMode != _context.PlayMode)
                        _context.PlayMode = perceptorState.PlayMode;
                    if (perceptorState.UniformNumber.HasValue)
                    {
                        Debug.Assert(perceptorState.UniformNumber > 0);
                        _context.UniformNumber = perceptorState.UniformNumber;
                    }

                    // Let the agent perform its magic
                    agent.Think(perceptorState);

                    // Visit all hinges again to compute any control functions
                    foreach (var hinge in agent.Body.AllHinges)
                        hinge.ComputeControlFunction(Context, perceptorState);

                    // Collate list of commands to send
                    _context.FlushCommands(commands);
                    commands.AddRange(agent.Body.AllHinges.Where(hinge => hinge.IsDesiredSpeedChanged).Select(hinge => hinge.GetCommand()).Cast<IEffectorCommand>());

                    // Append a (syn) message to the outbound string to support agent sync mode
                    commands.Add(new SynchroniseCommand());
                    
                    // Send messages and clear out list for reuse next cycle
                    SendCommands(stream, commands);
                    commands.Clear();
                }

                agent.OnShuttingDown();
            }
        }

        /// <summary>
        /// Instructs the host to stop running.  After calling this method, the <see cref="Run"/> method will return.
        /// </summary>
        public void Stop()
        {
            _stopRequested = true;
        }

        private static void SendCommands(NetworkStream stream, IEnumerable<IEffectorCommand> commands)
        {
            string commandStr = ConcatCommandStrings(commands);
            NetworkUtil.WriteStringWith32BitLengthPrefix(stream, commandStr);
        }

        private static string ConcatCommandStrings(IEnumerable<IEffectorCommand> commands)
        {
            var sb = new StringBuilder();
            foreach (var command in commands)
                command.AppendSExpression(sb);
            return sb.ToString();
        }
    }
}