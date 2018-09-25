// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChange{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyChange type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// Defines a property change.
    /// </summary>
    /// <typeparam name="T">the type of the property.</typeparam>
    public class PropertyChange<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChange{T}"/> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public PropertyChange(T originalValue)
        {
            this.OriginalValue = originalValue;
        }

        /// <summary>
        /// Gets the original value.
        /// </summary>
        public T OriginalValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Changes the value.
        /// </summary>
        /// <param name="value">the new value.</param>
        /// <returns>This property change object with the updated value.</returns>
        public PropertyChange<T> ChangeValue(T value)
        {
            this.Value = value;
            return this;
        }
    }
}