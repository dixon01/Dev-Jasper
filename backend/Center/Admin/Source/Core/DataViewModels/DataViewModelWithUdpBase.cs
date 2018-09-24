// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelWithUdpBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelWithUdpBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Base class for <see cref="DataViewModelBase"/> implementations that have
    /// user defined properties. This class implements <see cref="ICustomTypeDescriptor"/>
    /// to provide these custom properties.
    /// </summary>
    public abstract class DataViewModelWithUdpBase : DataViewModelBase, ICustomTypeDescriptor
    {
        private readonly IList<string> userDefinedPropertyNames;

        private PropertyDescriptorCollection allProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelWithUdpBase"/> class.
        /// </summary>
        /// <param name="readOnlyDataViewModel">
        /// The read-only data view model.
        /// </param>
        /// <param name="factory">
        /// The factory that created this object.
        /// </param>
        /// <param name="userDefinedPropertyNames">
        /// The names of all user defined properties, can be null.
        /// </param>
        protected DataViewModelWithUdpBase(
            ReadOnlyDataViewModelBase readOnlyDataViewModel,
            DataViewModelFactory factory,
            IEnumerable<string> userDefinedPropertyNames)
            : base(readOnlyDataViewModel, factory)
        {
            if (userDefinedPropertyNames != null)
            {
                this.userDefinedPropertyNames = userDefinedPropertyNames.ToList();
            }
        }

        /// <summary>
        /// Gets the names of all user defined properties.
        /// </summary>
        /// <returns>
        /// The names of all user defined properties or null if they were not provided when constructed.
        /// </returns>
        public IList<string> GetUserDefinedPropertyNames()
        {
            return this.userDefinedPropertyNames == null ? null : this.userDefinedPropertyNames.ToList();
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            if (this.allProperties != null)
            {
                return this.allProperties;
            }

            if (this.userDefinedPropertyNames == null || this.userDefinedPropertyNames.Count == 0)
            {
                this.allProperties = TypeDescriptor.GetProperties(this, true);
            }
            else
            {
                var properties =
                    TypeDescriptor.GetProperties(this, true)
                        .OfType<PropertyDescriptor>()
                        .Concat(this.userDefinedPropertyNames.Select(this.CreatePropertyDescriptor));
                this.allProperties = new PropertyDescriptorCollection(properties.ToArray(), true);
            }

            return this.allProperties;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        /// <summary>
        /// Gets the value of a user defined property.
        /// </summary>
        /// <param name="name">
        /// The name of the user defined property.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the property or null if it was never set.
        /// </returns>
        protected abstract string GetUserDefinedPropertyValue(string name);

        /// <summary>
        /// Sets the value of a user defined property.
        /// </summary>
        /// <param name="name">
        /// The name of the user defined property.
        /// </param>
        /// <param name="value">
        /// The new value of the property.
        /// </param>
        protected abstract void SetUserDefinedPropertyValue(string name, string value);

        private UserDefinedPropertyDescriptor CreatePropertyDescriptor(string propertyName)
        {
            return new UserDefinedPropertyDescriptor(
                propertyName,
                this.GetType(),
                () => this.GetUserDefinedPropertyValue(propertyName),
                value => this.SetUserDefinedPropertyValue(propertyName, value));
        }
    }
}