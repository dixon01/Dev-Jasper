// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationRegistration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IApplicationRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// The application registration interface to be used by clients of the System Manager.
    /// </summary>
    public interface IApplicationRegistration
    {
        /// <summary>
        /// Event that is fired when the registration
        /// (started with <see cref="Register"/>) was successful.
        /// </summary>
        event EventHandler Registered;

        /// <summary>
        /// Event that is fired when the watchdog was kicked.
        /// You can set <see cref="CancelEventArgs.Cancel"/> to true to
        /// prevent the client from sending a response to the request.
        /// This will then at some point trigger a re-launch from the System Manager.
        /// </summary>
        event CancelEventHandler WatchdogKicked;

        /// <summary>
        /// Event that is fired when a application re-launch was requested by System Manager.
        /// </summary>
        event EventHandler RelaunchRequested;

        /// <summary>
        /// Event that is fired when an application exit was requested by System Manager.
        /// </summary>
        event EventHandler ExitRequested;

        /// <summary>
        /// Gets the state of this application.
        /// </summary>
        ApplicationState State { get; }

        /// <summary>
        /// Gets the information about this application.
        /// </summary>
        ApplicationInfo Info { get; }

        /// <summary>
        /// Begins to register this registration with the System Manager.
        /// When the registration was successful, <see cref="Registered"/> will be fired.
        /// </summary>
        void Register();

        /// <summary>
        /// Sets the state of this application to <see cref="ApplicationState.Running"/>.
        /// This should be called once the application's main functionality is running
        /// (it's not starting up anymore).
        /// </summary>
        void SetRunning();

        /// <summary>
        /// Sets the state of this application to <see cref="ApplicationState.Exiting"/>.
        /// This should be called once the application received the <see cref="ExitRequested"/>
        /// event and is exiting.
        /// </summary>
        void SetExiting();

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        void Exit(string reason);

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        void Relaunch(string reason);

        /// <summary>
        /// Deregisters this application from the System Manager.
        /// This will set the <see cref="State"/> to <see cref="ApplicationState.Exiting"/>.
        /// </summary>
        void Deregister();

        /// <summary>
        /// Creates a new loop observer with the given name and a timeout.
        /// </summary>
        /// <param name="name">
        /// The name of the observer, used for logging.
        /// </param>
        /// <param name="timeout">
        /// The timeout after which the observer should ask the application to restart.
        /// </param>
        /// <returns>
        /// The new <see cref="ILoopObserver"/>.
        /// </returns>
        ILoopObserver CreateLoopObserver(string name, TimeSpan timeout);
    }
}