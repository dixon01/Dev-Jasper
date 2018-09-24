// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTransferSessionProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileTransferSessionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;

    using Gorba.Common.Medi.Core.Transport.File.Local;

    /// <summary>
    /// Base class for different implementations of file system access.
    /// </summary>
    internal abstract partial class FileTransferSessionProvider
    {
        /// <summary>
        /// Factory method to create instances of <see cref="FileTransferSessionProvider"/>.
        /// Creates an instance depending on the <see cref="Uri.Scheme"/> of the
        /// <see cref="baseUrl"/>.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url for which the provider should be created.
        /// </param>
        /// <param name="username">
        /// The user name used to authenticate when creating the session.
        /// </param>
        /// <param name="password">
        /// The password used to authenticate when creating the session.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="FileTransferSessionProvider"/> that
        /// can be used to create <see cref="IFileTransferSession"/>s.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// if the scheme of the <see cref="baseUrl"/> is not supported.
        /// </exception>
        public static FileTransferSessionProvider Create(Uri baseUrl, string username, string password)
        {
            switch (baseUrl.Scheme)
            {
                case "file":
                    return new LocalFileTransferSessionProvider(baseUrl);
                case "ftp":
                    return CreateFtpFileTransferSessionProvider(baseUrl, username, password);
                default:
                    throw new NotSupportedException("Unsupported file protocol: " + baseUrl.Scheme);
            }
        }

        /// <summary>
        /// Opens a new session to the file system defined when calling
        /// <see cref="Create"/>. A session is not thread safe and should only
        /// be accessed from a single thread at the time.
        /// </summary>
        /// <returns>
        /// A new <see cref="IFileTransferSession"/> implementation.
        /// </returns>
        public abstract IFileTransferSession OpenSession();
    }
}
