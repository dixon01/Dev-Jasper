// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleApplicationHost.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using System;
    using System.Threading;
    using Gorba.Common.Utility.OSWrappers.Console;

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
            try
            {
                using (var ctrlHandler = new ConsoleCtrlHandler())
                {
                    ctrlHandler.ConsoleClosed += this.CtrlHandlerOnConsoleClosed;

                    var consoleReader = new Thread(this.ReadConsole) { IsBackground = true };
                    consoleReader.Start();

                    base.Run(name);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception Starting Application {0}", ex.Message);
                if (System.Environment.UserInteractive)
                {
                    Console.WriteLine(ex.Message);
                }

                throw;
            }
        }

        private void CtrlHandlerOnConsoleClosed(object sender, ConsoleCtrlEventArgs e)
        {
            switch (e.CtrlType)
            {
                case CtrlTypes.CtrlCEvent:
                    this.Application.Exit("Ctrl-C pressed");
                    break;
                case CtrlTypes.CtrlCloseEvent:
                    this.Application.Exit("Console window closed");
                    break;
                case CtrlTypes.CtrlLogoffEvent:
                    this.Application.Exit("Console logoff");
                    break;
                case CtrlTypes.CtrlShutdownEvent:
                    this.Application.Exit("Console shutdown");
                    break;
            }
        }

        private void ReadConsole()
        {
            string line;
            do
            {
                Console.WriteLine("Press q to exit or r to relaunch");
                line = Console.ReadLine();
            }
            while (line != "q" && line != "r");

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
