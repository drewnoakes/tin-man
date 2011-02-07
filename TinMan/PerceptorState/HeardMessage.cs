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
// Created 07/05/2010 03:15

using System;

namespace TinMan
{
    /// <summary>
    /// Represents a heard message by an agent on the field.  Models both the contents of the message
    /// along with the time it was heard and from what direction.  Note that an agent may here
    /// their own message, so check <see cref="IsFromSelf"/>.
    /// </summary>
    public sealed class HeardMessage
    {
        /// <summary>
        /// Gets a value indicating whether the agent heard their own message.
        /// </summary>
        public bool IsFromSelf
        {
            get { return RelativeDirection.IsNaN; }
        }

        /// <summary>
        /// The time at which the message was heard.  This value is relative to <see cref="PerceptorState.GameTime"/>,
        /// <see cref="PerceptorState.SimulationTime"/>.
        /// </summary>
        public TimeSpan HeardAtTime { get; private set; }

        /// <summary>
        /// Gets the relative direction from which this message was heard.  Note that this direction is only in one axis.
        /// TODO determine and document whether this direction is relative to the agent's head, body or field
        /// </summary>
        public Angle RelativeDirection { get; private set; }

        /// <summary>
        /// Gets the message text.  See <see cref="Message"/> for more information about messages and their text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Initialises a heard message.
        /// </summary>
        /// <remarks>
        /// Most users will not need to use this constructor as this type is only for inbound messages.
        /// To send a message, use <see cref="ISimulationContext.Say"/>.
        /// This constructor is public to allow for unit testing.
        /// </remarks>
        public HeardMessage(TimeSpan time, Angle direction, Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            HeardAtTime = time;
            RelativeDirection = direction;
            Text = message.Text;
        }

        public override string ToString()
        {
            return string.Format("Message \"{0}\" at {1} from {2}",
                                 Text, HeardAtTime,
                                 IsFromSelf ? "self" : RelativeDirection.ToString());
        }
    }
}