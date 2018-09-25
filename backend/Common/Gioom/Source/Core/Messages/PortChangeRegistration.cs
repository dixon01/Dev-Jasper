// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortChangeRegistration.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortChangeRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Messages
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// It is only public to allow XML serialization.
    /// Registration to get <see cref="PortChangeNotification"/>s from a given port.
    /// </summary>
    public class PortChangeRegistration
    {
        /// <summary>
        /// Gets or sets the name of the port.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the timeout after which this registration becomes invalid.
        /// This is used to prevent "dead" clients from receiving notifications about ports.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout in XML duration format.
        /// </summary>
        [XmlElement("Timeout", DataType = "duration")]
        public string TimeoutXml
        {
            get
            {
                return XmlConvert.ToString(this.Timeout);
            }

            set
            {
                this.Timeout = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }
    }
}