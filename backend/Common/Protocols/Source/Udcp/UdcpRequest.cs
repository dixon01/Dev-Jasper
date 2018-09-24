// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using Gorba.Common.Protocols.Udcp.Datagram;

    /// <summary>
    /// The UDCP request datagram.
    /// </summary>
    public class UdcpRequest : UdcpDatagram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpRequest"/> class.
        /// </summary>
        /// <param name="type">
        /// The datagram type.
        /// </param>
        /// <param name="unitAddress">
        /// The unit address.
        /// </param>
        public UdcpRequest(DatagramType type, UdcpAddress unitAddress)
            : this(new Header(HeaderFlags.None, type, unitAddress))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpRequest"/> class.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        internal UdcpRequest(Header header)
            : base(header)
        {
        }
    }
}