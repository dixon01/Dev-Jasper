// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpFileTransferSessionProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpFileTransferSessionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Ftp
{
    using System;
    using System.Net;

    /// <summary>
    /// Implementation of <see cref="FileTransferSessionProvider"/> that supports
    /// the FTP file system (ftp:// scheme).
    /// </summary>
    internal partial class FtpFileTransferSessionProvider : FileTransferSessionProvider
    {
        private readonly Uri baseUrl;
        private readonly NetworkCredential credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpFileTransferSessionProvider"/> class.
        /// This constructor should never be called directly, rather use
        /// <see cref="FileTransferSessionProvider.Create"/> instead.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        internal FtpFileTransferSessionProvider(Uri baseUrl, string username, string password)
        {
            this.baseUrl = baseUrl;
            this.credentials = new NetworkCredential(username, password);
        }

        /// <summary>
        /// Opens a new session to the file system defined when calling
        /// <see cref="FileTransferSessionProvider.Create"/>. A session is not thread safe and should only
        /// be accessed from a single thread at the time.
        /// </summary>
        /// <returns>
        /// A new <see cref="IFileTransferSession"/> implementation.
        /// </returns>
        public override IFileTransferSession OpenSession()
        {
            return new FtpFileTransferSession(this.baseUrl, this.credentials);
        }
    }
}