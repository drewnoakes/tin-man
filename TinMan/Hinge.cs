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
// Created 16/05/2010 15:46

using System;

namespace TinMan
{
    /// <summary>
    /// Represents a single hinge joint within an agent's body.
    /// </summary>
    public sealed class Hinge
    {
        // TODO we could have properties for SpeedLastCycle / MoveLastCycle -- these should be set to 0 or NaN if we're beamed though

        #region Properties

        /// <summary>Gets the minimum angle at which this hinge may be positioned.</summary>
        public Angle MinAngle { get; private set; }

        /// <summary>Gets the maximum angle at which this hinge may be positioned.</summary>
        public Angle MaxAngle { get; private set; }

        /// <summary>Gets the label for the perceptor of this hinge.</summary>
        public string PerceptorLabel { get; private set; }

        /// <summary>Gets the label for the effector of this hinge.</summary>
        public string EffectorLabel { get; private set; }

        /// <summary>
        /// Gets the current anglular position of this hinge.  This value is updated for each
        /// simulation cycle before any agent code is executed.
        /// </summary>
        public Angle Angle { get; internal set; }

        internal bool IsDesiredSpeedChanged { get; private set; }

        private AngularSpeed _desiredSpeed = AngularSpeed.NaN;

        /// <summary>
        /// Gets and sets the desired speed of this joint.  Setting this value during the processing
        /// of <see cref="IAgent.Think"/> results in the necessary command being sent to the
        /// simulation server.
        /// </summary>
        public AngularSpeed DesiredSpeed
        {
            get { return _desiredSpeed; }
            set
            {
                _controlFunction = null;
                SetDesiredSpeedInternal(value);
            }
        }

        #endregion

        /// <summary>
        /// Creates a new hinge.  Note that most users will not need to create their own hinges, instead using
        /// one of the built-in <see cref="IBody"/> implementations such as <see cref="NaoBody"/> that come with
        /// prepopulated hinges that match the corresponding simulator models.
        /// </summary>
        /// <remarks>
        /// SimSpark models use different labels for the perceptor and effector of each hinge.
        /// </remarks>
        /// <param name="perceptorLabel">The string label used for the hinge's perceptor.</param>
        /// <param name="effectorLabel">The string label used for the hinge's effector.</param>
        /// <param name="minAngle">The minimum angle that this hinge may reach.</param>
        /// <param name="maxAngle">The maximum angle that this hinge may reach.</param>
        /// <exception cref="ArgumentNullException"><paramref name="perceptorLabel"/> or
        /// <paramref name="effectorLabel"/> are <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="maxAngle"/> is less than <paramref name="minAngle"/>.</exception>
        public Hinge(string perceptorLabel, string effectorLabel, Angle minAngle, Angle maxAngle)
        {
            if (perceptorLabel == null)
                throw new ArgumentNullException("perceptorLabel");
            if (effectorLabel == null)
                throw new ArgumentNullException("effectorLabel");
            if (maxAngle < minAngle)
                throw new ArgumentException("maxAngle cannot be less than minAngle.");
            PerceptorLabel = perceptorLabel;
            EffectorLabel = effectorLabel;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }

        internal HingeSpeedCommand GetCommand()
        {
            if (!IsDesiredSpeedChanged)
                throw new InvalidOperationException("The speed value for this hinge was not changed.  Check IsSpeedChanged before calling this method.");
            IsDesiredSpeedChanged = false;
            return new HingeSpeedCommand(this, DesiredSpeed);
        }

        /// <summary>
        /// Sets the desired speed without clearing any control function.  Setting <see cref="DesiredSpeed"/>
        /// directly would clear any control function.
        /// </summary>
        /// <param name="desiredSpeed"></param>
        private void SetDesiredSpeedInternal(AngularSpeed desiredSpeed)
        {
            if (_desiredSpeed == desiredSpeed)
                return;
            _desiredSpeed = desiredSpeed;
            IsDesiredSpeedChanged = true;
        }

        #region Control function

        private Func<Hinge, ISimulationContext, PerceptorState, AngularSpeed> _controlFunction;

        /// <summary>
        /// Sets a function that controls the speed of the hinge.  This function is called
        /// every cycle of the simulation.  Only one function may be applied to a given joint
        /// at a time, and setting a control function when one already exists will replace it.
        /// </summary>
        /// <remarks>
        /// For more information on hinge control functions, see the project wiki.
        /// </remarks>
        /// <param name="controlFunction">The control function to apply to this hinge.</param>
        public void SetControlFunction(Func<Hinge, ISimulationContext, PerceptorState, AngularSpeed> controlFunction)
        {
            _controlFunction = controlFunction;
        }

        /// <summary>
        /// Removes any control function from this hinge.
        /// </summary>
        /// <remarks>
        /// Note that this method does not set <see cref="DesiredSpeed"/> to zero, so any prior speed
        /// value will remain set on the hinge. If you want to stop all motion, you must also modify
        /// that property.
        /// <para/>
        /// For more information on hinge control functions, see the project wiki.
        /// </remarks>
        public void ClearControlFunction()
        {
            _controlFunction = null;
        }

        /// <summary>
        /// Gets a value indicating whether this hinge has a control function applied.
        /// </summary>
        public bool HasControlFunction
        {
            get { return _controlFunction != null; }
        }

        internal void ComputeControlFunction(ISimulationContext context, PerceptorState perceptorState)
        {
            // Copy the reference for multi-threading safety
            Func<Hinge, ISimulationContext, PerceptorState, AngularSpeed> fun = _controlFunction;
            if (fun != null)
                SetDesiredSpeedInternal(fun(this, context, perceptorState));
        }

        #endregion

        #region Angular limits

        /// <summary>
        /// Throws an exception if <paramref name="angle"/> is outside the the range of
        /// <see cref="MinAngle"/> and <see cref="MaxAngle"/>.
        /// </summary>
        /// <param name="angle">The angle to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="angle"/> is outside the range of
        /// <see cref="MinAngle"/> and <see cref="MaxAngle"/>.</exception>
        public void ValidateAngle(Angle angle)
        {
            if (angle.IsNaN)
                throw new ArgumentOutOfRangeException("angle", "NaN is invalid as an angle.");
            if (!IsAngleValid(angle))
                throw new ArgumentOutOfRangeException("angle", string.Format("{0} is not a valid angle for hinge {1}.  The range is between {2} and {3}.", angle, EffectorLabel, MinAngle, MaxAngle));
        }

        /// <summary>
        /// Gets a value indicating whether <paramref name="angle"/> is within the range of
        /// <see cref="MinAngle"/> and <see cref="MaxAngle"/>.
        /// </summary>
        /// <param name="angle">The angle to validate.</param>
        /// <returns><c>true</c> if <paramref name="angle"/> is within the range of
        /// <see cref="MinAngle"/> and <see cref="MaxAngle"/>, otherwise <c>false</c>.</returns>
        public bool IsAngleValid(Angle angle)
        {
            return angle <= MaxAngle && angle >= MinAngle;
        }

        /// <summary>
        /// Returns the closest value to <paramref name="angle"/> that is within the range of
        /// <see cref="MinAngle"/> and <see cref="MaxAngle"/>.
        /// </summary>
        /// <param name="angle">The angle to limit.</param>
        /// <returns>The limited angle.</returns>
        public Angle LimitAngle(Angle angle)
        {
            return angle.Limit(MinAngle, MaxAngle);
        }

        #endregion
    }
}