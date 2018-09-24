// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementProperty.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    /// <summary>
    /// Base class for management properties.
    /// Usually you should use <see cref="ManagementProperty{T}"/> instead.
    /// </summary>
    public abstract class ManagementProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProperty"/> class.
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
        protected ManagementProperty(string name, object value, bool readOnly)
        {
            this.Name = name;
            this.Value = value;
            this.ReadOnly = readOnly;
        }

        /// <summary>
        /// Gets the name of this property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object is read-only.
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// Gets or sets the value of this property.
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Gets the value of this property as a string.
        /// </summary>
        public abstract string StringValue { get; }

        /// <summary>
        /// Converts this object to a string.
        /// </summary>
        /// <returns>
        /// the string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Name, this.StringValue);
        }
    }
}
