// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingDriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting
{
    using System.Globalization;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// <see cref="IWritableDriveInfo"/> implementation for
    /// <see cref="TestingFileSystem"/>.
    /// </summary>
    public class TestingDriveInfo : IWritableDriveInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingDriveInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system creating this object.
        /// </param>
        /// <param name="name">
        /// The drive name.
        /// </param>
        public TestingDriveInfo(TestingFileSystem fileSystem, char name)
        {
            this.FileSystem = fileSystem;
            this.Name = name.ToString(CultureInfo.InvariantCulture);
            this.RootDirectory = new TestingDirectoryInfo(fileSystem, string.Format("{0}:\\", name));
        }

        /// <summary>
        /// Gets the name of this drive.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file system this drive belongs to.
        /// </summary>
        public IWritableFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the amount of available free space on a drive, in bytes.
        /// </summary>
        public long AvailableFreeSpace { get; private set; }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        public long TotalSize { get; private set; }

        IFileSystem IDriveInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        /// <summary>
        /// Gets the root directory of this drive.
        /// </summary>
        public IWritableDirectoryInfo RootDirectory { get; private set; }

        IDirectoryInfo IDriveInfo.RootDirectory
        {
            get
            {
                return this.RootDirectory;
            }
        }

        /// <summary>
        /// Sets the available free space and total size of a drive.
        /// </summary>
        /// <param name="availableFreeSpace">
        /// The available free space.
        /// </param>
        /// <param name="totalSize">
        /// The total size.
        /// </param>
        public void SetDriveSpace(long availableFreeSpace, long totalSize)
        {
            this.AvailableFreeSpace = availableFreeSpace;
            this.TotalSize = totalSize;
        }
    }
}