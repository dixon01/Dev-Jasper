// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceSettings.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the configuration for the local resources.
    /// </summary>
    public class ResourceSettings
    {
        /// <summary>
        /// Gets or sets the local resource path.
        /// </summary>
        public string LocalResourcePath { get; set; }

        /// <summary>
        /// Gets or sets the maximum used disk space used by local resources.
        /// </summary>
        /// <value>
        /// The value is in bytes.
        /// </value>
        public long MaxUsedDiskSpace { get; set; }

        /// <summary>
        /// Gets or sets the minimum remaining free disk space.
        /// </summary>
        /// <value>
        /// The value is in bytes.
        /// </value>
        public long MinRemainingDiskSpace { get; set; }

        /// <summary>
        /// Gets or sets the duration after which a local resource should be deleted.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RemoveLocalResourceAfter { get; set; }

        /// <summary>
        /// Gets or sets the duration after which a local resource should be deleted. It is needed for serialization.
        /// </summary>
        [XmlElement("RemoveLocalResourceAfter", DataType = "duration")]
        public string RemoveLocalResourceAfterXml
        {
            get
            {
                return XmlConvert.ToString(this.RemoveLocalResourceAfter);
            }

            set
            {
                this.RemoveLocalResourceAfter = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
