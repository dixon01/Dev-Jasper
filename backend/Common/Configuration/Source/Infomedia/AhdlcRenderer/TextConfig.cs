// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for text rendering.
    /// </summary>
    [Serializable]
    public class TextConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextConfig"/> class.
        /// </summary>
        public TextConfig()
        {
            this.AlternationInterval = TimeSpan.FromSeconds(7);
        }

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
    }
}