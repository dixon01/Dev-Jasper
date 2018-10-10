// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Settings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Protran core settings.
    /// </summary>
    [Serializable]
    [XmlRoot("Protran")]
    public class ProtranConfig
    {
        private static readonly Version CurrentVersion = new Version(2, 0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtranConfig"/> class.
        /// </summary>
        public ProtranConfig()
        {
            this.Version = CurrentVersion;
            this.Protocols = new List<ProtocolConfig>();
            this.Persistence = new PersistenceConfig();
        }

        #region PROPERTIES

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(ProtranConfig).Assembly.GetManifestResourceStream(
                            typeof(ProtranConfig), "protran.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find protran.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the version of this file.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the version as an XML serializable string.
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
        /// Gets or sets the enabled protocols with their settings.
        /// </summary>
        [XmlArrayItem(ElementName = "Protocol")]
        public List<ProtocolConfig> Protocols { get; set; }

        /// <summary>
        /// Gets or sets Persistence.
        /// </summary>
        [XmlElement(ElementName = "Persistence")]
        public PersistenceConfig Persistence { get; set; }
        #endregion PROPERTIES
    }
}
