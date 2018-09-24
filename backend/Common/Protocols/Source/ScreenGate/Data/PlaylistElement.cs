// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaylistElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlaylistElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// A single element in the playlist.
    /// </summary>
    public class PlaylistElement
    {
        /// <summary>
        /// Gets or sets the element id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the duration of the element in seconds.
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the id of the <see cref="Content"/> to be played at this position.
        /// </summary>
        [JsonProperty("content_id")]
        public int ContentId { get; set; }

        /// <summary>
        /// Gets or sets the broadcast planning for this element.
        /// </summary>
        [JsonProperty("broadcast_planning")]
        public BroadcastPlanning BroadcastPlanning { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the broadcast starts.
        /// </summary>
        [JsonProperty("broadcast_start")]
        public DateTime BroadcastStart { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the broadcast ends.
        /// </summary>
        [JsonProperty("broadcast_end")]
        public DateTime BroadcastEnd { get; set; }
    }
}