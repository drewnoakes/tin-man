#region License

// This file is part of TinMan.
// 
// TinMan is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// TinMan is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with TinMan.  If not, see <http://www.gnu.org/licenses/>.

#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 15/06/2010 23:27

using System;
using TinMan;

namespace TinManSamples.CSharp
{
    /// <summary>
    /// An agent that sends and receives messages.  Messages are sent randomly.
    /// </summary>
    public sealed class SocialAgent : AgentBase<NaoBody>
    {
        public SocialAgent()
            : base(new NaoBody())
        {}

        private readonly Log _log = Log.Create();
        private readonly Random _random = new Random();

        public override void Think(PerceptorState state)
        {
            // Check if we heard anything and log
            if (state.HeardMessages != null)
            {
                foreach (var message in state.HeardMessages)
                    _log.Verbose("Heard message: " + message);
            }

            // Send a message 
            if (_random.NextDouble() < 0.05)
                Context.Say("Message" + _random.Next(1000));
        }
    }
}