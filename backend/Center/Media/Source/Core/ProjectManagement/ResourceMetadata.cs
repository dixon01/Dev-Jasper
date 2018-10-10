// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceMetadata.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceMetadata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines metadata properties of a resource.
    /// This class is not to be used outside this namespace, it is only public to support XML serialization.
    /// </summary>
    [XmlRoot("ResourceMetadata")]
    public class ResourceMetadata
    {
        /// <summary>
        /// Gets or sets the hash of the resource.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the offset of the resource in the file (from the beginning of the file).
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Gets or sets the size of the resource.
        /// </summary>
        public long Size { get; set; }
    }
}