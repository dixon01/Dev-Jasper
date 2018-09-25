// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.IO
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for a serial port of which the I/O's are provided over GIOoM.
    /// </summary>
    [Serializable]
    public class SerialPortConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortConfig"/> class.
        /// </summary>
        public SerialPortConfig()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the name of the serial port (COM + number).
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this serial port is enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the output port that is used for the RTS signal.
        /// </summary>
        [XmlElement("RTS")]
        public string Rts { get; set; }

        /// <summary>
        /// Gets or sets the name of the input port that is used for the CTS signal.
        /// </summary>
        [XmlElement("CTS")]
        public string Cts { get; set; }

        /// <summary>
        /// Gets or sets the name of the output port that is used for the DTR signal.
        /// </summary>
        [XmlElement("DTR")]
        public string Dtr { get; set; }

        /// <summary>
        /// Gets or sets the name of the input port that is used for the DSR signal.
        /// </summary>
        [XmlElement("DSR")]
        public string Dsr { get; set; }
    }
}