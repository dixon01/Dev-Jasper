// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreventPopupsConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PreventPopupsConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    using global::Gorba.Common;

    /// <summary>
    /// The popup blocking config.
    /// </summary>
    [Serializable]
    public class PreventPopupsConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreventPopupsConfig"/> class.
        /// </summary>
        public PreventPopupsConfig()
        {
            this.Enabled = true;
            this.CheckInterval = TimeSpan.FromSeconds(30);
            this.Popups = new List<PopupConfig>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether popup blocking is enabled.
        /// Default value is true.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the interval at which to check open windows
        /// (see <see cref="Popups"/>).
        /// </summary>
        [XmlIgnore]
        public TimeSpan CheckInterval { get; set; }

        /// <summary>
        /// Gets or sets the interval at which to check open windows as an XML string.
        /// </summary>
        [XmlAttribute("CheckInterval", DataType = "duration")]
        public string CheckIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.CheckInterval);
            }

            set
            {
                this.CheckInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the popups to prevent.
        /// </summary>
        [XmlElement("Popup", typeof(PopupConfig))]
        public List<PopupConfig> Popups { get; set; }
    }
}