// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetTimeSyncRequestMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetTimeSyncRequestMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Qnet message request for time synchronization with a server.
    /// </summary>
    public class QnetTimeSyncRequestMessage : QnetRequestMessage
    {
        private readonly DateTime receiveTime;

        private readonly DateTime originateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetTimeSyncRequestMessage"/> class.
        /// </summary>
        /// <param name="sourceAdress">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="originateTime">
        /// The originate time.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway address.
        /// </param>
        public QnetTimeSyncRequestMessage(
            ushort sourceAdress, ushort destAddr, DateTime originateTime, ushort gatewayAddress)
            : base(sourceAdress, destAddr, gatewayAddress)
        {
            this.receiveTime = DateTime.Now;
            this.originateTime = originateTime;
        }

        /// <summary>
        /// Gets the time when the message was received.
        /// </summary>
        public DateTime ReceiveTime
        {
            get { return this.receiveTime; }
        }

        /// <summary>
        /// Gets the time when the message was originated.
        /// </summary>
        public DateTime OriginateTime
        {
            get { return this.originateTime; }
        }
    }
}