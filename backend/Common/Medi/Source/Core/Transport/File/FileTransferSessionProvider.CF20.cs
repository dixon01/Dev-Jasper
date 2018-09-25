// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTransferSessionProvider.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileTransferSessionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;

    /// <summary>
    /// Base class for different implementations of file system access.
    /// </summary>
    internal abstract partial class FileTransferSessionProvider
    {
        private static FileTransferSessionProvider CreateFtpFileTransferSessionProvider(
            Uri baseUrl, string username, string password)
        {
            throw new NotSupportedException("Unsupported file protocol: ftp");
        }
    }
}
