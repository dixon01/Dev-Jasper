// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICpuUsageObserverFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICpuUsageObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Factory interface to create <see cref="ICpuUsageObserver"/>s.
    /// </summary>
    public interface ICpuUsageObserverFactory
    {
        /// <summary>
        /// Creates a new observer for the given process or the overall usage.
        /// You should call <see cref="IDisposable.Dispose"/> on the
        /// returned object once you don't need it anymore.
        /// </summary>
        /// <param name="process">
        /// The process or null to get the overall CPU usage.
        /// </param>
        /// <returns>
        /// A new <see cref="ICpuUsageObserver"/> implementation.
        /// </returns>
        ICpuUsageObserver CreateObserver(Process process);
    }
}