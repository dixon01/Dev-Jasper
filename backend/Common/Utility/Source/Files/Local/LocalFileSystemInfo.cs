// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileSystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileSystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local file system info.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="FileSystemInfo"/> this item represents.
    /// </typeparam>
    internal abstract class LocalFileSystemInfo<T> : IWritableFileSystemInfo
        where T : FileSystemInfo
    {
        private bool isValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileSystemInfo{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="fileSystem">
        /// The file system creating this info.
        /// </param>
        protected LocalFileSystemInfo(T item, IWritableFileSystem fileSystem)
        {
            this.Item = item;
            this.isValid = true;
            this.FileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the file attributes.
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                this.VerifyVaild();
                return this.Item.Attributes;
            }
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName
        {
            get
            {
                this.VerifyVaild();
                return this.Item.FullName;
            }
        }

        /// <summary>
        /// Gets the name including the extension.
        /// </summary>
        public string Name
        {
            get
            {
                this.VerifyVaild();
                return this.Item.Name;
            }
        }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public string Extension
        {
            get
            {
                this.VerifyVaild();
                return this.Item.Extension;
            }
        }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                this.VerifyVaild();
                return this.Item.LastWriteTime;
            }
        }

        /// <summary>
        /// Gets the file system this item belongs to.
        /// </summary>
        public IWritableFileSystem FileSystem { get; private set; }

        FileAttributes IWritableFileSystemInfo.Attributes
        {
            get
            {
                this.VerifyVaild();
                return this.Item.Attributes;
            }

            set
            {
                this.Item.Attributes = value;
            }
        }

        IFileSystem IFileSystemInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        /// <summary>
        /// Gets the underlying item.
        /// </summary>
        protected T Item { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is valid.
        /// </summary>
        protected bool IsValid
        {
            get
            {
                return this.isValid;
            }

            set
            {
                this.VerifyVaild();
                this.isValid = value;
            }
        }

        /// <summary>
        /// Deletes this item.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
        /// </summary>
        public void Delete()
        {
            this.VerifyVaild();
            this.Item.Delete();
            this.IsValid = false;
        }

        /// <summary>
        /// Verifies the validity of this object.
        /// If it is invalid, an <see cref="IOException"/> is thrown.
        /// </summary>
        /// <exception cref="IOException">
        /// If this object is invalid (e.g. deleted, moved, ...)
        /// </exception>
        protected void VerifyVaild()
        {
            if (!this.IsValid)
            {
                throw new IOException("Path is no more valid: " + this.Item.FullName);
            }
        }
    }
}