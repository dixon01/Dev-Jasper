// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureContentResourceServiceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The content resource service provider that provides content resource service working with Azure storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Resources
{
    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.Common.ServiceModel;

    using NLog;

    /// <summary>
    /// The content resource service provider that provides content resource service working with Azure storage.
    /// </summary>
    public class AzureContentResourceServiceProvider : ContentResourceServiceProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a new content resource service that works with Azure storage
        /// </summary>
        /// <returns>
        /// The <see cref="IResourceService"/>.
        /// </returns>
        public override ContentResourceServiceBase Create()
        {
            Logger.Debug("Creating the resource service.");
            var service = new AzureContentResourceService();
            service.InitializeStorageAsync().Wait();
            return service;
        }
    }
}
