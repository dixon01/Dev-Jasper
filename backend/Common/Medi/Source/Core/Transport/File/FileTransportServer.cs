// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileTransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Implementation of <see cref="ITransportServer"/> that reads from / writes to
    /// a local or remote directory to exchange messages between different peers.
    /// </summary>
    internal class FileTransportServer : TransportServer
    {
        private static readonly string DirectoryEntryFormat =
            "d{0}_{1:" + FileMessageTransport.TimeStampFormat + "}.msg";

        private static readonly Regex DirectoryEntryRegex =
            new Regex(@"^d([0-9a-f]+)_(\d+).msg$", RegexOptions.IgnoreCase);

        private readonly ManualResetEvent messageLoopWait = new ManualResetEvent(false);

        private readonly Dictionary<string, FileMessageTransport> transports =
            new Dictionary<string, FileMessageTransport>(StringComparer.InvariantCultureIgnoreCase);

        private FileSessionId localSessionId;

        private FileTransportServerConfig config;

        private FileTransferSessionProvider sessionProvider;

        private Thread communicationThread;

        private bool running;

        /// <summary>
        /// Configures this transport server with the given config.
        /// </summary>
        /// <param name="cfg">
        /// The config.
        /// </param>
        public override void Configure(TransportServerConfig cfg)
        {
            var tcpConfig = cfg as FileTransportServerConfig;
            if (tcpConfig == null)
            {
                throw new ArgumentException("FileTransportServerConfig expected", "cfg");
            }

            this.config = tcpConfig;
        }

        /// <summary>
        /// Starts the transport implementation, connecting it with the given codec.
        /// </summary>
        /// <param name="medi">
        ///     The local message dispatcher implementation
        /// </param>
        /// <param name="messageTrans">
        ///     The message transcoder that is on top of this transport.
        /// </param>
        public override void Start(IMessageDispatcherImpl medi, MessageTranscoder messageTrans)
        {
            this.Stop();

            this.localSessionId = new FileSessionId(medi.LocalAddress);
            this.sessionProvider = FileTransferSessionProvider.Create(
                new Uri(this.config.DropLocation), this.config.Username, this.config.Password);
            this.communicationThread = new Thread(this.Run) { IsBackground = true };
            this.running = true;
            this.communicationThread.Start();

            // TODO: use the two transcoders
            Logger.Info("Server started, listening to changes in {0}", this.config.DropLocation);
        }

        /// <summary>
        /// Stops the transport and releases all resources.
        /// </summary>
        public override void Stop()
        {
            if (this.communicationThread == null)
            {
                return;
            }

            this.running = false;
            this.communicationThread = null;

            base.Stop();
        }

        private bool TryParseDirectoryEntry(string fileName, out DateTime timestamp, out string hash)
        {
            var match = DirectoryEntryRegex.Match(fileName);
            if (!match.Success)
            {
                timestamp = default(DateTime);
                hash = null;
                return false;
            }

            hash = match.Groups[1].Value;
            if (ParserUtil.TryParseExact(
                match.Groups[2].Value,
                FileMessageTransport.TimeStampFormat,
                null,
                DateTimeStyles.AssumeUniversal,
                out timestamp))
            {
                timestamp = timestamp.ToUniversalTime();
                return true;
            }

            this.Logger.Debug("Directory entry timestamp does not match expected format: {0}", fileName);
            return false;
        }

        private void Run()
        {
            this.RaiseStarted(EventArgs.Empty);

            while (this.running)
            {
                try
                {
                    using (var session = this.sessionProvider.OpenSession())
                    {
                        var availableFiles = session.ListFiles();
                        Array.Sort(availableFiles, StringComparer.InvariantCultureIgnoreCase);
                        if (this.Logger.IsTraceEnabled)
                        {
                            this.Logger.Trace("Got file list:");
                            foreach (var file in availableFiles)
                            {
                                this.Logger.Trace(" - {0}", file);
                            }
                        }

                        this.UpdateDirectoryEntry(session, availableFiles);
                        this.UpdateSessions(availableFiles);
                        this.messageLoopWait.Reset();
                        this.UploadMessages(session);
                        this.DownloadMessages(session, availableFiles);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't perform sync with " + this.config.DropLocation);

                    // try again after one second
                    Thread.Sleep(1000);
                    continue;
                }

                if (this.messageLoopWait.WaitOne(this.config.PollInterval, false))
                {
                    // when we got the signal, wait another 100 ms to catch any additional messages
                    // that might be coming in immediately after the first one
                    Thread.Sleep(100);
                }
            }
        }

        private void UpdateDirectoryEntry(IFileTransferSession session, string[] availableFiles)
        {
            string oldEntry = null;
            foreach (var file in availableFiles)
            {
                DateTime oldTimestamp;
                string hash;
                if (!this.TryParseDirectoryEntry(file, out oldTimestamp, out hash) ||
                    !hash.Equals(this.localSessionId.Hash, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (oldTimestamp - TimeSpan.FromMilliseconds(this.config.MessageTimeToLive * 0.5) > DateTime.UtcNow)
                {
                    // message is still young enough (has still more than half of its TTL before being outdated)
                    return;
                }

                oldEntry = file;
                break;
            }

            var timestamp = DateTime.UtcNow + TimeSpan.FromMilliseconds(this.config.MessageTimeToLive);
            var fileName = string.Format(DirectoryEntryFormat, this.localSessionId.Hash, timestamp);
            using (var file = session.OpenWrite(fileName))
            {
                var bytes = Encoding.ASCII.GetBytes(this.localSessionId.Hash);
                file.Write(bytes, 0, bytes.Length);
            }

            if (oldEntry != null)
            {
                try
                {
                    session.DeleteFile(oldEntry);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't delete old directory entry.");
                }
            }
        }

        private void UpdateSessions(string[] availableFiles)
        {
            var now = DateTime.UtcNow;
            foreach (var file in availableFiles)
            {
                DateTime timestamp;
                string hash;
                if (!this.TryParseDirectoryEntry(file, out timestamp, out hash) ||
                    hash.Equals(this.localSessionId.Hash, StringComparison.InvariantCultureIgnoreCase) ||
                    timestamp < now)
                {
                    continue;
                }

                FileMessageTransport transport;
                if (!this.transports.TryGetValue(hash, out transport))
                {
                    Logger.Info("Found new peer directory entry: {0}", file);
                    transport = new FileMessageTransport(this.localSessionId, new FileSessionId(hash), this.config);
                    transport.MessageSendEnqueued += this.TransportOnMessageSendEnqueued;
                    transport.Session.Disconnected += (s, e) =>
                        {
                            transport.MessageSendEnqueued -= this.TransportOnMessageSendEnqueued;
                            this.transports.Remove(hash);
                        };
                    this.transports.Add(hash, transport);
                    var client = new Client(this, transport);
                    this.AddClient(client);
                    transport.Start();

                    this.RaiseSessionConnected(new SessionEventArgs(transport.Session));
                }

                transport.SetExpiry(timestamp);
            }
        }

        private void TransportOnMessageSendEnqueued(object sender, EventArgs eventArgs)
        {
            this.messageLoopWait.Set();
        }

        private void UploadMessages(IFileTransferSession session)
        {
            foreach (var transport in this.transports.Values)
            {
                transport.UploadMessages(session);
            }
        }

        private void DownloadMessages(IFileTransferSession session, string[] availableFiles)
        {
            foreach (var transport in this.transports.Values)
            {
                transport.DownloadMessages(session, availableFiles);
            }
        }
    }
}
