// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessFinder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessFinder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Utility
{
    using System;
    using System.Diagnostics;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Helper class to get the parent process of the current process.
    /// </summary>
    internal static class ProcessFinder
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(ProcessFinder));

        /// <summary>
        /// Gets the name for a given <see cref="Process"/>.
        /// </summary>
        /// <param name="process">
        /// The process.
        /// </param>
        /// <returns>
        /// The name of the process.
        /// </returns>
        public static string GetName(Process process)
        {
            return process.ProcessName;
        }

        /// <summary>
        /// Searches for a process with the given path.
        /// </summary>
        /// <param name="path">
        /// The path of the main executable.
        /// </param>
        /// <returns>
        /// The <see cref="Process"/> if found, otherwise null.
        /// </returns>
        public static Process FindProcess(string path)
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileName.Equals(
                        path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return process;
                    }
                }
                catch (Exception ex)
                {
                    // ignore this process
                    Logger.Trace(ex, "FindProcess Couldn't get file name of process");
                }
            }

            return null;
        }
    }
}