// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleApplicationHost.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using System;
    using System.Threading;

    /// <summary>
    /// Application host for console applications.
    /// This host allows to quit it through pressing "q" in the console.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application to be run in this host.
    /// </typeparam>
    public partial class ConsoleApplicationHost<T>
        where T : IRunnableApplication, new()
    {
        /// <summary>
        /// Runs this application.
        /// </summary>
        /// <param name="name">
        /// The name of the application.
        /// </param>
        public override void Run(string name)
        {
            var consoleReader = new Thread(this.ReadConsole) { IsBackground = true };
            consoleReader.Start();

            base.Run(name);
        }

        private void ReadConsole()
        {
            string line;
            do
            {
                Console.WriteLine("Press q to exit or r to relaunch");
                line = Console.ReadLine();
            }
            while (line != null && line != "q" && line != "r");

            if (line == "q")
            {
                this.Application.Exit("User pressed q to exit");
            }

            if (line == "r")
            {
                this.Application.Relaunch("User pressed r to relaunch");
            }
        }
    }
}
