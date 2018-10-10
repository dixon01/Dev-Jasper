// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelScopeFactoryUtility.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelScopeFactoryUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Utility methods to configure channel scope factories.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    public static class ChannelScopeFactoryUtility<T>
        where T : class
    {
        /// <summary>
        /// Configures the factory for a data service.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serviceName">The name.</param>
        public static void ConfigureAsDataService(
            RemoteServicesConfiguration configuration,
            string serviceName)
        {
            var factory = new DataServicesConfigurationChannelScopeFactory<T>(configuration, serviceName);
            ChannelScopeFactory<T>.SetCurrent(factory);
        }

        /// <summary>
        /// The configure as functional service.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serviceName">The name.</param>
        public static void ConfigureAsFunctionalService(
            RemoteServicesConfiguration configuration,
            string serviceName)
        {
            var factory = new FunctionalServicesConfigurationChannelScopeFactory<T>(configuration, serviceName);
            ChannelScopeFactory<T>.SetCurrent(factory);
        }
    }
}