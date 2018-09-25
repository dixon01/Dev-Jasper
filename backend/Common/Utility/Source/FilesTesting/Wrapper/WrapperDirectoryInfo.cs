// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperDirectoryInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperDirectoryInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting.Wrapper
{
    using System;
    using System.Linq;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Wrapper around an <see cref="IWritableDirectoryInfo"/>.
    /// </summary>
    public class WrapperDirectoryInfo : WrapperFileSystemInfo, IWritableDirectoryInfo
    {
        private readonly IWritableDirectoryInfo wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperDirectoryInfo"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped object.
        /// </param>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        public WrapperDirectoryInfo(IWritableDirectoryInfo wrapped, WrapperFileSystem fileSystem)
            : base(wrapped, fileSystem)
        {
            this.wrapped = wrapped;
        }

        /// <summary>
        /// Gets the root directory of this directory.
        /// </summary>
        public virtual IWritableDirectoryInfo Root
        {
            get
            {
                return this.WrapperFileSystem.CreateDirectoryInfo(this.wrapped.Root);
            }
        }

        IDirectoryInfo IDirectoryInfo.Root
        {
            get
            {
                return this.Root;
            }
        }

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        public virtual IWritableFileInfo[] GetFiles()
        {
            return
                this.wrapped.GetFiles()
                    .Select(f => (IWritableFileInfo)this.WrapperFileSystem.CreateFileInfo(f))
                    .ToArray();
        }

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        public virtual IWritableDirectoryInfo[] GetDirectories()
        {
            return
                this.wrapped.GetDirectories()
                    .Select(d => (IWritableDirectoryInfo)this.WrapperFileSystem.CreateDirectoryInfo(d))
                    .ToArray();
        }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        public virtual IWritableFileSystemInfo[] GetFileSystemInfos()
        {
            return this.wrapped.GetFileSystemInfos().Select(this.CreateFileSystemInfo).ToArray();
        }

        /// <summary>
        /// Moves the directory to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
        /// After this call, use the returned <see cref="IWritableDirectoryInfo"/>
        /// instead of this object.
        /// </summary>
        /// <param name="newDirectoryName">
        /// The new location where the directory should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableDirectoryInfo"/> that describes this directory at the
        /// new location.
        /// </returns>
        public virtual IWritableDirectoryInfo MoveTo(string newDirectoryName)
        {
            return this.WrapperFileSystem.CreateDirectoryInfo(this.wrapped.MoveTo(newDirectoryName));
        }

        IFileInfo[] IDirectoryInfo.GetFiles()
        {
            return this.GetFiles().Cast<IFileInfo>().ToArray();
        }

        IDirectoryInfo[] IDirectoryInfo.GetDirectories()
        {
            return this.GetDirectories().Cast<IDirectoryInfo>().ToArray();
        }

        IFileSystemInfo[] IDirectoryInfo.GetFileSystemInfos()
        {
            return this.GetFileSystemInfos().Cast<IFileSystemInfo>().ToArray();
        }

        private IWritableFileSystemInfo CreateFileSystemInfo(IWritableFileSystemInfo info)
        {
            var file = info as IWritableFileInfo;
            if (file != null)
            {
                return this.WrapperFileSystem.CreateFileInfo(file);
            }

            var directory = info as IWritableDirectoryInfo;
            if (directory != null)
            {
                return this.WrapperFileSystem.CreateDirectoryInfo(directory);
            }

            throw new ArgumentException("Unknown info type");
        }
    }
}