// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceContainerLocator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceContainerLocator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility.Container
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The service locator implementation that provides the Gorba service container.
    /// </summary>
    public class ServiceContainerLocator : ServiceLocatorImplBase
    {
        private readonly IServiceContainer serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainerLocator"/> class.
        /// </summary>
        /// <param name="serviceContainer">
        /// The service container.
        /// </param>
        public ServiceContainerLocator(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }

        /// <summary>
        /// Resolves the given service type with the given name and returns an instance of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <param name="key">
        /// The name of the service (can be null).
        /// </param>
        /// <returns>
        /// The instance implementing the given service type that was registered with the given name.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.serviceContainer.Resolve(serviceType, key);
        }

        /// <summary>
        /// Resolves the given service type and returns all registered instances of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// All registered instances of that service type.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.serviceContainer.ResolveAll(serviceType);
        }
    }
}