// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceFilter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the properties of a filter for namespace entities.
    /// </summary>
    public class NamespaceFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceFilter"/> class.
        /// </summary>
        public NamespaceFilter()
        {
            this.DataViewModelFilters = new ChildItemCollection<NamespaceFilter, DataViewModelFilter>(this);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets the data view model filters.
        /// </summary>
        [XmlElement("Entity")]
        public ChildItemCollection<NamespaceFilter, DataViewModelFilter> DataViewModelFilters { get; private set; }
    }
}