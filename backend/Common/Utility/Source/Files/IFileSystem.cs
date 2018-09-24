// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    using System.IO;

    /// <summary>
    /// An abstraction layer above a local or remote file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        IDriveInfo[] GetDrives();

        /// <summary>
        /// Gets a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IFileInfo"/>.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// if the given file can't be found.
        /// </exception>
        IFileInfo GetFile(string path);

        /// <summary>
        /// Tries to get a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="file">
        /// The <see cref="IFileInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the file was found.
        /// </returns>
        bool TryGetFile(string path, out IFileInfo file);

        /// <summary>
        /// Gets a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryInfo"/>.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// if the given directory can't be found.
        /// </exception>
        IDirectoryInfo GetDirectory(string path);

        /// <summary>
        /// Tries to get a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="directory">
        /// The <see cref="IDirectoryInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the directory was found.
        /// </returns>
        bool TryGetDirectory(string path, out IDirectoryInfo directory);
    }
}
