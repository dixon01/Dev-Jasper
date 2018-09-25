// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Providers
{
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Interface for all update providers
    /// </summary>
    public interface IUpdateProvider : IUpdateSink
    {
        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        void Configure(UpdateProviderConfigBase config, IUpdateContext context);

        /// <summary>
        /// Starts the update provider
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the update provider
        /// </summary>
        void Stop();
    }
}
