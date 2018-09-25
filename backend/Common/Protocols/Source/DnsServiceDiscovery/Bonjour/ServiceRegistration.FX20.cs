// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistration.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour
{
    using global::Bonjour;

    /// <summary>
    /// The <see cref="IServiceRegistration"/> implementation for Bonjour.
    /// </summary>
    internal partial class ServiceRegistration : IServiceRegistration
    {
        private readonly IDNSSDService server;

        private readonly DNSSDEventManagerClass eventManager;

        private DNSSDService registrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistration"/> class.
        /// </summary>
        /// <param name="server">
        /// The DNS-SD service.
        /// </param>
        public ServiceRegistration(IDNSSDService server)
        {
            this.server = server;

            this.eventManager = new DNSSDEventManagerClass();
            this.eventManager.ServiceRegistered += this.EventManagerOnServiceRegistered;
        }

        /// <summary>
        /// Gets a value indicating whether is the service is already registered.
        /// Registration can take some time since any possible name conflicts have to be checked first.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Starts registering the given service with Bonjour.
        /// </summary>
        /// <param name="serviceName">
        /// The service name (without protocol or ".local").
        /// </param>
        /// <param name="protocol">
        /// The protocol name (without ".local").
        /// </param>
        /// <param name="domain">
        /// The domain name (e.g. "local"). Can be null for default domain.
        /// </param>
        /// <param name="port">
        /// The TCP or UDP port.
        /// </param>
        /// <param name="txtRecord">
        /// The DNS txt record.
        /// </param>
        public void Register(string serviceName, string protocol, string domain, int port, TXTRecord txtRecord)
        {
            this.registrar = this.server.Register(
                0, 0, serviceName, protocol, domain, null, (ushort)port, txtRecord, this.eventManager);
        }

        /// <summary>
        /// Stops this registration and unregisters the service from Bonjour.
        /// </summary>
        public void Stop()
        {
            if (this.registrar != null)
            {
                this.registrar.Stop();
                this.registrar = null;
                this.IsRegistered = false;
            }
        }

        private void EventManagerOnServiceRegistered(
            DNSSDService service, DNSSDFlags flags, string name, string regtype, string domain)
        {
            this.IsRegistered = true;
        }
    }
}