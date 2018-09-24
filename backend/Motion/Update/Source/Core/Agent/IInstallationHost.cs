// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstallationHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInstallationHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// Host for an <see cref="IInstallationEngine"/>.
    /// This interface provides all necessary method for an engine to
    /// execute an update.
    /// </summary>
    public interface IInstallationHost
    {
        /// <summary>
        /// Gets the full path of this application.
        /// </summary>
        string ExecutablePath { get; }

        /// <summary>
        /// Restarts this application.
        /// </summary>
        /// <param name="reason">
        /// The reason for restarting the application.
        /// </param>
        void Relaunch(string reason);

        /// <summary>
        /// Exits this application.
        /// </summary>
        /// <param name="reason">
        /// The reason for exiting the application.
        /// </param>
        void Exit(string reason);

        /// <summary>
        /// Stops this application immediately (without <see cref="SystemManagerClient"/>).
        /// </summary>
        void ForceExit();

        /// <summary>
        /// Starts a new process.
        /// </summary>
        /// <param name="startInfo">
        /// The start info.
        /// </param>
        void StartProcess(ProcessStartInfo startInfo);

        /// <summary>
        /// Gets the list of running (update relevant) applications.
        /// </summary>
        /// <returns>
        /// The list of applications.
        /// </returns>
        IList<ApplicationInfo> GetRunningApplications();

        /// <summary>
        /// Restarts the given application.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="reason">
        /// The reason for restarting the application.
        /// </param>
        void RelaunchApplication(ApplicationInfo application, string reason);

        /// <summary>
        /// Exits the given application.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="reason">
        /// The reason for exiting the application.
        /// </param>
        void ExitApplication(ApplicationInfo application, string reason);

        /// <summary>
        /// Creates an <see cref="IApplicationStateObserver"/> for the given application.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <returns>
        /// The new <see cref="IApplicationStateObserver"/>.
        /// </returns>
        IApplicationStateObserver CreateApplicationStateObserver(ApplicationInfo application);
    }
}