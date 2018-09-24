// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Playlist.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Playlist type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The playlist that can be assigned to a surface.
    /// </summary>
    public class Playlist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Playlist"/> class.
        /// </summary>
        public Playlist()
        {
            this.Tags = new List<PlaylistTag>();
            this.Elements = new List<PlaylistElement>();
        }

        /// <summary>
        /// Gets or sets the list of tags assigned to this playlist.
        /// </summary>
        [JsonProperty("tags")]
        public List<PlaylistTag> Tags { get; set; }

        /// <summary>
        /// Gets or sets the list of elements in this playlist.
        /// They are to be played in the given order (when they are "active")
        /// </summary>
        [JsonProperty("elements")]
        public List<PlaylistElement> Elements { get; set; }
    }
}