// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evSpeechMediaType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evSpeechMediaType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// COS: 19 October 2010
    /// This class represents an event containing the information about
    /// a bus stop ID and its corresponding speech media type.
    /// </summary>
    public class evSpeechMediaType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechMediaType"/> class.
        /// </summary>
        public evSpeechMediaType()
        {
            this.StopId = 0;
            this.MediaType = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechMediaType"/> class.
        /// </summary>
        /// <param name="stopId">The bus stop ID</param>
        /// <param name="mediaType">The speech media type allowed for this bus stop.</param>
        public evSpeechMediaType(int stopId, byte mediaType)
        {
            this.StopId = stopId;
            this.MediaType = mediaType;
        }

        /// <summary>
        /// Gets or sets the bus stop ID.
        /// </summary>
        public int StopId { get; set; }

        /// <summary>
        /// Gets or sets the speech media type associated to this bus stop ID.
        /// </summary>
        /// <remarks>Attention: the values admitted are
        /// Radio = 0;
        /// GSM = 1;
        /// </remarks>
        public byte MediaType { get; set; }
    }
}