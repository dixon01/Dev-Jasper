// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ConsoleApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.SystemManager.Core;

    /// <summary>
    /// Main program of the system manager.
    /// </summary>
    public class Program
    {
#if __UseLuminatorTftDisplay
        private static ConsoleApplicationHost<SystemManagerConsoleApplication> application;
#endif

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            LaunchHelper.PrepareLaunch<SystemManagerOptions>(args);

            new ConsoleApplicationHost<SystemManagerConsoleApplication>(args).Run("SystemManager");
        }

#if __UseLuminatorTftDisplay

        /// <summary>
        /// This is invoked from the test application
        /// </summary>
        /// <param name="args"></param>
        public void Start(string[] args)
        {
            LaunchHelper.PrepareLaunch<SystemManagerOptions>(args);
            application = new ConsoleApplicationHost<SystemManagerConsoleApplication>(args);
            application.Run("SystemManager");
        }

        /// <summary>
        /// This is invoked from the test application
        /// </summary>
        public void Exit()
        {
            if (application == null) return;
            application.Application.Exit("Exit requested.");
        }
#endif
    }
}
