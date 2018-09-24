// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryInfoProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectoryInfoProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Implementation of <see cref="IDirectoryInfo"/> for <see cref="RemoteFileSystem"/>.
    /// </summary>
    internal class DirectoryInfoProvider : FileSystemInfoProviderBase, IDirectoryInfo
    {
        private IDirectoryInfo root;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryInfoProvider"/> class.
        /// </summary>
        /// <param name="managementProvider">
        /// The management provider representing the directory.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        public DirectoryInfoProvider(
            IRemoteManagementProvider managementProvider,
            RemoteFileSystem fileSystem)
            : this(managementProvider.Name, managementProvider, fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryInfoProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the directory.
        /// </param>
        /// <param name="managementProvider">
        /// The management provider representing the directory.
        /// </param>
        /// <param name="fileSystem">
        /// The owning file system.
        /// </param>
        public DirectoryInfoProvider(
            string name, IRemoteManagementProvider managementProvider, RemoteFileSystem fileSystem)
            : base(name, managementProvider, fileSystem)
        {
            this.Attributes = FileAttributes.Directory;
        }

        /// <summary>
        /// Gets the root directory of this directory.
        /// </summary>
        public IDirectoryInfo Root
        {
            get
            {
                if (this.root != null)
                {
                    return this.root;
                }

                var slashIndex = this.FullName.IndexOf('\\');
                if (slashIndex < 0)
                {
                    return this.root = this;
                }

                return this.root = this.FileSystem.GetDirectory(this.FullName.Substring(0, slashIndex));
            }
        }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        public IFileSystemInfo[] GetFileSystemInfos()
        {
            var infos = new List<IFileSystemInfo>();
            this.ReloadChildren();
            foreach (var child in this.ManagementProvider.Children)
            {
                var provider = child as IRemoteManagementProvider;
                if (this.FileSystem.IsFile(provider))
                {
                    infos.Add(new FileInfoProvider(provider, this.FileSystem));
                    continue;
                }

                if (this.FileSystem.IsDirectory(provider))
                {
                    infos.Add(new DirectoryInfoProvider(provider, this.FileSystem));
                }
            }

            return infos.ToArray();
        }

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        public IFileInfo[] GetFiles()
        {
            var infos = new List<IFileInfo>();
            this.ReloadChildren();
            foreach (var child in this.ManagementProvider.Children)
            {
                var provider = child as IRemoteManagementProvider;
                if (this.FileSystem.IsFile(provider))
                {
                    infos.Add(new FileInfoProvider(provider, this.FileSystem));
                }
            }

            return infos.ToArray();
        }

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        public IDirectoryInfo[] GetDirectories()
        {
            var infos = new List<IDirectoryInfo>();
            this.ReloadChildren();
            foreach (var child in this.ManagementProvider.Children)
            {
                var provider = child as IRemoteManagementProvider;
                if (this.FileSystem.IsDirectory(provider))
                {
                    infos.Add(new DirectoryInfoProvider(provider, this.FileSystem));
                }
            }

            return infos.ToArray();
        }

        private void ReloadChildren()
        {
            this.ManagementProvider.Reload();
            var waiting = new List<IRemoteManagementObjectProvider>();
            var done = new[] { false };
            var waiter = new ManualResetEvent(false);
            foreach (var child in this.ManagementProvider.Children)
            {
                var provider = child as IRemoteManagementObjectProvider;
                if (provider == null)
                {
                    continue;
                }

                lock (waiting)
                {
                    waiting.Add(provider);
                }

                provider.BeginReload(
                    ar =>
                        {
                            provider.EndReload(ar);
                            lock (waiting)
                            {
                                waiting.Remove(provider);
                                if (done[0] && waiting.Count == 0)
                                {
                                    waiter.Set();
                                }
                            }
                        },
                    null);
            }

            lock (waiting)
            {
                done[0] = true;
                if (waiting.Count == 0)
                {
                    waiter.Set();
                }
            }

            if (!waiter.WaitOne(20000))
            {
                throw new TimeoutException("Didn't get directory structure in time");
            }
        }
    }
}