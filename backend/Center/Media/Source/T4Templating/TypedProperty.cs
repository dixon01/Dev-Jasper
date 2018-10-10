// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedProperty.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypedProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines properties with type.
    /// </summary>
    public abstract class TypedProperty : PropertyBase
    {
        private readonly Lazy<DataViewModelEntityDescriptor> typeDataViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedProperty"/> class.
        /// </summary>
        protected TypedProperty()
        {
            this.typeDataViewModel = new Lazy<DataViewModelEntityDescriptor>(this.GetTypeDataViewModel);
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [XmlAttribute]
        public string Type { get; set; }

        /// <summary>
        /// Gets the type data view model.
        /// </summary>
        public DataViewModelEntityDescriptor TypeDataViewModel
        {
            get
            {
                return this.typeDataViewModel.Value;
            }
        }

        private DataViewModelEntityDescriptor GetTypeDataViewModel()
        {
            if (this.Type == "DynamicProperty")
            {
                return new DataViewModelEntityDescriptor
                           {
                               Name = "DynamicProperty",
                               ParentObject = new NamespaceEntityDescriptor
                                                  {
                                                      Name = "Eval"
                                                  }
                           };
            }

            var result = this.ParentObject.GetBaseElement(this.Type);
            if (result == null)
            {
                throw new InvalidDataException(string.Format("Can't find element for type '{0}'", this.Type));
            }

            return result;
        }
    }
}