// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetRequestMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetRequestMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Base class for requests message like QnetRefDataRequestMessage and QnetScheduledDataRequestMessage.
    /// </summary>
    public class QnetRequestMessage : QnetMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetRequestMessage"/> class.
        /// </summary>
        /// <param name="sourceAdress">
        /// The source address.
        /// </param>
        /// <param name="destAddress">
        /// The destination address.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetRequestMessage(ushort sourceAdress, ushort destAddress, ushort gatewayAddress)
            : base(sourceAdress, destAddress, gatewayAddress)
        {
        }
    }
}