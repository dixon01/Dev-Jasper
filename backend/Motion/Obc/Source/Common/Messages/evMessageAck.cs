// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evMessageAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Event to acknowledge (user) a received message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// Event to acknowledge (user) a received message
    /// </summary>
    public class evMessageAck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evMessageAck"/> class.
        /// </summary>
        /// <param name="messageId">
        /// Message ID to send the ACK for
        /// </param>
        public evMessageAck(int messageId)
        {
            this.MessageId = messageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evMessageAck"/> class.
        /// </summary>
        public evMessageAck()
        {
        }

        /// <summary>
        /// Gets or sets the message ID
        /// </summary>
        public int MessageId { get; set; }
    }
}