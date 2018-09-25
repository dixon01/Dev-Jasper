// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery.Messages
{
    using System.Collections.Generic;
    using System.Net;

    using Gorba.Common.Protocols.DnsServiceDiscovery;

    /// <summary>
    /// The service information.
    /// Do not use this class outside this namespace, it is only public for XML serialization.
    /// </summary>
    public class ServiceInfo : ServiceInfoBase, IServiceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        public ServiceInfo()
        {
            this.Addresses = new List<string>();
        }

        /// <summary>
        /// Gets or sets the IP addresses of the service.
        /// </summary>
        public List<string> Addresses { get; set; }

        string IServiceInfo.FullName
        {
            get
            {
                return this.Name + "." + this.Protocol;
            }
        }

        IServiceAttribute[] IServiceInfo.Attributes
        {
            get
            {
                return this.Attributes.ConvertAll(a => (IServiceAttribute)a).ToArray();
            }
        }

        IPAddress[] IServiceInfo.Addresses
        {
            get
            {
                // needed for CF 3.5:
                // ReSharper disable RedundantTypeArgumentsOfMethod
                return this.Addresses.ConvertAll<IPAddress>(IPAddress.Parse).ToArray();
                // ReSharper restore RedundantTypeArgumentsOfMethod
            }
        }

        string IServiceInfo.GetAttribute(string name)
        {
            var attribute = this.Attributes.Find(a => a.Name == name);
            return attribute == null ? null : attribute.Value;
        }
    }
}