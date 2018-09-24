// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalRepositoryConfigurationProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provider for the Update repository configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Configuration
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Common.Update.ServiceModel.Repository;

    /// <summary>
    /// Provider for the Update repository configuration.
    /// </summary>
    public abstract class PortalRepositoryConfigurationProvider
    {
        static PortalRepositoryConfigurationProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static PortalRepositoryConfigurationProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = new DefaultPortalRepositoryConfigurationProvider();
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(PortalRepositoryConfigurationProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the Update repository configuration.
        /// </summary>
        /// <returns>
        /// The task returning the <see cref="RepositoryConfig"/>.
        /// </returns>
        /// <remarks>
        /// In the default provider the repository configuration is not used and so returns an empty configuration.
        /// </remarks>
        public abstract Task<RepositoryConfig> GetRepositoryConfigurationAsync();

        private class DefaultPortalRepositoryConfigurationProvider : PortalRepositoryConfigurationProvider
        {
            /// <summary>
            /// Gets the Update repository configuration.
            /// </summary>
            /// <returns>
            /// The task returning the <see cref="RepositoryConfig"/>.
            /// </returns>
            public override Task<RepositoryConfig> GetRepositoryConfigurationAsync()
            {
                return Task.FromResult(new RepositoryConfig());
            }
        }
    }
}
