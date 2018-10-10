// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HtmlRendererTest
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using NLog;

    /// <summary>
    /// Main program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (s, e) =>
                LogManager.GetCurrentClassLogger().FatalException("Unhandled exception", (Exception)e.ExceptionObject);
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            var server = new Server();
            server.Start();

            Console.WriteLine("Press q to quit.");
            while (Console.ReadLine() != "q")
            {
            }
        }
    }
}
