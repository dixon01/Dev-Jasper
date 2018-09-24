// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperFileSystemInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperFileSystemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting.Wrapper
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Wrapper around an <see cref="IWritableFileSystemInfo"/>.
    /// </summary>
    public abstract class WrapperFileSystemInfo : IWritableFileSystemInfo
    {
        private readonly IWritableFileSystemInfo wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperFileSystemInfo"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped object.
        /// </param>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        protected WrapperFileSystemInfo(IWritableFileSystemInfo wrapped, WrapperFileSystem fileSystem)
        {
            this.wrapped = wrapped;
            this.WrapperFileSystem = fileSystem;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public virtual string FullName
        {
            get
            {
                return this.wrapped.FullName;
            }
        }

        /// <summary>
        /// Gets the name including the extension.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.wrapped.Name;
            }
        }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public virtual string Extension
        {
            get
            {
                return this.wrapped.Extension;
            }
        }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        public virtual DateTime LastWriteTime
        {
            get
            {
                return this.wrapped.LastWriteTime;
            }
        }

        /// <summary>
        /// Gets the owning <see cref="WrapperFileSystem"/>.
        /// </summary>
        public WrapperFileSystem WrapperFileSystem { get; private set; }

        IFileSystem IFileSystemInfo.FileSystem
        {
            get
            {
                return this.FileSystem;
            }
        }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public virtual FileAttributes Attributes
        {
            get
            {
                return this.wrapped.Attributes;
            }

            set
            {
                this.wrapped.Attributes = value;
            }
        }

        /// <summary>
        /// Gets the file system this item belongs to.
        /// </summary>
        public IWritableFileSystem FileSystem
        {
            get
            {
                return this.WrapperFileSystem;
            }
        }

        /// <summary>
        /// Deletes this item.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
        /// </summary>
        public virtual void Delete()
        {
            this.wrapped.Delete();
        }
    }
}