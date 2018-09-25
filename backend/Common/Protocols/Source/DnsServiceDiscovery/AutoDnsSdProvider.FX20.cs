// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoDnsSdProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AutoDnsSdProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour;
    using Gorba.Common.Protocols.DnsServiceDiscovery.Server;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IDnsSdProvider"/> that first tries to use a
    /// <see cref="BonjourProvider"/> and if this fails (i.e. no Bonjour installed)
    /// uses the <see cref="DnsSdServer"/>.
    /// </summary>
    public partial class AutoDnsSdProvider : IDnsSdProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IDnsSdProvider provider;

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public void Start()
        {
            try
            {
                this.provider = new ThreadSafeBonjourProvider();
                this.provider.Start();
            }
            catch (Exception ex)
            {
                Logger.Debug("Couldn't create Bonjour provider, creating local server {0}", ex.Message);
                this.provider = new DnsSdServer();
                this.provider.Start();
            }
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public void Stop()
        {
            this.provider.Stop();
        }

        /// <summary>
        /// Registers a new service with the given identity.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="protocol">
        /// The protocol name (without ".local").
        /// </param>
        /// <param name="port">
        /// The port number.
        /// </param>
        /// <param name="attributes">
        /// The service attributes.
        /// </param>
        /// <returns>
        /// The service registration to be used with <see cref="IDnsSdProvider.DeregisterService"/>.
        /// </returns>
        /// <exception cref="DnsSdException">
        /// If a service with the same name and protocol was already registered.
        /// </exception>
        public IServiceRegistration RegisterService(
            string serviceName, string protocol, int port, IDictionary<string, string> attributes)
        {
            return this.provider.RegisterService(serviceName, protocol, port, attributes);
        }

        /// <summary>
        /// Deregisters a previously registered service.
        /// <seealso cref="IDnsSdProvider.RegisterService"/>
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="registration"/> didn't come from <see cref="IDnsSdProvider.RegisterService"/>.
        /// </exception>
        public void DeregisterService(IServiceRegistration registration)
        {
            this.provider.DeregisterService(registration);
        }

        /// <summary>
        /// Starts a query for all services of a protocol.
        /// When this method returns, the returned query might already contain services.
        /// </summary>
        /// <param name="protocol">
        /// The protocol name without the ".local".
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/> that can be used to get the results.
        /// </returns>
        public IQuery CreateQuery(string protocol)
        {
            return this.provider.CreateQuery(protocol);
        }
    }
}
