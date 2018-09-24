// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.EPaper.MainUnit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The main unit configuration.
    /// </summary>
    [XmlRoot("MainUnit")]
    [Serializable]
    public class MainUnitConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainUnitConfig"/> class.
        /// </summary>
        public MainUnitConfig()
        {
            this.DisplayUnits = new List<DisplayUnitConfig>();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(MainUnitConfig).Assembly.GetManifestResourceStream(
                            typeof(MainUnitConfig), "MainUnit.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find MainUnit.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the hash of the firmware (using xxHash64 algorithm).
        /// </summary>
        [XmlAttribute("FirmwareHash")]
        public string FirmwareHash { get; set; }

        /// <summary>
        /// Gets or sets the update interval for URL based content.
        /// </summary>
        [XmlIgnore]
        public DateTime OperationDayStartUtc { get; set; }

        /// <summary>
        /// Gets or sets the update interval as an XML serializable string.
        /// </summary>
        [XmlAttribute("OperationDayStartUtc")]
        public string OperationDayStartUtcString
        {
            get
            {
                return this.OperationDayStartUtc.ToString("HH:mm:ss");
            }

            set
            {
                this.OperationDayStartUtc = DateTime.ParseExact(value, "HH:mm:ss", null);
            }
        }

        /// <summary>
        /// Gets or sets the enabled protocols with their settings.
        /// </summary>
        [XmlArrayItem(ElementName = "DisplayUnit")]
        public List<DisplayUnitConfig> DisplayUnits { get; set; }

        /// <summary>
        /// Gets or sets the LCD config.
        /// This is optional, since the LCD is not always available on a Main Unit.
        /// </summary>
        [XmlElement("LCD")]
        public LcdConfig LcdConfig { get; set; }
    }
}