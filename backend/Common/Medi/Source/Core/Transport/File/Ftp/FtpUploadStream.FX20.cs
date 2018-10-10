// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpUploadStream.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpUploadStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Ftp
{
    using System;
    using System.Net;

    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Stream used to upload a file using <see cref="FtpWebRequest"/>.
    /// This makes sure the request and response are handled correctly when the stream is closed.
    /// </summary>
    internal partial class FtpUploadStream : WrapperStream
    {
        private readonly FtpWebRequest request;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpUploadStream"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        public FtpUploadStream(Uri url, ICredentials credentials)
        {
            this.request = (FtpWebRequest)WebRequest.Create(url);
            this.request.Proxy = null;
            this.request.Credentials = credentials;
            this.request.Method = WebRequestMethods.Ftp.UploadFile;
            this.Open(this.request.GetRequestStream());
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles)
        /// associated with the current stream.
        /// </summary>
        public override void Close()
        {
            base.Close();
            var response = this.request.GetResponse();
            response.Close();
        }
    }
}