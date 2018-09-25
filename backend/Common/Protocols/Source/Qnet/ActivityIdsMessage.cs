// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityIdsMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the qnet vdv messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of the qnet vdv messages
    /// </summary>
    public class ActivityIdsMessage : QnetMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityIdsMessage"/> class. Used for QMAIL with TFTP protocol.
        /// </summary>
        /// <param name="srcAddr">
        /// The qnet source address of the sender of the message.
        /// </param>
        /// <param name="destAddr">
        /// The qnet destination address of the message.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public ActivityIdsMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
            this.ActivityIdsList = new List<uint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityIdsMessage"/> class.
        /// The source and destination addresses are set with <see cref="QnetConstantes.QnetAddrNone"/> by default.
        /// </summary>
        public ActivityIdsMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Gets the list of identifiers of active activities on iqube.
        /// </summary>
        public List<uint> ActivityIdsList { get; private set; }
    }
}