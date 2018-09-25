// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RssFeedDynamicContentPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Dynamic
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The url news feed dynamic content part.
    /// </summary>
    public class RssFeedDynamicContentPart : DynamicContentPartBase
    {
        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the row of the table 5000 which should contain the feed data.
        /// </summary>
        public int TableRow { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval xml.
        /// </summary>
        [XmlAttribute("RefreshInterval", DataType = "duration")]
        public string RefreshIntervalXml
        {
            get
            {
                return XmlConvert.ToString(this.RefreshInterval);
            }

            set
            {
                this.RefreshInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the validity in minutes.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Validity { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval xml.
        /// </summary>
        [XmlAttribute("Validity", DataType = "duration")]
        public string ValidityXml
        {
            get
            {
                return XmlConvert.ToString(this.Validity);
            }

            set
            {
                this.Validity = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}