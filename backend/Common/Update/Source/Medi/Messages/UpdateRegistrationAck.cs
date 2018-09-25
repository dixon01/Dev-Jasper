// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateRegistrationAck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateRegistrationAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi.Messages
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The update registration acknowledge message.
    /// </summary>
    public class UpdateRegistrationAck
    {
        /// <summary>
        /// Gets or sets the registration id used to match an <see cref="UpdateRegistration"/>.
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the duration during which the registration is valid.
        /// If the client doesn't re-register during that time, the registration will expire.
        /// </summary>
        [XmlIgnore]
        public TimeSpan ValidityTime { get; set; }

        /// <summary>
        /// Gets or sets the validity time as an XML serializable string.
        /// </summary>
        [XmlElement("ValidityTime")]
        public string ValidityTimeString
        {
            get
            {
                return XmlConvert.ToString(this.ValidityTime);
            }

            set
            {
                this.ValidityTime = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}