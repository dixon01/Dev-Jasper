// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemInfoResponse.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemInfoResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Response containing system resource information.
    /// This is the response to a <see cref="SystemInfoRequest"/>.
    /// </summary>
    public class SystemInfoResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInfoResponse"/> class.
        /// </summary>
        public SystemInfoResponse()
        {
            this.Disks = new List<DiskInfo>();
        }

        /// <summary>
        /// Gets or sets the system CPU usage in percent (0.0 .. 1.0).
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the currently available RAM in the system.
        /// </summary>
        public long AvailableRam { get; set; }

        /// <summary>
        /// Gets or sets the total amount of RAM installed in the system.
        /// </summary>
        public long TotalRam { get; set; }

        /// <summary>
        /// Gets or sets the list of disks.
        /// </summary>
        public List<DiskInfo> Disks { get; set; }
    }
}
