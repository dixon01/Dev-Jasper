// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Text;

    using global::Bonjour;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The <see cref="IServiceInfo"/> implementation for Bonjour.
    /// </summary>
    internal partial class ServiceInfo : IServiceInfo, INotifyPropertyChanged, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ServiceInfo>();

        private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

        private readonly string domain;

        private readonly string regType;

        private DNSSDEventManagerClass eventManager;

        private DNSSDService resolver;

        private DNSSDService addressQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The service name (without protocol or ".local").
        /// </param>
        /// <param name="regType">
        /// The registration service type (without ".local").
        /// </param>
        /// <param name="domain">
        /// The domain name (e.g. "local").
        /// </param>
        public ServiceInfo(string name, string regType, string domain)
        {
            this.Name = name;
            this.domain = domain;
            this.regType = regType;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the number of references to this service (e.g. from different network interfaces).
        /// </summary>
        public int ReferenceCount { get; set; }

        /// <summary>
        /// Gets the service name (without protocol or ".local").
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the protocol name (including ".local").
        /// </summary>
        public string Protocol
        {
            get
            {
                return this.regType.TrimEnd('.') + "." + this.domain.TrimEnd('.');
            }
        }

        /// <summary>
        /// Gets the full service name (including protocol and ".local").
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format("{0}.{1}", this.Name, this.Protocol);
            }
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the addresses where this service can be found.
        /// </summary>
        public IPAddress[] Addresses { get; private set; }

        IServiceAttribute[] IServiceInfo.Attributes
        {
            get
            {
                var result = new IServiceAttribute[this.attributes.Count];
                var i = 0;
                foreach (var attribute in this.attributes)
                {
                    result[i++] = new Attribute { Name = attribute.Key, Value = attribute.Value };
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the given attribute.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The attribute value or null if not found.
        /// </returns>
        public string GetAttribute(string name)
        {
            string value;
            this.attributes.TryGetValue(name, out value);
            return value;
        }

        /// <summary>
        /// Starts the resolution of this service information using the given <paramref name="server"/>.
        /// </summary>
        /// <param name="server">
        /// The DNS-SD service.
        /// </param>
        /// <param name="interfaceIndex">
        /// The network interface index.
        /// </param>
        public void Resolve(IDNSSDService server, uint interfaceIndex)
        {
            if (this.eventManager == null)
            {
                this.eventManager = new DNSSDEventManagerClass();
                this.eventManager.ServiceResolved += this.EventManagerOnServiceResolved;
                this.eventManager.AddressFound += this.EventManagerOnAddressFound;
                this.eventManager.OperationFailed += this.EventManagerOnOperationFailed;
            }

            this.resolver = server.Resolve(
                0, interfaceIndex, this.Name, this.regType, this.domain, this.eventManager);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.StopOperation(this.resolver);
            this.StopOperation(this.addressQuery);

            if (this.eventManager != null)
            {
                this.eventManager.ServiceResolved -= this.EventManagerOnServiceResolved;
                this.eventManager.AddressFound -= this.EventManagerOnAddressFound;
                this.eventManager.OperationFailed -= this.EventManagerOnOperationFailed;
                this.eventManager = null;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void StopOperation(IDNSSDService operation)
        {
            if (operation != null)
            {
                operation.Stop();
            }
        }

        private void EventManagerOnServiceResolved(
            DNSSDService service,
            DNSSDFlags flags,
            uint ifIndex,
            string fullname,
            string hostname,
            ushort port,
            TXTRecord record)
        {
            this.Port = port;
            this.RaisePropertyChanged("Port");

            if (record != null)
            {
                var count = record.GetCount();
                for (uint i = 0; i < count; i++)
                {
                    var key = record.GetKeyAtIndex(i);
                    var bytes = (byte[])record.GetValueAtIndex(i);

                    if (key.Length == 0)
                    {
                        continue;
                    }

                    var value = bytes != null ? Encoding.ASCII.GetString(bytes, 0, bytes.Length) : string.Empty;

                    this.attributes[key] = value;
                    this.RaisePropertyChanged("Attributes");
                }
            }

            this.StopOperation(this.resolver);

            this.addressQuery = service.GetAddrInfo(
                0, ifIndex, DNSSDAddressFamily.kDNSSDAddressFamily_IPv4, hostname, this.eventManager);
        }

        private void EventManagerOnAddressFound(
            DNSSDService service,
            DNSSDFlags flags,
            uint ifIndex,
            string hostname,
            DNSSDAddressFamily addressFamily,
            string address,
            uint ttl)
        {
            this.Addresses = new[] { IPAddress.Parse(address) };
            this.RaisePropertyChanged("Addresses");

            this.StopOperation(this.addressQuery);
        }

        private void EventManagerOnOperationFailed(DNSSDService service, DNSSDError error)
        {
            Logger.Warn("Resolution operation failed: {0}", error);
        }

        private class Attribute : IServiceAttribute
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}