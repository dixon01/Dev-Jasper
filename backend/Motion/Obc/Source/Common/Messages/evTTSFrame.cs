// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evTTSFrame.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evTTSFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;

    /// <summary>
    /// Event send text to be displayed and spoken (TTS)
    /// </summary>
    public class evTTSFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evTTSFrame"/> class.
        /// </summary>
        public evTTSFrame()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTTSFrame"/> class.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="outDst">
        /// The out destination.
        /// </param>
        /// <param name="duration">
        /// Time in seconds to show a message.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="speechText">
        /// The speech text.
        /// </param>
        /// <param name="cycleTime">
        /// The cycle time.
        /// </param>
        /// <param name="totalDuration">
        /// The total duration.
        /// </param>
        public evTTSFrame(
            int messageId,
            OutDest outDst,
            short duration,
            string displayText,
            string speechText,
            short cycleTime,
            short totalDuration)
        {
            this.MessageId = messageId;
            this.OutDst = outDst;
            this.Duration = duration;
            this.DisplayText = displayText;
            this.SpeechText = speechText;
            this.CycleTime = cycleTime;
            this.TotalDuration = totalDuration;
        }

        /// <summary>
        /// Enum for the output destination
        /// </summary>
        [Flags]
        public enum OutDest
        {
            /// <summary>
            /// Only for speaker
            /// </summary>
            Speaker = 1,

            /// <summary>
            /// Only for the display
            /// </summary>
            Display = 2,

            /// <summary>
            /// For the speaker and display
            /// </summary>
            SpeakerAndDisplay = 3,
        }

        /// <summary>
        /// Gets or sets the ID for this message
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the message displayed
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the message spoken (TTS)
        /// </summary>
        public string SpeechText { get; set; }

        /// <summary>
        /// Gets or sets the output source (Display, Speaker, Both)
        /// </summary>
        public OutDest OutDst { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds to show a message
        /// </summary>
        public short Duration { get; set; }

        /// <summary>
        /// Gets or sets the cycle time in minutes
        /// </summary>
        public short CycleTime { get; set; }

        /// <summary>
        /// Gets or sets the total duration the message is active in minutes
        /// </summary>
        public short TotalDuration { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evTTSFrame. id: " + this.MessageId + ", Displaytext: " + this.DisplayText;
        }
    }
}