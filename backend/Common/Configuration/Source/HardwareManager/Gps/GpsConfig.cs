// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Gps
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The GPS configuration.
    /// </summary>
    [Serializable]
    public class GpsConfig
    {
        /// <summary>
        /// Gets or sets the connection type.
        /// </summary>
        [XmlElement("ConnectionType")]
        public GpsConnectionType ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the client configuration.
        /// This can be any of the subclasses of <see cref="GpsClientConfigBase"/> or null (to disable GPS handling).
        /// </summary>
        [XmlElement("GpsPilot", typeof(GpsPilotConfig))]
        public GpsClientConfigBase Client { get; set; }

        /// <summary>
        /// Gets or sets the serial port configuration.
        /// This can be any of the subclasses of <see cref="GpsClientConfigBase"/> or null (to disable GPS handling).
        /// </summary>
        [XmlElement("GpsSerial", typeof(GpsSerialPortConfig))]
        public GpsSerialPortConfig GpsSerialClient { get; set; }
    }
}