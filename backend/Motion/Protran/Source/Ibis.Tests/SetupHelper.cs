// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetupHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests
{
    using System;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Helper class for setting up all necessary services.
    /// </summary>
    public static class SetupHelper
    {
        /// <summary>
        /// Creates the service container and registers all <see cref="Gorba.Motion.Protran.Core"/>
        /// services.
        /// </summary>
        public static void SetupCoreServices()
        {
            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));
            Core.Protran.SetupCoreServices();

            var persistenceService = serviceContainer.Resolve<IPersistenceService>() as IPersistenceServiceImpl;
            if (persistenceService != null)
            {
                persistenceService.Configure(
                    PathManager.Instance.CreatePath(FileType.Data, "Persistence.xml"), TimeSpan.Zero, false);
            }
        }
    }
}
