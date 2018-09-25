// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EciAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The ECI acknowledge message.
    /// </summary>
    public class EciAck : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Ack;
            }
        }

        /// <summary>
        /// Gets or sets the type of acknowledgment.
        /// </summary>
        public AckType Type { get; set; }

        /// <summary>
        /// Gets or sets the value related to the type.
        /// </summary>
        public int Value { get; set; }
    }
}
