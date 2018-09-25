// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistenceContext{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPersistenceContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;

    /// <summary>
    /// Persistence context that holds all information about the stored value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to be stored in the context. This type has to be
    /// XML serializable (i.e. public and with empty constructor).
    /// </typeparam>
    public interface IPersistenceContext<T>
        where T : new()
    {
        /// <summary>
        /// Gets or sets the validity period.
        /// </summary>
        TimeSpan Validity { get; set; }

        /// <summary>
        /// Gets a value indicating whether the value is still valid.
        /// This method checks the <see cref="Validity"/> period.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// Gets or sets the serializable value object.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Makes this context valid again (setting the timestamp to the current value).
        /// </summary>
        void Revalidate();
    }
}
