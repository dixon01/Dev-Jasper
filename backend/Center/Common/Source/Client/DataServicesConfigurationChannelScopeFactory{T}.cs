// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServicesConfigurationChannelScopeFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataServicesConfigurationChannelScopeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Defines a channel scope factory for data services.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    internal class DataServicesConfigurationChannelScopeFactory<T> : RemoteServicesConfigurationChannelScopeFactory<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServicesConfigurationChannelScopeFactory{T}"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        public DataServicesConfigurationChannelScopeFactory(
            RemoteServicesConfiguration configuration,
            string serviceName)
            : base(configuration, serviceName)
        {
        }

        /// <summary>
        /// Creates the internal channel factory.
        /// </summary>
        /// <returns>
        /// The internal <see cref="ChannelFactory"/>.
        /// </returns>
        protected override ChannelFactory<T> CreateInternalChannelFactory()
        {
            return this.Configuration.CreateDataServicesChannelFactory<T>(this.ServiceName);
        }
    }
}