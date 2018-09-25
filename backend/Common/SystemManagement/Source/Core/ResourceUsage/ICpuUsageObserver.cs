// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICpuUsageObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICpuUsageObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;

    /// <summary>
    /// Observer of CPU usage.
    /// </summary>
    public interface ICpuUsageObserver : IDisposable
    {
        /// <summary>
        /// Event that is fired when the <see cref="Usage"/> was updated.
        /// </summary>
        event EventHandler Updated;

        /// <summary>
        /// Gets the usage in percent (0.0 .. 1.0).
        /// </summary>
        double Usage { get; }
    }
}