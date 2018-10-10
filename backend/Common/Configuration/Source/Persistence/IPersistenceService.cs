// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistenceService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPersistenceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;

    /// <summary>
    /// Interface for the persistence service that allows to store
    /// some (XML serializable) information for a certain amount of time.
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// Event that is fired before all contexts get serialized.
        /// </summary>
        event EventHandler Saving;

        /// <summary>
        /// Event that is fired after all contexts were serialized.
        /// </summary>
        event EventHandler Saved;

        /// <summary>
        /// Gets or sets the default validity period for newly created contexts.
        /// </summary>
        TimeSpan DefaultValidity { get; set; }

        /// <summary>
        /// Gets the default context for a given type.
        /// The persistence context is used to store and retrieve the value object
        /// and check its validity.
        /// </summary>
        /// <typeparam name="T">the data type stored in the context.</typeparam>
        /// <returns>the context. If necessary, a new context is created.</returns>
        IPersistenceContext<T> GetContext<T>() where T : new();

        /// <summary>
        /// Gets a named context for a given type.
        /// The persistence context is used to store and retrieve the value object
        /// and check its validity.
        /// </summary>
        /// <param name="name">The name of the context.</param>
        /// <typeparam name="T">the data type stored in the context.</typeparam>
        /// <returns>the context. If necessary, a new context is created.</returns>
        IPersistenceContext<T> GetContext<T>(string name) where T : new();
    }
}
