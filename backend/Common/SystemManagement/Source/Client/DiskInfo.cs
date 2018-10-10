// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiskInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System.ComponentModel;

    /// <summary>
    /// Information about the available space on a disk.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DiskInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        internal DiskInfo(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the disk.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the available free space on the disk in bytes.
        /// </summary>
        public long AvailableFreeSpace { get; internal set; }

        /// <summary>
        /// Gets the total size of the disk in bytes.
        /// </summary>
        public long TotalSize { get; internal set; }
    }
}