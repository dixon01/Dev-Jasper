// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsPilotConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsPilotConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Gps
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// GPS client that connects to the legacy GPS Pilot application and can also be used with ADT.
    /// </summary>
    [Serializable]
    public class GpsPilotConfig : GpsClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpsPilotConfig"/> class.
        /// </summary>
        public GpsPilotConfig()
        {
            this.Port = 1599;
        }

        /// <summary>
        /// Gets or sets the remote IP address of the GPS pilot application.
        /// </summary>
        [XmlAttribute("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the remote TCP port of the GPS pilot application.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(1599)]
        public int Port { get; set; }
    }
}
