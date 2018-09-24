// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiskInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Messages
{
    /// <summary>
    /// The disk information part of a <see cref="SystemInfoResponse"/>.
    /// </summary>
    public class DiskInfo
    {
        /// <summary>
        /// Gets or sets the name of the disk.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the available free space on the disk in bytes.
        /// </summary>
        public long AvailableFreeSpace { get; set; }

        /// <summary>
        /// Gets or sets the total size of the disk in bytes.
        /// </summary>
        public long TotalSize { get; set; }
    }
}