// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local file system.
    /// </summary>
    internal partial class LocalFileSystem : IWritableFileSystem
    {
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
        public IFileInfo GetFile(string path)
        {
            return this.GetFile(path, false, true);
        }

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
        public bool TryGetFile(string path, out IFileInfo file)
        {
            file = this.GetFile(path, false, false);
            return file != null;
        }

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
        public IDirectoryInfo GetDirectory(string path)
        {
            return this.GetDirectory(path, false, true);
        }

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
        public bool TryGetDirectory(string path, out IDirectoryInfo directory)
        {
            directory = this.GetDirectory(path, false, false);
            return directory != null;
        }

        IWritableFileInfo IWritableFileSystem.GetFile(string path)
        {
            return this.GetFile(path, false, true);
        }

        bool IWritableFileSystem.TryGetFile(string path, out IWritableFileInfo file)
        {
            file = this.GetFile(path, false, false);
            return file != null;
        }

        IWritableFileInfo IWritableFileSystem.CreateFile(string path)
        {
            return this.GetFile(path, true, false);
        }

        IWritableDirectoryInfo IWritableFileSystem.GetDirectory(string path)
        {
            return this.GetDirectory(path, false, true);
        }

        bool IWritableFileSystem.TryGetDirectory(string path, out IWritableDirectoryInfo directory)
        {
            directory = this.GetDirectory(path, false, false);
            return directory != null;
        }

        IWritableDirectoryInfo IWritableFileSystem.CreateDirectory(string path)
        {
            return this.GetDirectory(path, true, false);
        }

        /// <summary>
        /// Create a <see cref="LocalFileInfo"/> or <see cref="LocalDirectoryInfo"/>
        /// for the given <see cref="FileSystemInfo"/>.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        /// <returns>
        /// The <see cref="IWritableFileSystemInfo"/>.
        /// </returns>
        internal static IWritableFileSystemInfo CreateFileSystemInfo(
            FileSystemInfo item, IWritableFileSystem fileSystem)
        {
            var file = item as FileInfo;
            if (file != null)
            {
                return new LocalFileInfo(file, fileSystem);
            }

            var dir = item as DirectoryInfo;
            if (dir != null)
            {
                return new LocalDirectoryInfo(dir, fileSystem);
            }

            throw new NotSupportedException("Unknown type: " + item.GetType());
        }

        private LocalFileInfo GetFile(string path, bool create, bool throwException)
        {
            var info = new FileInfo(path);
            if (!info.Exists)
            {
                if (!create)
                {
                    if (throwException)
                    {
                        throw new FileNotFoundException("Couldn't find file: " + path);
                    }

                    return null;
                }

                info.Create().Close();
            }

            return new LocalFileInfo(info, this);
        }

        private LocalDirectoryInfo GetDirectory(string path, bool create, bool throwException)
        {
            var info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                if (!create)
                {
                    if (throwException)
                    {
                        throw new DirectoryNotFoundException("Couldn't find directory " + path);
                    }

                    return null;
                }

                info.Create();
            }

            return new LocalDirectoryInfo(info, this);
        }
    }
}