// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskLimitConfigList.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiskLimitConfigList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// A list of <see cref="DiskLimitConfig"/>s.
    /// </summary>
    [Serializable]
    public class DiskLimitConfigList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskLimitConfigList"/> class.
        /// </summary>
        public DiskLimitConfigList()
        {
            this.Enabled = true;
            this.Disks = new List<DiskLimitConfig>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether disk limit observation is enabled.
        /// Default value is true.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the configured disks and their limits.
        /// </summary>
        [XmlElement("Disk")]
        public List<DiskLimitConfig> Disks { get; set; }
    }
}