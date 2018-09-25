namespace Gorba.Center.BackgroundSystem.Core
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

    using Gorba.Center.BackgroundSystem.Core.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    using Version = Gorba.Center.Common.ServiceModel.ChangeTracking.Version;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class AuthorizationChangeTrackingDataService : ChangeTrackingServiceBase, IAuthorizationDataService
        {
            private readonly IAuthorizationDataService dataService;

            public AuthorizationChangeTrackingDataService(
                IAuthorizationDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Authorization> AddAsync(Authorization entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Authorization entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Authorization> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Authorization>> QueryAsync(AuthorizationQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(AuthorizationQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Authorization> UpdateAsync(Authorization entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UserRoleChangeTrackingDataService : ChangeTrackingServiceBase, IUserRoleDataService
        {
            private readonly IUserRoleDataService dataService;

            public UserRoleChangeTrackingDataService(
                IUserRoleDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UserRole> AddAsync(UserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UserRole> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UserRole>> QueryAsync(UserRoleQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UserRoleQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UserRole> UpdateAsync(UserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class MediaConfigurationChangeTrackingDataService : ChangeTrackingServiceBase, IMediaConfigurationDataService
        {
            private readonly IMediaConfigurationDataService dataService;

            public MediaConfigurationChangeTrackingDataService(
                IMediaConfigurationDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<MediaConfiguration> AddAsync(MediaConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(MediaConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<MediaConfiguration> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<MediaConfiguration>> QueryAsync(MediaConfigurationQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(MediaConfigurationQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<MediaConfiguration> UpdateAsync(MediaConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UnitConfigurationChangeTrackingDataService : ChangeTrackingServiceBase, IUnitConfigurationDataService
        {
            private readonly IUnitConfigurationDataService dataService;

            public UnitConfigurationChangeTrackingDataService(
                IUnitConfigurationDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UnitConfiguration> AddAsync(UnitConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UnitConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UnitConfiguration> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UnitConfiguration>> QueryAsync(UnitConfigurationQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UnitConfigurationQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UnitConfiguration> UpdateAsync(UnitConfiguration entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class DocumentChangeTrackingDataService : ChangeTrackingServiceBase, IDocumentDataService
        {
            private readonly IDocumentDataService dataService;

            public DocumentChangeTrackingDataService(
                IDocumentDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Document> AddAsync(Document entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Document entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Document> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Document>> QueryAsync(DocumentQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(DocumentQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Document> UpdateAsync(Document entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class DocumentVersionChangeTrackingDataService : ChangeTrackingServiceBase, IDocumentVersionDataService
        {
            private readonly IDocumentVersionDataService dataService;

            public DocumentVersionChangeTrackingDataService(
                IDocumentVersionDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<DocumentVersion> AddAsync(DocumentVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(DocumentVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<DocumentVersion> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<DocumentVersion>> QueryAsync(DocumentVersionQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(DocumentVersionQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<DocumentVersion> UpdateAsync(DocumentVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class AssociationTenantUserUserRoleChangeTrackingDataService : ChangeTrackingServiceBase, IAssociationTenantUserUserRoleDataService
        {
            private readonly IAssociationTenantUserUserRoleDataService dataService;

            public AssociationTenantUserUserRoleChangeTrackingDataService(
                IAssociationTenantUserUserRoleDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<AssociationTenantUserUserRole> AddAsync(AssociationTenantUserUserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(AssociationTenantUserUserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<AssociationTenantUserUserRole> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<AssociationTenantUserUserRole>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(AssociationTenantUserUserRoleQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<AssociationTenantUserUserRole> UpdateAsync(AssociationTenantUserUserRole entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class TenantChangeTrackingDataService : ChangeTrackingServiceBase, ITenantDataService
        {
            private readonly ITenantDataService dataService;

            public TenantChangeTrackingDataService(
                ITenantDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Tenant> AddAsync(Tenant entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Tenant entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Tenant> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Tenant>> QueryAsync(TenantQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(TenantQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Tenant> UpdateAsync(Tenant entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UserChangeTrackingDataService : ChangeTrackingServiceBase, IUserDataService
        {
            private readonly IUserDataService dataService;

            public UserChangeTrackingDataService(
                IUserDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<User> AddAsync(User entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(User entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<User> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<User>> QueryAsync(UserQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UserQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<User> UpdateAsync(User entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class SystemConfigChangeTrackingDataService : ChangeTrackingServiceBase, ISystemConfigDataService
        {
            private readonly ISystemConfigDataService dataService;

            public SystemConfigChangeTrackingDataService(
                ISystemConfigDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<SystemConfig> AddAsync(SystemConfig entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(SystemConfig entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<SystemConfig> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<SystemConfig>> QueryAsync(SystemConfigQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(SystemConfigQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<SystemConfig> UpdateAsync(SystemConfig entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UserDefinedPropertyChangeTrackingDataService : ChangeTrackingServiceBase, IUserDefinedPropertyDataService
        {
            private readonly IUserDefinedPropertyDataService dataService;

            public UserDefinedPropertyChangeTrackingDataService(
                IUserDefinedPropertyDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UserDefinedProperty> AddAsync(UserDefinedProperty entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UserDefinedProperty entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UserDefinedProperty> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UserDefinedProperty>> QueryAsync(UserDefinedPropertyQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UserDefinedPropertyQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UserDefinedProperty> UpdateAsync(UserDefinedProperty entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class ContentResourceChangeTrackingDataService : ChangeTrackingServiceBase, IContentResourceDataService
        {
            private readonly IContentResourceDataService dataService;

            public ContentResourceChangeTrackingDataService(
                IContentResourceDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<ContentResource> AddAsync(ContentResource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(ContentResource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<ContentResource> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<ContentResource>> QueryAsync(ContentResourceQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(ContentResourceQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<ContentResource> UpdateAsync(ContentResource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class ResourceChangeTrackingDataService : ChangeTrackingServiceBase, IResourceDataService
        {
            private readonly IResourceDataService dataService;

            public ResourceChangeTrackingDataService(
                IResourceDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Resource> AddAsync(Resource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Resource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Resource> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Resource>> QueryAsync(ResourceQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(ResourceQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Resource> UpdateAsync(Resource entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class PackageChangeTrackingDataService : ChangeTrackingServiceBase, IPackageDataService
        {
            private readonly IPackageDataService dataService;

            public PackageChangeTrackingDataService(
                IPackageDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Package> AddAsync(Package entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Package entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Package> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Package>> QueryAsync(PackageQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(PackageQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Package> UpdateAsync(Package entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class PackageVersionChangeTrackingDataService : ChangeTrackingServiceBase, IPackageVersionDataService
        {
            private readonly IPackageVersionDataService dataService;

            public PackageVersionChangeTrackingDataService(
                IPackageVersionDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<PackageVersion> AddAsync(PackageVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(PackageVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<PackageVersion> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<PackageVersion>> QueryAsync(PackageVersionQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(PackageVersionQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<PackageVersion> UpdateAsync(PackageVersion entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class ProductTypeChangeTrackingDataService : ChangeTrackingServiceBase, IProductTypeDataService
        {
            private readonly IProductTypeDataService dataService;

            public ProductTypeChangeTrackingDataService(
                IProductTypeDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<ProductType> AddAsync(ProductType entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(ProductType entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<ProductType> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<ProductType>> QueryAsync(ProductTypeQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(ProductTypeQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<ProductType> UpdateAsync(ProductType entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UnitChangeTrackingDataService : ChangeTrackingServiceBase, IUnitDataService
        {
            private readonly IUnitDataService dataService;

            public UnitChangeTrackingDataService(
                IUnitDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<Unit> AddAsync(Unit entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(Unit entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<Unit> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<Unit>> QueryAsync(UnitQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UnitQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<Unit> UpdateAsync(Unit entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UpdateCommandChangeTrackingDataService : ChangeTrackingServiceBase, IUpdateCommandDataService
        {
            private readonly IUpdateCommandDataService dataService;

            public UpdateCommandChangeTrackingDataService(
                IUpdateCommandDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UpdateCommand> AddAsync(UpdateCommand entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UpdateCommand entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UpdateCommand> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UpdateCommand>> QueryAsync(UpdateCommandQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UpdateCommandQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UpdateCommand> UpdateAsync(UpdateCommand entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UpdateFeedbackChangeTrackingDataService : ChangeTrackingServiceBase, IUpdateFeedbackDataService
        {
            private readonly IUpdateFeedbackDataService dataService;

            public UpdateFeedbackChangeTrackingDataService(
                IUpdateFeedbackDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UpdateFeedback> AddAsync(UpdateFeedback entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UpdateFeedback entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UpdateFeedback> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UpdateFeedback>> QueryAsync(UpdateFeedbackQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UpdateFeedbackQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UpdateFeedback> UpdateAsync(UpdateFeedback entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UpdateGroupChangeTrackingDataService : ChangeTrackingServiceBase, IUpdateGroupDataService
        {
            private readonly IUpdateGroupDataService dataService;

            public UpdateGroupChangeTrackingDataService(
                IUpdateGroupDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UpdateGroup> AddAsync(UpdateGroup entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UpdateGroup entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UpdateGroup> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UpdateGroup>> QueryAsync(UpdateGroupQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UpdateGroupQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UpdateGroup> UpdateAsync(UpdateGroup entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }

        [ErrorHandler]
        [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
        public partial class UpdatePartChangeTrackingDataService : ChangeTrackingServiceBase, IUpdatePartDataService
        {
            private readonly IUpdatePartDataService dataService;

            public UpdatePartChangeTrackingDataService(
                IUpdatePartDataService dataService,
                BackgroundSystemConfiguration backgroundSystemConfiguration,
                NotificationSubscriptionConfiguration configuration)
                : base(backgroundSystemConfiguration, configuration)
            {
                this.dataService = dataService;
            }

            public async Task<UpdatePart> AddAsync(UpdatePart entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var addedEntity = await this.dataService.AddAsync(entity);
                    await this.OnEntityAddedAsync(addedEntity);
                    return addedEntity;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityAdded);
                exceptionDispatchInfo.Throw();
                return null;
            }

            public async Task DeleteAsync(UpdatePart entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await this.dataService.DeleteAsync(entity);
                    await this.OnEntityDeletedAsync(entity);
                    return;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }

                await this.OnErrorAsync(entity.Id, DeltaNotificationType.EntityRemoved);
                exceptionDispatchInfo.Throw();
            }

            public async Task<UpdatePart> GetAsync(int id)
            {
                return await this.dataService.GetAsync(id);
            }

            public async Task<IEnumerable<UpdatePart>> QueryAsync(UpdatePartQuery query = null)
            {
                return await this.dataService.QueryAsync(query);
            }

            public async Task<int> CountAsync(UpdatePartQuery query = null)
            {
                return await this.dataService.CountAsync(query);
            }

            public async Task<UpdatePart> UpdateAsync(UpdatePart entity)
            {
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    var original = await this.GetAsync(entity.Id);
                    var updated = await this.dataService.UpdateAsync(entity);
                    await this.OnEntityUpdatedAsync(original, updated);
                    return updated;
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                }
                
                await this.OnErrorAsync(entity.Id, DeltaNotificationType.PropertiesChanged);
                exceptionDispatchInfo.Throw();
                return null;
            }
        }
    }
}
