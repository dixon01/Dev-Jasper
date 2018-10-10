// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaProtocolImpl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GorbaProtocolImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.GorbaProtocol
{
    using System;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.GorbaProtocol;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.Core.Protocols;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The implementation of the Gorba protocol.
    /// </summary>
    public class GorbaProtocolImpl : IProtocol
    {
        /// <summary>
        /// The name of the configuration file.
        /// Attention (this is not the absolute file's name).
        /// </summary>
        private const string GorbaCfgFileName = "gorba.xml";

        private static readonly Logger Logger;

        /// <summary>
        /// Event used to manage the running status of this protocol.
        /// </summary>
        private readonly AutoResetEvent liveEvent = new AutoResetEvent(false);

        private readonly IMessageDispatcher dispatcher;

        private readonly ConfigManager<GorbaConfig> configManager;

        static GorbaProtocolImpl()
        {
            Logger = LogManager.GetLogger(typeof(GorbaProtocolImpl).FullName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaProtocolImpl"/> class.
        /// </summary>
        public GorbaProtocolImpl()
        {
            this.dispatcher = MessageDispatcher.Instance.GetNamedDispatcher("GorbaProtocol");
            this.configManager = new ConfigManager<GorbaConfig>
                                    {
                                        FileName = PathManager.Instance.GetPath(FileType.Config, GorbaCfgFileName)
                                    };
            this.configManager.XmlSchema = GorbaConfig.Schema;

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        public IProtocolHost Host { get; private set; }

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "GORBA";
            }
        }

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        public void Stop()
        {
            this.liveEvent.Set();
        }

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        public void Run(IProtocolHost host)
        {
            this.Host = host;
            this.RegisterHandler(new NewsFeedUpdateMessageHandler());
            this.RaiseStarted();
            Logger.Info("Gorba protocol is running");
            this.liveEvent.WaitOne();
        }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        ///     The generic view dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        protected virtual void RaiseStarted()
        {
            var handler = this.Started;
            if (handler == null)
            {
                return;
            }

            handler(this, EventArgs.Empty);
        }

        private void RegisterHandler<T>(IMessageHandler<T> messageHandler) where T : GorbaMessage
        {
            messageHandler.XimpleCreated += this.OnMessageHandlerXimpleCreated;
            this.dispatcher.Subscribe<T>(
                (obj, args) =>
                    {
                        var reply = new GorbaMessageAck { Id = args.Message.Id };
                        Logger.Debug("Acknowledging message {0} from '{1}'", args.Message.Id, args.Source);
                        this.dispatcher.Send(args.Source, reply);
                        Logger.Trace("Processing message");
                        messageHandler.ProcessMessage(args.Message);
                    });
            Logger.Info("Registered IMessageHandler<{0}>", typeof(T).Name);
        }

        private void OnMessageHandlerXimpleCreated(object sender, XimpleEventArgs ximpleEventArgs)
        {
            Logger.Debug("Forwarding ximple {0} to host", ximpleEventArgs.Ximple);
            this.Host.OnDataFromProtocol(this, ximpleEventArgs.Ximple);
        }
    }
}