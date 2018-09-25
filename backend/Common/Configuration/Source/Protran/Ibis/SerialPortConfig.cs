// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.IO.Ports;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of the settings about the channel with the IBIS master.
    /// </summary>
    [Serializable]
    public class SerialPortConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortConfig"/> class.
        /// </summary>
        public SerialPortConfig()
        {
            this.ComPort = "COM1";
            this.BaudRate = 1200;
            this.DataBits = 7;
            this.StopBits = StopBits.Two;
            this.Parity = Parity.Even;

            this.RetryCount = 0;
            this.SerialPortReopen = SerialPortReopen.FrameOnly;
        }

        /// <summary>
        /// Gets or sets the serial port's name. Free text (case insensitive) Definition COM1.
        /// </summary>
        public string ComPort { get; set; }

        /// <summary>
        /// Gets or sets the XML field called BaudRate
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Gets or sets the XML field called DataBits
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// Gets or sets the XML field called StopBits
        /// </summary>
        [XmlElement("StopBits", Type = typeof(StopBits))]
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Gets or sets the XML field called Parity
        /// </summary>
        [XmlElement("Parity", Type = typeof(Parity))]
        public Parity Parity { get; set; }

        /// <summary>
        /// Gets or sets the XML field called RetryCount.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets SerialPortReopen.
        /// </summary>
        public SerialPortReopen SerialPortReopen { get; set; }
    }
}