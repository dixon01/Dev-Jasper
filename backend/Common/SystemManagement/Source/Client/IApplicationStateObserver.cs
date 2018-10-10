// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationStateObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IApplicationStateObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Observer for the state of an application. This observer will notify
    /// any registered listeners in case the state of an application changes.
    /// </summary>
    public interface IApplicationStateObserver : IDisposable
    {
        /// <summary>
        /// Event that is fired when <see cref="State"/> changes.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the application name.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        ApplicationState State { get; }
    }
}