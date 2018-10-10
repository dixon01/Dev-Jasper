// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceContainer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility.Container
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The Gorba service container implementation for the .NET Framework 2.0.
    /// </summary>
    public partial class ServiceContainer
    {
        private IUnityContainer container;

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
        public IServiceContainer RegisterInstance<TInterface>(TInterface instance)
        {
            this.container.RegisterInstance(instance);
            return this;
        }

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
        public IServiceContainer RegisterInstance<TInterface>(string name, TInterface instance)
        {
            this.container.RegisterInstance(name, instance);
            return this;
        }

        /// <summary>
        /// Resolves the given service type and returns an instance of that service type.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The instance implementing the given service type.
        /// </returns>
        public object Resolve(Type serviceType)
        {
            return this.container.Resolve(serviceType);
        }

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
        public object Resolve(Type serviceType, string name)
        {
            try
            {
                return this.container.Resolve(serviceType, name);
            }
            catch (ResolutionFailedException resolutionFailedException)
            {
                Debug.WriteIf(serviceType.IsInterface, string.Format("Type is Interface and cannot be created Type:{0}, Name:[{1}], Exception:{2}", serviceType, name, resolutionFailedException.Message));
                throw;
            }
        }

        /// <summary>
        /// Resolves the given service type and returns an instance of that service type.
        /// </summary>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// The instance implementing the given service type.
        /// </returns>
        public T Resolve<T>()
        {
            return this.container.Resolve<T>();
        }

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
        public T Resolve<T>(string name)
        {
            return this.container.Resolve<T>(name);
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
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return this.container.ResolveAll(serviceType);
        }

        /// <summary>
        /// Resolves the given service type and returns all registered instances of that service type.
        /// </summary>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// All registered instances of that service type.
        /// </returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return this.container.ResolveAll<T>();
        }

        partial void Initialize()
        {
            this.container = new UnityContainer();
        }
    }
}