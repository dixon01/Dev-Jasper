// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// The general player config.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets the widgets URL.
        /// </summary>
        [JsonProperty("widgets_url")]
        public string WidgetsUrl { get; set; }

        /// <summary>
        /// Gets or sets the remote resource URL where resources can be downloaded.
        /// This should normally not be used since resources are available from multi-status.
        /// </summary>
        [JsonProperty("remote_resource_url")]
        public string RemoteResourceUrl { get; set; }

        /// <summary>
        /// Gets or sets the ping URL to which the player should report every
        /// <see cref="BroadcastLocation.ReportInterval"/> seconds.
        /// </summary>
        [JsonProperty("ping_url")]
        public string PingUrl { get; set; }

        /// <summary>
        /// Gets or sets the player id.
        /// </summary>
        [JsonProperty("player_id")]
        public int PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the unique player token.
        /// </summary>
        [JsonProperty("player_token")]
        public string PlayerToken { get; set; }
    }
}