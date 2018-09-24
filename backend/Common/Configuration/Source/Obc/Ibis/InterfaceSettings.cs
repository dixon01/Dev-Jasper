// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceSettings.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InterfaceSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.IO.Ports;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Obc.Common;

    /// <summary>
    /// The IBIS interface settings.
    /// </summary>
    [Serializable]
    public class InterfaceSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceSettings"/> class.
        /// </summary>
        public InterfaceSettings()
        {
            this.SerialPortConfig = new SerialPortConfig(
                "COM2", 1200, 7, Parity.Even, StopBits.Two, false, false, true);
        }

        /// <summary>
        /// Gets or sets the section for the IBIS serial port.
        /// </summary>
        [XmlElement(ElementName = "SerialPortConfig")]
        public SerialPortConfig SerialPortConfig { get; set; }
    }
}