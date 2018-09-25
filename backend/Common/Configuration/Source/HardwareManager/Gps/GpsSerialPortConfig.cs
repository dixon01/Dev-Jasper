// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsSerialPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.HardwareManager.Gps
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Obc.Common;

    /// <summary>
    /// Configuration of the serial port to which the GPS receiver is connected.
    /// </summary>
    [Serializable]
    public class GpsSerialPortConfig : GpsClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpsSerialPortConfig" /> class.
        /// Creates a new serial port with predefined parameters.
        /// </summary>
        public GpsSerialPortConfig()
        {
            this.GpsSerialPort = new SerialPortConfig();
            this.Enabled = false;
        }

        /// <summary>
        /// Gets or sets the serial port configuration.
        /// </summary>
        [XmlElement("SerialPort", typeof(SerialPortConfig))]
        public SerialPortConfig GpsSerialPort { get; set; }
    }
}