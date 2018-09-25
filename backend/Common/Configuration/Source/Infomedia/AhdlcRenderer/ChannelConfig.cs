// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The channel configuration.
    /// </summary>
    [Serializable]
    public class ChannelConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelConfig"/> class.
        /// </summary>
        public ChannelConfig()
        {
            this.SerialPort = new SerialPortConfig();
            this.Signs = new List<SignConfig>();
        }

        /// <summary>
        /// Gets or sets the serial port configuration.
        /// </summary>
        public SerialPortConfig SerialPort { get; set; }

        /// <summary>
        /// Gets or sets the sign configurations.
        /// </summary>
        [XmlArrayItem("Sign")]
        public List<SignConfig> Signs { get; set; }
    }
}
