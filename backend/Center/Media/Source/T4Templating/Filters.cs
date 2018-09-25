// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Filters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Filters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the filters for entities.
    /// </summary>
    [XmlRoot("DataViewModelEntities")]
    public class Filters
    {
        /// <summary>
        /// Gets or sets the namespace filters.
        /// </summary>
        /// <value>
        /// The namespace filters.
        /// </value>
        [XmlElement("Namespace")]
        public List<NamespaceFilter> NamespaceFilters { get; set; }

        /// <summary>
        /// Gets or sets the base generated namespace.
        /// </summary>
        /// <value>
        /// The base generated namespace.
        /// </value>
        [XmlAttribute]
        public string BaseGeneratedNamespace { get; set; }
    }
}