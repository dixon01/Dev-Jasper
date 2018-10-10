// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Query.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Query type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using global::Bonjour;

    using NLog;

    /// <summary>
    /// The implementation of <see cref="IQuery"/> for Bonjour.
    /// </summary>
    internal partial class Query : IQuery
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DNSSDServiceClass server;

        private readonly string protocol;

        private readonly DNSSDEventManagerClass eventManager;

        private readonly Dictionary<string, ServiceInfo> services = new Dictionary<string, ServiceInfo>();

        private DNSSDService browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="server">
        /// The DNS-SD service.
        /// </param>
        /// <param name="protocol">
        /// The protocol name (e.g. <c>_http._tcp</c>).
        /// </param>
        public Query(DNSSDServiceClass server, string protocol)
        {
            this.server = server;
            this.protocol = protocol;

            this.eventManager = new DNSSDEventManagerClass();
            this.eventManager.ServiceFound += this.EventManagerOnServiceFound;
            this.eventManager.ServiceLost += this.EventManagerOnServiceLost;
            this.eventManager.OperationFailed += this.EventManagerOnOperationFailed;
        }

        /// <summary>
        /// Event that is fired when the <see cref="IQuery.Services"/> list changes.
        /// </summary>
        public event EventHandler ServicesChanged;

        /// <summary>
        /// Gets the list of found services.
        /// </summary>
        public IServiceInfo[] Services
        {
            get
            {
                lock (this.services)
                {
                    var result = new List<IServiceInfo>(this.services.Count);
                    foreach (var browseData in this.services.Values)
                    {
                        if (browseData.Addresses != null)
                        {
                            result.Add(browseData);
                        }
                    }

                    return result.ToArray();
                }
            }
        }

        /// <summary>
        /// Starts updating this query.
        /// </summary>
        public void Start()
        {
            if (this.browser != null)
            {
                return;
            }

            this.browser = this.server.Browse(0, 0, this.protocol, null, this.eventManager);
        }

        /// <summary>
        /// Stops updating this query.
        /// </summary>
        public void Stop()
        {
            if (this.browser != null)
            {
                this.browser.Stop();
                this.browser = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();

            this.eventManager.ServiceFound -= this.EventManagerOnServiceFound;
            this.eventManager.ServiceLost -= this.EventManagerOnServiceLost;
            this.eventManager.OperationFailed -= this.EventManagerOnOperationFailed;
        }

        private void RaiseServicesChanged()
        {
            var handler = this.ServicesChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void EventManagerOnServiceFound(
            DNSSDService service, DNSSDFlags flags, uint ifIndex, string serviceName, string regtype, string domain)
        {
            ServiceInfo serviceInfo;
            lock (this.services)
            {
                if (!this.services.TryGetValue(serviceName, out serviceInfo))
                {
                    serviceInfo = new ServiceInfo(serviceName, regtype, domain);
                    serviceInfo.PropertyChanged += this.BrowseDataOnPropertyChanged;
                    this.services.Add(serviceName, serviceInfo);
                }
            }

            lock (serviceInfo)
            {
                serviceInfo.ReferenceCount++;
                if (serviceInfo.ReferenceCount > 1)
                {
                    return;
                }
            }

            serviceInfo.Resolve(this.server, ifIndex);
        }

        private void EventManagerOnServiceLost(
            DNSSDService service, DNSSDFlags flags, uint ifIndex, string serviceName, string regtype, string domain)
        {
            lock (this.services)
            {
                ServiceInfo serviceInfo;
                if (!this.services.TryGetValue(serviceName, out serviceInfo))
                {
                    return;
                }

                lock (serviceInfo)
                {
                    serviceInfo.ReferenceCount--;
                    if (serviceInfo.ReferenceCount != 0)
                    {
                        return;
                    }
                }

                this.services.Remove(serviceName);
            }

            this.RaiseServicesChanged();
        }

        private void EventManagerOnOperationFailed(DNSSDService service, DNSSDError error)
        {
            Logger.Warn("Query operation failed: {0}", error);
        }

        private void BrowseDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Addresses")
            {
                this.RaiseServicesChanged();
            }
        }
    }
}