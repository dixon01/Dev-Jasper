// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelScopeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelScopeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    using System.ServiceModel;

    /// <summary>
    /// Defines a factory for <see cref="ChannelScope"/>s.
    /// </summary>
    public static class ChannelScopeFactory
    {
        /// <summary>
        /// Creates a new channel scope.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A new channel scope.</returns>
        public static ChannelScope Create(ServicesConfiguration configuration)
        {
            var endpoint = configuration.GetEndpoint();
            var channelFactory = new ChannelFactory<ISampleService>(endpoint);
            configuration.SetLogin(channelFactory);
            var channel = channelFactory.CreateChannel();
            return new ChannelScope(channel);
        }
    }
}