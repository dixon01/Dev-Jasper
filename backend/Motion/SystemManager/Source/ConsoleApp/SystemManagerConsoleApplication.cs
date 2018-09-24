// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerConsoleApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerConsoleApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ConsoleApp
{
    using System;

    using CommandLineParser;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Motion.SystemManager.Core;

    /// <summary>
    /// The system manager console application.
    /// </summary>
    internal class SystemManagerConsoleApplication : IRunnableApplication
    {
        private readonly SystemManagerApplication application;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagerConsoleApplication"/> class.
        /// </summary>
        public SystemManagerConsoleApplication()
        {
            this.application = new SystemManagerApplication();
        }

        /// <summary>
        /// Configures this application with the given name.
        /// The application should use the given name to register itself with the System Manager Client.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Configure(string name)
        {
        }

        /// <summary>
        /// Start is not supported.
        /// </summary>
        public void Start()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Stops this application.
        /// </summary>
        public void Stop()
        {
            this.Exit("Stop called");
        }

        /// <summary>
        /// Runs this application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public void Run(string[] args)
        {
            var options = new SystemManagerOptions();
            var parser = new CommandLineParser();
            parser.ExtractArgumentAttributes(options);

            try
            {
                parser.ParseCommandLine(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                parser.ShowUsage();
                return;
            }

            if (options.ShowUsage)
            {
                parser.ShowUsage();
                return;
            }

            if (!this.application.ShouldStart(options))
            {
                return;
            }

            this.application.Configure(options);

            this.application.Run();

            this.application.Dispose();
        }

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Relaunch(string reason)
        {
            this.application.Controller.RequestReboot(reason);
        }

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Exit(string reason)
        {
            this.application.Controller.Stop(true, ApplicationReason.ApplicationExit, reason);
        }
    }
}