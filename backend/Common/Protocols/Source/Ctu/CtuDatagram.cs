// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtuDatagram.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu
{
    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Representation of a whole CTU datagram
    /// using an object oriented style.
    /// </summary>
    public class CtuDatagram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtuDatagram"/> class
        /// with a the default header and the default payload.
        /// </summary>
        public CtuDatagram()
            : this(new Header(), new Payload())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtuDatagram"/> class
        /// with a specific header and the default payload.
        /// </summary>
        /// <param name="header">The header to use with this CTU datagram.</param>
        public CtuDatagram(Header header)
            : this(header, new Payload())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CtuDatagram"/> class
        /// with a specific header and a specific payload.
        /// </summary>
        /// <param name="header">The header to use with this CTU datagram.</param>
        /// <param name="payload">The payload to use with this CTU datagram.</param>
        public CtuDatagram(Header header, Payload payload)
        {
            this.Header = header;
            this.Payload = payload;
        }

        /// <summary>
        /// Gets or sets CTU datagram's header.
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Gets or sets CTU datagram's payload.
        /// </summary>
        public Payload Payload { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}]", this.Header, this.Payload);
        }
    }
}
