// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using Gorba.Common.SystemManagement.Messages;

    /// <summary>
    /// Information about the resource usage of a system.
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInfo"/> class.
        /// </summary>
        /// <param name="response">
        /// The response from which to create this information.
        /// </param>
        internal SystemInfo(SystemInfoResponse response)
        {
            this.CpuUsage = response.CpuUsage;
            this.AvailableRam = response.AvailableRam;
            this.TotalRam = response.TotalRam;

            this.Disks =
                response.Disks.ConvertAll(
                    d => new DiskInfo(d.Name) { AvailableFreeSpace = d.AvailableFreeSpace, TotalSize = d.TotalSize })
                    .ToArray();
        }

        /// <summary>
        /// Gets the system CPU usage in percent (0.0 .. 1.0).
        /// </summary>
        public double CpuUsage { get; private set; }

        /// <summary>
        /// Gets the currently available RAM in the system.
        /// </summary>
        public long AvailableRam { get; private set; }

        /// <summary>
        /// Gets the total amount of RAM installed in the system.
        /// </summary>
        public long TotalRam { get; private set; }

        /// <summary>
        /// Gets the list of disks.
        /// </summary>
        public DiskInfo[] Disks { get; private set; }
    }
}