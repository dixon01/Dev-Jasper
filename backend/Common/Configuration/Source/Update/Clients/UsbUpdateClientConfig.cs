// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbUpdateClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Clients
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration of USB update client
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Usb.UsbUpdateClient, Gorba.Common.Update.Usb")]
    public class UsbUpdateClientConfig : UpdateClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsbUpdateClientConfig"/> class.
        /// </summary>
        public UsbUpdateClientConfig()
        {
            this.RepositoryBasePath = string.Empty;
            this.UsbDetectionTimeOut = TimeSpan.FromSeconds(20);
            this.Name = "UsbClient";
        }

        /// <summary>
        /// Gets or sets the source path of the update information.
        /// </summary>
        public string RepositoryBasePath { get; set; }

        /// <summary>
        /// Gets or sets the USB detection time out.
        /// </summary>
        [XmlIgnore]
        public TimeSpan UsbDetectionTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the USB detection time out as an XML serializable string.
        /// </summary>
        [XmlElement("USBDetectionTimeOut")]
        public string UsbDetectionTimeOutString
        {
            get
            {
                return XmlConvert.ToString(this.UsbDetectionTimeOut);
            }

            set
            {
                this.UsbDetectionTimeOut = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the poll interval at which the repository is
        /// scanned.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the poll interval as an XML serializable string.
        /// </summary>
        [XmlElement("PollInterval")]
        public string PollIntervalString
        {
            get
            {
                return this.PollInterval.HasValue ? XmlConvert.ToString(this.PollInterval.Value) : null;
            }

            set
            {
                this.PollInterval = string.IsNullOrEmpty(value) ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
