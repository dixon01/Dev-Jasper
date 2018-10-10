// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoopObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    /// <summary>
    /// Interface for a loop observer. An application can create a new
    /// observer through <see cref="IApplicationRegistration.CreateLoopObserver"/>
    /// and then it has to call <see cref="Trigger"/> in intervals inferior to
    /// the timeout given to <see cref="IApplicationRegistration.CreateLoopObserver"/>.
    /// If the loop observer is not triggered, the <see cref="IApplicationRegistration.WatchdogKicked"/>
    /// will be cancelled which will at some point lead to a restart of the application
    /// by System Manager.
    /// </summary>
    public interface ILoopObserver : IDisposable
    {
        /// <summary>
        /// Triggers this loop observer.
        /// This method has to be called regularly.
        /// </summary>
        void Trigger();
    }
}
