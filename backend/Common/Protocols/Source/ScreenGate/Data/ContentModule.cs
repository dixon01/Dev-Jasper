// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentModule.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContentModule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// A single module in a <see cref="Content"/>.
    /// </summary>
    public class ContentModule
    {
        /// <summary>
        /// Gets or sets the kind of content.
        /// Currently <c>video</c> and <c>rss</c> are known, others might exist.
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate where the module should be shown on the surface.
        /// </summary>
        [JsonProperty("x")]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate where the module should be shown on the surface.
        /// </summary>
        [JsonProperty("y")]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the Z index of where to show the module.
        /// A higher Z index means the item is shown higher up.
        /// </summary>
        [JsonProperty("z")]
        public int Z { get; set; }

        /// <summary>
        /// Gets or sets the width of the module.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the module.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// This property is used for RSS.
        /// </summary>
        [JsonProperty("speed")]
        public int? Speed { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// Known values are <c>rtl</c> and <c>ltr</c>.
        /// This property is used for RSS.
        /// </summary>
        [JsonProperty("direction")]
        public string Direction { get; set; }

        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value as an object.
        /// </summary>
        [JsonProperty("object_value")]
        public object ObjectValue { get; set; }

        /// <summary>
        /// Gets or sets the style of the module.
        /// </summary>
        [JsonProperty("style")]
        public ModuleStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the URL where the resource for this module can be found.
        /// </summary>
        [JsonProperty("resource_url")]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource width.
        /// </summary>
        [JsonProperty("resource_width")]
        public int? ResourceWidth { get; set; }

        /// <summary>
        /// Gets or sets the resource height.
        /// </summary>
        [JsonProperty("resource_height")]
        public int? ResourceHeight { get; set; }

        /// <summary>
        /// Gets or sets the resource duration in seconds.
        /// </summary>
        [JsonProperty("resource_duration")]
        public double? ResourceDuration { get; set; }

        /// <summary>
        /// Gets or sets the URL of the resource (video) in MP4 format.
        /// </summary>
        [JsonProperty("resource_mp4_url")]
        public string ResourceMp4Url { get; set; }

        /// <summary>
        /// Gets or sets the URL of the resource (video) in H264 format.
        /// </summary>
        [JsonProperty("resource_h264_url")]
        public string ResourceH264Url { get; set; }

        /// <summary>
        /// Gets or sets the URL of the resource (video) in WebM format.
        /// </summary>
        [JsonProperty("resource_webm_url")]
        public string ResourceWebmUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to scale the contents to fit it to the given size.
        /// </summary>
        [JsonProperty("scale_to_fit")]
        public bool ScaleToFit { get; set; }
    }
}