// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWritableFileSystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWritableFileSystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Writable
{
    using System.IO;

    /// <summary>
    /// Base interface for writable file system items.
    /// </summary>
    public interface IWritableFileSystemInfo : IFileSystemInfo
    {
        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        new FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets the file system this item belongs to.
        /// </summary>
        new IWritableFileSystem FileSystem { get; }

        /// <summary>
        /// Deletes this item.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
        /// </summary>
        void Delete();
    }
}