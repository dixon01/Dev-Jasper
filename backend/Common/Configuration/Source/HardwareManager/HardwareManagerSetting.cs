// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagerSetting.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareManagerSetting type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Hardware manager settings with one or more conditions that must be met for the settings to be applied.
    /// </summary>
    [Serializable]
    public class HardwareManagerSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareManagerSetting"/> class.
        /// </summary>
        public HardwareManagerSetting()
        {
            this.Conditions = new List<IOCondition>();
            this.HostnameSource = HostnameSource.MacAddress;
            this.DnsServers = new List<XmlIpAddress>();
            this.UseDhcp = false;
        }

        /// <summary>
        /// Gets or sets the conditions.
        /// All conditions must be met for the setting to be used.
        /// </summary>
        [XmlArrayItem("IO")]
        public List<IOCondition> Conditions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hostname of the system should be updated.
        /// Default value is: true.
        /// </summary>
        public HostnameSource HostnameSource { get; set; }

        /// <summary>
        /// Gets or sets the time zone name to set.
        /// Possible names can be found in the registry under:
        /// <c>HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones</c>.
        /// Examples are:
        /// - <c>UTC</c>
        /// - <c>W. Europe Standard Time</c>
        /// - <c>Central Europe Standard Time</c>
        /// - <c>E. Europe Standard Time</c>
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        [XmlElement("IPAddress")]
        public XmlIpAddress IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the subnet mask.
        /// </summary>
        public XmlIpAddress SubnetMask { get; set; }

        /// <summary>
        /// Gets or sets the IP gateway address.
        /// </summary>
        public XmlIpAddress Gateway { get; set; }

        /// <summary>
        /// Gets or sets the list of DNS servers.
        /// </summary>
        [XmlElement("DNS")]
        public List<XmlIpAddress> DnsServers { get; set; }

            /// <summary>
        /// Gets or sets a value indicating whether to use DHCP instead of the IP address.
        /// </summary>
        [XmlElement("UseDHCP")]
        [DefaultValue(false)]
        public bool UseDhcp { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the displays available on hardware.
        /// </summary>
        public DisplayConfig Display { get; set; }
    }
}