// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TTSFrame.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TTSFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The TTS frame.
    /// </summary>
    public class TTSFrame : TTSoverIBIS
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TTSFrame"/> class.
        /// </summary>
        public TTSFrame()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TTSFrame"/> class.
        /// </summary>
        /// <param name="outDst">Out source (Display, Speaker, Both)</param>
        /// <param name="duration">Time in seconds to show a message</param>
        /// <param name="message">The message</param>
        public TTSFrame(evTTSFrame.OutDest outDst, int duration, string message)
        {
            this.OutDst = outDst;
            this.Duration = duration;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds to show a message
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the output source (Display, Speaker, Both)
        /// </summary>
        public evTTSFrame.OutDest OutDst { get; set; }
    }
}