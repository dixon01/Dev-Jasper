// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Content.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Content type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// A piece of content referenced from <see cref="PlaylistElement.ContentId"/>.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Gets or sets the id of this content.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL to the raster image used as the background of the content.
        /// If the content only contains images and texts, then this is the only thing to be displayed,
        /// in other cases the <see cref="Modules"/> contain all other elements to be displayed.
        /// </summary>
        [JsonProperty("raster_url")]
        public string RasterUrl { get; set; }

        /// <summary>
        /// Gets or sets the width of the entire content.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the entire content.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the list of modules in this content.
        /// </summary>
        [JsonProperty("modules")]
        public List<ContentModule> Modules { get; set; }
    }
}