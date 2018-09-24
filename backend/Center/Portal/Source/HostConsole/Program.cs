// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.HostConsole
{
    using System;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host;
    using Gorba.Center.Portal.Host.Api;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Utility.Compatibility;

    using NLog;

    /// <summary>
    /// Entry class for the application.
    /// </summary>
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Entry method for the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Logger.Info("Center Portal version {0}", ApplicationHelper.GetApplicationFileVersion());

            try
            {
                BackgroundSystemConfigurationProvider.Set(new PortalBackgroundSystemConfigurationProvider());
                var settings = PortalSettingsProvider.Current.GetSettings();
                using (GetPortalHost(settings.HttpPort))
                {
                    using (GetPortalHost(settings.HttpsPort, settings.EnableHttps, true))
                    {
                        var exit = false;
                        while (!exit)
                        {
                            Console.WriteLine("Type <ctrl + q> to exit");
                            var key = Console.ReadKey(true);
                            exit = key.Key == ConsoleKey.Q && key.Modifiers == ConsoleModifiers.Control;
                        }

                        Logger.Info("Web server is being stopped");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception, "Can't start the portal because of an unhandled exception");
            }
        }

        private static IDisposable GetPortalHost(int port, bool start = true, bool isHttps = false)
        {
            if (!start)
            {
                return new FakeHost();
            }

            var protocol = isHttps ? "https" : "http";
            var uri = string.Format("{0}://*:{1}", protocol, port);
            return PortalHost.Create(uri);
        }

        private class FakeHost : IDisposable
        {
            public void Dispose()
            {
                // Nothing to do here
            }
        }
    }
}