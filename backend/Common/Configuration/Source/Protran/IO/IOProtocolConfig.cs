// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOProtocolConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOProtocolConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The configuration of the I/O protocol.
    /// </summary>
    [XmlRoot("IO")]
    [Serializable]
    public class IOProtocolConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOProtocolConfig"/> class.
        /// </summary>
        public IOProtocolConfig()
        {
            this.SerialPorts = new List<SerialPortConfig>();

            this.Inputs = new List<InputHandlingConfig>();

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
                        typeof(IOProtocolConfig).Assembly.GetManifestResourceStream(
                            typeof(IOProtocolConfig), "io.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find io.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the XML element called SerialPorts.
        /// </summary>
        [XmlArrayItem("SerialPort")]
        public List<SerialPortConfig> SerialPorts { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Inputs.
        /// </summary>
        [XmlArrayItem("Input")]
        public List<InputHandlingConfig> Inputs { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Transformations.
        /// </summary>
        public List<Chain> Transformations { get; set; }
    }
}
