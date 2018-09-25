// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHelper.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Utility
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using OpenNETCF.ToolHelp;

    /// <summary>
    /// Helper class to get the parent process of the current process.
    /// </summary>
    internal static partial class ProcessHelper
    {
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
            foreach (var entry in ProcessEntry.GetProcesses())
            {
                if (entry.ProcessID == process.Id)
                {
                    return Path.GetFileNameWithoutExtension(entry.ExeFile);
                }
            }

            return string.Format("#{0:X8}", process.Id);
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
            foreach (var process in ProcessEntry.GetProcesses())
            {
                try
                {
                    if (process.ExeFile.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Process.GetProcessById((int)process.ProcessID);
                    }
                }
                catch (Exception ex)
                {
                    // ignore this process
                    Logger.TraceException("Couldn't get file name of process", ex);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the parent process of the current process
        /// </summary>
        /// <returns>
        /// Always null since parent processes are not supported in Windows CE.
        /// </returns>
        public static Process GetParentProcess()
        {
            return null;
        }
    }
}