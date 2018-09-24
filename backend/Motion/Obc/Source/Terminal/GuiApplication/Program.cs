// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Main Program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.GuiApplication
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Obc.Terminal.Control;

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
            new ApplicationHost<TerminalControlApplication>().Run(TerminalControlApplication.ManagementName);
        }
    }
}
