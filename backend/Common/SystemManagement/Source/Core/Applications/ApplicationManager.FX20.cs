// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The application manager that takes care of all <see cref="ApplicationControllerBase"/>s.
    /// </summary>
    public partial class ApplicationManager
    {
        partial void KillProcesses(List<ProcessConfig> toKill)
        {
            var processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                if (
                    toKill.Find(
                        c => c.ExecutablePath.IndexOf(
                            proc.ProcessName, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    == null)
                {
                    // filtering because proc.MainModule below is a pretty slow operation that often
                    // throws an exception
                    continue;
                }

                string processFileName;
                try
                {
                    processFileName = proc.MainModule.FileName;
                }
                catch (Exception)
                {
                    // ignore this exception, try the next process
                    continue;
                }

                if (toKill.Find(
                    c => c.ExecutablePath.Equals(processFileName, StringComparison.InvariantCultureIgnoreCase))
                        == null)
                {
                    continue;
                }

                Logger.Info("Killing already existing process {0} ({1})", proc.ProcessName, proc.Id);
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't kill " + proc.ProcessName, ex);
                }
            }
        }
    }
}