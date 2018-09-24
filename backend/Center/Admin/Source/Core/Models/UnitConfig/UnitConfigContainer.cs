// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigContainer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The container that is used to export a unit configuration with its meta-data.
    /// </summary>
    [XmlRoot("UnitConfiguration")]
    public class UnitConfigContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfigContainer"/> class.
        /// </summary>
        public UnitConfigContainer()
        {
            this.Version = 1;
        }

        /// <summary>
        /// Gets or sets the creation timestamp. This property is only used for informational purposes.
        /// </summary>
        [XmlAttribute("CreationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the version of the file format. Currently only version 1 is supported.
        /// </summary>
        [XmlAttribute("Version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the name. This property is only used for informational purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the product type for which this configuration was created.
        /// This can be used to verify if it can be seamlessly imported into another unit configuration.
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the actual unit config to be serialized.
        /// </summary>
        public UnitConfigData UnitConfig { get; set; }
    }
}
