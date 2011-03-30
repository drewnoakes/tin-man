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
// Created 15/03/2011 09:18

using System;

namespace TinMan
{
    /// <summary>
    /// A PID (Proportional, Integral, Derivative) controller that operates upon a single <see cref="Hinge" />.
    /// This class may operate as a P,PI,PD,etc controller by setting one or more of <see cref="ProportionalGain" />,
    /// <see cref="IntegralGain" /> and <see cref="DerivativeGain" /> to zero.
    /// </summary>
    public sealed class PidHingeController
    {
        private Angle _targetAngle;
        private TimeSpan _lastTime = TimeSpan.Zero;

        /// <summary>Gets and sets the proportional gain constant, <em>Kp</em>.</summary>
        public double ProportionalGain { get; set; }
        /// <summary>Gets and sets the integral gain constant, <em>Ki</em>.</summary>
        public double IntegralGain { get; set; }
        /// <summary>Gets and sets the derivative gain constant, <em>Kd</em>.</summary>
        public double DerivativeGain { get; set; }

        /// <summary>Gets the <see cref="Hinge"/> for which this <see cref="PidHingeController"/> was instantiated.</summary>
        public Hinge Hinge { get; private set; }
        /// <summary>Gets the error (in radians) observed during the last calculation.  This value is used to calculate
        /// the short-term error, which in used as a predictor of future error, and will contribute to the output value
        /// if <see cref="DerivativeGain"/> is non-zero.</summary>
        public double PreviousError { get; private set; }
        /// <summary>Gets the error integral observed during the last calculation.  This value serves as a 'memory'
        /// of historic error, and will contribute to the output value if <see cref="IntegralGain"/> is non-zero.</summary>
        public double Integral { get; private set; }

        /// <summary>
        /// Instantiates a new <see cref="PidHingeController"/> for the specified <paramref name="hinge"/>.
        /// </summary>
        /// <param name="hinge">The hinge upon which this controller will operate.</param>
        /// <exception cref="NullReferenceException"><paramref name="hinge"/> is <c>null</c>.</exception>
        public PidHingeController(Hinge hinge)
        {
            if (hinge == null)
                throw new ArgumentNullException("hinge");
            Hinge = hinge;
            ProportionalGain = 20;
            IntegralGain = 0.1;
            DerivativeGain = 1000;
        }

        /// <summary>
        /// Gets and sets the target angle to which this <see cref="PidHingeController"/> will regulate
        /// <see cref="Hinge"/>.
        /// </summary>
        public Angle TargetAngle
        {
            get { return _targetAngle; }
            set
            {
                // TODO what if attempting to set angle outside of hinge's range?
                _targetAngle = value;
                Hinge.SetControlFunction(ControlFunction);
            }
        }

        private AngularSpeed ControlFunction(Hinge hinge, ISimulationContext context, PerceptorState perceptorState)
        {
            if (_lastTime == TimeSpan.Zero || _lastTime < perceptorState.SimulationTime-AgentHost.CyclePeriod)
            {
                PreviousError = 0;
                Integral = 0;
            }

            _lastTime = perceptorState.SimulationTime;

            const double dt = AgentHost.CyclePeriodSeconds;

            double error = _targetAngle.Radians - hinge.Angle.Radians;
            Integral += error*dt;
            double derivative = (error - PreviousError)*dt;

            PreviousError = error;

            return AngularSpeed.FromRadiansPerSecond(
                ProportionalGain * error +
                IntegralGain * Integral +
                DerivativeGain * derivative
                );
        }

        public override string ToString()
        {
            return string.Format("<PID hinge={0} Kp={1} Ki={2} Kd={3}>",
                Hinge.PerceptorLabel, ProportionalGain, IntegralGain, DerivativeGain);
        }
    }
}