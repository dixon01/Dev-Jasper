// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The VDV 301 configuration object.
    /// </summary>
    [Serializable]
    public class Vdv301Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301Config"/> class.
        /// </summary>
        public Vdv301Config()
        {
            this.DeviceClass = DeviceClass.InteriorDisplay;
            this.TimeSync = new TimeSyncConfig();
            this.AutoStartServices = new List<ServiceName>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of received HTTP requests.
        /// </summary>
        [XmlAttribute]
        public bool ValidateHttpRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of created HTTP responses.
        /// </summary>
        [XmlAttribute]
        public bool ValidateHttpResponses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to verify the version
        /// (the <c>ver=x.y</c> in the TXT record) in services provided through DNS-SD.
        /// </summary>
        [XmlAttribute]
        public bool VerifyVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether VDV 301 is enabled in Hardware Manager.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the device class used in the response of
        /// IDeviceManagementService.GetDeviceInformation
        /// Default value is <see cref="Vdv301.DeviceClass.InteriorDisplay"/>
        /// </summary>
        [XmlAttribute]
        public DeviceClass DeviceClass { get; set; }

        /// <summary>
        /// Gets or sets the VDV 301 time synchronization configuration.
        /// </summary>
        [XmlElement("TimeSync")]
        public TimeSyncConfig TimeSync { get; set; }

        /// <summary>
        /// Gets or sets the list of services that should be started automatically.
        /// </summary>
        [XmlElement("AutoStartService")]
        public List<ServiceName> AutoStartServices { get; set; }
    }
}
