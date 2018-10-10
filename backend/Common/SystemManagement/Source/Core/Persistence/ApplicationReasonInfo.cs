// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationReasonInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationReasonInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Persistence
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Information about an application event used for persistence.
    /// </summary>
    public class ApplicationReasonInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationReasonInfo"/> class.
        /// </summary>
        public ApplicationReasonInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationReasonInfo"/> class.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        public ApplicationReasonInfo(ApplicationReason reason, string explanation)
        {
            this.Reason = reason;
            this.Explanation = explanation;
        }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        public ApplicationReason Reason { get; set; }

        /// <summary>
        /// Gets or sets the explanation as a user readable string.
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Gets or sets the start time as an XML serializable string.
        /// </summary>
        [XmlElement("TimestampUtc")]
        public string TimestampUtcString
        {
            get
            {
                return this.TimestampUtc == DateTime.MinValue
                           ? null
                           : XmlConvert.ToString(this.TimestampUtc, XmlDateTimeSerializationMode.Utc);
            }

            set
            {
                this.TimestampUtc = string.IsNullOrEmpty(value)
                                        ? DateTime.MinValue
                                        : XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the start time in UTC time.
        /// </summary>
        [XmlIgnore]
        public DateTime TimestampUtc { get; set; }
    }
}
