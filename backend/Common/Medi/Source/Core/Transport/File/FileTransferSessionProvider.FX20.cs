// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTransferSessionProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileTransferSessionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;

    using Gorba.Common.Medi.Core.Transport.File.Ftp;

    /// <summary>
    /// Base class for different implementations of file system access.
    /// </summary>
    internal abstract partial class FileTransferSessionProvider
    {
        private static FileTransferSessionProvider CreateFtpFileTransferSessionProvider(
            Uri baseUrl, string username, string password)
        {
            return new FtpFileTransferSessionProvider(baseUrl, username, password);
        }
    }
}
