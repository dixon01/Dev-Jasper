namespace Gorba.Center.BackgroundSystem.Core.Setup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.ExceptionServices;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    using Version = Gorba.Center.Common.ServiceModel.ChangeTracking.Version;
    using Gorba.Center.BackgroundSystem.Core.AccessControl;
    using Gorba.Center.BackgroundSystem.Core.Configurations;
    using Gorba.Center.BackgroundSystem.Core.Documents;
    using Gorba.Center.BackgroundSystem.Core.Log;
    using Gorba.Center.BackgroundSystem.Core.Membership;
    using Gorba.Center.BackgroundSystem.Core.Meta;
    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.BackgroundSystem.Core.Software;
    using Gorba.Center.BackgroundSystem.Core.Units;
    using Gorba.Center.BackgroundSystem.Core.Update;

    public static class ChangeTrackingDataServicesUtility
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<IEnumerable<DisposableServiceHost>> SetupChangeTrackingDataServicesAsync(
            BackgroundSystemConfiguration configuration)
        {
            var hosts = new List<DisposableServiceHost>();
            var tasks = new List<Task>();
            ServiceHost host;
            Task task;

            var associationTenantUserUserRoleChangeTrackingDataService = new AssociationTenantUserUserRoleChangeTrackingDataService(
                DataServicesUtility.CreateAssociationTenantUserUserRoleDataService(),
                configuration,
                CreateAssociationTenantUserUserRoleConfiguration(configuration));
            task = associationTenantUserUserRoleChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IAssociationTenantUserUserRoleDataService>(associationTenantUserUserRoleChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IAssociationTenantUserUserRoleDataService>(associationTenantUserUserRoleChangeTrackingDataService, "AssociationTenantUserUserRole");
            hosts.Add(new DisposableServiceHost("AssociationTenantUserUserRole", associationTenantUserUserRoleChangeTrackingDataService, host));

            var authorizationChangeTrackingDataService = new AuthorizationChangeTrackingDataService(
                DataServicesUtility.CreateAuthorizationDataService(),
                configuration,
                CreateAuthorizationConfiguration(configuration));
            task = authorizationChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IAuthorizationDataService>(authorizationChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IAuthorizationDataService>(authorizationChangeTrackingDataService, "Authorization");
            hosts.Add(new DisposableServiceHost("Authorization", authorizationChangeTrackingDataService, host));

            var contentResourceChangeTrackingDataService = new ContentResourceChangeTrackingDataService(
                DataServicesUtility.CreateContentResourceDataService(),
                configuration,
                CreateContentResourceConfiguration(configuration));
            task = contentResourceChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IContentResourceDataService>(contentResourceChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IContentResourceDataService>(contentResourceChangeTrackingDataService, "ContentResource");
            hosts.Add(new DisposableServiceHost("ContentResource", contentResourceChangeTrackingDataService, host));

            var documentChangeTrackingDataService = new DocumentChangeTrackingDataService(
                DataServicesUtility.CreateDocumentDataService(),
                configuration,
                CreateDocumentConfiguration(configuration));
            task = documentChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IDocumentDataService>(documentChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IDocumentDataService>(documentChangeTrackingDataService, "Document");
            hosts.Add(new DisposableServiceHost("Document", documentChangeTrackingDataService, host));

            var documentVersionChangeTrackingDataService = new DocumentVersionChangeTrackingDataService(
                DataServicesUtility.CreateDocumentVersionDataService(),
                configuration,
                CreateDocumentVersionConfiguration(configuration));
            task = documentVersionChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IDocumentVersionDataService>(documentVersionChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IDocumentVersionDataService>(documentVersionChangeTrackingDataService, "DocumentVersion");
            hosts.Add(new DisposableServiceHost("DocumentVersion", documentVersionChangeTrackingDataService, host));

            var mediaConfigurationChangeTrackingDataService = new MediaConfigurationChangeTrackingDataService(
                DataServicesUtility.CreateMediaConfigurationDataService(),
                configuration,
                CreateMediaConfigurationConfiguration(configuration));
            task = mediaConfigurationChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IMediaConfigurationDataService>(mediaConfigurationChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IMediaConfigurationDataService>(mediaConfigurationChangeTrackingDataService, "MediaConfiguration");
            hosts.Add(new DisposableServiceHost("MediaConfiguration", mediaConfigurationChangeTrackingDataService, host));

            var packageChangeTrackingDataService = new PackageChangeTrackingDataService(
                DataServicesUtility.CreatePackageDataService(),
                configuration,
                CreatePackageConfiguration(configuration));
            task = packageChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IPackageDataService>(packageChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IPackageDataService>(packageChangeTrackingDataService, "Package");
            hosts.Add(new DisposableServiceHost("Package", packageChangeTrackingDataService, host));

            var packageVersionChangeTrackingDataService = new PackageVersionChangeTrackingDataService(
                DataServicesUtility.CreatePackageVersionDataService(),
                configuration,
                CreatePackageVersionConfiguration(configuration));
            task = packageVersionChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IPackageVersionDataService>(packageVersionChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IPackageVersionDataService>(packageVersionChangeTrackingDataService, "PackageVersion");
            hosts.Add(new DisposableServiceHost("PackageVersion", packageVersionChangeTrackingDataService, host));

            var productTypeChangeTrackingDataService = new ProductTypeChangeTrackingDataService(
                DataServicesUtility.CreateProductTypeDataService(),
                configuration,
                CreateProductTypeConfiguration(configuration));
            task = productTypeChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IProductTypeDataService>(productTypeChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IProductTypeDataService>(productTypeChangeTrackingDataService, "ProductType");
            hosts.Add(new DisposableServiceHost("ProductType", productTypeChangeTrackingDataService, host));

            var resourceChangeTrackingDataService = new ResourceChangeTrackingDataService(
                DataServicesUtility.CreateResourceDataService(),
                configuration,
                CreateResourceConfiguration(configuration));
            task = resourceChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IResourceDataService>(resourceChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IResourceDataService>(resourceChangeTrackingDataService, "Resource");
            hosts.Add(new DisposableServiceHost("Resource", resourceChangeTrackingDataService, host));

            var systemConfigChangeTrackingDataService = new SystemConfigChangeTrackingDataService(
                DataServicesUtility.CreateSystemConfigDataService(),
                configuration,
                CreateSystemConfigConfiguration(configuration));
            task = systemConfigChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<ISystemConfigDataService>(systemConfigChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<ISystemConfigDataService>(systemConfigChangeTrackingDataService, "SystemConfig");
            hosts.Add(new DisposableServiceHost("SystemConfig", systemConfigChangeTrackingDataService, host));

            var tenantChangeTrackingDataService = new TenantChangeTrackingDataService(
                DataServicesUtility.CreateTenantDataService(),
                configuration,
                CreateTenantConfiguration(configuration));
            task = tenantChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<ITenantDataService>(tenantChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<ITenantDataService>(tenantChangeTrackingDataService, "Tenant");
            hosts.Add(new DisposableServiceHost("Tenant", tenantChangeTrackingDataService, host));

            var unitChangeTrackingDataService = new UnitChangeTrackingDataService(
                DataServicesUtility.CreateUnitDataService(),
                configuration,
                CreateUnitConfiguration(configuration));
            task = unitChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUnitDataService>(unitChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUnitDataService>(unitChangeTrackingDataService, "Unit");
            hosts.Add(new DisposableServiceHost("Unit", unitChangeTrackingDataService, host));

            var unitConfigurationChangeTrackingDataService = new UnitConfigurationChangeTrackingDataService(
                DataServicesUtility.CreateUnitConfigurationDataService(),
                configuration,
                CreateUnitConfigurationConfiguration(configuration));
            task = unitConfigurationChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUnitConfigurationDataService>(unitConfigurationChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUnitConfigurationDataService>(unitConfigurationChangeTrackingDataService, "UnitConfiguration");
            hosts.Add(new DisposableServiceHost("UnitConfiguration", unitConfigurationChangeTrackingDataService, host));

            var updateCommandChangeTrackingDataService = new UpdateCommandChangeTrackingDataService(
                DataServicesUtility.CreateUpdateCommandDataService(),
                configuration,
                CreateUpdateCommandConfiguration(configuration));
            task = updateCommandChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUpdateCommandDataService>(updateCommandChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUpdateCommandDataService>(updateCommandChangeTrackingDataService, "UpdateCommand");
            hosts.Add(new DisposableServiceHost("UpdateCommand", updateCommandChangeTrackingDataService, host));

            var updateFeedbackChangeTrackingDataService = new UpdateFeedbackChangeTrackingDataService(
                DataServicesUtility.CreateUpdateFeedbackDataService(),
                configuration,
                CreateUpdateFeedbackConfiguration(configuration));
            task = updateFeedbackChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUpdateFeedbackDataService>(updateFeedbackChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUpdateFeedbackDataService>(updateFeedbackChangeTrackingDataService, "UpdateFeedback");
            hosts.Add(new DisposableServiceHost("UpdateFeedback", updateFeedbackChangeTrackingDataService, host));

            var updateGroupChangeTrackingDataService = new UpdateGroupChangeTrackingDataService(
                DataServicesUtility.CreateUpdateGroupDataService(),
                configuration,
                CreateUpdateGroupConfiguration(configuration));
            task = updateGroupChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUpdateGroupDataService>(updateGroupChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUpdateGroupDataService>(updateGroupChangeTrackingDataService, "UpdateGroup");
            hosts.Add(new DisposableServiceHost("UpdateGroup", updateGroupChangeTrackingDataService, host));

            var updatePartChangeTrackingDataService = new UpdatePartChangeTrackingDataService(
                DataServicesUtility.CreateUpdatePartDataService(),
                configuration,
                CreateUpdatePartConfiguration(configuration));
            task = updatePartChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUpdatePartDataService>(updatePartChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUpdatePartDataService>(updatePartChangeTrackingDataService, "UpdatePart");
            hosts.Add(new DisposableServiceHost("UpdatePart", updatePartChangeTrackingDataService, host));

            var userChangeTrackingDataService = new UserChangeTrackingDataService(
                DataServicesUtility.CreateUserDataService(),
                configuration,
                CreateUserConfiguration(configuration));
            task = userChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUserDataService>(userChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUserDataService>(userChangeTrackingDataService, "User");
            hosts.Add(new DisposableServiceHost("User", userChangeTrackingDataService, host));

            var userDefinedPropertyChangeTrackingDataService = new UserDefinedPropertyChangeTrackingDataService(
                DataServicesUtility.CreateUserDefinedPropertyDataService(),
                configuration,
                CreateUserDefinedPropertyConfiguration(configuration));
            task = userDefinedPropertyChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUserDefinedPropertyDataService>(userDefinedPropertyChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUserDefinedPropertyDataService>(userDefinedPropertyChangeTrackingDataService, "UserDefinedProperty");
            hosts.Add(new DisposableServiceHost("UserDefinedProperty", userDefinedPropertyChangeTrackingDataService, host));

            var userRoleChangeTrackingDataService = new UserRoleChangeTrackingDataService(
                DataServicesUtility.CreateUserRoleDataService(),
                configuration,
                CreateUserRoleConfiguration(configuration));
            task = userRoleChangeTrackingDataService.StartAsync();
            tasks.Add(task);
            DependencyResolver.Current.Register<IUserRoleDataService>(userRoleChangeTrackingDataService);
            host = DataServicesUtility.CreateServiceHost<IUserRoleDataService>(userRoleChangeTrackingDataService, "UserRole");
            hosts.Add(new DisposableServiceHost("UserRole", userRoleChangeTrackingDataService, host));
            await Task.WhenAll(tasks);
            return hosts;
        }

        private static NotificationSubscriptionConfiguration CreateAssociationTenantUserUserRoleConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "AssociationTenantUserUserRoles"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateAuthorizationConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Authorizations"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateContentResourceConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "ContentResources"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateDocumentConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Documents"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateDocumentVersionConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "DocumentVersions"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateMediaConfigurationConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "MediaConfigurations"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreatePackageConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Packages"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreatePackageVersionConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "PackageVersions"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateProductTypeConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "ProductTypes"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateResourceConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Resources"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateSystemConfigConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "SystemConfigs"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateTenantConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Tenants"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUnitConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Units"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUnitConfigurationConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UnitConfigurations"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUpdateCommandConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UpdateCommands"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUpdateFeedbackConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UpdateFeedbacks"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUpdateGroupConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UpdateGroups"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUpdatePartConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UpdateParts"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUserConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "Users"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUserDefinedPropertyConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UserDefinedProperties"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }

        private static NotificationSubscriptionConfiguration CreateUserRoleConfiguration(
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .NotificationsConnectionString,
                                        Path = "UserRoles"
                                    };
            return new NotificationSubscriptionConfiguration(
                                                        configuration,
                                                        "BackgroundSystem",
                                                        "Service")
                                                    {
                                                        Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NOT NULL"),
                                                        Timeout = TimeSpan.FromDays(31)
                                                    };
        }
    }
}
