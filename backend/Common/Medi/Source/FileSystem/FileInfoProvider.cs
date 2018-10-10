// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileInfoProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileInfoProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.FileSystem
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.FileSystem;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Implementation of <see cref="IFileInfo"/> for <see cref="RemoteFileSystem"/>.
    /// </summary>
    internal class FileInfoProvider : FileSystemInfoProviderBase, IFileInfo
    {
        private IDirectoryInfo directory;
        private long? size;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfoProvider"/> class.
        /// </summary>
        /// <param name="managementProvider">
        /// The management provider representing the file.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        public FileInfoProvider(IRemoteManagementProvider managementProvider, RemoteFileSystem fileSystem)
            : base(managementProvider, fileSystem)
        {
            this.Attributes = FileAttributes.ReadOnly;
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public IDirectoryInfo Directory
        {
            get
            {
                if (this.directory != null)
                {
                    return this.directory;
                }

                var parent = this.ManagementProvider.Parent;
                return this.directory = new DirectoryInfoProvider((IRemoteManagementProvider)parent, this.FileSystem);
            }
        }

        /// <summary>
        /// Gets the size of this file in bytes.
        /// </summary>
        public long Size
        {
            get
            {
                if (this.size.HasValue)
                {
                    return this.size.Value;
                }

                long sizeValue = 0;
                var property = this.FileSystem.GetProperty(
                    this.ManagementProvider,
                    FileSystemManagementProvider.SizePropertyName);
                if (property != null && property.Value is long)
                {
                    sizeValue = (long)property.Value;
                }

                this.size = sizeValue;
                return sizeValue;
            }
        }

        /// <summary>
        /// Opens this file for reading.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream OpenRead()
        {
            var result = this.FileSystem.BeginOpenFile(this, null, null);
            return this.FileSystem.EndOpenFile(result);
        }

        /// <summary>
        /// Opens this file to read UTF-8 text.
        /// </summary>
        /// <returns>
        /// The <see cref="TextReader"/>.
        /// </returns>
        public TextReader OpenText()
        {
            return new StreamReader(this.OpenRead(), Encoding.UTF8);
        }
    }
}