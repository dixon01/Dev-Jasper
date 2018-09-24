// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelFilter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a filter for a data view model.
    /// </summary>
    public class DataViewModelFilter : IChildItem<NamespaceFilter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelFilter"/> class.
        /// </summary>
        public DataViewModelFilter()
        {
            this.PropertyFilters = new ChildItemCollection<DataViewModelFilter, PropertyFilter>(this);
        }

        /// <summary>
        /// Gets the property filters.
        /// </summary>
        [XmlElement("Property")]
        public ChildItemCollection<DataViewModelFilter, PropertyFilter> PropertyFilters { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlAttribute("ModelName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        /// <value>
        /// The parent object.
        /// </value>
        [XmlIgnore]
        public NamespaceFilter ParentObject { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        NamespaceFilter IChildItem<NamespaceFilter>.Parent
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
        /// Gets or sets the default name of the user visible group.
        /// </summary>
        /// <value>
        /// The default name of the user visible group.
        /// </value>
        [XmlAttribute]
        public string DefaultUserVisibleGroupName { get; set; }

        /// <summary>
        /// Gets or sets the default user visible group order index.
        /// </summary>
        [XmlAttribute]
        public int DefaultUserVisibleGroupOrderIndex { get; set; }
    }
}