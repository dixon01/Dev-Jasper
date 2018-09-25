// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSourcesConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisSourcesConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The IBIS sources configuration.
    /// </summary>
    [Serializable]
    public class IbisSourcesConfig
    {
        /// <summary>
        /// Gets or sets the active IBIS source default is <see cref="IbisSourceType.None"/>.
        /// </summary>
        [XmlAttribute]
        public IbisSourceType Active { get; set; }

        /// <summary>
        /// Gets or sets the simulation configuration if the IBIS data
        /// should be played back from an existing recording.
        /// </summary>
        public SimulationConfig Simulation { get; set; }

        /// <summary>
        /// Gets or sets the serial port configuration if a COM port is used for IBIS.
        /// </summary>
        public SerialPortConfig SerialPort { get; set; }

        /// <summary>
        /// Gets or sets the UDP server configuration if a UDP server
        /// should listen for IBIS data.
        /// </summary>
        [XmlElement("UDPServer")]
        public UdpServerConfig UdpServer { get; set; }

        /// <summary>
        /// Gets or sets the JSON interface configuration if it is used for IBIS.
        /// </summary>
        [XmlElement("JSON")]
        public JsonConfig Json { get; set; }
    }
}