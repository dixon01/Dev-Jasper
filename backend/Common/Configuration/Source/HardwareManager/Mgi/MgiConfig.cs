// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MgiConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for MGI topbox
    /// </summary>
    [Serializable]
    public class MgiConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MgiConfig"/> class.
        /// </summary>
        public MgiConfig()
        {
            this.PollingInterval = TimeSpan.FromMilliseconds(200);
            this.Gpio = new GpioConfig();
            this.Enabled = false;   // LTG Default to False
            this.AtmelControlPort = 3011;
            this.DefaultBacklightValue = -1;
            this.DviLevelShifters = new List<DviLevelShifterConfig>();
            this.Transceivers = new List<TransceiverConfig>();
        }

        /// <summary>
        /// Gets or sets the polling interval at which the inputs of the
        /// system are read.
        /// </summary>
        [XmlIgnore]
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Gets or sets the polling interval as an XML serializable string.
        /// </summary>
        [XmlElement("PollingInterval", DataType = "duration")]
        public string PollingIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.PollingInterval);
            }

            set
            {
                this.PollingInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the GPIO in MGI topbox.
        /// </summary>
        [XmlElement("GPIO")]
        public GpioConfig Gpio { get; set; }

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        public string Button { get; set; }

        /// <summary>
        /// Gets or sets the update led.
        /// </summary>
        public string UpdateLed { get; set; }

        /// <summary>
        /// Gets or sets the RS-485 interface mode.
        /// If this property is null, the interface will not be changed (default).
        /// This property only makes sense on an MGI Compact, not on PC-2.
        /// There is no safeguard against using this on a PC-2, but AtmelControl
        /// will then return an error code (-1) if you try to change it to "CPU" mode.
        /// </summary>
        [XmlElement("RS485Interface")]
        public CompactRs485Switch? Rs485Interface { get; set; }

        /// <summary>
        /// Gets or sets the level shifters configuration.
        /// </summary>
        [XmlArrayItem("DviLevelShifter")]
        public List<DviLevelShifterConfig> DviLevelShifters { get; set; }

        /// <summary>
        /// Gets or sets the transceivers configuration.
        /// </summary>
        [XmlArrayItem("Transceiver")]
        public List<TransceiverConfig> Transceivers { get; set; }

        /// <summary>
        /// Gets or sets the AtmelControl TCP port.
        /// </summary>
        [DefaultValue(3011)]
        public int AtmelControlPort { get; set; }

        /// <summary>
        /// Gets or sets the default backlight value used for turning the backlight on.
        /// Default value is -1 (meaning: automatic regulation).
        /// </summary>
        public int DefaultBacklightValue { get; set; }

        /// <summary>
        /// Gets or sets the backlight control rate configuration for automatic regulation.
        /// </summary>
        public BacklightControlRateConfig BacklightControlRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Rs485Interface"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeRs485Interface()
        {
            return this.Rs485Interface.HasValue;
        }
    }
}
