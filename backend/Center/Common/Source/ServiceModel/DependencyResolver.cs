// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolver.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DependencyResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a dependency resolver to register and retrieve instances.
    /// </summary>
    public abstract class DependencyResolver
    {
        static DependencyResolver()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current dependency resolver.
        /// </summary>
        public static DependencyResolver Current { get; private set; }

        /// <summary>
        /// Resets the current resolver to the default one.
        /// </summary>
        public static void Reset()
        {
            Set(DefaultDependencyResolver.Instance);
        }

        /// <summary>
        /// Sets the current resolver to the given instance.
        /// </summary>
        /// <param name="instance">The instance to be set as current.</param>
        /// <exception cref="ArgumentNullException">A null instance is provided.</exception>
        public static void Set(DependencyResolver instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Registers an instance for the given type.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance to register.</param>
        public abstract void Register<T>(T instance);

        /// <summary>
        /// Gets the instance registered for the given type.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>The instance registered for the given type.</returns>
        /// <exception cref="KeyNotFoundException">The specified type was not registered.</exception>
        public abstract T Get<T>();

        private class DefaultDependencyResolver : DependencyResolver
        {
            private static readonly Lazy<DefaultDependencyResolver> LazyInstance =
                new Lazy<DefaultDependencyResolver>(() => new DefaultDependencyResolver());

            private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

            public static DefaultDependencyResolver Instance
            {
                get
                {
                    return LazyInstance.Value;
                }
            }

            public override void Register<T>(T instance)
            {
                this.instances[typeof(T)] = instance;
            }

            public override T Get<T>()
            {
                if (this.instances.ContainsKey(typeof(T)))
                {
                    return (T)this.instances[typeof(T)];
                }

                throw new KeyNotFoundException("Item of type '" + typeof(T).FullName + "' not registered");
            }
        }
    }
}