// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDefinedPropertyDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDefinedPropertyDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Implementation of <see cref="PropertyDescriptor"/> to support user defined properties.
    /// </summary>
    public class UserDefinedPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type componentType;

        private readonly Func<string> getter;

        private readonly Action<string> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefinedPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="componentType">
        /// The type of the class owning this property.
        /// </param>
        /// <param name="getter">
        /// The getter method to be called when getting this property.
        /// </param>
        /// <param name="setter">
        /// The setter method to be called when setting this property.
        /// If this argument is null, the property is considered to be read-only.
        /// </param>
        public UserDefinedPropertyDescriptor(
            string name, Type componentType, Func<string> getter, Action<string> setter = null)
            : base(name, null)
        {
            if (getter == null)
            {
                throw new ArgumentNullException("getter");
            }

            this.componentType = componentType;
            this.getter = getter;
            this.setter = setter;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of component this property is bound to.
        /// When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or
        /// <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/>
        /// methods are invoked, the object specified might be an instance of this type.
        /// </returns>
        public override Type ComponentType
        {
            get
            {
                return this.componentType;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <returns>
        /// true if the property is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly
        {
            get
            {
                return this.setter == null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the property.
        /// </returns>
        public override Type PropertyType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        /// <param name="comp">The component to test for reset capability. </param>
        public override bool CanResetValue(object comp)
        {
            return this.setter != null && !string.IsNullOrEmpty(this.getter());
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        /// <param name="comp">The component with the property for which to retrieve the value. </param>
        public override object GetValue(object comp)
        {
            return this.getter();
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property
        /// of the component to the default value.
        /// </summary>
        /// <param name="comp">The component with the property value that is to be reset to the default value. </param>
        public override void ResetValue(object comp)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            this.setter(null);
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="comp">The component with the property value that is to be set. </param>
        /// <param name="value">The new value. </param>
        public override void SetValue(object comp, object value)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException();
            }

            this.setter((string)value);
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating
        /// whether the value of this property needs to be persisted.
        /// </summary>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        /// <param name="comp">The component with the property to be examined for persistence.</param>
        public override bool ShouldSerializeValue(object comp)
        {
            return false;
        }
    }
}