// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheLimitsConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CacheLimitsConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for the limits set on the folder in Data\Update\Pool.
    /// </summary>
    [Serializable]
    public class CacheLimitsConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLimitsConfig"/> class.
        /// </summary>
        public CacheLimitsConfig()
        {
            this.Enabled = false;
            this.NumberOfFiles = 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether folder limit observation is enabled.
        /// Default value is true.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum required free space on the disk in megabytes.
        /// </summary>
        [XmlElement("FreeSpaceMB")]
        public int? FreeSpaceMb { get; set; }

        /// <summary>
        /// Gets or sets the number of files to be checked in the folder.
        /// </summary>
        [DefaultValue(0)]
        public int NumberOfFiles { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="FreeSpaceMb"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeFreeSpaceMb()
        {
            return this.FreeSpaceMb.HasValue;
        }
    }
}