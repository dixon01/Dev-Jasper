// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisUdpServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisUdpServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// The UDP server that multicasts IBIS-IP data (for multicast-services).
    /// </summary>
    public class IbisUdpServer : IDisposable
    {
        private readonly Dictionary<string, UdpServiceHandlerBase> handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisUdpServer"/> class.
        /// </summary>
        public IbisUdpServer()
        {
            this.handlers = new Dictionary<string, UdpServiceHandlerBase>();
        }

        /// <summary>
        /// Gets the local IP addresses on which this server is listening.
        /// </summary>
        public IPAddress[] LocalAddresses
        {
            get
            {
                var addresses = new List<IPAddress>();
                foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        addresses.Add(ip);
                    }
                }

                return addresses.ToArray();
            }
        }

        /// <summary>
        /// Adds a service to this server.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service.
        /// It must be an <see cref="IVdv301UdpService"/> as well as implementing
        /// <see cref="IVdv301ServiceImpl"/>.
        /// </typeparam>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The multicast IP address and port number used by this server to publish the given service.
        /// </returns>
        public IPEndPoint AddService<TService>(TService service)
            where TService : IVdv301UdpService
        {
            var handler = UdpServiceHandlerBase.Create(service);
            this.handlers.Add(handler.Name, handler);
            handler.Start();
            return handler.EndPoint;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var handler in this.handlers.Values)
            {
                handler.Stop();
            }

            this.handlers.Clear();
        }
    }
}
