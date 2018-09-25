// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObuConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObuConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Arriva
{
    using System.Xml.Serialization;

    /// <summary>
    /// The OBU connection configuration.
    /// </summary>
    public class ObuConfig
    {
        /// <summary>
        /// Gets or sets the remote IP address.
        /// </summary>
        [XmlElement("RemoteIP")]
        public string RemoteIp { get; set; }

        /// <summary>
        /// Gets or sets the remote TCP port.
        /// </summary>
        public int RemotePort { get; set; }
    }
}