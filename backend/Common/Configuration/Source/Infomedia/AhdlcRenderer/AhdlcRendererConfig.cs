// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcRendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcRendererConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The config for the AHDLC renderer.
    /// </summary>
    [Serializable]
    [XmlRoot("AhdlcRenderer")]
    public class AhdlcRendererConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcRendererConfig"/> class.
        /// </summary>
        public AhdlcRendererConfig()
        {
            this.Channels = new List<ChannelConfig>();
            this.Text = new TextConfig();
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
                        typeof(AhdlcRendererConfig).Assembly.GetManifestResourceStream(
                            typeof(AhdlcRendererConfig), "AhdlcRenderer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find AhdlcRenderer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the configured channels.
        /// </summary>
        [XmlElement("Channel")]
        public List<ChannelConfig> Channels { get; set; }

        /// <summary>
        /// Gets or sets the text rendering configuration.
        /// </summary>
        public TextConfig Text { get; set; }
    }
}
