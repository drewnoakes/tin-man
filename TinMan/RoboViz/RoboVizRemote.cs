using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace TinMan.RoboViz
{
    public sealed class RoboVizOptions
    {
        /// <summary>The default UDP port exposed by the RoboViz monitor.</summary>
        public const int DefaultUdpPort = 32769;

        /// <summary>The default host name, <c>localhost</c>.</summary>
        private const string DefaultHostName = "localhost";

        // TODO validation in setters

        public bool UseDefaultPrefix { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }

        public RoboVizOptions()
        {
            UseDefaultPrefix = true;
            Port = DefaultUdpPort;
            HostName = DefaultHostName;
        }
    }

    public sealed class RoboVizRemote : IRoboVizRemote
    {
        private readonly List<ShapeSet> _sets = new List<ShapeSet>();
        private readonly ISimulationContext _simulationContext;
        private readonly UdpClient _udpClient;
        private readonly bool _useDefaultPrefix;
        private string _path;
        private bool _isAgentTextDirty;
        private string _agentText;
        private Color _agentTextColor;

        /// <summary>
        /// Initialises an instance of <see cref="RoboVizRemote"/> with default <see cref="RoboVizOptions"/>.
        /// This remote will be automatically disposed when the agent exits.
        /// </summary>
        /// <returns></returns>
        public RoboVizRemote(IAgent agent)
            : this(agent, new RoboVizOptions())
        {}

        /// <summary>
        /// Initialises an instance of <see cref="RoboVizRemote"/> with specified <paramref name="options"/>.
        /// This remote will be automatically disposed when the agent exits.
        /// </summary>
        /// <returns></returns>
        public RoboVizRemote(IAgent agent, RoboVizOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (agent == null)
                throw new ArgumentNullException("agent");

            _simulationContext = agent.Context;
            _useDefaultPrefix = options.UseDefaultPrefix;

            agent.ThinkCompleted += OnThinkCompleted;
            agent.ShuttingDown += OnShutDown;

            _udpClient = new UdpClient();
            _udpClient.Connect(options.HostName, options.Port);

            _isAgentTextDirty = false;
            _agentText = null;
            _agentTextColor = Color.LightSkyBlue;
        }

        public void Add(ShapeSet shapeSet)
        {
            if (shapeSet == null)
                throw new ArgumentNullException("shapeSet");
            _sets.Add(shapeSet);
            shapeSet.SetParent(this);
        }

        internal string Path
        {
            get 
            {
                if (_path == null)
                {
                    if (!_useDefaultPrefix)
                    {
                        _path = string.Empty;
                    }
                    else
                    {
                        if (!_simulationContext.UniformNumber.HasValue || _simulationContext.TeamSide==FieldSide.Unknown)
                            throw new InvalidOperationException("Cannot determine default prefix for RoboViz shape set path as the agent's uniform number and team side have not been reported yet.");
                        _path = _simulationContext.TeamSide.ToString()[0] + ".A" + _simulationContext.UniformNumber.Value;
                    }
                }
                return _path;
            }
        }

        public string AgentText
        {
            get { return _agentText; }
            set 
            {
                _agentText = value;
                _isAgentTextDirty = true;
            }
        }

        public Color AgentTextColor
        {
            get { return _agentTextColor; }
            set 
            {
                _agentTextColor = value;
                _isAgentTextDirty = true;
            }
        }

        private void OnThinkCompleted()
        {
            // Find shallowest nodes that are dirty
            var queue = new Queue<ShapeSet>(_sets);
            var dirtyNodes = new List<ShapeSet>();

            // TODO reduce number of buffer swaps -- if all sub-sets are dirty and the parent has no shapes, then the parent is as good as dirty

            while (queue.Count!=0)
            {
                var set = queue.Dequeue();
                if (set.IsDirty)
                {
                    dirtyNodes.Add(set);
                }
                else
                {
                    foreach (var subSet in set.SubSets)
                        queue.Enqueue(subSet);
                }
            }

            // For each of those, render them out and swap the buffers
            foreach (var set in dirtyNodes)
            {
                set.FlushMessages(_udpClient);

                SwapBuffer(set, _udpClient);
            }

            // If the agent text changed, send that message too
            if (_isAgentTextDirty)
            {
                if (_agentText==null || _agentText.Trim().Length==0)
                {
                    // clear text
                    var buf = new byte[] { 2, 2, GetAgentByte() };
                    _udpClient.Send(buf, buf.Length);
                }
                else
                {
                    // update text
                    var textBytes = Encoding.ASCII.GetBytes(_agentText);
                    var buf = new byte[7 + textBytes.Length];
                    buf[0] = 2;
                    buf[1] = 1;
                    buf[2] = GetAgentByte();
                    Shape.WriteColor(buf, 3, AgentTextColor, false);
                    textBytes.CopyTo(buf, 6);
                    _udpClient.Send(buf, buf.Length);
                }
                _isAgentTextDirty = false;
            }
        }

        private byte GetAgentByte()
        {
            if (_simulationContext.TeamSide == FieldSide.Unknown)
                throw new InvalidOperationException("Team side is unknown.");
            if (_simulationContext.UniformNumber == null)
                throw new InvalidOperationException("Uniform number is unknown.");
            if (_simulationContext.UniformNumber.Value < 1 || _simulationContext.UniformNumber.Value > 128)
                throw new InvalidOperationException("Uniform number is invalid.");

            checked
            {
                var val = _simulationContext.TeamSide == FieldSide.Left ? 0 : 128;
                return (byte)(val + _simulationContext.UniformNumber - 1);
            }
        }

        private static void SwapBuffer(ShapeSet set, UdpClient udpClient)
        {
            var pathBytes = set.PathBytes;
            var numBytes = 3 + pathBytes.Length;
            var buf = new byte[numBytes];
            pathBytes.CopyTo(buf, 2);
            udpClient.Send(buf, buf.Length);
        }

        #region Implementation of IDisposable

        private void OnShutDown()
        {
            // Send three zeroes to signify a complete buffer swap, effectively clearing any markers for the agent before shutdown
            _udpClient.Send(new byte[3], 3);
            _udpClient.Close();
        }

        #endregion
    }

    public interface IRoboVizRemote
    {
        void Add(ShapeSet shapeSet);
    }
}