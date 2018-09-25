// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evIraMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evIraMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The IRA message event.
    /// </summary>
    public class evIraMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evIraMessage"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public evIraMessage(int messageType, int message)
        {
            this.MessageType = messageType;
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evIraMessage"/> class.
        /// </summary>
        public evIraMessage()
        {
        }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        public int Message { get; set; }
    }
}