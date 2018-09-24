// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemInfoProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemInfoProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Gorba.Common.Medi.Core.Management.FileSystem;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Base class for all classes implementing <see cref="IFileSystemInfo"/> for <see cref="RemoteFileSystem"/>.
    /// </summary>
    internal abstract class FileSystemInfoProviderBase : IFileSystemInfo
    {
        private string fullName;

        private DateTime? lastWriteTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemInfoProviderBase"/> class.
        /// </summary>
        /// <param name="managementProvider">
        /// The management provider representing the item.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        protected FileSystemInfoProviderBase(IRemoteManagementProvider managementProvider, RemoteFileSystem fileSystem)
            : this(managementProvider.Name, managementProvider, fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemInfoProviderBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the item.
        /// </param>
        /// <param name="managementProvider">
        /// The management provider representing the directory.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        protected FileSystemInfoProviderBase(
            string name,
            IRemoteManagementProvider managementProvider,
            RemoteFileSystem fileSystem)
        {
            this.ManagementProvider = managementProvider;
            this.FileSystem = fileSystem;

            this.Name = name;

            this.Extension = Path.GetExtension(this.Name);
        }

        /// <summary>
        /// Gets or sets the file attributes.
        /// </summary>
        public FileAttributes Attributes { get; protected set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.fullName != null)
                {
                    return this.fullName;
                }

                var parts = new List<string>();
                for (var provider = this.ManagementProvider;
                     provider != null && provider != this.FileSystem.ManagementProvider;
                     provider = provider.Parent as IRemoteManagementProvider)
                {
                    parts.Add(provider.Name);
                }

                parts.Reverse();
                if (parts.Count == 0)
                {
                    return string.Empty;
                }

                var drive = parts[0];
                if (drive.Length > 1 && drive.EndsWith("\\"))
                {
                    parts[0] = drive.Substring(0, drive.Length - 1);
                }

                var builder = new StringBuilder();
                foreach (var part in parts)
                {
                    builder.Append(part).Append(drive.StartsWith("/") ? '/' : '\\');
                }

                if (this is FileInfoProvider)
                {
                    // remove the trailing slash if it is a file
                    builder.Length--;
                }

                return this.fullName = builder.ToString();
            }
        }

        /// <summary>
        /// Gets the name including the extension.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                if (this.lastWriteTime.HasValue)
                {
                    return this.lastWriteTime.Value;
                }

                var time = DateTime.MinValue;
                var property = this.FileSystem.GetProperty(
                    this.ManagementProvider,
                    FileSystemManagementProvider.LastModifiedPropertyName);
                if (property != null
                    && !DateTime.TryParse(
                        property.StringValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal,
                        out time))
                {
                    time = DateTime.MinValue;
                }

                this.lastWriteTime = time;
                return time;
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
        /// Gets the remote file system owning this item.
        /// </summary>
        protected RemoteFileSystem FileSystem { get; private set; }

        /// <summary>
        /// Gets the management provider represented by this item.
        /// </summary>
        protected IRemoteManagementProvider ManagementProvider { get; private set; }
    }
}