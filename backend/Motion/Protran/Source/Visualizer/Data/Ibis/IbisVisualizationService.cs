// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Handlers;
    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Implementation of the <see cref="IIbisVisualizationService"/>
    /// </summary>
    public class IbisVisualizationService : IIbisVisualizationService
    {
        private static VisualizationChannelFactory channelFactory;

        private VisualizerIbisSerialChannel serialChannel;

        private VisualizerChannel channel;

        private volatile bool initialized;

        private TelegramControl ctrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisVisualizationService"/> class.
        /// </summary>
        internal IbisVisualizationService()
        {
        }

        /// <summary>
        /// Event that is fired every time a new Ximple is created by the underlying channel.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired if a handler starts handling a telegram.
        /// </summary>
        public event EventHandler<TelegramHandlerEventArgs> HandlingTelegram;

        /// <summary>
        /// Event that is fired if a handler has finished handling a telegram.
        /// </summary>
        public event EventHandler<TelegramHandlerEventArgs> HandledTelegram;

        /// <summary>
        /// Event that is fired if a handler emits a new Ximple.
        /// Use this event only to listen to the Ximple object created by
        /// a handler, otherwise use <see cref="XimpleCreated"/>.
        /// </summary>
        public event EventHandler<TelegramHandlerXimpleEventArgs> HandlerCreatedXimple;

        /// <summary>
        /// Event that is fired every time a new telegram is being enqueued.
        /// </summary>
        public event EventHandler TelegramParsing;

        /// <summary>
        /// Event that is fired when a telegram was successfully parsed,
        /// i.e. a <see cref="Telegram"/> was created from the binary data.
        /// </summary>
        public event EventHandler<TelegramParsedEventArgs> TelegramParsed;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        public event EventHandler<TransformationChainEventArgs> TelegramTransformed;

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary
        {
            get
            {
                return this.IbisProtocol.Dictionary;
            }
        }

        /// <summary>
        /// Gets the IBIS config.
        /// </summary>
        public IbisConfig Config
        {
            get
            {
                return this.IbisProtocol.Config;
            }
        }

        private IbisProtocol IbisProtocol
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IbisProtocol>();
            }
        }

        /// <summary>
        /// Register a new instance of this class with the service container.
        /// This also registers all special factories used to
        /// intercept events inside the IBIS protocol.
        /// </summary>
        public static void Register()
        {
            Core.Protran.SetupCoreServices();
            var service = new IbisVisualizationService();
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance<IIbisVisualizationService>(service);
            channelFactory = new VisualizationChannelFactory(service);
            serviceContainer.RegisterInstance<ChannelFactory>(channelFactory);
            serviceContainer.RegisterInstance<TelegramHandlerFactory>(new VisualizerHandlerFactory(service));
        }

        /// <summary>
        /// Enqueue a new telegram into the channel of this service.
        /// </summary>
        /// <param name="telegram">
        /// The telegram data.
        /// </param>
        /// <param name="ignoreUnknown">
        /// If set to true, unknown telegrams (where no parser is found) will not be enqueued
        /// and false will be returned.
        /// </param>
        /// <returns>
        /// true if the telegram was enqueued successfully.
        /// </returns>
        public bool EnqueueTelegram(byte[] telegram, bool ignoreUnknown)
        {
            if (this.channel == null)
            {
                return false;
            }

            return this.channel.EnqueueTelegram(telegram, ignoreUnknown);
        }

        /// <summary>
        /// Enables or disabled the serial port.
        /// </summary>
        /// <param name="enableIbisBus">
        /// The enable ibis bus.
        /// </param>
        public void EnableSerialPort(bool enableIbisBus)
        {
            if (this.serialChannel == null)
            {
                return;
            }

            this.serialChannel.IbisBusEnabled = enableIbisBus;

            if (channelFactory == null)
            {
                return;
            }

            if (enableIbisBus)
            {
                channelFactory.OpenIbisChannel(this.ctrl);
            }
            else
            {
                channelFactory.CloseIbisChannel();
            }
        }

        /// <summary>
        /// Send the handle into the channel of this service
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public void SetControl(TelegramControl controller)
        {
            this.ctrl = controller;
        }

        private void Initialize()
        {
            if (!this.initialized)
            {
                lock (this)
                {
                    if (!this.initialized)
                    {
                        this.IbisProtocol.XimpleCreated += (s, ev) => this.OnXimpleCreated(ev);
                        this.initialized = true;
                    }
                }
            }
        }

        private void OnXimpleCreated(XimpleEventArgs e)
        {
            EventHandler<XimpleEventArgs> handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnHandlerCreatedXimple(TelegramHandlerXimpleEventArgs e)
        {
            var handler = this.HandlerCreatedXimple;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnHandlingTelegram(TelegramHandlerEventArgs e)
        {
            EventHandler<TelegramHandlerEventArgs> handler = this.HandlingTelegram;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnHandledTelegram(TelegramHandlerEventArgs e)
        {
            EventHandler<TelegramHandlerEventArgs> handler = this.HandledTelegram;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnTelegramParsing(EventArgs e)
        {
            EventHandler handler = this.TelegramParsing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnTelegramParsed(TelegramParsedEventArgs e)
        {
            EventHandler<TelegramParsedEventArgs> handler = this.TelegramParsed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnTelegramTransformed(TransformationChainEventArgs e)
        {
            EventHandler<TransformationChainEventArgs> handler = this.TelegramTransformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private class VisualizationChannelFactory : ChannelFactory
        {
            private readonly IbisVisualizationService service;

            private VisualizerIbisSerialChannel serialChannel;

            public VisualizationChannelFactory(IbisVisualizationService service)
            {
                this.service = service;
            }

            public override IEnumerable<IbisChannel> CreateChannels(IIbisConfigContext configContext)
            {
                var channel = new VisualizerChannel(configContext);
                channel.Opened += (s, e) => this.service.Initialize();
                channel.TelegramParsing += (s, e) => this.service.OnTelegramParsing(e);
                channel.TelegramParsed += (s, e) => this.service.OnTelegramParsed(e);
                channel.TelegramTransformed += (s, e) => this.service.OnTelegramTransformed(e);
                this.service.channel = channel;
                yield return channel;

                if (configContext.Config.Sources.SerialPort == null)
                {
                    yield break;
                }

                var ibisSerialChannel = new VisualizerIbisSerialChannel(configContext);
                ibisSerialChannel.Recorder = this.CreateRecorder(configContext);
                this.service.serialChannel = ibisSerialChannel;
                this.serialChannel = ibisSerialChannel;
                yield return ibisSerialChannel;
            }

            internal void OpenIbisChannel(TelegramControl ctrl)
            {
                if (this.serialChannel.IsOpen)
                {
                    this.serialChannel.Close();
                }

                try
                {
                    this.serialChannel.Open();
                }
                catch (Exception)
                {
                    // an error was occured opening the channel.
                    this.serialChannel.Close(); // for safety, I close it.
                }

                this.serialChannel.SetControl(ctrl);
            }

            internal void CloseIbisChannel()
            {
                this.serialChannel.Close();
            }
        }

        private class VisualizerHandlerFactory : TelegramHandlerFactory
        {
            private readonly IbisVisualizationService service;

            public VisualizerHandlerFactory(IbisVisualizationService service)
            {
                this.service = service;
            }

            public override ITelegramHandler CreateHandler(TelegramConfig config, IIbisConfigContext configContext)
            {
                var handler = base.CreateHandler(config, configContext);
                if (handler == null)
                {
                    return null;
                }

                var wrapper = new TelegramHandlerWrapper(handler);
                wrapper.XimpleCreated +=
                    (s, e) =>
                        this.service.OnHandlerCreatedXimple(new TelegramHandlerXimpleEventArgs(handler, e.Ximple));
                wrapper.HandlingTelegram +=
                    (s, e) => this.service.OnHandlingTelegram(new TelegramHandlerEventArgs(handler));
                wrapper.HandledTelegram +=
                    (s, e) => this.service.OnHandledTelegram(new TelegramHandlerEventArgs(handler));
                return wrapper;
            }
        }
    }
}
