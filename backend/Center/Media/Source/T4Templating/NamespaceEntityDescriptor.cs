// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceEntityDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceEntityDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Describes a namespace entity to be generated.
    /// </summary>
    public class NamespaceEntityDescriptor : IChildItem<DataViewModelTemplatedEntities>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceEntityDescriptor"/> class.
        /// </summary>
        public NamespaceEntityDescriptor()
        {
            this.DataViewModelEntityDescriptors =
                new ChildItemCollection<NamespaceEntityDescriptor, DataViewModelEntityDescriptor>(this);
        }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets the data view model entity descriptors.
        /// </summary>
        /// <value>
        /// The data view model entity descriptors.
        /// </value>
        [XmlElement("Class", typeof(DataViewModelEntityDescriptor))]
        [XmlElement("ElementClass", typeof(DataViewModelElementEntityDescriptor))]
        public ChildItemCollection<NamespaceEntityDescriptor, DataViewModelEntityDescriptor>
            DataViewModelEntityDescriptors
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        /// <value>
        /// The parent object.
        /// </value>
        [XmlIgnore]
        public DataViewModelTemplatedEntities ParentObject { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        DataViewModelTemplatedEntities IChildItem<DataViewModelTemplatedEntities>.Parent
        {
            get
            {
                return this.ParentObject;
            }

            set
            {
                this.ParentObject = value;
            }
        }

        /// <summary>
        /// Gets the filtered data view model entity descriptors.
        /// </summary>
        public IEnumerable<DataViewModelEntityDescriptor> FilteredDataViewModelEntityDescriptors
        {
            get
            {
                return
                    this.DataViewModelEntityDescriptors.Where(
                        descriptor =>
                        descriptor.ShouldGenerate
                        && this.ParentObject.Filters.NamespaceFilters.Any(
                            filter =>
                            filter.Name == this.Name
                            && filter.DataViewModelFilters.Any(modelFilter => modelFilter.Name == descriptor.Name)));
            }
        }
    }
}