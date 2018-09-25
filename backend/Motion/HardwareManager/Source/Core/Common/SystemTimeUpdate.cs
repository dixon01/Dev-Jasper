// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTimeUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemTimeUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The system time update message used to synchronize the system time between different units.
    /// </summary>
    public class SystemTimeUpdate
    {
        /// <summary>
        /// Gets or sets the system time as an XML serializable string.
        /// </summary>
        [XmlElement("SystemTimeUtc")]
        public string SystemTimeUtcString
        {
            get
            {
                return XmlConvert.ToString(this.SystemTimeUtc, XmlDateTimeSerializationMode.Utc);
            }

            set
            {
                this.SystemTimeUtc = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the system time in UTC time.
        /// </summary>
        [XmlIgnore]
        public DateTime SystemTimeUtc { get; set; }
    }
}