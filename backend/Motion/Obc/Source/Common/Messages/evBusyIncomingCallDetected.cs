// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evBusyIncomingCallDetected.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// COS: 20 October 2010
    /// This class represents an event launched
    /// when a speech call is incoming.
    /// </summary>
    public class evBusyIncomingCallDetected
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evBusyIncomingCallDetected"/> class.
        /// </summary>
        public evBusyIncomingCallDetected()
        {
            this.MediaType = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evBusyIncomingCallDetected"/> class.
        /// </summary>
        /// <param name="mediaType">The speech media type: { 0 == Radio ; 1 == GSM }</param>
        public evBusyIncomingCallDetected(byte mediaType)
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