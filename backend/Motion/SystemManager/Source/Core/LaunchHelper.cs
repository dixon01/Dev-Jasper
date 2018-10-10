// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LaunchHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LaunchHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using CommandLineParser;

    /// <summary>
    /// Helper class that can be used before actually launching System Manager.
    /// IMPORTANT: don't use NLog or Medi in this class!!!
    /// </summary>
    public static class LaunchHelper
    {
        /// <summary>
        /// Prepares the launch of the System Manager.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <typeparam name="T">
        /// The type of the options object to be used when parsing command line arguments.
        /// </typeparam>
        public static void PrepareLaunch<T>(string[] args)
            where T : SystemManagerOptions, new()
        {
            var options = new T();
            var parser = new CommandLineParser();
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);

            if (options.WaitForExit != 0)
            {
                WaitForExit(options.WaitForExit);
            }
        }

        private static void WaitForExit(int processId)
        {
            Console.WriteLine("Waiting for process {0} to exit", processId);
            try
            {
                var process = Process.GetProcessById(processId);
                for (var i = 0; !process.HasExited; i++)
                {
                    if (i > 0)
                    {
                        Console.WriteLine("Still waiting ({0} seconds)", i);
                    }

                    Thread.Sleep(1000);
                }

                Console.WriteLine("Process {0} has exited, continuing start-up", processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't find process to wait for, continuing start-up");
                Console.WriteLine(ex);
            }
        }
    }
}
