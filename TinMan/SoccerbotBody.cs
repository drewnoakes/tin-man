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
// Created 15/06/2010 03:32

using System;
using System.Collections.Generic;
using System.Linq;

namespace TinMan
{
    /// <summary>
    /// Models the body of the Soccerbot robot.
    /// </summary>
    /// <remarks>
    /// Note that the Soccerbot model has been replaced by <see cref="NaoBody"/> for RoboCup
    /// competitions.
    /// </remarks>
    public sealed class SoccerbotBody : IBody
    {
        /// <summary>Well-known path of the Ruby Scene Graph (RSG) file for the NAO model in the RCSS3D server package.</summary>
        public string RsgPath
        {
            get { return "rsg/agent/soccerbot058/soccerbot.rsg"; }
        }

        #region Hinges
        
        public Hinge HJ1 { get; private set; }
        public Hinge HJ2 { get; private set; }

        public Hinge LAJ1 { get; private set; }
        public Hinge LAJ2 { get; private set; }
        public Hinge LAJ3 { get; private set; }
        public Hinge LAJ4 { get; private set; }

        public Hinge RAJ1 { get; private set; }
        public Hinge RAJ2 { get; private set; }
        public Hinge RAJ3 { get; private set; }
        public Hinge RAJ4 { get; private set; }

        public Hinge LLJ1 { get; private set; }
        public Hinge LLJ2 { get; private set; }
        public Hinge LLJ3 { get; private set; }
        public Hinge LLJ4 { get; private set; }
        public Hinge LLJ5 { get; private set; }
        public Hinge LLJ6 { get; private set; }

        public Hinge RLJ1 { get; private set; }
        public Hinge RLJ2 { get; private set; }
        public Hinge RLJ3 { get; private set; }
        public Hinge RLJ4 { get; private set; }
        public Hinge RLJ5 { get; private set; }
        public Hinge RLJ6 { get; private set; }

        #endregion
        
        public IEnumerable<Hinge> AllHinges { get; private set; }

        public SoccerbotBody()
        {
            HJ1 = new Hinge("hj1", "he1", Angle.FromDegrees(-120), Angle.FromDegrees(120));
            HJ2 = new Hinge("hj2", "he2", Angle.FromDegrees(-45),  Angle.FromDegrees(45));

            LAJ1 = new Hinge("laj1", "lae1", Angle.FromDegrees(-90),  Angle.FromDegrees(180));
            LAJ2 = new Hinge("laj2", "lae2", Angle.FromDegrees(-10),  Angle.FromDegrees(180));
            LAJ3 = new Hinge("laj3", "lae3", Angle.FromDegrees(-135), Angle.FromDegrees(135));
            LAJ4 = new Hinge("laj4", "lae4", Angle.FromDegrees(-10),  Angle.FromDegrees(130));

            RAJ1 = new Hinge("raj1", "rae1", Angle.FromDegrees(-90),  Angle.FromDegrees(180));
            RAJ2 = new Hinge("raj2", "rae2", Angle.FromDegrees(-180), Angle.FromDegrees(10));
            RAJ3 = new Hinge("raj3", "rae3", Angle.FromDegrees(-135), Angle.FromDegrees(135));
            RAJ4 = new Hinge("raj4", "rae4", Angle.FromDegrees(-10),  Angle.FromDegrees(130));

            LLJ1 = new Hinge("llj1", "lle1", Angle.FromDegrees(-60),  Angle.FromDegrees(90));
            LLJ2 = new Hinge("llj2", "lle2", Angle.FromDegrees(-45),  Angle.FromDegrees(120));
            LLJ3 = new Hinge("llj3", "lle3", Angle.FromDegrees(-45),  Angle.FromDegrees(75));
            LLJ4 = new Hinge("llj4", "lle4", Angle.FromDegrees(-160), Angle.FromDegrees(10));
            LLJ5 = new Hinge("llj5", "lle5", Angle.FromDegrees(-90),  Angle.FromDegrees(905));
            LLJ6 = new Hinge("llj6", "lle6", Angle.FromDegrees(-45),  Angle.FromDegrees(45));

            RLJ1 = new Hinge("rlj1", "rle1", Angle.FromDegrees(-90),  Angle.FromDegrees(60));
            RLJ2 = new Hinge("rlj2", "rle2", Angle.FromDegrees(-45),  Angle.FromDegrees(120));
            RLJ3 = new Hinge("rlj3", "rle3", Angle.FromDegrees(-75),  Angle.FromDegrees(45));
            RLJ4 = new Hinge("rlj4", "rle4", Angle.FromDegrees(-160), Angle.FromDegrees(10));
            RLJ5 = new Hinge("rlj5", "rle5", Angle.FromDegrees(-90),  Angle.FromDegrees(90));
            RLJ6 = new Hinge("rlj6", "rle6", Angle.FromDegrees(-45),  Angle.FromDegrees(45));

            AllHinges = new[] {
                HJ1, HJ2,
                RAJ1, RAJ2, RAJ3, RAJ4,
                LAJ1, LAJ2, LAJ3, LAJ4,
                RLJ1, RLJ2, RLJ3, RLJ4, RLJ5, RLJ6,
                LLJ1, LLJ2, LLJ3, LLJ4, LLJ5, LLJ6
            };
        }

        public Hinge GetHingeForEffectorLabel(string effectorLabel)
        {
            return AllHinges.SingleOrDefault(
                h => string.Equals(h.EffectorLabel, effectorLabel, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Converts observation polar coordinates from camera space to a vector in torso space.</summary>
        public Vector3 ConvertCameraPolarToLocalVector(Polar cameraView)
        {
            // Soccerbot's camera is in his torso, so there's no need to compensate for
            // the head's positioning as we do with Nao.
            return cameraView.ToVector3();
        }
    }
}
