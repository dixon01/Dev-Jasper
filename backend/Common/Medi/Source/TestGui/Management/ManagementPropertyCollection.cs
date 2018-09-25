// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementPropertyCollection.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementPropertyCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui.Management
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// A collection of <see cref="ManagementProperty"/> objects.
    /// </summary>
    internal abstract class ManagementPropertyCollection : ICustomTypeDescriptor
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementPropertyCollection"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected ManagementPropertyCollection(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.name;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return this.name;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(null);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return ((ICustomTypeDescriptor)this).GetProperties();
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            var props = new PropertyDescriptorCollection(null);

            foreach (var property in this.GetProperties())
            {
                if (!property.GetType().IsGenericType)
                {
                    continue;
                }

                var descriptorType =
                    typeof(ManagementPropertyDescriptor<>).MakeGenericType(
                        property.GetType().GetGenericArguments()[0]);

                props.Add((PropertyDescriptor)Activator.CreateInstance(descriptorType, property));
            }

            return props;
        }

        /// <summary>
        /// Gets the list of properties.
        /// </summary>
        /// <returns>
        /// the list of properties.
        /// </returns>
        protected abstract IEnumerable<ManagementProperty> GetProperties();

        private class ManagementPropertyDescriptor<T> : PropertyDescriptor
        {
            private readonly ManagementProperty<T> property;

            public ManagementPropertyDescriptor(ManagementProperty<T> property)
                : base(property.Name, null)
            {
                this.property = property;
            }

            public override Type ComponentType
            {
                get
                {
                    return typeof(IManagementProvider);
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return this.property.ReadOnly;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return typeof(T);
                }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return this.GetRealProperty(component).Value;
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                this.GetRealProperty(component).Value = (T)value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            private ManagementProperty<T> GetRealProperty(object component)
            {
                var collection = (ManagementPropertyCollection)component;
                foreach (var prop in collection.GetProperties())
                {
                    if (prop.Name == this.Name)
                    {
                        return (ManagementProperty<T>)prop;
                    }
                }

                throw new ArgumentException("Couldn't find property " + this.Name);
            }
        }
    }
}