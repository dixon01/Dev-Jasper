// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests
{
    using System;

    using NLog;

    /// <summary>
    /// Integration tests main program
    /// </summary>
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Application main method.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    Logger.FatalException("Unhandled exception", e.ExceptionObject as Exception);
                    LogManager.Flush();
                };
            if (!new TestRunner(args).Run())
            {
                Environment.Exit(-1);
            }
        }
    }
}
