// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWritableDriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWritableDriveInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Writable
{
    /// <summary>
    /// Drive information that is also writable.
    /// </summary>
    public interface IWritableDriveInfo : IDriveInfo
    {
        /// <summary>
        /// Gets the root directory of this drive.
        /// </summary>
        new IWritableDirectoryInfo RootDirectory { get; }

        /// <summary>
        /// Gets the file system this drive belongs to.
        /// </summary>
        new IWritableFileSystem FileSystem { get; }
    }
}