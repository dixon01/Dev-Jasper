// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceServiceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System;

    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The provider to create content resource services.
    /// </summary>
    public abstract class ContentResourceServiceProvider
    {
        /// <summary>
        /// Initializes static members of the <see cref="ContentResourceServiceProvider"/> class.
        /// </summary>
        static ContentResourceServiceProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
          public static ContentResourceServiceProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = DefaultContentResourceServiceProvider.Instance;
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The instance is null.
        /// </exception>
        public static void Set(ContentResourceServiceProvider instance)
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
        /// The <see cref="IContentResourceService"/>.
        /// </returns>
        /// <remarks>
        /// The default content resource service provider returns the content resource service working with local files.
        /// </remarks>
        public abstract ContentResourceServiceBase Create();

        private class DefaultContentResourceServiceProvider : ContentResourceServiceProvider
        {
            static DefaultContentResourceServiceProvider()
            {
                Instance = new DefaultContentResourceServiceProvider();
            }

            public static DefaultContentResourceServiceProvider Instance { get; private set; }

            public override ContentResourceServiceBase Create()
            {
                return new LocalContentResourceService();
            }
        }
    }
}
