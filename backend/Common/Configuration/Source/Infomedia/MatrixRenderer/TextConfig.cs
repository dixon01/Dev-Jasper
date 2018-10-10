// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConfig.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.MatrixRenderer
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
            this.BlinkingPeriod = TimeSpan.FromSeconds(1);
            this.NumberOfBlinks = 0;
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

        /// <summary>
        /// Gets or sets the blink period.
        /// </summary>
        [XmlIgnore]
        public TimeSpan BlinkingPeriod { get; set; }

        /// <summary>
        /// Gets or sets the blink period (on + off times) as an XML serializable string.
        /// </summary>
        [XmlElement("BlinkingPeriod", DataType = "duration")]
        public string BlinkingPeriodXml
        {
            get
            {
                return XmlConvert.ToString(this.BlinkingPeriod);
            }

            set
            {
                this.BlinkingPeriod = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the number of blinks after which the text is always on..
        /// </summary>
        [XmlElement]
        public int NumberOfBlinks { get; set; }
    }
}