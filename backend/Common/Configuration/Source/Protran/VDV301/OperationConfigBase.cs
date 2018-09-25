// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for service operation configurations.
    /// </summary>
    [Serializable]
    public abstract class OperationConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationConfigBase"/> class.
        /// </summary>
        protected OperationConfigBase()
        {
            this.SubscriptionTimeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to subscribe to changes of the operation.
        /// </summary>
        [XmlAttribute("Subscribe")]
        public bool Subscribe { get; set; }

        /// <summary>
        /// Gets or sets the timeout after which a subscription is removed and re-added if no
        /// data was received. A value of <see cref="TimeSpan.Zero"/> disables this timeout.
        /// </summary>
        [XmlIgnore]
        public TimeSpan SubscriptionTimeout { get; set; }

        /// <summary>
        /// Gets or sets the XML string representation of the timeout after which
        /// a subscription is removed and re-added if no data was received.
        /// An empty string disables this timeout.
        /// </summary>
        [XmlAttribute("SubscriptionTimeout")]
        [DefaultValue("")]
        public string SubscriptionTimeoutString
        {
            get
            {
                return this.SubscriptionTimeout == TimeSpan.Zero
                           ? string.Empty
                           : XmlConvert.ToString(this.SubscriptionTimeout);
            }

            set
            {
                this.SubscriptionTimeout = string.IsNullOrEmpty(value) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }
    }
}