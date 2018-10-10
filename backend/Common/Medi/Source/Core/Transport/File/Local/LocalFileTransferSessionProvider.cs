// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileTransferSessionProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileTransferSessionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Local
{
    using System;

    /// <summary>
    /// Implementation of <see cref="FileTransferSessionProvider"/> that supports
    /// the local file system (file:// scheme).
    /// </summary>
    internal class LocalFileTransferSessionProvider : FileTransferSessionProvider
    {
        private readonly Uri baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileTransferSessionProvider"/> class.
        /// This constructor should never be called directly, rather use
        /// <see cref="FileTransferSessionProvider.Create"/> instead.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        internal LocalFileTransferSessionProvider(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        /// <summary>
        /// Opens a new session to the local file system. A session is not thread safe and should only
        /// be accessed from a single thread at the time.
        /// </summary>
        /// <returns>
        /// A new <see cref="LocalFileTransferSession"/> instance.
        /// </returns>
        public override IFileTransferSession OpenSession()
        {
            return new LocalFileTransferSession(this.baseUrl);
        }
    }
}