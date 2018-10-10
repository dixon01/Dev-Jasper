// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Bus
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The center client config.
    /// </summary>
    [Serializable]
    public class CenterClientConfig
    {
        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        [XmlElement("IpAddress")]
        [DefaultValue("127.0.0.1")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [XmlElement("Port")]
        [DefaultValue(3333)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the reconnect time timeout for the Center connection.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? ReconnectTime { get; set; }

        /// <summary>
        /// Gets or sets the reconnect time timeout for the Center connection as an XML compatible string.
        /// </summary>
        [XmlElement("ReconnectTime", DataType = "duration")]
        public string ReconnectTimeString
        {
            get
            {
                return this.ReconnectTime == null ? null : XmlConvert.ToString(this.ReconnectTime.Value);
            }

            set
            {
                this.ReconnectTime = value == null ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reboot the system if the ECI acknowledge was missing 10 times.
        /// </summary>
        [XmlElement("RebootIfAckMissing")]
        public bool RebootIfAckMissing { get; set; }
    }
}
