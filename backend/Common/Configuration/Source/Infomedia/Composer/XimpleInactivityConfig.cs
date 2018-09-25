// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleInactivityConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Composer
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for Ximple inactivity.
    /// </summary>
    [Serializable]
    public class XimpleInactivityConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleInactivityConfig"/> class.
        /// </summary>
        public XimpleInactivityConfig()
        {
            this.AtStartup = false;
            this.Timeout = TimeSpan.FromSeconds(60);
        }

        /// <summary>
        /// Gets or sets a value indicating whether at startup remote pc status must be set to false
        /// or not
        /// </summary>
        [XmlAttribute]
        public bool AtStartup { get; set; }

        /// <summary>
        /// Gets or sets the attribute called Timeout.
        /// Values admitted: positive, non-zero timespan. Default: 60 seconds.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the XML attribute called Timeout.
        /// Values admitted: positive, non-zero timespan. Default: 60 seconds.
        /// </summary>
        [XmlAttribute("Timeout", DataType = "duration")]
        public string TimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.Timeout);
            }

            set
            {
                this.Timeout = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
