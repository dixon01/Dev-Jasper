// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFileSystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    using System;
    using System.IO;

    /// <summary>
    /// Base interface for file system items.
    /// </summary>
    public interface IFileSystemInfo
    {
        /// <summary>
        /// Gets the file attributes.
        /// </summary>
        FileAttributes Attributes { get; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the name including the extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        DateTime LastWriteTime { get; }

        /// <summary>
        /// Gets the file system this item belongs to.
        /// </summary>
        IFileSystem FileSystem { get; }
    }
}