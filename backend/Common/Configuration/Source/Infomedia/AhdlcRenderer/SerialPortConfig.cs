// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    using System;
    using System.IO.Ports;
    using System.Xml.Serialization;

    /// <summary>
    /// The serial port configuration.
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
            this.BaudRate = 38400;
            this.DataBits = 8;
            this.StopBits = StopBits.Two;
            this.Parity = Parity.None;

            this.RtsMode = RtsMode.Default;

            this.IsHighSpeed = true;
            this.IgnoreFrameStart = false;
            this.IgnoreResponses = true;
            this.IgnoreResponseTime = TimeSpan.FromMilliseconds(10);
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
        /// Gets or sets the behavior how to set the serial port's RTS signal when transmitting data.
        /// </summary>
        [XmlElement("RTSMode", Type = typeof(RtsMode))]
        public RtsMode RtsMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether high speed mode should be used.
        /// If this flag is set to true, block numbers will occupy 2 bytes;
        /// this is needed for signs that are running over RS-485.
        /// Default value is true.
        /// </summary>
        [XmlElement("IsHighSpeed")]
        public bool IsHighSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the frame start.
        /// If the frame start is ignored, we will still decode a frame even if its
        /// start marker is missing. This can be used in cases where the first
        /// bits of the response are crippled because of timing issues.
        /// </summary>
        [XmlElement("IgnoreFrameStart")]
        public bool IgnoreFrameStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore responses.
        /// If responses are ignored, we will just send data (waiting for possible
        /// responses, but not evaluating them).
        /// </summary>
        [XmlElement("IgnoreResponses")]
        public bool IgnoreResponses { get; set; }

        /// <summary>
        /// Gets or sets the time to wait when <see cref="IgnoreResponses"/> is set to true.
        /// This wait time will always be added to the estimated wait time.
        /// </summary>
        [XmlIgnore]
        public TimeSpan IgnoreResponseTime { get; set; }

        /// <summary>
        /// Gets or sets the time to wait in milliseconds used for XML serialization.
        /// Using TimeSpan to serialize very small duration values is not very readable,
        /// therefore an int is used instead.
        /// </summary>
        [XmlElement("IgnoreResponseTime")]
        public int IgnoreResponseTimeValue
        {
            get
            {
                return (int)this.IgnoreResponseTime.TotalMilliseconds;
            }

            set
            {
                this.IgnoreResponseTime = TimeSpan.FromMilliseconds(value);
            }
        }
    }
}