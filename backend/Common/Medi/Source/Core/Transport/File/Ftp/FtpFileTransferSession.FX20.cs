// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpFileTransferSession.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpFileTransferSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Ftp
{
    using System;
    using System.IO;
    using System.Net;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IFileTransferSession"/> to access an FTP server.
    /// </summary>
    internal partial class FtpFileTransferSession : IFileTransferSession
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FtpFileTransferSession>();

        private readonly Uri baseUrl;
        private readonly NetworkCredential credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpFileTransferSession"/> class.
        /// This constructor should never be called directly, rather use
        /// <see cref="FileTransferSessionProvider.OpenSession"/> instead.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        internal FtpFileTransferSession(Uri baseUrl, NetworkCredential credentials)
        {
            this.baseUrl = baseUrl;
            this.credentials = credentials;

            Logger.Debug("Created new transfer session for {0}", baseUrl);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Lists all files in the working directory.
        /// </summary>
        /// <returns>
        /// A list of file names that can later be used
        /// to call any of the following methods. The file names are
        /// in relative, local format and don't contain directory information.
        /// </returns>
        public string[] ListFiles()
        {
            Logger.Trace("Listing file inside {0}", this.baseUrl);
            var req = this.CreateWebRequest(this.baseUrl, WebRequestMethods.Ftp.ListDirectory);

            var response = req.GetResponse();
            string[] files;
            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw new IOException("No response stream!");
                    }

                    var reader = new StreamReader(responseStream);
                    var listing = reader.ReadToEnd();
                    files = listing.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            finally
            {
                response.Close();
            }

            return files;
        }

        /// <summary>
        /// Opens the given file for writing, creating it if it doesn't exist yet.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// A stream that can be used to write to the file.
        /// <see cref="Stream.Close"/> should be called before calling any other
        /// method on this session.
        /// </returns>
        public Stream OpenWrite(string fileName)
        {
            var url = new Uri(this.baseUrl, fileName);
            Logger.Trace("Opening {0} for writing", url);
            return new FtpUploadStream(url, this.credentials);
        }

        /// <summary>
        /// Opens the given file for reading.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// A stream that can be used to read from the file.
        /// <see cref="Stream.Close"/> should be called before calling any other
        /// method on this session.
        /// </returns>
        public Stream OpenRead(string fileName)
        {
            var url = new Uri(this.baseUrl, fileName);
            Logger.Trace("Opening {0} for reading", url);
            return new FtpDownloadStream(url, this.credentials);
        }

        /// <summary>
        /// Renames the file from the original name to the new one.
        /// </summary>
        /// <param name="origFileName">
        /// The original file name.
        /// </param>
        /// <param name="newFileName">
        /// The new file name.
        /// </param>
        public void Rename(string origFileName, string newFileName)
        {
            Logger.Trace("Renaming {0} to {1}", origFileName, newFileName);
            var req = this.CreateWebRequest(origFileName, WebRequestMethods.Ftp.Rename);
            req.RenameTo = newFileName;

            req.GetResponse().Close();
        }

        /// <summary>
        /// Deletes the given file from the file system.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void DeleteFile(string fileName)
        {
            Logger.Trace("Deleting {0}", fileName);
            var req = this.CreateWebRequest(fileName, WebRequestMethods.Ftp.DeleteFile);

            req.GetResponse().Close();
        }

        private FtpWebRequest CreateWebRequest(string fileName, string method)
        {
            var url = new Uri(this.baseUrl, fileName);
            return this.CreateWebRequest(url, method);
        }

        private FtpWebRequest CreateWebRequest(Uri url, string method)
        {
            var req = (FtpWebRequest)WebRequest.Create(url);
            req.Credentials = this.credentials;
            req.Method = method;
            return req;
        }
    }
}