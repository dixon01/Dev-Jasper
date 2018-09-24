namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.Client.ChangeTracking.AccessControl;
    using Gorba.Center.Common.Client.ChangeTracking.Configurations;
    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Meta;
    using Gorba.Center.Common.Client.ChangeTracking.Resources;
    using Gorba.Center.Common.Client.ChangeTracking.Software;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Security;

    public abstract partial class ChangeTrackingManagementBootstrapper
    {
        private const int TotalCount = 21;

        public IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; private set; }

        public IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; private set; }

        public ITenantChangeTrackingManager TenantChangeTrackingManager { get; private set; }

        public IUserChangeTrackingManager UserChangeTrackingManager { get; private set; }

        public IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; private set; }

        public IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; private set; }

        public IUnitChangeTrackingManager UnitChangeTrackingManager { get; private set; }

        public IResourceChangeTrackingManager ResourceChangeTrackingManager { get; private set; }

        public IContentResourceChangeTrackingManager ContentResourceChangeTrackingManager { get; private set; }

        public IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; private set; }

        public IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; private set; }

        public IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; private set; }

        public IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; private set; }

        public IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; private set; }

        public IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; private set; }

        public IPackageChangeTrackingManager PackageChangeTrackingManager { get; private set; }

        public IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; private set; }

        public IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; private set; }

        public IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; private set; }

        public IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; private set; }

        public ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; private set; }

        public IEnumerable<string> GetPaths()
        {
            yield return "UserRoles";
            yield return "Authorizations";
            yield return "Tenants";
            yield return "Users";
            yield return "AssociationTenantUserUserRoles";
            yield return "ProductTypes";
            yield return "Units";
            yield return "Resources";
            yield return "ContentResources";
            yield return "UpdateGroups";
            yield return "UpdateParts";
            yield return "UpdateCommands";
            yield return "UpdateFeedbacks";
            yield return "Documents";
            yield return "DocumentVersions";
            yield return "Packages";
            yield return "PackageVersions";
            yield return "UnitConfigurations";
            yield return "MediaConfigurations";
            yield return "UserDefinedProperties";
            yield return "SystemConfigs";
        }

        private async Task CreateAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration)
        {
            switch (configuration.NotificationManagerConfiguration.Path)
            {
                case "UserRoles":
                    this.Result.UserRoleChangeTrackingManager = await this.CreateUserRolesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Authorizations":
                    this.Result.AuthorizationChangeTrackingManager = await this.CreateAuthorizationsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Tenants":
                    this.Result.TenantChangeTrackingManager = await this.CreateTenantsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Users":
                    this.Result.UserChangeTrackingManager = await this.CreateUsersAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "AssociationTenantUserUserRoles":
                    this.Result.AssociationTenantUserUserRoleChangeTrackingManager = await this.CreateAssociationTenantUserUserRolesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "ProductTypes":
                    this.Result.ProductTypeChangeTrackingManager = await this.CreateProductTypesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Units":
                    this.Result.UnitChangeTrackingManager = await this.CreateUnitsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Resources":
                    this.Result.ResourceChangeTrackingManager = await this.CreateResourcesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "ContentResources":
                    this.Result.ContentResourceChangeTrackingManager = await this.CreateContentResourcesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UpdateGroups":
                    this.Result.UpdateGroupChangeTrackingManager = await this.CreateUpdateGroupsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UpdateParts":
                    this.Result.UpdatePartChangeTrackingManager = await this.CreateUpdatePartsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UpdateCommands":
                    this.Result.UpdateCommandChangeTrackingManager = await this.CreateUpdateCommandsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UpdateFeedbacks":
                    this.Result.UpdateFeedbackChangeTrackingManager = await this.CreateUpdateFeedbacksAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Documents":
                    this.Result.DocumentChangeTrackingManager = await this.CreateDocumentsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "DocumentVersions":
                    this.Result.DocumentVersionChangeTrackingManager = await this.CreateDocumentVersionsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "Packages":
                    this.Result.PackageChangeTrackingManager = await this.CreatePackagesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "PackageVersions":
                    this.Result.PackageVersionChangeTrackingManager = await this.CreatePackageVersionsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UnitConfigurations":
                    this.Result.UnitConfigurationChangeTrackingManager = await this.CreateUnitConfigurationsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "MediaConfigurations":
                    this.Result.MediaConfigurationChangeTrackingManager = await this.CreateMediaConfigurationsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "UserDefinedProperties":
                    this.Result.UserDefinedPropertyChangeTrackingManager = await this.CreateUserDefinedPropertiesAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                case "SystemConfigs":
                    this.Result.SystemConfigChangeTrackingManager = await this.CreateSystemConfigsAsync(
                        configuration, userCredentials, backgroundSystemConfiguration).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        public void CreateRemoteDataService(RemoteServicesConfiguration dataServices, string path)
        {
            switch (path)
            {
                case "UserRoles":
                            ChannelScopeFactoryUtility<IUserRoleDataService>.ConfigureAsDataService(
                                dataServices,
                                "UserRole");
                            break;
                case "Authorizations":
                            ChannelScopeFactoryUtility<IAuthorizationDataService>.ConfigureAsDataService(
                                dataServices,
                                "Authorization");
                            break;
                case "Tenants":
                            ChannelScopeFactoryUtility<ITenantDataService>.ConfigureAsDataService(
                                dataServices,
                                "Tenant");
                            break;
                case "Users":
                            ChannelScopeFactoryUtility<IUserDataService>.ConfigureAsDataService(
                                dataServices,
                                "User");
                            break;
                case "AssociationTenantUserUserRoles":
                            ChannelScopeFactoryUtility<IAssociationTenantUserUserRoleDataService>.ConfigureAsDataService(
                                dataServices,
                                "AssociationTenantUserUserRole");
                            break;
                case "ProductTypes":
                            ChannelScopeFactoryUtility<IProductTypeDataService>.ConfigureAsDataService(
                                dataServices,
                                "ProductType");
                            break;
                case "Units":
                            ChannelScopeFactoryUtility<IUnitDataService>.ConfigureAsDataService(
                                dataServices,
                                "Unit");
                            break;
                case "Resources":
                            ChannelScopeFactoryUtility<IResourceDataService>.ConfigureAsDataService(
                                dataServices,
                                "Resource");
                            break;
                case "ContentResources":
                            ChannelScopeFactoryUtility<IContentResourceDataService>.ConfigureAsDataService(
                                dataServices,
                                "ContentResource");
                            break;
                case "UpdateGroups":
                            ChannelScopeFactoryUtility<IUpdateGroupDataService>.ConfigureAsDataService(
                                dataServices,
                                "UpdateGroup");
                            break;
                case "UpdateParts":
                            ChannelScopeFactoryUtility<IUpdatePartDataService>.ConfigureAsDataService(
                                dataServices,
                                "UpdatePart");
                            break;
                case "UpdateCommands":
                            ChannelScopeFactoryUtility<IUpdateCommandDataService>.ConfigureAsDataService(
                                dataServices,
                                "UpdateCommand");
                            break;
                case "UpdateFeedbacks":
                            ChannelScopeFactoryUtility<IUpdateFeedbackDataService>.ConfigureAsDataService(
                                dataServices,
                                "UpdateFeedback");
                            break;
                case "Documents":
                            ChannelScopeFactoryUtility<IDocumentDataService>.ConfigureAsDataService(
                                dataServices,
                                "Document");
                            break;
                case "DocumentVersions":
                            ChannelScopeFactoryUtility<IDocumentVersionDataService>.ConfigureAsDataService(
                                dataServices,
                                "DocumentVersion");
                            break;
                case "Packages":
                            ChannelScopeFactoryUtility<IPackageDataService>.ConfigureAsDataService(
                                dataServices,
                                "Package");
                            break;
                case "PackageVersions":
                            ChannelScopeFactoryUtility<IPackageVersionDataService>.ConfigureAsDataService(
                                dataServices,
                                "PackageVersion");
                            break;
                case "UnitConfigurations":
                            ChannelScopeFactoryUtility<IUnitConfigurationDataService>.ConfigureAsDataService(
                                dataServices,
                                "UnitConfiguration");
                            break;
                case "MediaConfigurations":
                            ChannelScopeFactoryUtility<IMediaConfigurationDataService>.ConfigureAsDataService(
                                dataServices,
                                "MediaConfiguration");
                            break;
                case "UserDefinedProperties":
                            ChannelScopeFactoryUtility<IUserDefinedPropertyDataService>.ConfigureAsDataService(
                                dataServices,
                                "UserDefinedProperty");
                            break;
                case "SystemConfigs":
                            ChannelScopeFactoryUtility<ISystemConfigDataService>.ConfigureAsDataService(
                                dataServices,
                                "SystemConfig");
                            break;
                default:
                    throw new NotSupportedException();
            }
        }


        private async Task<IUserRoleChangeTrackingManager> CreateUserRolesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UserRoleChangeTrackingManager(configuration, userCredentials);
            this.UserRoleChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UserRoleChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUserRoleChangeTrackingManager, IUserRoleDataService>(
                manager,
                backgroundSystemConfiguration,
                "UserRole");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUserRoleChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IAuthorizationChangeTrackingManager> CreateAuthorizationsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new AuthorizationChangeTrackingManager(configuration, userCredentials);
            this.AuthorizationChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.AuthorizationChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IAuthorizationChangeTrackingManager, IAuthorizationDataService>(
                manager,
                backgroundSystemConfiguration,
                "Authorization");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IAuthorizationChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<ITenantChangeTrackingManager> CreateTenantsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new TenantChangeTrackingManager(configuration, userCredentials);
            this.TenantChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.TenantChangeTrackingManager);
            this.ConfigureChannelScopeFactory<ITenantChangeTrackingManager, ITenantDataService>(
                manager,
                backgroundSystemConfiguration,
                "Tenant");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<ITenantChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUserChangeTrackingManager> CreateUsersAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UserChangeTrackingManager(configuration, userCredentials);
            this.UserChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UserChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUserChangeTrackingManager, IUserDataService>(
                manager,
                backgroundSystemConfiguration,
                "User");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUserChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IAssociationTenantUserUserRoleChangeTrackingManager> CreateAssociationTenantUserUserRolesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new AssociationTenantUserUserRoleChangeTrackingManager(configuration, userCredentials);
            this.AssociationTenantUserUserRoleChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.AssociationTenantUserUserRoleChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IAssociationTenantUserUserRoleChangeTrackingManager, IAssociationTenantUserUserRoleDataService>(
                manager,
                backgroundSystemConfiguration,
                "AssociationTenantUserUserRole");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IAssociationTenantUserUserRoleChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IProductTypeChangeTrackingManager> CreateProductTypesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new ProductTypeChangeTrackingManager(configuration, userCredentials);
            this.ProductTypeChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.ProductTypeChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IProductTypeChangeTrackingManager, IProductTypeDataService>(
                manager,
                backgroundSystemConfiguration,
                "ProductType");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IProductTypeChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUnitChangeTrackingManager> CreateUnitsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UnitChangeTrackingManager(configuration, userCredentials);
            this.UnitChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UnitChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUnitChangeTrackingManager, IUnitDataService>(
                manager,
                backgroundSystemConfiguration,
                "Unit");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUnitChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IResourceChangeTrackingManager> CreateResourcesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new ResourceChangeTrackingManager(configuration, userCredentials);
            this.ResourceChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.ResourceChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IResourceChangeTrackingManager, IResourceDataService>(
                manager,
                backgroundSystemConfiguration,
                "Resource");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IResourceChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IContentResourceChangeTrackingManager> CreateContentResourcesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new ContentResourceChangeTrackingManager(configuration, userCredentials);
            this.ContentResourceChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.ContentResourceChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IContentResourceChangeTrackingManager, IContentResourceDataService>(
                manager,
                backgroundSystemConfiguration,
                "ContentResource");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IContentResourceChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUpdateGroupChangeTrackingManager> CreateUpdateGroupsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UpdateGroupChangeTrackingManager(configuration, userCredentials);
            this.UpdateGroupChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UpdateGroupChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUpdateGroupChangeTrackingManager, IUpdateGroupDataService>(
                manager,
                backgroundSystemConfiguration,
                "UpdateGroup");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUpdateGroupChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUpdatePartChangeTrackingManager> CreateUpdatePartsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UpdatePartChangeTrackingManager(configuration, userCredentials);
            this.UpdatePartChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UpdatePartChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUpdatePartChangeTrackingManager, IUpdatePartDataService>(
                manager,
                backgroundSystemConfiguration,
                "UpdatePart");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUpdatePartChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUpdateCommandChangeTrackingManager> CreateUpdateCommandsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UpdateCommandChangeTrackingManager(configuration, userCredentials);
            this.UpdateCommandChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UpdateCommandChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUpdateCommandChangeTrackingManager, IUpdateCommandDataService>(
                manager,
                backgroundSystemConfiguration,
                "UpdateCommand");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUpdateCommandChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUpdateFeedbackChangeTrackingManager> CreateUpdateFeedbacksAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UpdateFeedbackChangeTrackingManager(configuration, userCredentials);
            this.UpdateFeedbackChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UpdateFeedbackChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUpdateFeedbackChangeTrackingManager, IUpdateFeedbackDataService>(
                manager,
                backgroundSystemConfiguration,
                "UpdateFeedback");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUpdateFeedbackChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IDocumentChangeTrackingManager> CreateDocumentsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new DocumentChangeTrackingManager(configuration, userCredentials);
            this.DocumentChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.DocumentChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IDocumentChangeTrackingManager, IDocumentDataService>(
                manager,
                backgroundSystemConfiguration,
                "Document");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IDocumentChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IDocumentVersionChangeTrackingManager> CreateDocumentVersionsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new DocumentVersionChangeTrackingManager(configuration, userCredentials);
            this.DocumentVersionChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.DocumentVersionChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IDocumentVersionChangeTrackingManager, IDocumentVersionDataService>(
                manager,
                backgroundSystemConfiguration,
                "DocumentVersion");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IDocumentVersionChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IPackageChangeTrackingManager> CreatePackagesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new PackageChangeTrackingManager(configuration, userCredentials);
            this.PackageChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.PackageChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IPackageChangeTrackingManager, IPackageDataService>(
                manager,
                backgroundSystemConfiguration,
                "Package");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IPackageChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IPackageVersionChangeTrackingManager> CreatePackageVersionsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new PackageVersionChangeTrackingManager(configuration, userCredentials);
            this.PackageVersionChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.PackageVersionChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IPackageVersionChangeTrackingManager, IPackageVersionDataService>(
                manager,
                backgroundSystemConfiguration,
                "PackageVersion");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IPackageVersionChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUnitConfigurationChangeTrackingManager> CreateUnitConfigurationsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UnitConfigurationChangeTrackingManager(configuration, userCredentials);
            this.UnitConfigurationChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UnitConfigurationChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUnitConfigurationChangeTrackingManager, IUnitConfigurationDataService>(
                manager,
                backgroundSystemConfiguration,
                "UnitConfiguration");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUnitConfigurationChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IMediaConfigurationChangeTrackingManager> CreateMediaConfigurationsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new MediaConfigurationChangeTrackingManager(configuration, userCredentials);
            this.MediaConfigurationChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.MediaConfigurationChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IMediaConfigurationChangeTrackingManager, IMediaConfigurationDataService>(
                manager,
                backgroundSystemConfiguration,
                "MediaConfiguration");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IMediaConfigurationChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<IUserDefinedPropertyChangeTrackingManager> CreateUserDefinedPropertiesAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new UserDefinedPropertyChangeTrackingManager(configuration, userCredentials);
            this.UserDefinedPropertyChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.UserDefinedPropertyChangeTrackingManager);
            this.ConfigureChannelScopeFactory<IUserDefinedPropertyChangeTrackingManager, IUserDefinedPropertyDataService>(
                manager,
                backgroundSystemConfiguration,
                "UserDefinedProperty");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<IUserDefinedPropertyChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private async Task<ISystemConfigChangeTrackingManager> CreateSystemConfigsAsync(
            NotificationSubscriptionConfiguration configuration,
            UserCredentials userCredentials,
            BackgroundSystemConfiguration backgroundSystemConfiguration
            )
        {
            var manager = new SystemConfigChangeTrackingManager(configuration, userCredentials);
            this.SystemConfigChangeTrackingManager = manager;
            DependencyResolver.Current.Register(this.SystemConfigChangeTrackingManager);
            this.ConfigureChannelScopeFactory<ISystemConfigChangeTrackingManager, ISystemConfigDataService>(
                manager,
                backgroundSystemConfiguration,
                "SystemConfig");

            var waitStarted = new ManualResetEventSlim();
            EventHandler onRunning = (sender, args) => waitStarted.Set();
            manager.Running += onRunning;
            Task.Run(async () => await manager.RunAsync());
            waitStarted.Wait();
            manager.Running -= onRunning;

            // TODO is this still required
            //DependencyResolver.Current.Register<ISystemConfigChangeTrackingManager>(manager);
            await manager.WaitReadyAsync();
            this.UpdateProgress();
            return manager;
        }

        private void HandleRunManagerException(Task task)
        {
            if (task.Exception != null)
            {
                Logger.Error(task.Exception.Flatten(), "Error while running change tracking manager" );
                this.Result.Exceptions.Add(task.Exception);
            }
        }
    }
}