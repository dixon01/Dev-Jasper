// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyFilter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a property filter.
    /// </summary>
    public class PropertyFilter : IChildItem<DataViewModelFilter>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this property is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this property is hidden; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsHidden { get; set; }

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
        public DataViewModelFilter ParentObject { get; set; }

        DataViewModelFilter IChildItem<DataViewModelFilter>.Parent
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
        public string UserVisibleGroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user visible field.
        /// </summary>
        /// <value>
        /// The name of the user visible field.
        /// </value>
        [XmlAttribute]
        public string UserVisibleFieldName { get; set; }

        /// <summary>
        /// Gets or sets the user visible order index.
        /// </summary>
        /// <remarks>
        /// This field is used to customize the order of the properties of an element
        /// </remarks>
        [XmlAttribute]
        public int UserVisibleOrderIndex { get; set; }

        /// <summary>
        /// Gets or sets the user visible group order index.
        /// </summary>
        [XmlAttribute]
        public int UserVisibleGroupOrderIndex { get; set; }
    }
}