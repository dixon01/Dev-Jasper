// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Arriva
{
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration of Arriva for connections.
    /// </summary>
    [XmlRoot("Arriva")]
    public class ArrivaConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaConfig"/> class.
        /// </summary>
        public ArrivaConfig()
        {
            this.Obu = new ObuConfig();
            this.Ftp = new FtpConfig();
            this.Behaviour = new BehaviourConfig();
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
                        typeof(ArrivaConfig).Assembly.GetManifestResourceStream(
                            typeof(ArrivaConfig), "Arriva.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find Arriva.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the XML element called OBU.
        /// </summary>
        [XmlElement("OBU")]
        public ObuConfig Obu { get; set; }

        /// <summary>
        /// Gets or sets the XML element called FTP.
        /// </summary>
        [XmlElement("FTP")]
        public FtpConfig Ftp { get; set; }

        /// <summary>
        /// Gets or sets Behaviour.
        /// </summary>
        [XmlElement("Behaviour")]
        public BehaviourConfig Behaviour { get; set; }
    }
}
