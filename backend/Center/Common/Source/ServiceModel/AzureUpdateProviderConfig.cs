// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureUpdateProviderConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureUpdateProviderConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The configuration for the Azure update provider.
    /// </summary>
    public class AzureUpdateProviderConfig : UpdateProviderConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureUpdateProviderConfig"/> class.
        /// </summary>
        public AzureUpdateProviderConfig()
        {
            this.RetryInterval = TimeSpan.FromMinutes(1);
        }

        /// <summary>
        /// Gets or sets the interval to retry to send remaining update commands.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the interval to retry to send remaining update commands.
        /// </summary>
        [XmlElement("RetryInterval")]
        public string RetryIntervalValue
        {
            get
            {
                return XmlConvert.ToString(this.RetryInterval);
            }

            set
            {
                this.RetryInterval = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}