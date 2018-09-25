// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BonjourProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BonjourProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using global::Bonjour;

    /// <summary>
    /// DNS-SD provider that relies on Bonjour.
    /// </summary>
    public partial class BonjourProvider : IDnsSdProvider
    {
        private readonly List<Query> queries = new List<Query>();

        private readonly List<ServiceRegistration> registrations = new List<ServiceRegistration>();

        private DNSSDServiceClass server;

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public void Start()
        {
            this.server = new DNSSDServiceClass();
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public void Stop()
        {
            lock (this.queries)
            {
                foreach (var query in this.queries)
                {
                    query.Stop();
                }

                this.queries.Clear();
            }

            lock (this.registrations)
            {
                foreach (var registration in this.registrations)
                {
                    registration.Stop();
                }

                this.registrations.Clear();
            }

            if (this.server != null)
            {
                this.server.Stop();
            }
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
        public IServiceRegistration RegisterService(
            string serviceName, string protocol, int port, IDictionary<string, string> attributes)
        {
            var txtRecord = new TXTRecordClass();
            foreach (var pair in attributes)
            {
                txtRecord.SetValue(pair.Key, Encoding.ASCII.GetBytes(pair.Value));
            }

            var registration = new ServiceRegistration(this.server);
            lock (this.registrations)
            {
                this.registrations.Add(registration);
            }

            registration.Register(serviceName, protocol, null, port, txtRecord);

            return registration;
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
            var reg = (ServiceRegistration)registration;

            lock (this.registrations)
            {
                if (!this.registrations.Remove(reg))
                {
                    return;
                }
            }

            reg.Stop();
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
            var query = new Query(this.server, protocol);
            lock (this.queries)
            {
                this.queries.Add(query);
            }

            return query;
        }
    }
}