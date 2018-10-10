// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.ServiceModel
{
    /// <summary>
    /// The state of an application managed by System Manager.
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>
        /// The initial state, shouldn't be used anywhere.
        /// </summary>
        Unknown,

        /// <summary>
        /// The application will soon start, e.g. waiting for the launch delay or another app.
        /// </summary>
        AwaitingLaunch,

        /// <summary>
        /// The application process is started, but no feedback from application yet.
        /// </summary>
        Launching,

        /// <summary>
        /// The application is registered and starting up.
        /// </summary>
        Starting,

        /// <summary>
        /// The application is running (normal state).
        /// </summary>
        Running,

        /// <summary>
        /// The application is exiting, but process is still running.
        /// </summary>
        Exiting,

        /// <summary>
        /// The application has exited. This state is only reached if an application
        /// should not be re-launched by SM (e.g. during shutdown or when someone
        /// requests the application to exit).
        /// </summary>
        Exited
    }
}
