// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpResponse.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using Gorba.Common.Protocols.Udcp.Datagram;

    /// <summary>
    /// The UDCP response datagram.
    /// </summary>
    public class UdcpResponse : UdcpDatagram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpResponse"/> class.
        /// </summary>
        /// <param name="type">
        /// The datagram type.
        /// </param>
        /// <param name="unitAddress">
        /// The unit address.
        /// </param>
        public UdcpResponse(DatagramType type, UdcpAddress unitAddress)
            : this(new Header(HeaderFlags.Response, type, unitAddress))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpResponse"/> class.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        internal UdcpResponse(Header header)
            : base(header)
        {
        }
    }
}