// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceContainer.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility.Container
{
    using System;
    using System.Collections.Generic;

    using CompactContainer;

    /// <summary>
    /// The Gorba service container implementation for the .NET Compact Framework 3.5.
    /// </summary>
    public partial class ServiceContainer
    {
        private ICompactContainer container;

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
            return this.RegisterInstance(string.Empty, instance);
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
            this.container.Register(
                Component.For<TInterface>().Named(GetFullName<TInterface>(name)).Instance(instance));
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
            return this.container.Resolve(GetFullName(serviceType, name));
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
            return this.container.Resolve<T>(GetFullName<T>(name));
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
            return this.container.GetServices(serviceType);
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
            return this.container.GetServices<T>();
        }

        private static string GetFullName<T>(string name)
        {
            return GetFullName(typeof(T), name);
        }

        private static string GetFullName(Type type, string name)
        {
            // this ensures that service names are unique when used with different types
            // Unity does that correctly, CompactContainer unfortunately not
            return type.FullName + "#" + name;
        }

        partial void Initialize()
        {
            this.container = new CompactContainer();
        }
    }
}