// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LcdConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.EPaper.MainUnit
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The LCD config.
    /// </summary>
    [Serializable]
    [XmlRoot("Lcd")]
    public class LcdConfig
    {
        /// <summary>
        /// Gets or sets the refresh interval after which the unit requests the L.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RefreshInterval { get; set; }

        /// <summary>
        /// Gets or sets the refresh interval as an XML serializable string.
        /// </summary>
        [XmlAttribute("RefreshInterval")]
        public string RefreshIntervalString
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
    }
}