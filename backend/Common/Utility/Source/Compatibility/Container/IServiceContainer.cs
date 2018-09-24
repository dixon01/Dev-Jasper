// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceContainer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility.Container
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the Gorba inversion of control container.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Registers a service instance.
        /// </summary>
        /// <param name="instance">
        /// The service instance.
        /// </param>
        /// <typeparam name="TInterface">
        /// The type of the service to register, usually an interface.
        /// </typeparam>
        /// <returns>
        /// This object.
        /// </returns>
        IServiceContainer RegisterInstance<TInterface>(TInterface instance);

        /// <summary>
        /// Registers a service instance with a name.
        /// </summary>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <param name="instance">
        /// The service instance.
        /// </param>
        /// <typeparam name="TInterface">
        /// The type of the service to register, usually an interface.
        /// </typeparam>
        /// <returns>
        /// This object.
        /// </returns>
        IServiceContainer RegisterInstance<TInterface>(string name, TInterface instance);

        /// <summary>
        /// Resolves the given service type and returns an instance of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The instance implementing the given service type.
        /// </returns>
        object Resolve(Type serviceType);

        /// <summary>
        /// Resolves the given service type with the given name and returns an instance of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <returns>
        /// The instance implementing the given service type that was registered with the given name.
        /// </returns>
        object Resolve(Type serviceType, string name);

        /// <summary>
        /// Resolves the given service type and returns an instance of that service type.
        /// </summary>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// The instance implementing the given service type.
        /// </returns>
        T Resolve<T>();

        /// <summary>
        /// Resolves the given service type and returns an instance of that service type.
        /// </summary>
        /// <param name="name">
        /// The name of the service.
        /// </param>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// The instance implementing the given service type that was registered with the given name.
        /// </returns>
        T Resolve<T>(string name);

        /// <summary>
        /// Resolves the given service type and returns all registered instances of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// All registered instances of that service type.
        /// </returns>
        IEnumerable<object> ResolveAll(Type serviceType);

        /// <summary>
        /// Resolves the given service type and returns all registered instances of that service type.
        /// </summary>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// All registered instances of that service type.
        /// </returns>
        IEnumerable<T> ResolveAll<T>();
    }
}
