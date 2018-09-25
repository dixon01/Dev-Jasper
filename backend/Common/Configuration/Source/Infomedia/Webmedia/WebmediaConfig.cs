// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// The web media config root element.
    /// </summary>
    [XmlRoot("WebMedia")]
    [Serializable]
    public class WebmediaConfig
    {
        /// <summary>
        /// The current file version number (1.2).
        /// </summary>
        public static readonly Version CurrentVersion = new Version(1, 2);

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaConfig"/> class.
        /// </summary>
        public WebmediaConfig()
        {
            this.Version = CurrentVersion;
            this.Cycles = new List<WebmediaCycleConfig>();
        }

        /// <summary>
        /// Gets or sets the file version number as a string for XML serialization.
        /// </summary>
        [XmlAttribute("Version")]
        public string VersionString
        {
            get
            {
                return this.Version.ToString();
            }

            set
            {
                this.Version = new Version(value);
            }
        }

        /// <summary>
        /// Gets or sets the file version number.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the created timestamp as a string for XML serialization.
        /// This information is not used by the application, 
        /// but rather for the understandability of the file contents.
        /// </summary>
        [XmlAttribute("Created")]
        public string CreationDateString
        {
            get
            {
                return this.CreationDate.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            }

            set
            {
                this.CreationDate = DateTime.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets or sets the created timestamp.
        /// This information is not used by the application, 
        /// but rather for the understandability of the file contents.
        /// </summary>
        [XmlIgnore]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the cycles.
        /// </summary>
        [XmlArrayItem("Cycle")]
        public List<WebmediaCycleConfig> Cycles { get; set; }
    }
}
