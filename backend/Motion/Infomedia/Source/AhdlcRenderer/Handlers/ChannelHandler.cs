// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Master;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The handler for a single channel.
    /// A channel is one serial port with one or more signs connected to it.
    /// </summary>
    public class ChannelHandler
    {
        private readonly Dictionary<int, SignHandler> handlers = new Dictionary<int, SignHandler>();

        private readonly object writeLock = new object();

        private Logger logger;

        private ChannelConfig config;

        private AhdlcRendererConfig rendererConfig;

        private AhdlcMaster ahdlcMaster;

        private IFrameHandler frameHandler;

        /// <summary>
        /// Gets the sign handlers of this channel.
        /// </summary>
        public ICollection<SignHandler> Handlers
        {
            get
            {
                return this.handlers.Values;
            }
        }

        /// <summary>
        /// Configures this channel handler.
        /// </summary>
        /// <param name="channelConfig">
        /// The channel config.
        /// </param>
        /// <param name="ahdlcRendererConfig">
        /// The renderer config.
        /// </param>
        public void Configure(ChannelConfig channelConfig, AhdlcRendererConfig ahdlcRendererConfig)
        {
            this.config = channelConfig;
            this.rendererConfig = ahdlcRendererConfig;
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + this.config.SerialPort.ComPort);
        }

        /// <summary>
        /// Starts this channel handler.
        /// This opens the serial port and sets up all handlers.
        /// </summary>
        public void Start()
        {
            if (this.ahdlcMaster != null)
            {
                return;
            }

            this.logger.Info("Starting");
            var factory = ServiceLocator.Current.GetInstance<FrameHandlerFactory>();
            this.frameHandler = factory.CreateFrameHandler(this.config);
            this.ahdlcMaster = new AhdlcMaster(this.frameHandler);
            this.ahdlcMaster.StatusReceived += this.AhdlcMasterOnStatusReceived;
            foreach (var signConfig in this.config.Signs)
            {
                var signHandler = new SignHandler();
                signHandler.Configure(signConfig, this.rendererConfig);
                signHandler.FramesCreated += this.SignHandlerOnFramesCreated;
                this.handlers.Add(signConfig.Address, signHandler);
            }

            this.ahdlcMaster.IgnoreResponses = this.config.SerialPort.IgnoreResponses;
            this.ahdlcMaster.IgnoreResponseTime = this.config.SerialPort.IgnoreResponseTime;
            this.ahdlcMaster.Start();

            foreach (var renderer in this.handlers.Values)
            {
                renderer.Start();
            }
        }

        /// <summary>
        /// Stops this channel handler.
        /// </summary>
        public void Stop()
        {
            if (this.ahdlcMaster == null)
            {
                return;
            }

            foreach (var renderer in this.handlers.Values)
            {
                renderer.Stop();
            }

            this.handlers.Clear();

            this.ahdlcMaster.Stop();

            var disposable = this.frameHandler as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            this.ahdlcMaster = null;
        }

        /// <summary>
        /// Updates all handlers in this channel.
        /// </summary>
        /// <param name="updates">
        /// The updates to be sent to the handlers.
        /// </param>
        public void UpdateScreen(List<ScreenUpdate> updates)
        {
            foreach (var renderer in this.handlers.Values)
            {
                renderer.UpdateScreen(updates);
            }
        }

        private void AhdlcMasterOnStatusReceived(object sender, StatusResponseEventArgs e)
        {
            SignHandler handler;
            if (this.handlers.TryGetValue(e.Status.Address, out handler))
            {
                handler.HandleStatusResponse(e.Status);
            }
        }

        private void SignHandlerOnFramesCreated(object sender, FramesEventArgs e)
        {
            var master = this.ahdlcMaster;
            var handler = sender as SignHandler;
            if (master == null || handler == null)
            {
                return;
            }

            lock (this.writeLock)
            {
                foreach (var frame in e.GetFrames())
                {
                    frame.Address = handler.Address;
                    master.EnqueueFrame(frame);
                }
            }
        }
    }
}