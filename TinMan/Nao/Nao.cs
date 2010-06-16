/*
 * Created by Drew, 06/05/2010 15:08.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinMan
{
    public static class Nao {
        /// <summary>Approximate weight of the Nao robot is 4.5kg.</summary>
        public const double WeightKilograms = 4.5;
        /// <summary>Approximate height of the Nao robot is 57cm.</summary>
        public const double Height = 0.57;
        /// <summary>Well-known path of the Ruby Scene Graph (RSG) file for the NAO model in the RCSS3D server package.</summary>
        public const string RsgPath = "rsg/agent/nao/nao.rsg";
    }
}
