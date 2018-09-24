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

    using Gorba.Center.BackgroundSystem.Core.ChangeTracking;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
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

    public static partial class DataServicesUtility
    {
        public static IEnumerable<IServiceHost> SetupNonChangeTrackingDataServices(
            BackgroundSystemConfiguration configuration)
        {
            ServiceHost host;

            var serviceLogEntry = CreateLogEntryDataService();
            DependencyResolver.Current.Register(serviceLogEntry);
            host = DataServicesUtility.CreateServiceHost<ILogEntryDataService>(
                serviceLogEntry, "LogEntry");
            yield return new ServiceHostWrapper("LogEntry", host);
	
        }

        internal static IAssociationTenantUserUserRoleDataService CreateAssociationTenantUserUserRoleDataService()
        {
            var service = new AssociationTenantUserUserRoleDataService();
            var concurrentService = new AssociationTenantUserUserRoleConcurrentDataService(service);
            ChannelScopeFactory<IAssociationTenantUserUserRoleDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IAssociationTenantUserUserRoleDataService>(concurrentService));
            return new AssociationTenantUserUserRoleRemoteDataService(concurrentService);
        }

        internal static IAuthorizationDataService CreateAuthorizationDataService()
        {
            var service = new AuthorizationDataService();
            var concurrentService = new AuthorizationConcurrentDataService(service);
            ChannelScopeFactory<IAuthorizationDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IAuthorizationDataService>(concurrentService));
            return new AuthorizationRemoteDataService(concurrentService);
        }

        internal static IContentResourceDataService CreateContentResourceDataService()
        {
            var service = new ContentResourceDataService();
            var concurrentService = new ContentResourceConcurrentDataService(service);
            ChannelScopeFactory<IContentResourceDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IContentResourceDataService>(concurrentService));
            return new ContentResourceRemoteDataService(concurrentService);
        }

        internal static IDocumentDataService CreateDocumentDataService()
        {
            var service = new DocumentDataService();
            var concurrentService = new DocumentConcurrentDataService(service);
            ChannelScopeFactory<IDocumentDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IDocumentDataService>(concurrentService));
            return new DocumentRemoteDataService(concurrentService);
        }

        internal static IDocumentVersionDataService CreateDocumentVersionDataService()
        {
            var service = new DocumentVersionDataService();
            var concurrentService = new DocumentVersionConcurrentDataService(service);
            ChannelScopeFactory<IDocumentVersionDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IDocumentVersionDataService>(concurrentService));
            return new DocumentVersionRemoteDataService(concurrentService);
        }

        internal static ILogEntryDataService CreateLogEntryDataService()
        {
            var service = new LogEntryDataService();
            var concurrentService = new LogEntryConcurrentDataService(service);
            ChannelScopeFactory<ILogEntryDataService>.SetCurrent(
                new InstanceChannelScopeFactory<ILogEntryDataService>(concurrentService));
            return new LogEntryRemoteDataService(concurrentService);
        }

        internal static IMediaConfigurationDataService CreateMediaConfigurationDataService()
        {
            var service = new MediaConfigurationDataService();
            var concurrentService = new MediaConfigurationConcurrentDataService(service);
            ChannelScopeFactory<IMediaConfigurationDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IMediaConfigurationDataService>(concurrentService));
            return new MediaConfigurationRemoteDataService(concurrentService);
        }

        internal static IPackageDataService CreatePackageDataService()
        {
            var service = new PackageDataService();
            var concurrentService = new PackageConcurrentDataService(service);
            ChannelScopeFactory<IPackageDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IPackageDataService>(concurrentService));
            return new PackageRemoteDataService(concurrentService);
        }

        internal static IPackageVersionDataService CreatePackageVersionDataService()
        {
            var service = new PackageVersionDataService();
            var concurrentService = new PackageVersionConcurrentDataService(service);
            ChannelScopeFactory<IPackageVersionDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IPackageVersionDataService>(concurrentService));
            return new PackageVersionRemoteDataService(concurrentService);
        }

        internal static IProductTypeDataService CreateProductTypeDataService()
        {
            var service = new ProductTypeDataService();
            var concurrentService = new ProductTypeConcurrentDataService(service);
            ChannelScopeFactory<IProductTypeDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IProductTypeDataService>(concurrentService));
            return new ProductTypeRemoteDataService(concurrentService);
        }

        internal static IResourceDataService CreateResourceDataService()
        {
            var service = new ResourceDataService();
            var concurrentService = new ResourceConcurrentDataService(service);
            ChannelScopeFactory<IResourceDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IResourceDataService>(concurrentService));
            return new ResourceRemoteDataService(concurrentService);
        }

        internal static ISystemConfigDataService CreateSystemConfigDataService()
        {
            var service = new SystemConfigDataService();
            var concurrentService = new SystemConfigConcurrentDataService(service);
            ChannelScopeFactory<ISystemConfigDataService>.SetCurrent(
                new InstanceChannelScopeFactory<ISystemConfigDataService>(concurrentService));
            return new SystemConfigRemoteDataService(concurrentService);
        }

        internal static ITenantDataService CreateTenantDataService()
        {
            var service = new TenantDataService();
            var concurrentService = new TenantConcurrentDataService(service);
            ChannelScopeFactory<ITenantDataService>.SetCurrent(
                new InstanceChannelScopeFactory<ITenantDataService>(concurrentService));
            return new TenantRemoteDataService(concurrentService);
        }

        internal static IUnitDataService CreateUnitDataService()
        {
            var service = new UnitDataService();
            var concurrentService = new UnitConcurrentDataService(service);
            ChannelScopeFactory<IUnitDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUnitDataService>(concurrentService));
            return new UnitRemoteDataService(concurrentService);
        }

        internal static IUnitConfigurationDataService CreateUnitConfigurationDataService()
        {
            var service = new UnitConfigurationDataService();
            var concurrentService = new UnitConfigurationConcurrentDataService(service);
            ChannelScopeFactory<IUnitConfigurationDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUnitConfigurationDataService>(concurrentService));
            return new UnitConfigurationRemoteDataService(concurrentService);
        }

        internal static IUpdateCommandDataService CreateUpdateCommandDataService()
        {
            var service = new UpdateCommandDataService();
            var concurrentService = new UpdateCommandConcurrentDataService(service);
            ChannelScopeFactory<IUpdateCommandDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUpdateCommandDataService>(concurrentService));
            return new UpdateCommandRemoteDataService(concurrentService);
        }

        internal static IUpdateFeedbackDataService CreateUpdateFeedbackDataService()
        {
            var service = new UpdateFeedbackDataService();
            var concurrentService = new UpdateFeedbackConcurrentDataService(service);
            ChannelScopeFactory<IUpdateFeedbackDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUpdateFeedbackDataService>(concurrentService));
            return new UpdateFeedbackRemoteDataService(concurrentService);
        }

        internal static IUpdateGroupDataService CreateUpdateGroupDataService()
        {
            var service = new UpdateGroupDataService();
            var concurrentService = new UpdateGroupConcurrentDataService(service);
            ChannelScopeFactory<IUpdateGroupDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUpdateGroupDataService>(concurrentService));
            return new UpdateGroupRemoteDataService(concurrentService);
        }

        internal static IUpdatePartDataService CreateUpdatePartDataService()
        {
            var service = new UpdatePartDataService();
            var concurrentService = new UpdatePartConcurrentDataService(service);
            ChannelScopeFactory<IUpdatePartDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUpdatePartDataService>(concurrentService));
            return new UpdatePartRemoteDataService(concurrentService);
        }

        internal static IUserDataService CreateUserDataService()
        {
            var service = new UserDataService();
            var concurrentService = new UserConcurrentDataService(service);
            ChannelScopeFactory<IUserDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUserDataService>(concurrentService));
            return new UserRemoteDataService(concurrentService);
        }

        internal static IUserDefinedPropertyDataService CreateUserDefinedPropertyDataService()
        {
            var service = new UserDefinedPropertyDataService();
            var concurrentService = new UserDefinedPropertyConcurrentDataService(service);
            ChannelScopeFactory<IUserDefinedPropertyDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUserDefinedPropertyDataService>(concurrentService));
            return new UserDefinedPropertyRemoteDataService(concurrentService);
        }

        internal static IUserRoleDataService CreateUserRoleDataService()
        {
            var service = new UserRoleDataService();
            var concurrentService = new UserRoleConcurrentDataService(service);
            ChannelScopeFactory<IUserRoleDataService>.SetCurrent(
                new InstanceChannelScopeFactory<IUserRoleDataService>(concurrentService));
            return new UserRoleRemoteDataService(concurrentService);
        }
    }
}
