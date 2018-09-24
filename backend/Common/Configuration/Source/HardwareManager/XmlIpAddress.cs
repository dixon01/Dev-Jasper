// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlIpAddress.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlIpAddress type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.Net;
    using System.Xml.Serialization;

    /// <summary>
    /// IP address that can be XML serialized.
    /// </summary>
    [Serializable]
    public class XmlIpAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIpAddress"/> class.
        /// </summary>
        public XmlIpAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIpAddress"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public XmlIpAddress(IPAddress value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIpAddress"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public XmlIpAddress(string value)
        {
            this.StringValue = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="IPAddress"/> value.
        /// </summary>
        [XmlIgnore]
        public IPAddress Value { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        [XmlText]
        public string StringValue
        {
            get
            {
                return this.Value == null ? null : this.Value.ToString();
            }

            set
            {
                this.Value = string.IsNullOrEmpty(value) ? null : IPAddress.Parse(value);
            }
        }

        /// <summary>
        /// Converts an <see cref="XmlIpAddress"/> to an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The <see cref="XmlIpAddress"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPAddress"/> or null if it was not specified.
        /// </returns>
        public static implicit operator IPAddress(XmlIpAddress address)
        {
            return address == null ? null : address.Value;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.StringValue;
        }
    }
}