// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpHandler.FX20.cs" company="Gorba AG">
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
    using System.Reflection;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Update.ServiceModel.Utility;

    using NLog;

    /// <summary>
    /// The FtpHandler interface.
    /// </summary>
    public interface IFtpHandler
    {
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
        bool FileExists(string path);

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
        bool DirectoryExists(string path);

        /// <summary>
        /// Gets all files and folders in the given directory on the FTP server.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// All entries with their FULL path.
        /// </returns>
        IList<string> GetEntries(string path);

        /// <summary>
        /// Gets the size of a file on the remote server.
        /// </summary>
        /// <param name="path">
        /// The path to the file.
        /// </param>
        /// <returns>
        /// The size of the file in bytes.
        /// </returns>
        long GetFileSize(string path);

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
        /// The size of the file to be downloaded (<seealso cref="FtpHandler.GetFileSize"/>)
        /// </param>
        /// <param name="progressMonitor">
        /// The monitor used for reporting the download progress.
        /// </param>
        void DownloadFile(string fileName, string localFile, long size, IPartProgressMonitor progressMonitor);
    }
    
    /// <summary>
    /// Handler for FTP protocol using <see cref="WebRequest"/>.
    /// </summary>
    public partial class FtpHandler : IDisposable, IFtpHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFtpUpdateConfig config;

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
            var request = this.CreateRequest(path);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            this.HandleResponse(request);
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
            Logger.Info(MethodBase.GetCurrentMethod().Name + " --------------- Enter: path=" + path );
            path = this.CreateDirectoryPath(path);
           
            if (this.DirectoryExists(path))
            {
                Logger.Info(MethodBase.GetCurrentMethod().Name + " Exit as Directory already exists");
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
           
            var request = this.CreateRequest(path);
           
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
          
            this.HandleResponse(request);

            Logger.Info(MethodBase.GetCurrentMethod().Name + " ---------------Exit ");
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
            try
            {
                this.GetFileSize(path);
                return true;
            }
            catch (WebException)
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
            path = this.CreateDirectoryPath(path);
            var request = this.CreateRequest(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            try
            {
                this.HandleResponse(request);
                return true;
            }
            catch (WebException)
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
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Enter: path = " + path);
            var request = this.CreateRequest(path);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            var response = this.GetResponse(request);
            var size = response.ContentLength;
            response.Close();
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Exit: size = " + size);
            return size;
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
            path = this.CreateDirectoryPath(path);
            var request = this.CreateRequest(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            using (var response = this.GetResponse(request))
            {
                var stream = response.GetResponseStream();
                if (stream == null)
                {
                    throw new WebException("Couldn't get response stream");
                }

                var lines = new List<string>();
                using (var reader = new StreamReader(stream))
                {
                    string entry;
                    while ((entry = reader.ReadLine()) != null)
                    {
                        if (!entry.Contains("\\") && !entry.Contains("/"))
                        {
                            entry = Path.Combine(path, entry);
                        }

                        lines.Add(entry);
                    }
                }

                return lines;
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
            var request = this.CreateRequest(originalPath);
            request.RenameTo = NormalizePath(newPath);
            request.Method = WebRequestMethods.Ftp.Rename;
            this.HandleResponse(request);
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
            var request = this.CreateRequest(fileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            var dir = Path.GetDirectoryName(localFile);
            if (dir != null)
            {
                Directory.CreateDirectory(dir);
            }

            using (var response = this.GetResponse(request))
            {
                using (var input = response.GetResponseStream())
                {
                    if (input == null)
                    {
                        throw new WebException("Couldn't get response stream");
                    }

                    using (var output = File.Create(localFile))
                    {
                        this.CopyStream(input, output, size, progressMonitor);
                    }
                }
            }
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
            var localInfo = new FileInfo(localFile);
            if (!localInfo.Exists || localInfo.Length == 0)
            {
                this.DownloadFile(fileName, localFile, size, progressMonitor);
                return;
            }

            var available = localInfo.Length;
            var request = this.CreateRequest(fileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.ContentOffset = available;
            var dir = Path.GetDirectoryName(localFile);
            if (dir != null)
            {
                Directory.CreateDirectory(dir);
            }

            using (var output = File.OpenWrite(localFile))
            {
                output.Seek(0, SeekOrigin.End);
                using (var response = this.GetResponse(request))
                {
                    using (var input = response.GetResponseStream())
                    {
                        if (input == null)
                        {
                            throw new WebException("Couldn't get response stream");
                        }

                        this.CopyStream(input, output, size - available, progressMonitor);
                    }
                }
            }
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
            Logger.Trace(MethodBase.GetCurrentMethod().Name + " Enter");
            long remoteSize = -1;
            try
            {
                remoteSize = this.GetFileSize(path);
                Logger.Trace(MethodBase.GetCurrentMethod().Name + " FileSize to be uploaded = " + remoteSize);
            }
            catch (WebException ex)
            {
                Logger.Trace(MethodBase.GetCurrentMethod().Name + ex.Message + ex.InnerException?.Message);
                var response = ex.Response as FtpWebResponse;
                if (response == null || response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    throw;
                }
            }

            if (remoteSize > 0)
            {
                if (size == remoteSize)
                {
                    Logger.Debug("File is already completely on server");
                    return;
                }

                if (size < remoteSize)
                {
                    Logger.Warn(
                        "File has wrong size on server: expected max {0}, got {1}; re-uploading", size, remoteSize);
                    this.DeleteFile(path);
                    remoteSize = -1;
                }
                else
                {
                    stream.Seek(remoteSize, SeekOrigin.Begin);
                    size -= remoteSize;
                }
            }

            var request = this.CreateRequest(path);
            request.Method = remoteSize >= 0 ? WebRequestMethods.Ftp.AppendFile : WebRequestMethods.Ftp.UploadFile;
            request.ContentLength = size;

            using (var requestStream = request.GetRequestStream())
            {
                this.CopyStream(stream, requestStream, size, progressMonitor);
                stream.Close();
            }

            this.HandleResponse(request);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // nothing to do here
        }

        public override string ToString()
        {
            if (this.config != null)
            {
                return string.Format(
               "FTP Host={0}:{1}, RepositoryPath={2} User={3}, Password={4}",
               this.config.Host,
               this.config.Port,
               this.config.RepositoryBasePath,
               this.config.Username,
               this.config.Password);
            }
            return base.ToString();
        }

        private static string NormalizePath(string path)
        {
            path = path.Replace('\\', '/');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            return path;
        }

        private string CreateDirectoryPath(string path)
        {
            if (!path.EndsWith("\\") && !path.EndsWith("/"))
            {
                path += "/";
            }

            return path;
        }

        private Uri CreateUri(string path)
        {
            path = NormalizePath(path);
            return new Uri(string.Format("ftp://{0}:{1}/%2f{2}", this.config.Host, this.config.Port, path));
        }

        private FtpWebRequest CreateRequest(string path)
        {
            var uri = this.CreateUri(path);
            Logger.Trace("Creating FTP request for {0}", uri);
            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(this.config.Username, this.config.Password);
            return request;
        }

        private FtpWebResponse GetResponse(FtpWebRequest request)
        {
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Enter");
            Logger.Trace("Requesting {0} for {1} ",request.Method,request.RequestUri.AbsolutePath);
            FtpWebResponse response = null;
            try
            {
                 response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception exception)
            {
                Logger.Trace(exception.Message + exception.InnerException?.Message);
                Logger.Trace(
                   "Request {0} for {1} returned {2}: {3}",
                   request.Method,
                   request.RequestUri.AbsolutePath,
                   response?.StatusCode,
                   response?.StatusDescription);
                   response?.Close();
                throw exception;
                
            }
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Exit");
            return response;
        }

        private void HandleResponse(FtpWebRequest request)
        {
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Enter");
            var response = this.GetResponse(request);
            response?.Close();
            Logger.Info(MethodBase.GetCurrentMethod().Name + " Exit");
        }

        private void CopyStream(Stream input, Stream output, long size, IPartProgressMonitor progressMonitor)
        {
            var buffer = new byte[4096];
            long totalRead = 0;
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (progressMonitor != null && progressMonitor.IsCancelled)
                {
                    throw new IOException("Copy of stream was cancelled");
                }

                output.Write(buffer, 0, read);
                totalRead += read;
                if (progressMonitor != null)
                {
                    progressMonitor.Progress(1.0 * totalRead / size, string.Format("{0} of {1} bytes", totalRead, size));
                }
            }
        }
    }
}
