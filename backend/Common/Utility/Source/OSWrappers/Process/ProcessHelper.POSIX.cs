// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHelper.POSIX.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Process
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.Utility.Core;

    using Mono.Unix.Native;

    using NLog;

    /// <summary>
    /// Helper class to get the parent process of the current process.
    /// </summary>
    public static class ProcessHelper
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(ProcessHelper));

        /// <summary>
        /// Returns the parent process of the current process
        /// </summary>
        /// <returns>The parent process of current process (or null if not found).</returns>
        public static Process GetParentProcess()
        {
            int parentPid = Syscall.getppid();

            try
            {
                Logger.Debug("Getting parent process.");
                if (parentPid <= 0)
                {
                    return null;
                }

                return Process.GetProcessById(parentPid);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}