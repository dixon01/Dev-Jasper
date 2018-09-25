// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTextMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EciTextMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The ECI text message.
    /// </summary>
    public class EciTextMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.TextMessage;
            }
        }

        /// <summary>
        /// Gets or sets the sub-type.
        /// </summary>
        public char SubType { get; set; }

        /// <summary>
        /// Gets or sets the message id used for acknowledgment.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the target of the message.
        /// </summary>
        public MessageTarget Target { get; set; }

        /// <summary>
        /// Gets or sets the duration for which the message is shown at once.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the cycle time after which the message is to be re-shown.
        /// </summary>
        public TimeSpan CycleTime { get; set; }

        /// <summary>
        /// Gets or sets the total duration for which the message is shown.
        /// </summary>
        public TimeSpan TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the MP3 announcement ID.
        /// </summary>
        public int MessageMp3 { get; set; }

        /// <summary>
        /// Gets or sets the displayed text.
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the TTS text.
        /// </summary>
        public string TtsText { get; set; }
    }
}