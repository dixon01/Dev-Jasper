// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Common
{
    using System;
    using System.IO.Ports;
    using System.Xml.Serialization;

    using Gorba.Motion.Obc.CommonEmb;

    /// <summary>
    ///   Serial port settings which can be used for the WinCEWrapper.StreamDriver
    /// </summary>
    [Serializable]
    public class SerialPortConfig
    {
        /*
        /// <summary>
        /// Enum matches with the DCB FDTRControl definition.
        /// </summary>
        public enum DtrControl : int
        {
            Disable = 0,
            Enable = 1,
            Handshake = 2
        };
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortConfig"/> class.
        /// Will be used the following default values:
        ///   com port name = "COM1"
        ///   baud rate = 9600;
        ///   data bits = 8;
        ///   parity = none;
        ///   stop bits = one;
        ///   DTR control = false;
        ///   RTS control = false;
        ///   parity check enabled = false;
        /// </summary>
        public SerialPortConfig()
            : this("COM1", 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortConfig"/> class.
        /// </summary>
        /// <param name = "comPort">The serial port's name (without ':' at the end of the name).</param>
        /// <param name = "baudRate">The serial port's baud rate.</param>
        /// <param name = "dataBits">The serial port's data bits.</param>
        /// <param name = "parity">The serial port's parity.</param>
        /// <param name = "stopBits">The serial port's stop bit.</param>
        /// <param name = "dtrControlEnable">The serial port's DTR control.</param>
        /// <param name = "rtsControlEnable">The serial port's parity check.</param>
        /// <param name = "enableParityCheck">The serial port's RTS control.</param>
        public SerialPortConfig(
            string comPort,
            int baudRate,
            int dataBits,
            Parity parity,
            StopBits stopBits,
            bool dtrControlEnable,
            bool rtsControlEnable,
            bool enableParityCheck)
        {
            this.ReadTotalMultiplierTimeout = 100;
            this.ReadTotalTimeout = 10;
            this.ReadIntervalTimeout = 100;

            // Initialize parity
            string partiyDescription = "Possible values: " + EnumUtil.GetAllEnumValues<Parity>();
            this.Parity = new ConfigItem<Parity>(System.IO.Ports.Parity.None, partiyDescription);

            // Initialize stopbits
            string stopBitDescription = "Possible values: " + EnumUtil.GetAllEnumValues<StopBits>();
            this.StopBits = new ConfigItem<StopBits>(System.IO.Ports.StopBits.One, stopBitDescription);

            this.ComPort = comPort;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity.Value = parity;
            this.StopBits.Value = stopBits;
            this.DtrControl = dtrControlEnable;
            this.RtsControl = rtsControlEnable;
            this.FParity = enableParityCheck;
        }

        /// <summary>
        /// Gets or sets the version (always 1).
        /// </summary>
        [XmlAttribute("Version")]
        public int Version
        {
            get
            {
                return 1;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description
        {
            get
            {
                return "Possible COM-Ports: Com1, Com2, ..., RSB1";
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the COM port name.
        /// </summary>
        [XmlElement(ElementName = "ComPort")]
        public string ComPort { get; set; }

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        [XmlElement(ElementName = "BaudRate")]
        public int BaudRate { get; set; }

        /// <summary>
        /// Gets or sets the data bits.
        /// </summary>
        [XmlElement(ElementName = "DataBits")]
        public int DataBits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether parity should be checked.
        /// </summary>
        [XmlElement(ElementName = "FParity")]
        public bool FParity { get; set; }

        /// <summary>
        /// Gets or sets the parity.
        /// </summary>
        [XmlElement(ElementName = "Parity")]
        public ConfigItem<Parity> Parity { get; set; }

        /// <summary>
        /// Gets or sets the stop bits.
        /// </summary>
        [XmlElement(ElementName = "StopBits")]
        public ConfigItem<StopBits> StopBits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DTR control is enabled.
        /// </summary>
        [XmlElement(ElementName = "DtrControl")]
        public bool DtrControl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RTS control is enabled.
        /// </summary>
        [XmlElement(ElementName = "RtsControl")]
        public bool RtsControl { get; set; }

        /// <summary>
        /// Gets or sets the serial port's read interval timeout value.
        /// (Attention: this value is not coming from an XML TAG).
        /// </summary>
        public uint ReadIntervalTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port's read total timeout value
        /// (Attention: this value is not coming from an XML TAG).
        /// </summary>
        public uint ReadTotalTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port's read total multiplier timeout value
        /// (Attention: this value is not coming from an XML TAG).
        /// </summary>
        public uint ReadTotalMultiplierTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port's write total timeout value
        /// (Attention: this value is not coming from an XML TAG).
        /// </summary>
        public uint WriteTotalTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port's write multiplier timeout value
        /// (Attention: this value is not coming from an XML TAG).
        /// </summary>
        public uint WriteMultiplierTimeout { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            string msg = "Com port: " + this.ComPort + "\nBaudrate: " + this.BaudRate + "\nDatabits: " + this.DataBits
                         + "\nParity: " + this.Parity + "\nStopbits: " + this.StopBits;
            return base.ToString() + "\n" + msg;
        }
    }
}