// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcastSubscriptionRequest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BroadcastSubscriptionRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Subscription;

    /// <summary>
    /// The broadcast subscription request used by <see cref="IBroadcastSubscriptionService"/>.
    /// </summary>
    public class BroadcastSubscriptionRequest
    {
        /// <summary>
        /// Gets or sets the type of message to subscribe to.
        /// </summary>
        public TypeName Type { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout in XML serializable format.
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
                this.Timeout = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}