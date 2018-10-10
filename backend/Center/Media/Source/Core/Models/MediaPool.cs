// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaPool.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaPool type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a media pool.
    /// </summary>
    [XmlRoot("MediaPool")]
    public class MediaPool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPool"/> class.
        /// </summary>
        public MediaPool()
        {
            this.ResourceReferences = new List<ResourceReference>();
        }

        /// <summary>
        /// Gets or sets the name of the media pool.
        /// </summary>
        /// <value>
        /// The name of the media pool.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource references.
        /// </summary>
        /// <value>
        /// The resource references.
        /// </value>
        public List<ResourceReference> ResourceReferences { get; set; }
    }
}