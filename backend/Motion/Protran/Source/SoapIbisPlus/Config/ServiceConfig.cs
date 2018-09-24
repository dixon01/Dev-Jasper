// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// The Web Service configuration.
    /// </summary>
    [Serializable]
    public class ServiceConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfig"/> class.
        /// </summary>
        public ServiceConfig()
        {
            this.Port = 9091;
            this.Uri = "MFDCustomerService.soap";
            this.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Gets or sets the HTTP port number.
        /// Default value: 9091
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the HTTP URI under which the service can be found.
        /// Default value: <code>MFDCustomerService.soap</code>
        /// </summary>
        [XmlAttribute("URI")]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the timeout to send the heartbeat to the OBU.
        /// Values admitted: positive, non-zero timespan. Default: 30 seconds.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets Timeout in an XML serializable format.
        /// </summary>
        [XmlAttribute("Timeout", DataType = "duration")]
        public string PollTimeString
        {
            get
            {
                return XmlConvert.ToString(this.Timeout);
            }

            set
            {
                this.Timeout = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the usage of the connection status (fallback mode) to fill a generic cell.
        /// </summary>
        public GenericUsage ConnectionStatusUsedFor { get; set; }
    }
}