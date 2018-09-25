// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceServiceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The provider to create resource services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System;

    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The provider to create resource services.
    /// </summary>
    public abstract class ResourceServiceProvider
    {
        static ResourceServiceProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static ResourceServiceProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = DefaultResourceServiceProvider.Instance;
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(ResourceServiceProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Creates a new resource service
        /// </summary>
        /// <returns>
        /// The <see cref="IResourceService"/>.
        /// </returns>
        /// <remarks>
        /// The default resource service provider returns the resource service working with local files.
        /// </remarks>
        public abstract ResourceServiceBase Create();

        private class DefaultResourceServiceProvider : ResourceServiceProvider
        {
            static DefaultResourceServiceProvider()
            {
                Instance = new DefaultResourceServiceProvider();
            }

            public static DefaultResourceServiceProvider Instance { get; private set; }

            public override ResourceServiceBase Create()
            {
                return new LocalResourceService();
            }
        }
    }
}
