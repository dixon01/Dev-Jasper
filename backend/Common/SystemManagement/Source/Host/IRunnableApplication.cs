// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRunnableApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRunnableApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// An application that supports the Run method as well as ways to re-launch and exit it.
    /// Usually you should use <see cref="ApplicationBase"/> instead of implementing this interface
    /// yourself.
    /// </summary>
    public interface IRunnableApplication : IApplication
    {
        /// <summary>
        /// Runs this application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        void Run(string[] args);

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        void Relaunch(string reason);

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        void Exit(string reason);
    }
}