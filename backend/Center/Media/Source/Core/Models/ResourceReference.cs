// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceReference.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a reference to a resource.
    /// </summary>
    [XmlRoot("ResourceReference")]
    public class ResourceReference
    {
        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string Hash { get; set; }
    }
}