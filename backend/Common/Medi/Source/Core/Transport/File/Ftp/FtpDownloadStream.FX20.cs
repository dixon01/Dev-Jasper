// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpDownloadStream.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpDownloadStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Ftp
{
    using System;
    using System.Net;

    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Stream used to download a file using <see cref="FtpWebRequest"/>.
    /// This provides also the <see cref="Length"/> property and makes sure the
    /// request request and response are handled correctly when the stream is closed.
    /// </summary>
    internal partial class FtpDownloadStream : WrapperStream
    {
        private readonly long length;

        private readonly WebResponse response;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpDownloadStream"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        public FtpDownloadStream(Uri url, ICredentials credentials)
        {
            var req = (FtpWebRequest)WebRequest.Create(url);
            req.Proxy = null;
            req.Credentials = credentials;
            req.Method = WebRequestMethods.Ftp.GetFileSize;

            using (var resp = req.GetResponse())
            {
                this.length = resp.ContentLength;
            }

            req = (FtpWebRequest)WebRequest.Create(url);
            req.Proxy = null;
            req.Credentials = credentials;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            this.response = req.GetResponse();
            this.Open(this.response.GetResponseStream());
        }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles)
        /// associated with the current stream.
        /// </summary>
        public override void Close()
        {
            base.Close();
            this.response.Close();
        }
    }
}