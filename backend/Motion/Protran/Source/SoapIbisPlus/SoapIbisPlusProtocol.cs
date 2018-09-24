// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoapIbisPlusProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoapIbisPlusProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus
{
    using System;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Http;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Protran.Core.Protocols;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;
    using Gorba.Motion.Protran.SoapIbisPlus.Service;

    using MFD.MFDCustomerService;

    using NLog;

    /// <summary>
    /// The implementation of the SOAP protocol for Trapeze IBIS plus.
    /// </summary>
    public class SoapIbisPlusProtocol : IProtocol
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ManualResetEvent stopWait = new ManualResetEvent(false);

        private IProtocolHost protocolHost;

        private Dictionary genericDictionary;

        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        public string Name
        {
            get
            {
                return "SOAP/IBISplus";
            }
        }

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        public void Stop()
        {
            this.stopWait.Set();
        }

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        public void Run(IProtocolHost host)
        {
            this.stopWait.Reset();

            this.protocolHost = host;

            var configFile = PathManager.Instance.GetPath(FileType.Config, "SoapIbisPlus.xml");
            Logger.Debug("Loading config from {0}", configFile);

            var configManager = new ConfigManager<SoapIbisPlusConfig> { FileName = configFile, EnableCaching = true };
            var config = configManager.Config;

            Logger.Debug("Creating service implementation");
            var serviceImpl = new MfdServiceImpl(config, this.genericDictionary);
            serviceImpl.XimpleCreated += this.ServiceXimpleCreated;
            ServiceInterface.RegisterServiceImpl(serviceImpl);

            Logger.Debug(
                "Configuring HTTP channel (Port: {0}, URI:'{1}')", config.Service.Port, config.Service.Uri);
            var channel = new HttpChannel(config.Service.Port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServiceInterface), config.Service.Uri, WellKnownObjectMode.Singleton);

            Logger.Debug("Running");

            this.RaiseStarted(EventArgs.Empty);

            this.stopWait.WaitOne();

            Logger.Debug("Stopping");

            ServiceInterface.UnRegisterServiceImpl(serviceImpl);
            serviceImpl.Dispose();
        }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        /// The generic view dictionary.
        /// </param>
        public void Configure(Dictionary dictionary)
        {
            this.genericDictionary = dictionary;
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

        private void ServiceXimpleCreated(object sender, XimpleEventArgs e)
        {
            Logger.Debug("Created Ximple");
            Logger.Trace(e.Ximple.ToXmlString);
            this.protocolHost.OnDataFromProtocol(this, e.Ximple);
        }
    }
}
