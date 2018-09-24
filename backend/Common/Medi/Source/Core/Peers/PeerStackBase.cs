// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerStackBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PeerStackBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.Provider;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The core entity of an endpoint in the Medi network.
    /// </summary>
    /// <typeparam name="TTransport">
    /// The type of the transport being used by this peer,
    /// this should be either <see cref="ITransportClient"/>
    /// or <see cref="ITransportServer"/>.
    /// </typeparam>
    /// <typeparam name="TTransportConfig">
    /// The type of the transport configuration used to
    /// configure the transport.
    /// </typeparam>
    internal abstract class PeerStackBase<TTransport, TTransportConfig>
        : MediPeerBase, IConfigurable<PeerStackConfig<TTransportConfig>>, IManageable
        where TTransport : class, IMessageTransport, ITransportImplementation
        where TTransportConfig : TransportConfig
    {
        private readonly ProducerConsumerQueue<MediMessageEventArgs> receiveQueue;

        private MessageTranscoder messageTranscoder;

        private IManagementProvider managementProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeerStackBase{TTransport,TTransportConfig}"/> class.
        /// </summary>
        protected PeerStackBase()
        {
            this.receiveQueue =
                new ProducerConsumerQueue<MediMessageEventArgs>(
                    e => this.PostMessageToMedi(e.Message, e.Session), 10000);
        }

        /// <summary>
        /// Gets the transport implementation.
        /// </summary>
        protected TTransport Transport { get; private set; }

        /// <summary>
        /// Configures this peer with the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public virtual void Configure(PeerStackConfig<TTransportConfig> config)
        {
            this.messageTranscoder =
                new MessageTranscoder(ConfigImplementationFactory.CreateFromConfig<IMessageCodec>(config.Codec));
            this.Transport = ConfigImplementationFactory.CreateFromConfig<TTransport>(config.Transport);

            this.Transport.Started += this.TransportStarted;
            this.Transport.SessionConnected += this.TransportSessionConnected;

            Logger.Debug(
                "Configured {0} with {1}", this.Transport.GetType().Name, this.messageTranscoder.Codec.GetType().Name);
        }

        /// <summary>
        /// Starts this peer by registering to all necessary events
        /// and starting the transport (which in the case of a client
        /// transport will connect to the server).
        /// </summary>
        /// <param name="medi">
        /// The message dispatcher to be used by this peer.
        /// </param>
        public override void Start(IMessageDispatcherImpl medi)
        {
            base.Start(medi);

            var mgmtParent =
                (IModifiableManagementProvider)
                this.Dispatcher.ManagementProviderFactory.LocalRoot.GetDescendant(
                    true, MessageDispatcher.ManagementName, PeersManagementName);
            this.managementProvider =
                new ManageableManagementProvider(
                    this.Dispatcher.ManagementProviderFactory.CreateUniqueName(mgmtParent, typeof(TTransport).Name),
                    this,
                    mgmtParent);
            mgmtParent.AddChild(this.managementProvider);

            this.messageTranscoder.MessageDecoded += this.TranscoderMessageDecoded;

            this.Transport.Start(medi, this.messageTranscoder);

            this.receiveQueue.StartConsumer();
        }

        /// <summary>
        /// Stops this peer by deregistering from all necessary events and
        /// stopping the transport.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            this.receiveQueue.StopConsumer();

            this.messageTranscoder.StopDecode();
            this.messageTranscoder.MessageDecoded -= this.TranscoderMessageDecoded;

            this.Transport.Stop();

            if (this.managementProvider != null)
            {
                this.managementProvider.Dispose();
                this.managementProvider = null;
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            if (this.messageTranscoder != null)
            {
                yield return
                    parent.Factory.CreateManagementProvider(
                        this.messageTranscoder.GetType().Name, parent, this.messageTranscoder);
            }

            if (this.Transport != null)
            {
                yield return
                    parent.Factory.CreateManagementProvider(this.Transport.GetType().Name, parent, this.Transport);
            }
        }

        /// <summary>
        /// Do not use this method to send a message, use
        /// <see cref="MediPeerBase.EnqueueMessage"/> instead!
        /// Implementations will have to do the actual sending in this method.
        /// This is called when a message was de-queued from the internal queue.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given message must be written.
        /// </param>
        protected override void SendMessage(IMessage message, ISessionId destinationSessionId)
        {
            try
            {
                var mediMessage = message as MediMessage;
                if (mediMessage != null)
                {
                    Logger.Debug("Sending message to {0}: {1}", message.Destination, mediMessage.Payload);
                    this.messageTranscoder.Encode(mediMessage, this.Transport, destinationSessionId);
                    return;
                }

                var streamMessage = message as StreamMessage;
                if (streamMessage == null)
                {
                    throw new NotSupportedException("Unknown message type " + message.GetType());
                }

                this.Transport.WriteStream(streamMessage, destinationSessionId);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Could not encode message " + message);
            }
        }

        private void TransportStarted(object sender, EventArgs e)
        {
            this.messageTranscoder.StartDecode(this.Transport);

            if (this.Dispatcher.GetService<IResourceServiceImpl>() != null)
            {
                // start decoding streams only if we have resources enabled
                this.Transport.BeginReadStream(this.StreamRead, null);
            }
        }

        private void StreamRead(IAsyncResult ar)
        {
            StreamReadResult result;
            try
            {
                result = this.Transport.EndReadStream(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't read stream");
                return;
            }

            try
            {
                var id = new ResourceId(result.Message.Header.Hash);
                var handler = this.Dispatcher.GetService<IResourceServiceImpl>().CreateDownloadHandler(id);
                handler.Download(result.Message, result.Session);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't handle incoming stream");
            }

            try
            {
                this.Transport.BeginReadStream(this.StreamRead, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't start reading stream");
            }
        }

        private void TransportSessionConnected(object sender, SessionEventArgs e)
        {
            this.Logger.Debug("TransportSessionConnected: SessionId={0}", e.Session.SessionId);

            this.RegisterSession(e.Session);
        }

        private void TranscoderMessageDecoded(object sender, MediMessageEventArgs e)
        {
            var message = e.Message;

            if (message == null || message.Payload == null)
            {
                Logger.Warn("Got an empty message from transcoder, ignoring it");
                return;
            }

            this.Transport.PreviewDecodedMessage(e.Session, ref message);

            if (message == null)
            {
                return;
            }

            if (!this.receiveQueue.Enqueue(e))
            {
                Logger.Error("Receive queue overflow");
            }
        }
    }
}
