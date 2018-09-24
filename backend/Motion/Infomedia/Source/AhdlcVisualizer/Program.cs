// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// The program.
    /// </summary>
    public static partial class Program
    {
        private static void Run()
        {
            new ApplicationHost<AhdlcVisualizerApplication>().Run("AhdlcVisualizer");
        }
    }
}
