// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingFileSystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingFileSystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// <see cref="IWritableFileSystemInfo"/> implementation for
    /// <see cref="TestingFileSystem"/>.
    /// </summary>
    public abstract class TestingFileSystemInfo : IWritableFileSystemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingFileSystemInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        protected TestingFileSystemInfo(TestingFileSystem fileSystem, string path)
        {
            this.FileSystem = fileSystem;
            this.FullName = path;
            var index = path.LastIndexOf(Path.DirectorySeparatorChar, path.Length - 2, path.Length - 1) + 1;
            this.Name = path.Substring(index);
            this.Extension = string.Empty;
            this.LastWriteTime = TimeProvider.Current.Now;
        }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets or sets the name including the extension.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        public DateTime LastWriteTime { get; private set; }

        IWritableFileSystem IWritableFileSystemInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        IFileSystem IFileSystemInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        FileAttributes IFileSystemInfo.Attributes
        {
            get
            {
                return this.Attributes;
            }
        }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        protected TestingFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Deletes this item.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.FullName;
        }

        /// <summary>
        /// Renames this item.
        /// </summary>
        /// <param name="path">
        /// The new full path.
        /// </param>
        internal virtual void Rename(string path)
        {
            this.FullName = path;
            var index = path.LastIndexOf(Path.DirectorySeparatorChar, path.Length - 2, path.Length - 1) + 1;
            this.Name = path.Substring(index);
        }
    }
}