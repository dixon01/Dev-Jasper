// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetRefDataRequestMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetRefDataRequestMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// QnetRefDataRequestMessage class represents a message to request to a server to send back a reference data texts
    /// </summary>
    public class QnetRefDataRequestMessage : QnetRequestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetRefDataRequestMessage"/> class.
        /// The message contains no data. The message itself is enough
        /// </summary>
        /// <param name="sourceAdress">
        /// Qnet address of the requester
        /// </param>
        /// <param name="destAddr">
        /// Qnet address of the recipient
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetRefDataRequestMessage(ushort sourceAdress, ushort destAddr, ushort gatewayAddress)
            : base(sourceAdress, destAddr, gatewayAddress)
        {
        }
    }
}