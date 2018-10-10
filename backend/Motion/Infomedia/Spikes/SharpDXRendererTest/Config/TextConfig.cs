// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
{
    using System;
    using System.ComponentModel;

    using SharpDX.Direct3D9;

    /// <summary>
    /// The text config.
    /// </summary>
    [Serializable]
    public class TextConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextConfig"/> class.
        /// </summary>
        public TextConfig()
        {
            this.TextMode = TextMode.FontSprite;
            this.FontQuality = FontQuality.Antialiased;
        }

        /// <summary>
        /// Gets or sets the text mode.
        /// Default value is <see cref="Config.TextMode.FontSprite"/>.
        /// </summary>
        [DefaultValue(TextMode.FontSprite)]
        public TextMode TextMode { get; set; }

        /// <summary>
        /// Gets or sets the font quality.
        /// Default value is <see cref="SharpDX.Direct3D9.FontQuality.Antialiased"/>.
        /// </summary>
        [DefaultValue(FontQuality.Antialiased)]
        public FontQuality FontQuality { get; set; }
    }
}
