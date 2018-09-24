// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpDatagram.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpDatagram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;

    /// <summary>
    /// Base class for all UDCP datagrams.
    /// </summary>
    public abstract class UdcpDatagram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpDatagram"/> class.
        /// </summary>
        /// <param name="header">
        /// The header of the datagram.
        /// </param>
        protected UdcpDatagram(Header header)
        {
            this.Header = header;
            this.Fields = new List<Field>();
        }

        /// <summary>
        /// Gets the UDCP header of this datagram.
        /// </summary>
        public Header Header { get; private set; }

        /// <summary>
        /// Gets all the fields defined in this datagram.
        /// </summary>
        public List<Field> Fields { get; private set; }

        /// <summary>
        /// Gets the first field of a given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of field to be returned
        /// </typeparam>
        /// <returns>
        /// The first field of the given type or null if no field of that type exists.
        /// </returns>
        public T GetField<T>() where T : Field
        {
            foreach (var field in this.Fields)
            {
                var result = field as T;
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
