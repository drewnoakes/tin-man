using System;
using System.Collections.Generic;
using System.Net.Sockets;

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

    public sealed class RoboVizRemote : IRoboVizRemote,  IDisposable
    {
        private readonly List<ShapeSet> _sets = new List<ShapeSet>();
        private readonly ISimulationContext _simulationContext;
        private readonly UdpClient _udpClient;
        private readonly bool _useDefaultPrefix;
        private string _path;

        internal RoboVizRemote(RoboVizOptions options, ISimulationContext simulationContext)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (simulationContext == null)
                throw new ArgumentNullException("simulationContext");

            _simulationContext = simulationContext;
            _useDefaultPrefix = options.UseDefaultPrefix;

            _udpClient = new UdpClient();
            _udpClient.Connect(options.HostName, options.Port);
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

        internal void FlushMessages()
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

        public void Dispose()
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