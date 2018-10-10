namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using System.Xml;
    
    using Gorba.Center.BackgroundSystem.Core.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.BackgroundSystem.Data.Model.Configurations;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.BackgroundSystem.Data.Model.Log;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.BackgroundSystem.Data.Model.Software;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    using NLog;

    public static class RemoteDataServicesUtility
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<ServiceHost> SetupRemoteDataServices()
        {
            yield return CreateServiceHost<IAuthorizationDataService>(new AuthorizationRemoteDataService(new AuthorizationConcurrentDataService(new AuthorizationDataService())), "Authorization");
            yield return CreateServiceHost<IUserRoleDataService>(new UserRoleRemoteDataService(new UserRoleConcurrentDataService(new UserRoleDataService())), "UserRole");
            yield return CreateServiceHost<IUnitConfigurationDataService>(new UnitConfigurationRemoteDataService(new UnitConfigurationConcurrentDataService(new UnitConfigurationDataService())), "UnitConfiguration");
            yield return CreateServiceHost<IMediaConfigurationDataService>(new MediaConfigurationRemoteDataService(new MediaConfigurationConcurrentDataService(new MediaConfigurationDataService())), "MediaConfiguration");
            yield return CreateServiceHost<IDocumentDataService>(new DocumentRemoteDataService(new DocumentConcurrentDataService(new DocumentDataService())), "Document");
            yield return CreateServiceHost<IDocumentVersionDataService>(new DocumentVersionRemoteDataService(new DocumentVersionConcurrentDataService(new DocumentVersionDataService())), "DocumentVersion");
            yield return CreateServiceHost<ILogEntryDataService>(new LogEntryRemoteDataService(new LogEntryConcurrentDataService(new LogEntryDataService())), "LogEntry");
            yield return CreateServiceHost<ITenantDataService>(new TenantRemoteDataService(new TenantConcurrentDataService(new TenantDataService())), "Tenant");
            yield return CreateServiceHost<IUserDataService>(new UserRemoteDataService(new UserConcurrentDataService(new UserDataService())), "User");
            yield return CreateServiceHost<IAssociationTenantUserUserRoleDataService>(new AssociationTenantUserUserRoleRemoteDataService(new AssociationTenantUserUserRoleConcurrentDataService(new AssociationTenantUserUserRoleDataService())), "AssociationTenantUserUserRole");
            yield return CreateServiceHost<IUserDefinedPropertyDataService>(new UserDefinedPropertyRemoteDataService(new UserDefinedPropertyConcurrentDataService(new UserDefinedPropertyDataService())), "UserDefinedProperty");
            yield return CreateServiceHost<ISystemConfigDataService>(new SystemConfigRemoteDataService(new SystemConfigConcurrentDataService(new SystemConfigDataService())), "SystemConfig");
            yield return CreateServiceHost<IResourceDataService>(new ResourceRemoteDataService(new ResourceConcurrentDataService(new ResourceDataService())), "Resource");
            yield return CreateServiceHost<IContentResourceDataService>(new ContentResourceRemoteDataService(new ContentResourceConcurrentDataService(new ContentResourceDataService())), "ContentResource");
            yield return CreateServiceHost<IPackageDataService>(new PackageRemoteDataService(new PackageConcurrentDataService(new PackageDataService())), "Package");
            yield return CreateServiceHost<IPackageVersionDataService>(new PackageVersionRemoteDataService(new PackageVersionConcurrentDataService(new PackageVersionDataService())), "PackageVersion");
            yield return CreateServiceHost<IProductTypeDataService>(new ProductTypeRemoteDataService(new ProductTypeConcurrentDataService(new ProductTypeDataService())), "ProductType");
            yield return CreateServiceHost<IUnitDataService>(new UnitRemoteDataService(new UnitConcurrentDataService(new UnitDataService())), "Unit");
            yield return CreateServiceHost<IUpdateGroupDataService>(new UpdateGroupRemoteDataService(new UpdateGroupConcurrentDataService(new UpdateGroupDataService())), "UpdateGroup");
            yield return CreateServiceHost<IUpdatePartDataService>(new UpdatePartRemoteDataService(new UpdatePartConcurrentDataService(new UpdatePartDataService())), "UpdatePart");
            yield return CreateServiceHost<IUpdateCommandDataService>(new UpdateCommandRemoteDataService(new UpdateCommandConcurrentDataService(new UpdateCommandDataService())), "UpdateCommand");
            yield return CreateServiceHost<IUpdateFeedbackDataService>(new UpdateFeedbackRemoteDataService(new UpdateFeedbackConcurrentDataService(new UpdateFeedbackDataService())), "UpdateFeedback");
	
        }

        private static ServiceHost CreateServiceHost<T>(object instance, string name)
        {
            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            var endpoint = configuration.DataServices.CreateDataServicesEndpoint<T>(name);
            Logger.Debug("Creating endpoint for type <{0}> named '{1}'", typeof(T).Name, name);
            var serviceHost = new ServiceHost(instance);
            serviceHost.AddServiceEndpoint(endpoint);
            return serviceHost;
        }
    }


    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class AuthorizationRemoteDataService : RemoteDataServiceBase, IAuthorizationDataService
    {
        private IAuthorizationDataService dataService;

        public AuthorizationRemoteDataService(IAuthorizationDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.Authorization>> QueryAsync(AuthorizationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(AuthorizationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.Authorization> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.Authorization entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UserRoleRemoteDataService : RemoteDataServiceBase, IUserRoleDataService
    {
        private IUserRoleDataService dataService;

        public UserRoleRemoteDataService(IUserRoleDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> AddAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.AccessControl.UserRole>> QueryAsync(UserRoleQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UserRoleQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.AccessControl.UserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.AccessControl.UserRole entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UnitConfigurationRemoteDataService : RemoteDataServiceBase, IUnitConfigurationDataService
    {
        private IUnitConfigurationDataService dataService;

        public UnitConfigurationRemoteDataService(IUnitConfigurationDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration>> QueryAsync(UnitConfigurationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UnitConfigurationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class MediaConfigurationRemoteDataService : RemoteDataServiceBase, IMediaConfigurationDataService
    {
        private IMediaConfigurationDataService dataService;

        public MediaConfigurationRemoteDataService(IMediaConfigurationDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> AddAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration>> QueryAsync(MediaConfigurationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(MediaConfigurationQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration> UpdateAsync(Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class DocumentRemoteDataService : RemoteDataServiceBase, IDocumentDataService
    {
        private IDocumentDataService dataService;

        public DocumentRemoteDataService(IDocumentDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> AddAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.Document>> QueryAsync(DocumentQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(DocumentQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.Document> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.Document entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class DocumentVersionRemoteDataService : RemoteDataServiceBase, IDocumentVersionDataService
    {
        private IDocumentVersionDataService dataService;

        public DocumentVersionRemoteDataService(IDocumentVersionDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> AddAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion>> QueryAsync(DocumentVersionQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(DocumentVersionQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Documents.DocumentVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Documents.DocumentVersion entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class LogEntryRemoteDataService : RemoteDataServiceBase, ILogEntryDataService
    {
        private ILogEntryDataService dataService;

        public LogEntryRemoteDataService(ILogEntryDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> AddAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task AddRangeAsync(IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry> entities)
        {
            try
            {
                var list = entities.ToList();
                this.Logger.Trace("Adding {0} item(s)", list.Count);
                await this.dataService.AddRangeAsync(list);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> DeleteAsync(LogEntryFilter filter)
        {
            try
            {
                this.Logger.Trace("Deleting items");
                var deletedItems = await this.dataService.DeleteAsync(filter);
                this.Logger.Trace("Deleted {0} item(s)", deletedItems);
                return deletedItems;
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Log.LogEntry>> QueryAsync(LogEntryQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(LogEntryQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Log.LogEntry> UpdateAsync(Gorba.Center.Common.ServiceModel.Log.LogEntry entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class TenantRemoteDataService : RemoteDataServiceBase, ITenantDataService
    {
        private ITenantDataService dataService;

        public TenantRemoteDataService(ITenantDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> AddAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.Tenant>> QueryAsync(TenantQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(TenantQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.Tenant> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.Tenant entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UserRemoteDataService : RemoteDataServiceBase, IUserDataService
    {
        private IUserDataService dataService;

        public UserRemoteDataService(IUserDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> AddAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.User>> QueryAsync(UserQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UserQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.User> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.User entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class AssociationTenantUserUserRoleRemoteDataService : RemoteDataServiceBase, IAssociationTenantUserUserRoleDataService
    {
        private IAssociationTenantUserUserRoleDataService dataService;

        public AssociationTenantUserUserRoleRemoteDataService(IAssociationTenantUserUserRoleDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> AddAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole>> QueryAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(AssociationTenantUserUserRoleQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole> UpdateAsync(Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UserDefinedPropertyRemoteDataService : RemoteDataServiceBase, IUserDefinedPropertyDataService
    {
        private IUserDefinedPropertyDataService dataService;

        public UserDefinedPropertyRemoteDataService(IUserDefinedPropertyDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> AddAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty>> QueryAsync(UserDefinedPropertyQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UserDefinedPropertyQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class SystemConfigRemoteDataService : RemoteDataServiceBase, ISystemConfigDataService
    {
        private ISystemConfigDataService dataService;

        public SystemConfigRemoteDataService(ISystemConfigDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> AddAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Meta.SystemConfig>> QueryAsync(SystemConfigQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(SystemConfigQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Meta.SystemConfig> UpdateAsync(Gorba.Center.Common.ServiceModel.Meta.SystemConfig entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class ResourceRemoteDataService : RemoteDataServiceBase, IResourceDataService
    {
        private IResourceDataService dataService;

        public ResourceRemoteDataService(IResourceDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.Resource>> QueryAsync(ResourceQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(ResourceQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.Resource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.Resource entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class ContentResourceRemoteDataService : RemoteDataServiceBase, IContentResourceDataService
    {
        private IContentResourceDataService dataService;

        public ContentResourceRemoteDataService(IContentResourceDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> AddAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Resources.ContentResource>> QueryAsync(ContentResourceQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(ContentResourceQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Resources.ContentResource> UpdateAsync(Gorba.Center.Common.ServiceModel.Resources.ContentResource entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class PackageRemoteDataService : RemoteDataServiceBase, IPackageDataService
    {
        private IPackageDataService dataService;

        public PackageRemoteDataService(IPackageDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> AddAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.Package>> QueryAsync(PackageQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(PackageQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.Package> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.Package entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class PackageVersionRemoteDataService : RemoteDataServiceBase, IPackageVersionDataService
    {
        private IPackageVersionDataService dataService;

        public PackageVersionRemoteDataService(IPackageVersionDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> AddAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Software.PackageVersion>> QueryAsync(PackageVersionQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(PackageVersionQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Software.PackageVersion> UpdateAsync(Gorba.Center.Common.ServiceModel.Software.PackageVersion entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class ProductTypeRemoteDataService : RemoteDataServiceBase, IProductTypeDataService
    {
        private IProductTypeDataService dataService;

        public ProductTypeRemoteDataService(IProductTypeDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> AddAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.ProductType>> QueryAsync(ProductTypeQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(ProductTypeQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.ProductType> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.ProductType entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UnitRemoteDataService : RemoteDataServiceBase, IUnitDataService
    {
        private IUnitDataService dataService;

        public UnitRemoteDataService(IUnitDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> AddAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Units.Unit>> QueryAsync(UnitQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UnitQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Units.Unit> UpdateAsync(Gorba.Center.Common.ServiceModel.Units.Unit entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UpdateGroupRemoteDataService : RemoteDataServiceBase, IUpdateGroupDataService
    {
        private IUpdateGroupDataService dataService;

        public UpdateGroupRemoteDataService(IUpdateGroupDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateGroup>> QueryAsync(UpdateGroupQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UpdateGroupQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateGroup> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateGroup entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UpdatePartRemoteDataService : RemoteDataServiceBase, IUpdatePartDataService
    {
        private IUpdatePartDataService dataService;

        public UpdatePartRemoteDataService(IUpdatePartDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdatePart>> QueryAsync(UpdatePartQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UpdatePartQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdatePart> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdatePart entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UpdateCommandRemoteDataService : RemoteDataServiceBase, IUpdateCommandDataService
    {
        private IUpdateCommandDataService dataService;

        public UpdateCommandRemoteDataService(IUpdateCommandDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateCommand>> QueryAsync(UpdateCommandQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UpdateCommandQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateCommand> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateCommand entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }

    [ErrorHandler]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class UpdateFeedbackRemoteDataService : RemoteDataServiceBase, IUpdateFeedbackDataService
    {
        private IUpdateFeedbackDataService dataService;

        public UpdateFeedbackRemoteDataService(IUpdateFeedbackDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> AddAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            try
            {
                this.Logger.Trace("Adding entity {0}", entity);
                return await this.dataService.AddAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task DeleteAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            try
            {
                this.Logger.Trace("Deleting entity {0}");
                await this.dataService.DeleteAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<IEnumerable<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback>> QueryAsync(UpdateFeedbackQuery query = null)
        {
            try
            {
                this.Logger.Trace("Querying entities");
                return await this.dataService.QueryAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<int> CountAsync(UpdateFeedbackQuery query = null)
        {
            try
            {
                this.Logger.Trace("Counting entities");
                return await this.dataService.CountAsync(query);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> GetAsync(int id)
        {
            try
            {
                this.Logger.Trace("Getting entity '{0}'", id);
                return await this.dataService.GetAsync(id);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }

        public async Task<Gorba.Center.Common.ServiceModel.Update.UpdateFeedback> UpdateAsync(Gorba.Center.Common.ServiceModel.Update.UpdateFeedback entity)
        {
            try
            {
                this.Logger.Trace("Updating entity {0}", entity);
                return await this.dataService.UpdateAsync(entity);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
                throw new FaultException(exception.Message);
            }
        }
    }
}
