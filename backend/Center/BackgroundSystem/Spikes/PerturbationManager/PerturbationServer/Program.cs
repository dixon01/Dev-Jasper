using System;
using System.Collections.Generic;
using System.Linq;

namespace PerturbationServer
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Xml;

    using Core;

    using Microsoft.Owin;

    using NLog;
    using NLog.Config;

    using Owin;

    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Startup Startup = new Startup();

        private static readonly PerturbationManagerServer Server = new PerturbationManagerServer(Startup);

        private static Thread ServerThread;

        static void Main(string[] args)
        {
            SimpleConfigurator.ConfigureForConsoleLogging();
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Logger.FatalException(
                    "Unhandled Exception; terminating=" + e.IsTerminating, e.ExceptionObject as Exception);
                LogManager.Flush();
            };

            var version = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            Logger.Info("Starting Perturbation client console application {0}", version.FileVersion);

            try
            {
                Server.ServerStarted = () =>
                    {
                        Console.WriteLine("Perturbation Server started...");
                        Console.WriteLine("Press q to quit");
                    };
                Server.ServerStopped = () => Console.WriteLine("...Perturbation Server stopped");
                ServerThread = new Thread(Server.Start);
                ServerThread.Start();
                while (!string.Equals(Console.ReadLine(), "q", StringComparison.InvariantCultureIgnoreCase))
                {
                }

                Console.WriteLine("...stopping the Perturbation Server...");
                Server.Stop();
            }
            catch (Exception e)
            {
                Logger.FatalException("Error while running PerturbationClient; terminating=true", e);
                LogManager.Flush();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
