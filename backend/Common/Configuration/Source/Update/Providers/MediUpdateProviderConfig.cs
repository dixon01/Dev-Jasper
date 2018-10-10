// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediUpdateProviderConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediUpdateProviderConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Providers
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the MediUpdateProvider.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Medi.MediUpdateProvider, Gorba.Common.Update.Medi")]
    public class MediUpdateProviderConfig : UpdateProviderConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediUpdateProviderConfig"/> class.
        /// </summary>
        public MediUpdateProviderConfig()
        {
            this.Name = "MediProvider";
            this.RegistrationTimeOut = TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Gets or sets the registration time out.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RegistrationTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the registration time out as an XML serializable string.
        /// </summary>
        [XmlElement("RegistrationTimeOut")]
        public string RegistrationTimeOutTimeOutString
        {
            get
            {
                return XmlConvert.ToString(this.RegistrationTimeOut);
            }

            set
            {
                this.RegistrationTimeOut = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
