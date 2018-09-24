// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpHandler.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Protocols.Ftp;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.IO;

    using NLog;

    /// <summary>
    /// Handler for FTP protocol using <see cref="WebRequest"/>.
    /// </summary>
    public partial class FtpHandler : IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FtpHandler>();

        private static readonly Regex PathReplaceRegex = new Regex("//+");

        private readonly IFtpUpdateConfig config;

        private readonly List<FtpClientUsage> clientUsages = new List<FtpClientUsage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public FtpHandler(IFtpUpdateConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Delete the given file on the FTP server.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public void DeleteFile(string path)
        {
            var fullPath = this.CreateFullPath(path);
            using (var usage = this.GetFtpClientUsage())
            {
                usage.Client.RemoveFile(fullPath);
            }
        }

        /// <summary>
        /// Create the given directory on the FTP server if it doesn't exist yet.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="recursive">
        /// A flag indicating if parent directories should also be created.
        /// </param>
        public void CreateDirectory(string path, bool recursive)
        {
            if (this.DirectoryExists(path))
            {
                // directory already exists
                return;
            }

            if (recursive)
            {
                var parent = Path.GetDirectoryName(path);
                if (parent != null)
                {
                    parent = Path.GetDirectoryName(parent);
                    if (parent != null)
                    {
                        this.CreateDirectory(parent, true);
                    }
                }
            }

            var fullPath = this.CreateFullPath(path);
            using (var usage = this.GetFtpClientUsage())
            {
                usage.Client.MakeDir(fullPath);
            }
        }

        /// <summary>
        /// Checks if the given file exists on the FTP server.
        /// Returns false if the path points to a directory.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool FileExists(string path)
        {
            var fullPath = this.CreateFullPath(path);
            try
            {
                using (var usage = this.GetFtpClientUsage())
                {
                    return usage.Client.GetFileSize(fullPath) >= 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given directory exists on the FTP server.
        /// Returns false if the path points to a file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DirectoryExists(string path)
        {
            var fullPath = this.CreateFullPath(path);
            try
            {
                using (var usage = this.GetFtpClientUsage())
                {
                    usage.Client.ChangeDir(fullPath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the size of a file on the remote server.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <returns>
        /// The size of the file in bytes.
        /// </returns>
        public long GetFileSize(string path)
        {
            var fullPath = this.CreateFullPath(path);
            using (var usage = this.GetFtpClientUsage())
            {
                return usage.Client.GetFileSize(fullPath);
            }
        }

        /// <summary>
        /// Gets all files and folders in the given directory on the FTP server.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// All entries with their FULL path.
        /// </returns>
        public IList<string> GetEntries(string path)
        {
            var fullPath = this.CreateFullPath(path);
            using (var usage = this.GetFtpClientUsage())
            {
                usage.Client.ChangeDir(fullPath);
                var files = usage.Client.ListNames();
                var entries = new List<string>();
                foreach (string file in files)
                {
                    if (file.IndexOfAny(new[] { '\\', '/' }) < 0)
                    {
                        entries.Add(this.CreateFullPath(path + "/" + file));
                    }
                    else
                    {
                        entries.Add(file);
                    }
                }

                return entries;
            }
        }

        /// <summary>
        /// Moves the given name on the server to the new path.
        /// </summary>
        /// <param name="originalPath">
        /// The original path of the file that exists on the server.
        /// </param>
        /// <param name="newPath">
        /// The new path on the server.
        /// </param>
        public void MoveFile(string originalPath, string newPath)
        {
            using (var usage = this.GetFtpClientUsage())
            {
                usage.Client.RenameFile(this.CreateFullPath(originalPath), this.CreateFullPath(newPath));
            }
        }

        /// <summary>
        /// Downloads a file from the FTP server to a local file.
        /// </summary>
        /// <param name="fileName">
        /// The file name on the FTP server.
        /// </param>
        /// <param name="localFile">
        /// The full path to the local file where to download the file.
        /// </param>
        /// <param name="size">
        /// The size of the file to be downloaded (<seealso cref="GetFileSize"/>)
        /// </param>
        /// <param name="progressMonitor">
        /// The monitor used for reporting the download progress.
        /// </param>
        public void DownloadFile(string fileName, string localFile, long size, IPartProgressMonitor progressMonitor)
        {
            this.DownloadFile(fileName, localFile, size, false, progressMonitor);
        }

        /// <summary>
        /// Continues the download of a file from the FTP server to a local file.
        /// </summary>
        /// <param name="fileName">
        /// The file name on the FTP server.
        /// </param>
        /// <param name="localFile">
        /// The full path to the local file where to download the file.
        /// </param>
        /// <param name="size">
        /// The size of the file to be downloaded (<seealso cref="GetFileSize"/>)
        /// </param>
        /// <param name="progressMonitor">
        /// The monitor used for reporting the download progress.
        /// </param>
        public void ContinueDownloadFile(
            string fileName, string localFile, long size, IPartProgressMonitor progressMonitor)
        {
            this.DownloadFile(fileName, localFile, size, true, progressMonitor);
        }

        /// <summary>
        /// Uploads the given stream to the given path on the FTP server.
        /// If the file already exists on the server, the content of the stream is appended
        /// (starting from the <see cref="stream"/>'s offset set to the current length of the
        /// file on the server).
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="path">
        /// The file path on the server.
        /// </param>
        /// <param name="size">
        /// The size of the file to be uploaded.
        /// </param>
        /// <param name="progressMonitor">
        /// The monitor used for reporting the upload progress.
        /// </param>
        public void UploadStream(Stream stream, string path, long size, IPartProgressMonitor progressMonitor)
        {
            var fileStream = stream as FileStream;
            var tempStream = stream as TemporaryFileStream;
            string localFile;
            if (fileStream != null)
            {
                localFile = fileStream.Name;
            }
            else if (tempStream != null)
            {
                localFile = tempStream.Name;
            }
            else
            {
                localFile = Path.GetTempFileName();
                using (var output = File.Create(localFile))
                {
                    StreamCopy.Copy(stream, output);
                }
            }

            try
            {
                var fullPath = this.CreateFullPath(path);
                this.UploadFile(localFile, fullPath, size, progressMonitor);
            }
            finally
            {
                if (fileStream == null && tempStream == null)
                {
                    File.Delete(localFile);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (this.clientUsages)
            {
                foreach (var usage in this.clientUsages)
                {
                    usage.Destroy();
                }

                this.clientUsages.Clear();
            }
        }

        private string CreateFullPath(string path)
        {
            var fullPath = "/" + path;
            fullPath = fullPath.Replace('\\', '/');
            fullPath = PathReplaceRegex.Replace(fullPath, "/");
            Logger.Trace("Creating FTP request for {0}", fullPath);
            return fullPath;
        }

        private void DownloadFile(
            string fileName, string localFile, long size, bool resume, IPartProgressMonitor progressMonitor)
        {
            var localDir = Path.GetDirectoryName(localFile);
            Directory.CreateDirectory(localDir);

            var fullPath = this.CreateFullPath(fileName);
            using (var usage = this.GetFtpClientUsage())
            {
                if (!usage.Client.OpenDownload(fullPath, localFile, resume))
                {
                    throw new IOException("Couldn't find file " + fullPath);
                }

                var localInfo = new FileInfo(localFile);
                if (resume && localInfo.Exists)
                {
                    size -= localInfo.Length;
                }

                try
                {
                    long read;
                    var total = 0L;
                    while ((read = usage.Client.DoDownload()) > 0)
                    {
                        if (progressMonitor.IsCancelled)
                        {
                            throw new IOException("Download was cancelled");
                        }

                        total += read;
                        progressMonitor.Progress(1.0 * total / size, string.Format("{0} of {1} bytes", total, size));
                    }
                }
                finally
                {
                    usage.Client.CloseTransfer();
                }
            }
        }

        private void UploadFile(string localFile, string remotePath, long size, IPartProgressMonitor progressMonitor)
        {
            using (var usage = this.GetFtpClientUsage())
            {
                var remoteSize = -1L;
                try
                {
                    remoteSize = usage.Client.GetFileSize(remotePath);
                }
                catch (Exception ex)
                {
                    Logger.TraceException(
                        "Couldn't get remote file size, assuming file doesn't exist: " + remotePath, ex);
                }

                if (size == remoteSize)
                {
                    Logger.Debug("File is already completely on server");
                    return;
                }

                if (size < remoteSize)
                {
                    Logger.Warn(
                        "File has wrong size on server: expected max {0}, got {1}; re-uploading", size, remoteSize);
                    usage.Client.RemoveFile(remotePath);
                    remoteSize = -1;
                }

                if (remoteSize > 0)
                {
                    size -= remoteSize;
                }

                usage.Client.OpenUpload(localFile, remotePath, remoteSize > 0);

                try
                {
                    long written;
                    var total = 0L;
                    while ((written = usage.Client.DoUpload()) > 0)
                    {
                        if (progressMonitor.IsCancelled)
                        {
                            throw new IOException("Upload was cancelled");
                        }

                        total += written;
                        progressMonitor.Progress(1.0 * total / size, string.Format("{0} of {1} bytes", total, size));
                    }
                }
                finally
                {
                    usage.Client.CloseTransfer();
                }
            }
        }

        private FtpClientUsage GetFtpClientUsage()
        {
            FtpClientUsage usage = null;
            lock (this.clientUsages)
            {
                foreach (var client in this.clientUsages)
                {
                    if (!client.Used)
                    {
                        usage = client;
                        break;
                    }
                }

                if (usage == null)
                {
                    usage = new FtpClientUsage(this.config);
                    this.clientUsages.Add(usage);
                    Logger.Trace("Created FtpClientUsage, now at {0}", this.clientUsages.Count);
                }

                usage.Use();
            }

            try
            {
                if (!usage.Client.IsConnected)
                {
                    Logger.Debug(
                        "Connecting to {0}@{1}:{2}", this.config.Username, this.config.Host, this.config.Port);
                    usage.Client.Connect();
                }
            }
            catch
            {
                usage.Release();
                throw;
            }

            return usage;
        }

        private class FtpClientUsage : IDisposable
        {
            public FtpClientUsage(IFtpUpdateConfig config)
            {
                this.Client = new FtpClient(config.Host, config.Port, config.Username, config.Password);
            }

            public FtpClient Client { get; private set; }

            public bool Used { get; private set; }

            public void Use()
            {
                this.Used = true;
            }

            public void Release()
            {
                this.Used = false;
            }

            public void Destroy()
            {
                this.Client.Disconnect();
            }

            /// <summary>
            /// Warning: this is not a proper disposal of the underlying client but it's just
            /// marking this object again as unused.
            /// </summary>
            void IDisposable.Dispose()
            {
                this.Release();
            }
        }
    }
}
