// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleStyle.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ModuleStyle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// The style of a module.
    /// This is only relevant for dynamic text like RSS.
    /// </summary>
    public class ModuleStyle
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        [JsonProperty("font_family")]
        public string FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        [JsonProperty("font_size")]
        public int? FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        [JsonProperty("font_style")]
        public string FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        [JsonProperty("font_weight")]
        public string FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        [JsonProperty("text_align")]
        public string TextAlign { get; set; }

        /// <summary>
        /// Gets or sets the text decoration.
        /// </summary>
        [JsonProperty("text_decoration")]
        public string TextDecoration { get; set; }
    }
}