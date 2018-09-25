// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    /// <summary>
    /// Drive information.
    /// </summary>
    public interface IDriveInfo
    {
        /// <summary>
        /// Gets the root directory of this drive.
        /// </summary>
        IDirectoryInfo RootDirectory { get; }

        /// <summary>
        /// Gets the name of this drive.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the file system this drive belongs to.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the amount of available free space on a drive, in bytes.
        /// </summary>
        long AvailableFreeSpace { get; }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes.
        /// </summary>
        long TotalSize { get; }
    }
}