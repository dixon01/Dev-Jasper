// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementProperty{T}.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System;

    /// <summary>
    /// Generic management property implementation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of this property.
    /// </typeparam>
    public class ManagementProperty<T> : ManagementProperty
    {
        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProperty{T}"/> class.
        /// The object will be writable and the value is set to the default value of <see cref="T"/>.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public ManagementProperty(string name)
            : base(name, default(T), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProperty{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="readOnly">
        /// Flag to tell if this object is read-only.
        /// </param>
        public ManagementProperty(string name, T value, bool readOnly)
            : base(name, value, readOnly)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the value as a typed object.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// if the property is readonly and you try to set its value.
        /// </exception>
        public new virtual T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.ReadOnly)
                {
                    throw new NotSupportedException("Can't set a readonly property");
                }

                this.value = value;
                base.Value = value;
            }
        }

        /// <summary>
        /// Gets the value of this property as a string.
        /// </summary>
        public override string StringValue
        {
            get
            {
                return string.Format("{0}", this.Value);
            }
        }
    }
}
