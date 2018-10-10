// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperDriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting.Wrapper
{
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Wrapper around an <see cref="IWritableDriveInfo"/>.
    /// </summary>
    public class WrapperDriveInfo : IWritableDriveInfo
    {
        private readonly IWritableDriveInfo wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperDriveInfo"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped object.
        /// </param>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        public WrapperDriveInfo(IWritableDriveInfo wrapped, WrapperFileSystem fileSystem)
        {
            this.wrapped = wrapped;
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the name of this drive.
        /// </summary>
        public string Name
        {
            get
            {
                return this.wrapped.Name;
            }
        }

        /// <summary>
        /// Gets the root directory of this drive.
        /// </summary>
        public IWritableDirectoryInfo RootDirectory
        {
            get
            {
                return ((WrapperFileSystem)this.FileSystem).CreateDirectoryInfo(this.wrapped.RootDirectory);
            }
        }

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