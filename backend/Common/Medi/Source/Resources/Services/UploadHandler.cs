// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UploadHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The upload handler implementation.
    /// </summary>
    internal class UploadHandler : IUploadHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UploadHandler>();

        private readonly MediAddress destination;

        private readonly ResourceServiceBase service;

        private readonly IAsyncResult getResourceInfoResult;

        private StreamMessage streamMessage;

        private bool completed;
        private bool completedSucessfully;
        private bool sentUploadState;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadHandler"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        public UploadHandler(ResourceId id, MediAddress destination, ResourceServiceBase service)
        {
            this.destination = destination;
            this.service = service;
            this.getResourceInfoResult = this.service.BeginGetResourceInfo(
                id, false, ar => this.CompleteUpload(), null);
        }

        /// <summary>
        /// Gets the total bytes uploaded until now.
        /// </summary>
        public int TotalBytesUploaded { get; private set; }

        /// <summary>
        /// Writes the given message to the given stream.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="output">
        /// The output to write to.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        public void Upload(StreamMessage message, Stream output, ITransportSession session)
        {
            Logger.Debug("Uploading resource {0} to {1}", message.Header.Hash, session.SessionId);
            this.streamMessage = message;
            using (var input = message.OpenRead())
            {
                var streamWriteBuffer = new byte[4096];
                int read;
                while ((read = input.Read(streamWriteBuffer, 0, streamWriteBuffer.Length)) > 0)
                {
                    output.Write(streamWriteBuffer, 0, read);
                    this.TotalBytesUploaded += read;
                }
            }

            Logger.Debug("Sucessfully uploaded resource {0} to {1}", message.Header.Hash, session.SessionId);
        }

        /// <summary>
        /// Tells the upload handler that this upload has completed
        /// (either successfully or not).
        /// </summary>
        /// <param name="successful">
        /// A flag telling whether the upload was successful.
        /// </param>
        public void Complete(bool successful)
        {
            if (this.completed)
            {
                throw new NotSupportedException("Can't complete UploadHandler twice");
            }

            this.completedSucessfully = successful;
            this.completed = true;

            if (successful)
            {
                this.CompleteUpload();
            }
            else
            {
                this.ResendStreamMessage();
            }
        }

        private void ResendStreamMessage()
        {
            // try to resend the stream message in 10 seconds
            var timer = TimerFactory.Current.CreateTimer("ResendStreamMessage");
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
                {
                    // retry every second until we can send the message
                    // TODO: perhaps this is too much and a node might get tons of
                    // timers like this, checking the routing table might be a good idea
                    timer.Interval = TimeSpan.FromMilliseconds(1000);
                    if (this.service.Dispatcher.Send(
                        this.service.Dispatcher.LocalAddress,
                        this.streamMessage.Header.Destination,
                        this.streamMessage,
                        null))
                    {
                        // TODO: we should check if we really upload the file
                        // (what if the client disconnects while we try to resend?)
                        timer.Enabled = false;
                        timer.Dispose();
                    }
                };
            timer.Enabled = true;
        }

        private void CompleteUpload()
        {
            lock (this)
            {
                if (this.sentUploadState || !this.completed || !this.getResourceInfoResult.IsCompleted)
                {
                    return;
                }

                this.sentUploadState = true;
                if (!this.completedSucessfully)
                {
                    return;
                }
            }

            // this will not block since we test for the request completion above
            var info = this.service.EndGetResourceInfo(this.getResourceInfoResult);
            this.service.ClearUploading(info.Id, this.destination);
            if (info.IsTemporary)
            {
                // remove the resource if it was only temporary
                this.service.BeginRemoveResource(info.Id, ar => this.service.EndRemoveResource(ar), null);
            }
        }
    }
}