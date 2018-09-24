// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

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
            this.FontQuality = FontQualities.AntiAliased;
            this.AlternationInterval = TimeSpan.FromSeconds(3);
            this.BlinkInterval = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Gets or sets the text mode.
        /// Default value is <see cref="DirectXRenderer.TextMode.FontSprite"/>.
        /// </summary>
        [DefaultValue(TextMode.FontSprite)]
        public TextMode TextMode { get; set; }

        /// <summary>
        /// Gets or sets the font quality.
        /// Default value is <see cref="FontQualities.AntiAliased"/>.
        /// </summary>
        [DefaultValue(FontQualities.AntiAliased)]
        public FontQualities FontQuality { get; set; }

        /// <summary>
        /// Gets or sets the alternation interval.
        /// </summary>
        [XmlIgnore]
        public TimeSpan AlternationInterval { get; set; }

        /// <summary>
        /// Gets or sets the alternation interval as an XML serializable string.
        /// </summary>
        [XmlElement("AlternationInterval", DataType = "duration")]
        public string AlternationIntervalXml
        {
            get
            {
                return XmlConvert.ToString(this.AlternationInterval);
            }

            set
            {
                this.AlternationInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the blink interval.
        /// </summary>
        [XmlIgnore]
        public TimeSpan BlinkInterval { get; set; }

        /// <summary>
        /// Gets or sets the blink interval as an XML serializable string.
        /// </summary>
        [XmlElement("BlinkInterval", DataType = "duration")]
        public string BlinkIntervalXml
        {
            get
            {
                return XmlConvert.ToString(this.BlinkInterval);
            }

            set
            {
                this.BlinkInterval = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
