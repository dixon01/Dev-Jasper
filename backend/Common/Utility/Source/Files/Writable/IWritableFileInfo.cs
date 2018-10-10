// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWritableFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWritableFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Writable
{
    using System.IO;

    /// <summary>
    /// File information that is also writable.
    /// </summary>
    public interface IWritableFileInfo : IFileInfo, IWritableFileSystemInfo
    {
        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        new IWritableDirectoryInfo Directory { get; }

        /// <summary>
        /// Opens this file for writing.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream OpenWrite();

        /// <summary>
        /// Opens this file for writing by appending to an existing file.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream OpenAppend();

        /// <summary>
        /// Moves the file to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
        /// After this call, use the returned <see cref="IWritableFileInfo"/>
        /// instead of this object.
        /// </summary>
        /// <param name="newFileName">
        /// The new location where the file should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableFileInfo"/> that describes this file at the
        /// new location.
        /// </returns>
        IWritableFileInfo MoveTo(string newFileName);

        /// <summary>
        /// Copies this file to a new location.
        /// </summary>
        /// <param name="newFileName">
        /// The new location where the file should be copied to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableFileInfo"/> that describes the copy of this
        /// file at the new location.
        /// </returns>
        /// <exception cref="IOException">
        /// If there is already a file at <see cref="newFileName"/>.
        /// </exception>
        IWritableFileInfo CopyTo(string newFileName);
    }
}