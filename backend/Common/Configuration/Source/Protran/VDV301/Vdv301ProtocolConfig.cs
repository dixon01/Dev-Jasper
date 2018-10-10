// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301ProtocolConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301ProtocolConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The root of the VDV 301 protocol configuration.
    /// </summary>
    [XmlRoot("VDV301")]
    [Serializable]
    public class Vdv301ProtocolConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301ProtocolConfig"/> class.
        /// </summary>
        public Vdv301ProtocolConfig()
        {
            this.Services = new ServicesConfig();
            this.Languages = new List<LanguageMappingConfig>();
            this.Transformations = new List<Chain>();
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
                        typeof(Vdv301ProtocolConfig).Assembly.GetManifestResourceStream(
                            typeof(Vdv301ProtocolConfig), "vdv301.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find vdv301.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the available services.
        /// </summary>
        public ServicesConfig Services { get; set; }

        /// <summary>
        /// Gets or sets the language mappings.
        /// </summary>
        [XmlArrayItem("Language")]
        public List<LanguageMappingConfig> Languages { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Transformations.
        /// </summary>
        public List<Chain> Transformations { get; set; }
    }
}
