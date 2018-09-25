// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.FileSystem;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.IO;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// File system that uses Medi to access the file system on a remote unit.
    /// </summary>
    public class RemoteFileSystem : IFileSystem, IDisposable
    {
        private readonly MediAddress address;

        private readonly IRootMessageDispatcher messageDispatcher;

        private readonly IRemoteManagementProvider managementRoot;

        private IRemoteManagementProvider managementProvider;

        private IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileSystem"/> class.
        /// </summary>
        /// <param name="address">
        /// The address of the application to which the queries will be sent.
        /// </param>
        public RemoteFileSystem(MediAddress address)
            : this(address, MessageDispatcher.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteFileSystem"/> class.
        /// </summary>
        /// <param name="address">
        /// The address of the application to which the queries will be sent.
        /// </param>
        /// <param name="messageDispatcher">
        /// The message dispatcher through which the queries will be sent.
        /// </param>
        public RemoteFileSystem(MediAddress address, IRootMessageDispatcher messageDispatcher)
        {
            this.address = address;
            this.messageDispatcher = messageDispatcher;
            this.managementRoot = this.messageDispatcher.ManagementProviderFactory.CreateRemoteProvider(address);
        }

        /// <summary>
        /// Gets the management provider responsible for the file system.
        /// </summary>
        internal IRemoteManagementProvider ManagementProvider
        {
            get
            {
                return this.managementProvider
                       ?? (this.managementProvider =
                           (IRemoteManagementProvider)
                           this.managementRoot.GetChild(FileSystemManagementProvider.RootName));
            }
        }

        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        public IDriveInfo[] GetDrives()
        {
            var drives = new List<IDriveInfo>();
            foreach (IRemoteManagementProvider child in this.ManagementProvider.Children)
            {
                if (!child.Name.Contains(":"))
                {
                    // we are in a single root system (e.g. Windows CE)
                    return new IDriveInfo[] { new DriveInfoProvider("\\", this.ManagementProvider, this) };
                }

                drives.Add(new DriveInfoProvider(child.Name, child, this));
            }

            return drives.ToArray();
        }

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
            IFileInfo file;
            if (!this.TryGetFile(path, out file))
            {
                throw new FileNotFoundException(string.Format("Couldn't find {0} on {1}", path, this.address));
            }

            return file;
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
            var parts = path.Trim('\\').Split('\\');
            var provider = this.ManagementProvider.GetDescendant(parts) as IRemoteManagementProvider;
            if (provider == null || !this.IsFile(provider))
            {
                file = null;
                return false;
            }

            file = new FileInfoProvider(provider, this);
            return true;
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
            IDirectoryInfo directory;
            if (!this.TryGetDirectory(path, out directory))
            {
                throw new DirectoryNotFoundException(string.Format("Couldn't find {0} on {1}", path, this.address));
            }

            return directory;
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
            var parts = path.TrimEnd('\\').Split('\\');
            parts[0] += '\\';
            var provider = this.ManagementProvider.GetDescendant(parts) as IRemoteManagementProvider;
            if (provider == null || !this.IsDirectory(provider))
            {
                directory = null;
                return false;
            }

            directory = new DirectoryInfoProvider(provider, this);
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.managementRoot.Dispose();
            if (this.managementProvider != null)
            {
                this.managementProvider.Dispose();
            }
        }

        /// <summary>
        /// Begins to download asynchronously a file from the remote unit.
        /// This method is a convenience method, to simplify downloading instead of using
        /// <see cref="IFileInfo.OpenRead"/> and then manually saving the contents to a file.
        /// </summary>
        /// <param name="file">
        /// The remote file. This must be an <see cref="IFileInfo"/> returned by this file system.
        /// </param>
        /// <param name="localPath">
        /// The local path where the file should be downloaded to.
        /// The given file must not exist, otherwise the download will fail.
        /// </param>
        /// <param name="callback">
        /// The async callback.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndDownloadFile"/>.
        /// </returns>
        public IAsyncResult BeginDownloadFile(IFileInfo file, string localPath, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<int>(callback, state);
            if (File.Exists(localPath))
            {
                throw new IOException("File already exists: " + localPath);
            }

            this.RequestFile(
                file,
                e =>
                {
                    if (result.IsCompleted)
                    {
                        return;
                    }

                    try
                    {
                        e.CopyTo(localPath);
                        result.Complete(0, false);
                    }
                    catch (Exception ex)
                    {
                        result.TryCompleteException(ex, false);
                    }
                });
            return result;
        }

        /// <summary>
        /// Ends the asynchronous download of a file.
        /// </summary>
        /// <param name="ar">
        /// The async result returned by <see cref="BeginDownloadFile"/>.
        /// </param>
        public void EndDownloadFile(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<int>;
            if (result == null)
            {
                throw new ArgumentException(
                    "Call EndDownloadFile() method with result provided by BeginDownloadFile()");
            }

            result.WaitForCompletionAndVerify();
        }

        /// <summary>
        /// Begins to open a remote file asynchronously.
        /// </summary>
        /// <param name="file">
        /// The file to open.
        /// </param>
        /// <param name="callback">
        /// The async callback.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndOpenFile"/>.
        /// </returns>
        internal IAsyncResult BeginOpenFile(IFileInfo file, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<Stream>(callback, state);
            this.RequestFile(
                file,
                e =>
                    {
                        if (result.IsCompleted)
                        {
                            return;
                        }

                        try
                        {
                            var tempFile = Path.GetTempFileName();
                            File.Delete(tempFile);
                            e.CopyTo(tempFile);
                            result.Complete(new TempFileStream(tempFile), false);
                        }
                        catch (Exception ex)
                        {
                            result.TryCompleteException(ex, false);
                        }
                    });
            return result;
        }

        /// <summary>
        /// Ends the asynchronous opening of a file.
        /// </summary>
        /// <param name="ar">
        /// The async result returned from <see cref="BeginOpenFile"/>.
        /// </param>
        /// <returns>
        /// A stream to read from.
        /// </returns>
        internal Stream EndOpenFile(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<Stream>;
            if (result == null)
            {
                throw new ArgumentException("Call EndOpenFile() method with result provided by BeginOpenFile()");
            }

            return result.Value;
        }

        /// <summary>
        /// Checks if the given provider depicts a file.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// True if the given provider is representing a file on the remote system.
        /// </returns>
        internal bool IsFile(IRemoteManagementProvider provider)
        {
            var type = this.GetProperty(provider, FileSystemManagementProvider.TypePropertyName);
            return type.StringValue == FileSystemManagementProvider.TypeValueFile;
        }

        /// <summary>
        /// Checks if the given provider depicts a directory.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// True if the given provider is representing a directory on the remote system.
        /// </returns>
        internal bool IsDirectory(IRemoteManagementProvider provider)
        {
            var type = this.GetProperty(provider, FileSystemManagementProvider.TypePropertyName);
            return type.StringValue == FileSystemManagementProvider.TypeValueDirectory;
        }

        /// <summary>
        /// Gets a management property of the given provider with the given name.
        /// </summary>
        /// <param name="provider">
        /// The provider from which we want to get the property.
        /// </param>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// The <see cref="ManagementProperty"/> if found, otherwise null.
        /// </returns>
        internal ManagementProperty GetProperty(IRemoteManagementProvider provider, string name)
        {
            var obj = provider as IManagementObjectProvider;
            return obj == null ? null : obj.GetProperty(name);
        }

        private void RequestFile(IFileInfo file, Action<FileReceivedEventArgs> callback)
        {
            if (file.FileSystem != this)
            {
                throw new ArgumentException("File must be a from this file system");
            }

            if (this.resourceService == null)
            {
                this.resourceService = this.messageDispatcher.GetService<IResourceService>();
                if (this.resourceService == null)
                {
                    throw new NotSupportedException("Can't download files if resource service was not configured");
                }
            }

            this.resourceService.FileReceived += (s, e) =>
                {
                    if (!e.OriginalFileName.Equals(file.FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }

                    callback(e);
                };
            this.resourceService.RequestFile(file.FullName, this.address);
        }

        private class TempFileStream : WrapperStream
        {
            private readonly string tempFile;

            public TempFileStream(string tempFile)
            {
                this.tempFile = tempFile;
                this.Open(File.OpenRead(tempFile));
            }

            public override void Close()
            {
                base.Close();
                File.Delete(this.tempFile);
            }
        }
    }
}
