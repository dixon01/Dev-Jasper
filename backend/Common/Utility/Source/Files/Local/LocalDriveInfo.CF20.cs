// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDriveInfo.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local drive info.
    /// </summary>
    internal partial class LocalDriveInfo : IWritableDriveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDriveInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system creating this info.
        /// </param>
        public LocalDriveInfo(IWritableFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "\\";
            }
        }

        /// <summary>
        /// Gets the root directory.
        /// </summary>
        public IWritableDirectoryInfo RootDirectory
        {
            get
            {
                return this.FileSystem.GetDirectory(this.Name);
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
                long available = 0;
                long size = 0;
                long total = 0;
                NativeMethods.GetDiskFreeSpaceEx(this.Name, ref available, ref size, ref total);
                return available;
            }
        }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        public long TotalSize
        {
            get
            {
                long available = 0;
                long size = 0;
                long total = 0;
                NativeMethods.GetDiskFreeSpaceEx(this.Name, ref available, ref size, ref total);
                return size;
            }
        }

        IDirectoryInfo IDriveInfo.RootDirectory
        {
            get
            {
                return this.RootDirectory;
            }
        }

        IFileSystem IDriveInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        private static class NativeMethods
        {
            [DllImport("coredll.dll", SetLastError = true)]
            public static extern bool GetDiskFreeSpaceEx(
                string directoryName, ref long freeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);
        }
    }
}