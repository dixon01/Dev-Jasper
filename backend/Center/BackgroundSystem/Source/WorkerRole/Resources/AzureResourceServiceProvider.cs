// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureResourceServiceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The resource service provider that provides resource service working with Azure storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Resources
{
    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.Common.ServiceModel;

    using NLog;

    /// <summary>
    /// The resource service provider that provides resource service working with Azure storage.
    /// </summary>
    public class AzureResourceServiceProvider : ResourceServiceProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a new resource service that works with Azure storage
        /// </summary>
        /// <returns>
        /// The <see cref="IResourceService"/>.
        /// </returns>
        public override ResourceServiceBase Create()
        {
            Logger.Debug("Creating the resource service.");
            var service = new AzureResourceService();
            service.InitializeStorageAsync().Wait();
            return service;
        }
    }
}
