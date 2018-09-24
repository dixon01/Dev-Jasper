// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerData.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The root item of the player data read from data.json on ScreenGate.
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerData"/> class.
        /// </summary>
        public PlayerData()
        {
            this.Config = new Config();
            this.BroadcastLocations = new List<BroadcastLocation>();
            this.WidgetsResources = new List<WidgetsResource>();
            this.Contents = new List<Content>();
        }

        /// <summary>
        /// Gets or sets the general player config.
        /// </summary>
        [JsonProperty("config")]
        public Config Config { get; set; }

        /// <summary>
        /// Gets or sets the list of broadcast locations.
        /// </summary>
        [JsonProperty("broadcast_locations")]
        public List<BroadcastLocation> BroadcastLocations { get; set; }

        /// <summary>
        /// Gets or sets the list of widgets resources.
        /// </summary>
        [JsonProperty("widgets_resources")]
        public List<WidgetsResource> WidgetsResources { get; set; }

        /// <summary>
        /// Gets or sets the list of contents.
        /// </summary>
        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }
    }
}
