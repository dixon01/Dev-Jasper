// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.TestApps.ConsoleTest
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main function.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Command line arguments:");
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            Console.WriteLine("Configuring medi");
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            var exitWait = new ManualResetEvent(false);

            Console.WriteLine("Creating registration");
            var registration = SystemManagerClient.Instance.CreateRegistration("Console Test");
            registration.Registered += (s, e) => Console.WriteLine("Registered with System Manager");
            registration.WatchdogKicked += (s, e) => Console.WriteLine("Watchdog kicked");
            registration.ExitRequested += (s, e) =>
                {
                    registration.SetExiting();
                    exitWait.Set();
                };

            Console.WriteLine("Registering");
            registration.Register();
            registration.SetRunning();

            Console.WriteLine("Console Test started, press <enter> to quit");
            var readLine = new Thread(
                () =>
                    {
                        Console.ReadLine();
                        exitWait.Set();
                    });
            readLine.IsBackground = true;
            readLine.Start();

            exitWait.WaitOne();

            registration.Deregister();
        }
    }
}
