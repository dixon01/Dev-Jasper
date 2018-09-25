// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The SNTP client config.
    /// </summary>
    [Serializable]
    public class SntpConfig : SntpConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SntpConfig"/> class.
        /// </summary>
        public SntpConfig()
        {
            this.Host = RemoteSntpServer.DefaultHostName;
            this.Port = RemoteSntpServer.DefaultPort;
            this.UpdateInterval = TimeSpan.FromDays(1); // once every day
        }

        /// <summary>
        /// Gets or sets the host name or IP address of the SNTP server.
        /// </summary>
        [XmlAttribute("Host")]
        [DefaultValue(RemoteSntpServer.DefaultHostName)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the UDP port of the SNTP server.
        /// </summary>
        [XmlAttribute("Port")]
        [DefaultValue(RemoteSntpServer.DefaultPort)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the update interval after which the time should be synchronized again.
        /// The time is measured from the start of the application.
        /// </summary>
        [XmlIgnore]
        public TimeSpan UpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the update interval as an XML serializable string.
        /// </summary>
        [XmlAttribute("UpdateInterval", DataType = "duration")]
        [DefaultValue("P1D")]
        public string UpdateIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.UpdateInterval);
            }

            set
            {
                this.UpdateInterval = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}