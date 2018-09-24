// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Core
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about the
    /// Protran Persistence service.
    /// </summary>
    [Serializable]
    public class PersistenceConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceConfig"/> class.
        /// </summary>
        public PersistenceConfig()
        {
            this.IsEnabled = false;
            this.DefaultValidity = TimeSpan.FromMinutes(10);
            this.PersistenceFile = "Persistence.xml";
        }

        /// <summary>
        /// Gets or sets a value indicating whether the persistence service
        /// is enabled or not. True, if it's enabled, else disabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the absolute file name for the file that
        /// will be used by Protran to store/load information.
        /// Default value: Persistence.xml
        /// </summary>
        public string PersistenceFile { get; set; }

        /// <summary>
        /// Gets or sets the default time for the information's validity.
        /// Default value is 10 minutes.
        /// </summary>
        [XmlIgnore]
        public TimeSpan DefaultValidity { get; set; }

        /// <summary>
        /// Gets or sets the default time for the information's validity
        /// in an XML serializable format.
        /// </summary>
        [XmlElement("DefaultValidity", DataType = "duration")]
        public string DefaultValidityString
        {
            get
            {
                return XmlConvert.ToString(this.DefaultValidity);
            }

            set
            {
                this.DefaultValidity = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
