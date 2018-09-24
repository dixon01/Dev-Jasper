// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Surface.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Surface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// The description of a single surface on a broadcast location.
    /// </summary>
    public class Surface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Surface"/> class.
        /// </summary>
        public Surface()
        {
            this.Playlist = new Playlist();
        }

        /// <summary>
        /// Gets or sets the X coordinate (in pixels) where the surface starts.
        /// </summary>
        [JsonProperty("x")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate (in pixels) where the surface starts.
        /// </summary>
        [JsonProperty("y")]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the surface in pixels.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the surface in pixels.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the sound volume between 0 and 1.
        /// </summary>
        [JsonProperty("volume")]
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the playlist assigned to this surface.
        /// </summary>
        [JsonProperty("playlist")]
        public Playlist Playlist { get; set; }
    }
}