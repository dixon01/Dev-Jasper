// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWritableFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWritableFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Writable
{
    using System.IO;

    /// <summary>
    /// A file system that is writeable.
    /// Implementations of this interface should always
    /// subclass from <see cref="IFileSystem"/>.
    /// </summary>
    public interface IWritableFileSystem : IFileSystem
    {
        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        new IWritableDriveInfo[] GetDrives();

        /// <summary>
        /// Gets a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IWritableFileInfo"/>.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// if the given file can't be found.
        /// </exception>
        new IWritableFileInfo GetFile(string path);

        /// <summary>
        /// Tries to get a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="file">
        /// The <see cref="IWritableFileInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the file was found.
        /// </returns>
        bool TryGetFile(string path, out IWritableFileInfo file);

        /// <summary>
        /// Gets or creates a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IFileInfo"/>.
        /// </returns>
        IWritableFileInfo CreateFile(string path);

        /// <summary>
        /// Gets a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IWritableDirectoryInfo"/>.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// if the given directory can't be found.
        /// </exception>
        new IWritableDirectoryInfo GetDirectory(string path);

        /// <summary>
        /// Tries to get a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="directory">
        /// The <see cref="IWritableDirectoryInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the directory was found.
        /// </returns>
        bool TryGetDirectory(string path, out IWritableDirectoryInfo directory);

        /// <summary>
        /// Gets or creates file a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryInfo"/>.
        /// </returns>
        IWritableDirectoryInfo CreateDirectory(string path);
    }
}