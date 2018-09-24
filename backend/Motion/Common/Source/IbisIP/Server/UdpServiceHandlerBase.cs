// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpServiceHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpServiceHandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for all generated UDP service handlers.
    /// </summary>
    internal abstract partial class UdpServiceHandlerBase
    {
        private static readonly Random Random = new Random();

        private readonly UdpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServiceHandlerBase"/> class.
        /// </summary>
        protected UdpServiceHandlerBase()
        {
            // generate a random address in the 239.0.0.0/24 range
            var address = new byte[4];
            Random.NextBytes(address);
            address[0] = 239;
            if (address[3] == 255 || address[3] == 0)
            {
                // not sure if we are allowed to have "broadcast" (.255) addresses in multicast networks
                // not sure if we are allowed to have "network" (.0) addresses in multicast networks
                address[3] = 1;
            }

            this.EndPoint = new IPEndPoint(new IPAddress(address), Random.Next(1024, 65535));
            this.client = new UdpClient();

            // TODO: do we need this? (not available in CF 3.5)
            ////this.client.ExclusiveAddressUse = true;
            ////this.client.MulticastLoopback = false;
            this.client.Connect(this.EndPoint);
        }

        /// <summary>
        /// Gets the name of this handler to identify it.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the multicast IP address and UDP port on which this handler will
        /// multicast the datagrams.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// Starts this handler by joining the multicast group.
        /// </summary>
        public virtual void Start()
        {
            this.client.JoinMulticastGroup(this.EndPoint.Address);
        }

        /// <summary>
        /// Stops this handler by leaving the multicast group.
        /// </summary>
        public virtual void Stop()
        {
            this.client.DropMulticastGroup(this.EndPoint.Address);
        }

        /// <summary>
        /// Multicasts a datagram with the given object as an XML structure to
        /// everybody that is registered in the multicast group (<seealso cref="EndPoint"/>).
        /// </summary>
        /// <param name="data">
        /// The data object to be serialized.
        /// </param>
        /// <typeparam name="T">
        /// The type of object passed as an argument.
        /// </typeparam>
        protected void SendDatagram<T>(T data)
        {
            using (var memory = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(memory, data);

                this.client.Send(memory.ToArray(), (int)memory.Length);
            }
        }
    }
}
