// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemResourceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISystemResourceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    /// <summary>
    /// Access to information about system resources (RAM and CPU).
    /// </summary>
    public interface ISystemResourceInfo
    {
        /// <summary>
        /// Gets the system CPU usage in percent (0.0 .. 1.0).
        /// </summary>
        double CpuUsage { get; }

        /// <summary>
        /// Gets the currently available RAM in the system.
        /// </summary>
        long AvailableRam { get; }

        /// <summary>
        /// Gets the total amount of RAM installed in the system.
        /// </summary>
        long TotalRam { get; }
    }
}