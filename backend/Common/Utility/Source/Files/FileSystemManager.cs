// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    using Gorba.Common.Utility.Files.Local;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The static entry point to get the local file system.
    /// </summary>
    public static class FileSystemManager
    {
        static FileSystemManager()
        {
            Local = new LocalFileSystem();
        }

        /// <summary>
        /// Gets the local file system.
        /// The returned instance might also implement
        /// <see cref="IWritableFileSystemInfo"/>.
        /// </summary>
        public static IFileSystem Local { get; private set; }

        /// <summary>
        /// Changes the local file system returned by
        /// <see cref="get_Local"/>.
        /// </summary>
        /// <param name="fileSystem">
        /// The new file system.
        /// </param>
        public static void ChangeLocalFileSystem(IFileSystem fileSystem)
        {
            Local = fileSystem;
        }
    }
}