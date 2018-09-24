// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMapper.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    /// <summary>
    /// Interface for a type that maps integer keys to
    /// values and vice versa.
    /// </summary>
    /// <typeparam name="T">
    /// The type to be mapped.
    /// </typeparam>
    internal interface IMapper<T>
    {
        /// <summary>
        /// Gets the value for a given key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The value for the given key or null if 
        /// the key does not exist.
        /// </returns>
        T this[int key] { get; }

        /// <summary>
        /// Gets the key for a given value.
        /// This method might create a new key if it
        /// does not yet exist.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The key for the given value.
        /// </returns>
        int this[T value] { get; }
    }
}