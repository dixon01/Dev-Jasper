// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDriveInfo.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System.IO;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local drive info.
    /// </summary>
    internal partial class LocalDriveInfo : IWritableDriveInfo
    {
        private readonly DriveInfo drive;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDriveInfo"/> class.
        /// </summary>
        /// <param name="drive">
        /// The drive.
        /// </param>
        /// <param name="fileSystem">
        /// The file system creating this info.
        /// </param>
        public LocalDriveInfo(DriveInfo drive, IWritableFileSystem fileSystem)
        {
            this.drive = drive;
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.drive.Name;
            }
        }

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        public IDirectoryInfo RootDirectory
        {
            get
            {
                return ((IWritableDriveInfo)this).RootDirectory;
            }
        }

        /// <summary>
        /// Gets the file system this drive belongs to.
        /// </summary>
        public IWritableFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the amount of available free space on a drive, in bytes.
        /// </summary>
        public long AvailableFreeSpace
        {
            get
            {
                return this.drive.AvailableFreeSpace;
            }
        }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        public long TotalSize
        {
            get
            {
                return this.drive.TotalSize;
            }
        }

        IWritableDirectoryInfo IWritableDriveInfo.RootDirectory
        {
            get
            {
                DirectoryInfo directory = this.drive.RootDirectory;
                return new LocalDirectoryInfo(directory, this.FileSystem);
            }
        }

        IFileSystem IDriveInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }
    }
}