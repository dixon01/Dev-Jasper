// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortForwarderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortForwarderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Forwarder
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports.Config;
    using Gorba.Common.Medi.Ports.Messages;
    using Gorba.Common.Utility.Core.Factory;

    using NLog;

    /// <summary>
    /// Base class for all port forwarders.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of config this port forwarder uses to configure itself.
    /// </typeparam>
    internal abstract class PortForwarderBase<TConfig> : IPortForwarder, IConfigurable<TConfig>
        where TConfig : ForwardingEndPointConfig
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private IMessageDispatcher dispatcher;

        private string localId;
        private string remoteId;
        private MediAddress remoteAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortForwarderBase{TConfig}"/> class.
        /// </summary>
        protected PortForwarderBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the config.
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public void Configure(TConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Starts this forwarder.
        /// </summary>
        /// <param name="disp">
        /// The message dispatcher to use for sending messages.
        /// </param>
        /// <param name="localForwardingId">
        /// The local forwarding id.
        /// </param>
        /// <param name="remoteForwardingId">
        /// The remote forwarding id.
        /// </param>
        /// <param name="remoteMediAddress">
        /// The remote Medi address.
        /// </param>
        /// <returns>
        /// The real <see cref="ForwardingEndPointConfig"/> used.
        /// </returns>
        public ForwardingEndPointConfig Start(
            IMessageDispatcher disp,
            string localForwardingId,
            string remoteForwardingId,
            MediAddress remoteMediAddress)
        {
            this.Stop();

            this.Logger.Debug("Starting");

            this.dispatcher = disp;
            this.localId = localForwardingId;
            this.remoteId = remoteForwardingId;
            this.remoteAddress = remoteMediAddress;

            this.dispatcher.Subscribe<ConnectMessage>(this.HandleConnect);
            this.dispatcher.Subscribe<DataMessage>(this.HandleData);
            this.dispatcher.Subscribe<DisconnectMessage>(this.HandleDisconnect);

            var config = this.DoStart();

            this.Logger.Info("Started (local: {0}, remote: {1})", this.localId, this.remoteId);
            return config;
        }

        /// <summary>
        /// Stops this forwarder.
        /// </summary>
        public void Stop()
        {
            if (this.dispatcher == null)
            {
                return;
            }

            this.Logger.Debug("Stopping");
            this.DoStop();

            this.dispatcher.Unsubscribe<ConnectMessage>(this.HandleConnect);
            this.dispatcher.Unsubscribe<DataMessage>(this.HandleData);
            this.dispatcher.Unsubscribe<DisconnectMessage>(this.HandleDisconnect);

            this.dispatcher = null;
            this.Logger.Info("Stopped");
        }

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        /// <returns>
        /// The actual <see cref="TConfig"/> used.
        /// </returns>
        protected abstract TConfig DoStart();

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected abstract void DoStop();

        /// <summary>
        /// Handles the connect message for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected abstract void HandleConnect(MediAddress source, int streamId);

        /// <summary>
        /// Handles the reception of data for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        protected abstract void HandleData(MediAddress source, int streamId, byte[] data);

        /// <summary>
        /// Handles the disconnection of a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected abstract void HandleDisconnect(MediAddress source, int streamId);

        /// <summary>
        /// Sends a connect message with the given stream ID to the remote forwarder.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected void SendConnect(int streamId)
        {
            var medi = this.dispatcher;
            if (medi == null)
            {
                this.Logger.Warn("Couldn't send connect because this forwarder was stopped");
                return;
            }

            this.Logger.Trace("Sending connect for forwarding {0} to {1}", this.remoteId, streamId);
            var connect = new ConnectMessage { ForwardingId = this.remoteId, StreamId = streamId };
            medi.Send(this.remoteAddress, connect);
        }

        /// <summary>
        /// Sends data with the given stream ID to the remote forwarder.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        protected void SendData(int streamId, byte[] data, int offset, int length)
        {
            if (length <= 0)
            {
                return;
            }

            var medi = this.dispatcher;
            if (medi == null)
            {
                this.Logger.Warn("Couldn't send data because this forwarder was stopped");
                return;
            }

            this.Logger.Trace("Sending {0} bytes to {1}", length, streamId);
            var bytes = new byte[length];
            Array.Copy(data, offset, bytes, 0, length);
            var dataMessage = new DataMessage { ForwardingId = this.remoteId, StreamId = streamId, Data = bytes };
            medi.Send(this.remoteAddress, dataMessage);
        }

        /// <summary>
        /// Sends a disconnect message with the given stream ID to the remote forwarder.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected void SendDisconnect(int streamId)
        {
            var medi = this.dispatcher;
            if (medi == null)
            {
                this.Logger.Warn("Couldn't send disconnect because this forwarder was already stopped");
                return;
            }

            this.Logger.Trace("Sending disconnect for forwarding {0} to {1}", this.remoteId, streamId);
            var disconnect = new DisconnectMessage { ForwardingId = this.remoteId, StreamId = streamId };
            medi.Send(this.remoteAddress, disconnect);
        }

        private void HandleConnect(object sender, MessageEventArgs<ConnectMessage> e)
        {
            if (this.localId != e.Message.ForwardingId)
            {
                return;
            }

            this.Logger.Trace("Received connect for stream {0} from {1}", e.Message.StreamId, e.Source);
            this.HandleConnect(e.Source, e.Message.StreamId);
        }

        private void HandleData(object sender, MessageEventArgs<DataMessage> e)
        {
            if (this.localId != e.Message.ForwardingId)
            {
                return;
            }

            this.Logger.Trace(
                "Received {0} bytes for stream {1} from {2}",
                e.Message.Data.Length,
                e.Message.StreamId,
                e.Source);
            this.HandleData(e.Source, e.Message.StreamId, e.Message.Data);
        }

        private void HandleDisconnect(object sender, MessageEventArgs<DisconnectMessage> e)
        {
            if (this.localId != e.Message.ForwardingId)
            {
                return;
            }

            this.Logger.Trace("Received disconnect for stream {0} from {1}", e.Message.StreamId, e.Source);
            this.HandleDisconnect(e.Source, e.Message.StreamId);
        }
    }
}