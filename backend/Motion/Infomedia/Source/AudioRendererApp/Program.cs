// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Main Program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRendererApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Infomedia.AudioRenderer;

    /// <summary>
    /// Main Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            new ConsoleApplicationHost<AudioRendererApplication>().Run("AudioRenderer");
        }
    }
}
