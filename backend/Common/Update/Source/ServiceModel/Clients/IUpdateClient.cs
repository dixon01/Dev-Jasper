// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Clients
{
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Interface for all update clients
    /// </summary>
    public interface IUpdateClient : IUpdateSource
    {
        /// <summary>
        /// Configures the update client
        /// </summary>
        /// <param name="config">
        /// Update client configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        void Configure(UpdateClientConfigBase config, IUpdateContext context);

        /// <summary>
        /// Starts the update client
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the update client
        /// </summary>
        void Stop();
    }
}