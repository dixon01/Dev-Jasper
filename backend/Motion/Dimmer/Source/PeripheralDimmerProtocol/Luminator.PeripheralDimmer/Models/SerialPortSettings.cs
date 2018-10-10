// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="SerialPortSettings.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Xml.Serialization;

    using Luminator.PeripheralDimmer.Interfaces;

    /// <summary>The serial port settings.</summary>
    [Serializable]
    public class SerialPortSettings : ISerialPortSettings
    {
        #region Constants

        /// <summary>The default baud rate.</summary>
        public const int DefaultBaudRate = 115200;

        /// <summary>The default data bits.</summary>
        public const int DefaultDataBits = 8;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SerialPortSettings" /> class.</summary>
        public SerialPortSettings()
            : this(string.Empty)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SerialPortSettings"/> class.</summary>
        /// <param name="comPort">The com port.</param>
        /// <param name="baudRate">The baud rate. Default see DefaultBaudRate</param>
        /// <param name="dataBits">The data bits.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="stopBits">The stop bits.</param>
        /// <param name="dtrControl">The dtr control.</param>
        /// <param name="rtsControl">The rts control.</param>
        /// <param name="receivedBytesThreshold">The number of bytes in the internal input buffer before a DataReceived event
        public SerialPortSettings(
            string comPort, 
            int baudRate = DefaultBaudRate, 
            int dataBits = DefaultDataBits, 
            Parity parity = Parity.None, 
            StopBits stopBits = StopBits.One, 
            bool dtrControl = false, 
            bool rtsControl = false, 
            int receivedBytesThreshold = DimmerConstants.RecieveBytesThreshold)
        {
            this.ComPort = string.IsNullOrEmpty(comPort) ? DimmerConstants.DefaultComPort : comPort;
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.DtrControl = dtrControl;
            this.RtsControl = rtsControl;
            this.BufferSize = 4096;
            this.ReceivedBytesThreshold = receivedBytesThreshold;
            this.EnableBackgroundReader = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the COM port name.
        /// </summary>
        [XmlElement(ElementName = "ComPort")]
        public string ComPort { get; set; }

        /// <summary>Gets or sets the buffer size.</summary>
        [XmlElement(ElementName = "BufferSize")]
        public int BufferSize { get; set; }

        /// <summary>
        ///     Gets or sets the baud rate.
        /// </summary>
        [XmlElement(ElementName = "BaudRate")]
        public int BaudRate { get; set; }

        /// <summary>
        ///     Gets or sets the data bits.
        /// </summary>
        [XmlElement(ElementName = "DataBits")]
        public int DataBits { get; set; }

        /// <summary>
        ///     Gets or sets the parity.
        /// </summary>
        [XmlElement(ElementName = "Parity")]
        public Parity Parity { get; set; }

        /// <summary>
        ///     Gets or sets the stop bits.
        /// </summary>
        [XmlElement(ElementName = "StopBits")]
        public StopBits StopBits { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether DTR control is enabled.
        /// </summary>
        [XmlElement(ElementName = "DtrControl")]
        public bool DtrControl { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether parity should be checked.
        /// </summary>
        /// <summary>Gets or sets the refresh interval mili seconds.</summary>
        /// <summary>
        ///     Gets or sets a value indicating whether RTS control is enabled.
        /// </summary>
        [XmlElement(ElementName = "RtsControl")]
        public bool RtsControl { get; set; }

        /// <summary>Gets or sets the number of bytes in the internal input buffer before a DataReceived event occurs.</summary>
        [XmlElement(ElementName = "ReceivedBytesThreshold")]
        public int ReceivedBytesThreshold { get; set; }

        /// <summary>Gets or sets a value indicating whether enable background reader.</summary>
        public bool EnableBackgroundReader { get; set; }        

        #endregion

        #region Public Methods and Operators

        /// <summary>Read settings from xml file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="SerialPortSettings"/>.</returns>
        public static SerialPortSettings Read(string fileName)
        {
            using (var streamReader = new StreamReader(File.Open(fileName, FileMode.Open)))
            {
                var xmlSerializer = new XmlSerializer(typeof(SerialPortSettings));
                return (SerialPortSettings)xmlSerializer.Deserialize(streamReader);
            }
        }

        /// <summary>Write settings to xml file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="settings">The settings.</param>
        public static void Write(string fileName, SerialPortSettings settings)
        {
            using (var streamWriter = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate)))
            {
                var xmlSerializer = new XmlSerializer(typeof(SerialPortSettings));
                xmlSerializer.Serialize(streamWriter, settings);
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format(
                "Com: {0}, {1} Baud:{2}, Parity:{3}, ReceivedBytesThreshold={4}", 
                this.ComPort, 
                this.DataBits,
                this.BaudRate, 
                this.Parity, 
                this.ReceivedBytesThreshold);
        }

        #endregion
    }
}