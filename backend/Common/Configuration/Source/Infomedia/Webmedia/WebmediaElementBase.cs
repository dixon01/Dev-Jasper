// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaElementBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaElementBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Base class for all web.media elements used in a <see cref="WebmediaCycleConfig"/>.
    /// </summary>
    [Serializable]
    public abstract class WebmediaElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaElementBase"/> class.
        /// </summary>
        protected WebmediaElementBase()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the name.
        /// This information is not used by the application,
        /// but rather for the understandability of the file contents.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the frame id.
        /// Currently only 0 is supported, which means full-screen.
        /// </summary>
        [XmlAttribute("Frame")]
        public int Frame { get; set; }

        /// <summary>
        /// Gets or sets the duration as a string for XML serialization.
        /// </summary>
        [XmlAttribute("Duration")]
        public string DurationXml
        {
            get
            {
                return XmlConvert.ToString(this.Duration);
            }

            set
            {
                this.Duration = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the duration for which to show the element.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is enabled (should be shown).
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
    }
}