// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDirectoryInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDirectoryInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    /// <summary>
    /// Directory information.
    /// </summary>
    public interface IDirectoryInfo : IFileSystemInfo
    {
        /// <summary>
        /// Gets the root directory of this directory.
        /// </summary>
        IDirectoryInfo Root { get; }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        IFileSystemInfo[] GetFileSystemInfos();

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        IFileInfo[] GetFiles();

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        IDirectoryInfo[] GetDirectories();
    }
}