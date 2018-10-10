// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaCycleConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaCycleConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// The web.media cycle configuration.
    /// </summary>
    [Serializable]
    public class WebmediaCycleConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaCycleConfig"/> class.
        /// </summary>
        public WebmediaCycleConfig()
        {
            this.Enabled = true;
            this.Elements = new List<WebmediaElementBase>();
        }

        /// <summary>
        /// Gets or sets the name of the cycle.
        /// This information is not used by the application,
        /// but rather for the understandability of the file contents.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this cycle is enabled (should be shown).
        /// Default value is true.
        /// </summary>
        [XmlAttribute("Enabled")]
        [DefaultValue(true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the dynamic property configuration for the <see cref="Enabled"/> flag.
        /// </summary>
        [XmlElement("Enabled")]
        public DynamicProperty EnabledProperty { get; set; }

        /// <summary>
        /// Gets or sets the elements contained in this cycle.
        /// </summary>
        [XmlElement("Image", typeof(ImageWebmediaElement))]
        [XmlElement("Video", typeof(VideoWebmediaElement))]
        [XmlElement("Layout", typeof(LayoutWebmediaElement))]
        public List<WebmediaElementBase> Elements { get; set; }
    }
}