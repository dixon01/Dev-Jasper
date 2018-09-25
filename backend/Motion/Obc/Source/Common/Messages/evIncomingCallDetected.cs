// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evIncomingCallDetected.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evIncomingCallDetected type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// COS: 20 October 2010
    /// This class represents an event launched
    /// when a speech call is incoming.
    /// </summary>
    public class evIncomingCallDetected
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evIncomingCallDetected"/> class.
        /// </summary>
        public evIncomingCallDetected()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evIncomingCallDetected"/> class.
        /// </summary>
        /// <param name="mediaType">
        /// The speech media type: { 0 == Radio ; 1 == GSM }
        /// </param>
        public evIncomingCallDetected(byte mediaType)
        {
            this.MediaType = mediaType;
        }

        /// <summary>
        /// Gets or sets the media type for the incoming speech communication event
        /// just received.
        /// </summary>
        public byte MediaType { get; set; }
    }
}
