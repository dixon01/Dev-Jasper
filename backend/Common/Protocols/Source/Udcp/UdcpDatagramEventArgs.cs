// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpDatagramEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpDatagramEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System;

    /// <summary>
    /// Event arguments with a <see cref="UdcpDatagram"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The subtype of <see cref="UdcpDatagram"/> used in this object.
    /// </typeparam>
    public class UdcpDatagramEventArgs<T> : EventArgs
        where T : UdcpDatagram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpDatagramEventArgs{T}"/> class.
        /// </summary>
        /// <param name="datagram">
        /// The datagram.
        /// </param>
        public UdcpDatagramEventArgs(T datagram)
        {
            this.Datagram = datagram;
        }

        /// <summary>
        /// Gets the datagram.
        /// </summary>
        public T Datagram { get; private set; }
    }
}