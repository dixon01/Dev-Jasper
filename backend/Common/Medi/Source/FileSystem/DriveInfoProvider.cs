// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveInfoProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveInfoProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.FileSystem
{
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Implementation of <see cref="IDriveInfo"/> for <see cref="RemoteFileSystem"/>.
    /// </summary>
    internal class DriveInfoProvider : IDriveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveInfoProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the drive.
        /// </param>
        /// <param name="managementProvider">
        /// The management provider representing the drive.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        public DriveInfoProvider(string name, IRemoteManagementProvider managementProvider, RemoteFileSystem fileSystem)
        {
            this.Name = name;
            this.FileSystem = fileSystem;
            this.RootDirectory = new DirectoryInfoProvider(name, managementProvider, fileSystem);
        }

        /// <summary>
        /// Gets the root directory of this drive.
        /// </summary>
        public IDirectoryInfo RootDirectory { get; private set; }

        /// <summary>
        /// Gets the name of this drive.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file system this drive belongs to.
        /// </summary>
        public IFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the amount of available free space on a drive, in bytes.
        /// </summary>
        public long AvailableFreeSpace
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        public long TotalSize
        {
            get
            {
                return 0;
            }
        }
    }
}