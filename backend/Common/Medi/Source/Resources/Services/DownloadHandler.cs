// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DownloadHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// The download handler implementation.
    /// </summary>
    internal class DownloadHandler : IDownloadHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<DownloadHandler>();

        private readonly ResourceId id;

        private readonly ResourceServiceBase service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadHandler"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        public DownloadHandler(ResourceId id, ResourceServiceBase service)
        {
            this.id = id;
            this.service = service;
        }

        /// <summary>
        /// Downloads the contents of the message synchronously.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="session">
        /// The session from which the message came.
        /// </param>
        public void Download(StreamMessage message, ITransportSession session)
        {
            Logger.Debug("Downloading resource {0} from {1}", this.id, session.SessionId);

            // TODO: the base directory should be the one from the remote service if we are remotely
            var fileName = string.Format("{0}.trx", this.id.Hash);
            var path = Path.Combine(this.service.BaseDirectory.FullName, fileName);

            this.service.SetDownloadState(this.id, message.Header.Offset, path);

            var file = this.OpenFile(path, message.Header.Offset);

            try
            {
                var md5 = new MD5CryptoServiceProvider();
                md5.Initialize();
                using (var output = message.Header.Offset > 0 ? file.OpenAppend() : file.OpenWrite())
                {
                    var buffer = new byte[4096];
                    int read;

                    if (message.Header.Offset > 0)
                    {
                        output.Seek(0, SeekOrigin.Begin);
                        var totalRead = 0;

                        while (totalRead < message.Header.Offset
                            && (read = output.Read(
                                buffer, 0, (int)Math.Min(buffer.Length, message.Header.Offset - totalRead))) > 0)
                        {
                            totalRead += read;
                            md5.TransformBlock(buffer, 0, read, buffer, 0);
                        }
                    }

                    using (var input = message.OpenRead())
                    {
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            md5.TransformBlock(buffer, 0, read, buffer, 0);
                            output.Write(buffer, 0, read);
                        }

                        md5.TransformFinalBlock(buffer, 0, 0);
                    }
                }

                var receivedId = ResourceServiceBase.HashToId(md5.Hash);
                if (!receivedId.Hash.Equals(this.id.Hash, StringComparison.InvariantCultureIgnoreCase))
                {
                    file.Delete();
                    throw new IOException(
                        string.Format(
                            "Bad hash for incoming message: {0} instead of {1}",
                            receivedId.Hash,
                            this.id.Hash));
                }
            }
            catch (Exception)
            {
                this.service.SetDownloadState(this.id, file.Size, file.FullName);
                throw;
            }

            Logger.Debug("Sucessfully downloaded resource {0} from {1}", message.Header.Hash, session.SessionId);

            // pretend the resource is temporary, if it was announced as such, it will stay temporary, otherwise
            // this flag is anyways ignored
            this.service.BeginRegisterResource(
                file.FullName, message.Header.Source, true, true, ar => this.service.EndRegisterResource(ar), null);
        }

        private IWritableFileInfo OpenFile(string path, long offset)
        {
            IWritableFileInfo file;
            if (offset > 0)
            {
                file = this.service.BaseDirectory.FileSystem.GetFile(path);
                if (file.Size < offset)
                {
                    throw new IOException(
                        string.Format(
                            "Received partial file stream starting at wrong offset ({0} instead of {1})",
                            offset,
                            file.Size));
                }
            }
            else
            {
                if (this.service.BaseDirectory.FileSystem.TryGetFile(path, out file))
                {
                    file.Delete();
                }

                file = this.service.BaseDirectory.FileSystem.CreateFile(path);
            }

            return file;
        }
    }
}