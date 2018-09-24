// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication
{
    using System;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// Class to configure the application to work with the BackgroundSystem.
    /// </summary>
    public class BackgroundSystemConfig
    {
        /// <summary>
        /// Hashed password for the BackgroundSystem admin user.
        /// </summary>
        public const string HashedPassword = "60cdfeb0e638a30af9010fa7958640af";

        /// <summary>
        /// Configures the application to work with the BackgroundSystem.
        /// </summary>
        public static void Configure()
        {
            var backgroundSystemConfiguration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            ServiceBusNotificationManagerUtility.ConfigureServiceBusNotificationManager();
            CreateProductTypeChangeTrackingManager(backgroundSystemConfiguration);
            CreateUnitChangeTrackingManager(backgroundSystemConfiguration);
            CreateUserChangeTrackingManager(backgroundSystemConfiguration);
            CreateTenantChangeTrackingManager(backgroundSystemConfiguration);
            CreateAssociationTenantUserUserRoleChangeTrackingManager(backgroundSystemConfiguration);
            CreateUpdateGroupChangeTrackingManager(backgroundSystemConfiguration);
            CreateUserRoleChangeTrackingManager(backgroundSystemConfiguration);
            CreateAuthorizationChangeTrackingManager(backgroundSystemConfiguration);
        }

        private static void CreateUpdateGroupChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            Common.Client.ChangeTracking.Update.ChangeTrackingSetup.CreateUpdateGroupChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateProductTypeChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            Common.Client.ChangeTracking.Units.ChangeTrackingSetup.CreateProductTypeChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateAssociationTenantUserUserRoleChangeTrackingManager(
            BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<IAssociationTenantUserUserRoleDataService>(
                configuration.DataServices,
                "AssociationTenantUserUserRole");
            Common.Client.ChangeTracking.Membership.ChangeTrackingSetup
                .CreateAssociationTenantUserUserRoleChangeTrackingManager(
                    configuration,
                    new UserCredentials("admin", HashedPassword),
                    ErrorCallback);
        }

        private static void CreateUnitChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<IUnitDataService>(configuration.DataServices, "Unit");
            Common.Client.ChangeTracking.Units.ChangeTrackingSetup.CreateUnitChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateUserChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<IUserDataService>(configuration.DataServices, "User");
            Common.Client.ChangeTracking.Membership.ChangeTrackingSetup.CreateUserChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateTenantChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<ITenantDataService>(configuration.DataServices, "Tenant");
            Common.Client.ChangeTracking.Membership.ChangeTrackingSetup.CreateTenantChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateUserRoleChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<IUserRoleDataService>(configuration.DataServices, "UserRole");
            Common.Client.ChangeTracking.AccessControl.ChangeTrackingSetup.CreateUserRoleChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void CreateAuthorizationChangeTrackingManager(BackgroundSystemConfiguration configuration)
        {
            ConfigureDataService<IAuthorizationDataService>(configuration.DataServices, "Authorization");
            Common.Client.ChangeTracking.AccessControl.ChangeTrackingSetup.CreateAuthorizationChangeTrackingManager(
                configuration,
                new UserCredentials("admin", HashedPassword),
                ErrorCallback);
        }

        private static void ConfigureDataService<T>(RemoteServicesConfiguration configuration, string name)
            where T : class
        {
            ChannelScopeFactoryUtility<T>.ConfigureAsDataService(configuration, name);
        }

        private static void ErrorCallback(Exception exception)
        {
        }
    }
}