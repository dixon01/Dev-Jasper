// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcastLocation.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BroadcastLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The information about a single broadcast location.
    /// </summary>
    public class BroadcastLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastLocation"/> class.
        /// </summary>
        public BroadcastLocation()
        {
            this.Surfaces = new List<Surface>();
        }

        /// <summary>
        /// Gets or sets the broadcast location id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the broadcast location.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the report interval in seconds after which the player
        /// should report back to the backend.
        /// </summary>
        [JsonProperty("report_interval")]
        public int ReportInterval { get; set; }

        /// <summary>
        /// Gets or sets the latitude as a decimal string (e.g. 47.1192499).
        /// </summary>
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude as a decimal string (e.g. 7.2610375).
        /// </summary>
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets the country code (e.g. CH).
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the street name and number.
        /// </summary>
        [JsonProperty("street")]
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the zip code of the city.
        /// </summary>
        [JsonProperty("npa")]
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the width in pixels of the size of the screen.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in pixels of the size of the screen.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the background color in the format <c>#RRGGBB</c>.
        /// </summary>
        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the list of surfaces which can be used on this broadcast location.
        /// </summary>
        [JsonProperty("surfaces")]
        public List<Surface> Surfaces { get; set; }
    }
}