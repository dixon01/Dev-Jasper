// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301Protocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301Protocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301
{
    using System;
    using System.Net;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP.Discovery;
    using Gorba.Motion.Common.IbisIP.Server;
    using Gorba.Motion.Protran.Core.Protocols;
    using Gorba.Motion.Protran.Vdv301.Handlers;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IProtocol"/> that handles the VDV 301 protocol (IBIS-IP).
    /// </summary>
    public partial class Vdv301Protocol : IProtocol
    {
        private static readonly Logger Logger = LogHelper.GetLogger<Vdv301Protocol>();

        private readonly ManualResetEvent stopWait = new ManualResetEvent(false);

        private readonly IbisHttpServer callbackServer;

        private IIbisServiceLocator serviceLocator;

        private IProtocolHost protocolHost;

        private int ximplesSentCounter;

        private IElementHandlerFactory handlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301Protocol"/> class.
        /// </summary>
        public Vdv301Protocol()
        {
            var configManager = new ConfigManager<Vdv301ProtocolConfig>
            {
                FileName = PathManager.Instance.GetPath(FileType.Config, "vdv301.xml"),
                EnableCaching = true,
                XmlSchema = Vdv301ProtocolConfig.Schema
            };

            this.Config = configManager.Config;

            this.callbackServer = new IbisHttpServer(new IPEndPoint(IPAddress.Any, 0));

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance(this);
        }

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Event that is fired when this protocol creates a <see cref="Ximple"/> object.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired when this protocol gets updated data from a VDV 301 server.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<object>> DataUpdated;

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "VDV301";
            }
        }

        /// <summary>
        /// Gets the generic dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the VDV 301 protocol configuration.
        /// </summary>
        public Vdv301ProtocolConfig Config { get; private set; }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        /// The generic view dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        public void Stop()
        {
            this.callbackServer.Stop();

            this.stopWait.Set();

            if (this.serviceLocator != null)
            {
                this.serviceLocator.Dispose();
                this.serviceLocator = null;
            }
        }

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        public void Run(IProtocolHost host)
        {
            this.protocolHost = host;
            this.callbackServer.Start();

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            try
            {
                this.serviceLocator = ServiceLocator.Current.GetInstance<IIbisServiceLocator>();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't get IBIS service locator, using default Medi locator");
                var mediLocator = new MediIbisServiceLocator();
                var services = this.Config.Services;
                mediLocator.Configure(
                    services.ValidateHttpRequests, services.ValidateHttpResponses, services.VerifyVersion);
                this.serviceLocator = mediLocator;
                serviceContainer.RegisterInstance(this.serviceLocator);
            }

            try
            {
                this.handlerFactory = ServiceLocator.Current.GetInstance<IElementHandlerFactory>();
            }
            catch
            {
                // handler factory wasn't given from the outside, let's create our own
                this.handlerFactory = new ElementHandlerFactory();
                serviceContainer.RegisterInstance(this.handlerFactory);
            }

            this.InitializeCustomerInformationService();

            Logger.Debug("Running");
            this.RaiseStarted(EventArgs.Empty);

            this.stopWait.WaitOne();
        }

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStarted(EventArgs e)
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DataUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseDataUpdated(DataUpdateEventArgs<object> e)
        {
            var handler = this.DataUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            if (e.Ximple.Cells.Count == 0)
            {
                return;
            }

            // TODO: add remote computer state
            Logger.Info("Created a Ximple ({0}).", ++this.ximplesSentCounter);
            Logger.Trace(e.Ximple.ToXmlString);

            this.protocolHost.OnDataFromProtocol(this, e.Ximple);

            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
