// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceProperty.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines a reference property.
    /// </summary>
    public class ReferenceProperty : TypedProperty
    {
        private readonly Lazy<DataViewModelEntityDescriptor> referencedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceProperty"/> class.
        /// </summary>
        public ReferenceProperty()
        {
            this.referencedType = new Lazy<DataViewModelEntityDescriptor>(this.GetReferencedType);
        }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>
        /// The reference.
        /// </value>
        [XmlAttribute]
        public string Reference { get; set; }

        /// <summary>
        /// Gets the type of the referenced.
        /// </summary>
        /// <value>
        /// The type of the referenced.
        /// </value>
        public DataViewModelEntityDescriptor ReferencedType
        {
            get
            {
                return this.referencedType.Value;
            }
        }

        private DataViewModelEntityDescriptor GetReferencedType()
        {
            if (string.IsNullOrEmpty(this.Reference))
            {
                var message = string.Format("Reference value not found for property '{0}'", this.Name);
                throw new InvalidDataException(message);
            }

            if (!this.Reference.EndsWith(".Name"))
            {
                var message =
                    string.Format(
                        "Only reference on 'Name' property are supported. Can't generate reference '{0}'", this.Name);
                throw new InvalidDataException(message);
            }

            var reference = this.Reference.Substring(0, this.Reference.Length - 5); // 5: '.Name'
            var element = this.ParentObject.GetBaseElement(reference);
            return element;
        }
    }
}