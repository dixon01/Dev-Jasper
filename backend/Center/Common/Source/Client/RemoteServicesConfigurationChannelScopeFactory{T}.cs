// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteServicesConfigurationChannelScopeFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteServicesConfigurationChannelScopeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Defines a channel scope factory based on a <see cref="RemoteServicesConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    internal abstract class RemoteServicesConfigurationChannelScopeFactory<T> :
        ChannelScopeFactory<T>.ChannelFactoryChannelScopeFactory
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServicesConfigurationChannelScopeFactory{T}"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        protected RemoteServicesConfigurationChannelScopeFactory(
            RemoteServicesConfiguration configuration,
            string serviceName)
        {
            this.Configuration = configuration;
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        protected RemoteServicesConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the service name.
        /// </summary>
        protected string ServiceName { get; private set; }
    }
}