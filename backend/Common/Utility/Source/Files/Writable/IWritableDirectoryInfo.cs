// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWritableDirectoryInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWritableDirectoryInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Writable
{
    using System.IO;

    /// <summary>
    /// Directory information that is also writable.
    /// </summary>
    public interface IWritableDirectoryInfo : IDirectoryInfo, IWritableFileSystemInfo
    {
        /// <summary>
        /// Gets the root directory of this directory.
        /// </summary>
        new IWritableDirectoryInfo Root { get; }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        new IWritableFileSystemInfo[] GetFileSystemInfos();

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        new IWritableFileInfo[] GetFiles();

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        new IWritableDirectoryInfo[] GetDirectories();

        /// <summary>
        /// Moves the directory to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
        /// After this call, use the returned <see cref="IWritableDirectoryInfo"/>
        /// instead of this object.
        /// </summary>
        /// <param name="newDirectoryName">
        /// The new location where the directory should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableDirectoryInfo"/> that describes this directory at the
        /// new location.
        /// </returns>
        IWritableDirectoryInfo MoveTo(string newDirectoryName);
    }
}