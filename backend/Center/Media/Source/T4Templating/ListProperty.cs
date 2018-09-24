// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListProperty.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a list property.
    /// </summary>
    public class ListProperty : PropertyBase
    {
        private readonly Lazy<DataViewModelEntityDescriptor> elementTypeDataViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        public ListProperty()
        {
            this.elementTypeDataViewModel = new Lazy<DataViewModelEntityDescriptor>(this.GetElementTypeDataViewModel);
        }

        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        [XmlAttribute]
        public string ElementType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ListProperty"/> is inline.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inline; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool Inline { get; set; }

        /// <summary>
        /// Gets the element type data view model.
        /// </summary>
        public DataViewModelEntityDescriptor ElementTypeDataViewModel
        {
            get
            {
                return this.elementTypeDataViewModel.Value;
            }
        }

        private DataViewModelEntityDescriptor GetElementTypeDataViewModel()
        {
            var result = this.ParentObject.GetBaseElement(this.ElementType);
            if (result == null)
            {
                throw new InvalidDataException(string.Format("Can't find element for type '{0}'", this.ElementType));
            }

            return result;
        }
    }
}