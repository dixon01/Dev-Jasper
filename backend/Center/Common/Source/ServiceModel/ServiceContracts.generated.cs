namespace Gorba.Center.Common.ServiceModel
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Filters;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    [ServiceContract]
    public partial interface IUserRoleDataService
    {
        [OperationContract]
        Task<UserRole> AddAsync(UserRole entity);

        [OperationContract]
        Task DeleteAsync(UserRole entity);

        [OperationContract]
        Task<UserRole> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UserRole>> QueryAsync(UserRoleQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UserRoleQuery query = null);

        [OperationContract]
        Task<UserRole> UpdateAsync(UserRole entity);
    }

    [ServiceContract]
    public partial interface IAuthorizationDataService
    {
        [OperationContract]
        Task<Authorization> AddAsync(Authorization entity);

        [OperationContract]
        Task DeleteAsync(Authorization entity);

        [OperationContract]
        Task<Authorization> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Authorization>> QueryAsync(AuthorizationQuery query = null);

        [OperationContract]
        Task<int> CountAsync(AuthorizationQuery query = null);

        [OperationContract]
        Task<Authorization> UpdateAsync(Authorization entity);
    }

    [ServiceContract]
    public partial interface IUnitConfigurationDataService
    {
        [OperationContract]
        Task<UnitConfiguration> AddAsync(UnitConfiguration entity);

        [OperationContract]
        Task DeleteAsync(UnitConfiguration entity);

        [OperationContract]
        Task<UnitConfiguration> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UnitConfiguration>> QueryAsync(UnitConfigurationQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UnitConfigurationQuery query = null);

        [OperationContract]
        Task<UnitConfiguration> UpdateAsync(UnitConfiguration entity);
    }

    [ServiceContract]
    public partial interface IMediaConfigurationDataService
    {
        [OperationContract]
        Task<MediaConfiguration> AddAsync(MediaConfiguration entity);

        [OperationContract]
        Task DeleteAsync(MediaConfiguration entity);

        [OperationContract]
        Task<MediaConfiguration> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<MediaConfiguration>> QueryAsync(MediaConfigurationQuery query = null);

        [OperationContract]
        Task<int> CountAsync(MediaConfigurationQuery query = null);

        [OperationContract]
        Task<MediaConfiguration> UpdateAsync(MediaConfiguration entity);
    }

    [ServiceContract]
    public partial interface IDocumentDataService
    {
        [OperationContract]
        Task<Document> AddAsync(Document entity);

        [OperationContract]
        Task DeleteAsync(Document entity);

        [OperationContract]
        Task<Document> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Document>> QueryAsync(DocumentQuery query = null);

        [OperationContract]
        Task<int> CountAsync(DocumentQuery query = null);

        [OperationContract]
        Task<Document> UpdateAsync(Document entity);
    }

    [ServiceContract]
    public partial interface IDocumentVersionDataService
    {
        [OperationContract]
        Task<DocumentVersion> AddAsync(DocumentVersion entity);

        [OperationContract]
        Task DeleteAsync(DocumentVersion entity);

        [OperationContract]
        Task<DocumentVersion> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<DocumentVersion>> QueryAsync(DocumentVersionQuery query = null);

        [OperationContract]
        Task<int> CountAsync(DocumentVersionQuery query = null);

        [OperationContract]
        Task<DocumentVersion> UpdateAsync(DocumentVersion entity);
    }

    [ServiceContract]
    public partial interface ILogEntryDataService
    {
        [OperationContract]
        Task<LogEntry> AddAsync(LogEntry entity);
            
        [OperationContract]
        Task AddRangeAsync(IEnumerable<LogEntry> entities);
            
        [OperationContract(Name = "DeleteByFilter")]
        Task<int> DeleteAsync(LogEntryFilter filter);

        [OperationContract]
        Task DeleteAsync(LogEntry entity);

        [OperationContract]
        Task<LogEntry> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<LogEntry>> QueryAsync(LogEntryQuery query = null);

        [OperationContract]
        Task<int> CountAsync(LogEntryQuery query = null);

        [OperationContract]
        Task<LogEntry> UpdateAsync(LogEntry entity);
    }

    [ServiceContract]
    public partial interface ITenantDataService
    {
        [OperationContract]
        Task<Tenant> AddAsync(Tenant entity);

        [OperationContract]
        Task DeleteAsync(Tenant entity);

        [OperationContract]
        Task<Tenant> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Tenant>> QueryAsync(TenantQuery query = null);

        [OperationContract]
        Task<int> CountAsync(TenantQuery query = null);

        [OperationContract]
        Task<Tenant> UpdateAsync(Tenant entity);
    }

    [ServiceContract]
    public partial interface IUserDataService
    {
        [OperationContract]
        Task<User> AddAsync(User entity);

        [OperationContract]
        Task DeleteAsync(User entity);

        [OperationContract]
        Task<User> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<User>> QueryAsync(UserQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UserQuery query = null);

        [OperationContract]
        Task<User> UpdateAsync(User entity);
    }

    [ServiceContract]
    public partial interface IAssociationTenantUserUserRoleDataService
    {
        [OperationContract]
        Task<AssociationTenantUserUserRole> AddAsync(AssociationTenantUserUserRole entity);

        [OperationContract]
        Task DeleteAsync(AssociationTenantUserUserRole entity);

        [OperationContract]
        Task<AssociationTenantUserUserRole> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<AssociationTenantUserUserRole>> QueryAsync(AssociationTenantUserUserRoleQuery query = null);

        [OperationContract]
        Task<int> CountAsync(AssociationTenantUserUserRoleQuery query = null);

        [OperationContract]
        Task<AssociationTenantUserUserRole> UpdateAsync(AssociationTenantUserUserRole entity);
    }

    [ServiceContract]
    public partial interface IUserDefinedPropertyDataService
    {
        [OperationContract]
        Task<UserDefinedProperty> AddAsync(UserDefinedProperty entity);

        [OperationContract]
        Task DeleteAsync(UserDefinedProperty entity);

        [OperationContract]
        Task<UserDefinedProperty> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UserDefinedProperty>> QueryAsync(UserDefinedPropertyQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UserDefinedPropertyQuery query = null);

        [OperationContract]
        Task<UserDefinedProperty> UpdateAsync(UserDefinedProperty entity);
    }

    [ServiceContract]
    public partial interface ISystemConfigDataService
    {
        [OperationContract]
        Task<SystemConfig> AddAsync(SystemConfig entity);

        [OperationContract]
        Task DeleteAsync(SystemConfig entity);

        [OperationContract]
        Task<SystemConfig> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<SystemConfig>> QueryAsync(SystemConfigQuery query = null);

        [OperationContract]
        Task<int> CountAsync(SystemConfigQuery query = null);

        [OperationContract]
        Task<SystemConfig> UpdateAsync(SystemConfig entity);
    }

    [ServiceContract]
    public partial interface IResourceDataService
    {
        [OperationContract]
        Task<Resource> AddAsync(Resource entity);

        [OperationContract]
        Task DeleteAsync(Resource entity);

        [OperationContract]
        Task<Resource> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Resource>> QueryAsync(ResourceQuery query = null);

        [OperationContract]
        Task<int> CountAsync(ResourceQuery query = null);

        [OperationContract]
        Task<Resource> UpdateAsync(Resource entity);
    }

    [ServiceContract]
    public partial interface IContentResourceDataService
    {
        [OperationContract]
        Task<ContentResource> AddAsync(ContentResource entity);

        [OperationContract]
        Task DeleteAsync(ContentResource entity);

        [OperationContract]
        Task<ContentResource> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<ContentResource>> QueryAsync(ContentResourceQuery query = null);

        [OperationContract]
        Task<int> CountAsync(ContentResourceQuery query = null);

        [OperationContract]
        Task<ContentResource> UpdateAsync(ContentResource entity);
    }

    [ServiceContract]
    public partial interface IPackageDataService
    {
        [OperationContract]
        Task<Package> AddAsync(Package entity);

        [OperationContract]
        Task DeleteAsync(Package entity);

        [OperationContract]
        Task<Package> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Package>> QueryAsync(PackageQuery query = null);

        [OperationContract]
        Task<int> CountAsync(PackageQuery query = null);

        [OperationContract]
        Task<Package> UpdateAsync(Package entity);
    }

    [ServiceContract]
    public partial interface IPackageVersionDataService
    {
        [OperationContract]
        Task<PackageVersion> AddAsync(PackageVersion entity);

        [OperationContract]
        Task DeleteAsync(PackageVersion entity);

        [OperationContract]
        Task<PackageVersion> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<PackageVersion>> QueryAsync(PackageVersionQuery query = null);

        [OperationContract]
        Task<int> CountAsync(PackageVersionQuery query = null);

        [OperationContract]
        Task<PackageVersion> UpdateAsync(PackageVersion entity);
    }

    [ServiceContract]
    public partial interface IProductTypeDataService
    {
        [OperationContract]
        Task<ProductType> AddAsync(ProductType entity);

        [OperationContract]
        Task DeleteAsync(ProductType entity);

        [OperationContract]
        Task<ProductType> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<ProductType>> QueryAsync(ProductTypeQuery query = null);

        [OperationContract]
        Task<int> CountAsync(ProductTypeQuery query = null);

        [OperationContract]
        Task<ProductType> UpdateAsync(ProductType entity);
    }

    [ServiceContract]
    public partial interface IUnitDataService
    {
        [OperationContract]
        Task<Unit> AddAsync(Unit entity);

        [OperationContract]
        Task DeleteAsync(Unit entity);

        [OperationContract]
        Task<Unit> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<Unit>> QueryAsync(UnitQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UnitQuery query = null);

        [OperationContract]
        Task<Unit> UpdateAsync(Unit entity);
    }

    [ServiceContract]
    public partial interface IUpdateGroupDataService
    {
        [OperationContract]
        Task<UpdateGroup> AddAsync(UpdateGroup entity);

        [OperationContract]
        Task DeleteAsync(UpdateGroup entity);

        [OperationContract]
        Task<UpdateGroup> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UpdateGroup>> QueryAsync(UpdateGroupQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UpdateGroupQuery query = null);

        [OperationContract]
        Task<UpdateGroup> UpdateAsync(UpdateGroup entity);
    }

    [ServiceContract]
    public partial interface IUpdatePartDataService
    {
        [OperationContract]
        Task<UpdatePart> AddAsync(UpdatePart entity);

        [OperationContract]
        Task DeleteAsync(UpdatePart entity);

        [OperationContract]
        Task<UpdatePart> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UpdatePart>> QueryAsync(UpdatePartQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UpdatePartQuery query = null);

        [OperationContract]
        Task<UpdatePart> UpdateAsync(UpdatePart entity);
    }

    [ServiceContract]
    public partial interface IUpdateCommandDataService
    {
        [OperationContract]
        Task<UpdateCommand> AddAsync(UpdateCommand entity);

        [OperationContract]
        Task DeleteAsync(UpdateCommand entity);

        [OperationContract]
        Task<UpdateCommand> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UpdateCommand>> QueryAsync(UpdateCommandQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UpdateCommandQuery query = null);

        [OperationContract]
        Task<UpdateCommand> UpdateAsync(UpdateCommand entity);
    }

    [ServiceContract]
    public partial interface IUpdateFeedbackDataService
    {
        [OperationContract]
        Task<UpdateFeedback> AddAsync(UpdateFeedback entity);

        [OperationContract]
        Task DeleteAsync(UpdateFeedback entity);

        [OperationContract]
        Task<UpdateFeedback> GetAsync(int id);

        [OperationContract]
        Task<IEnumerable<UpdateFeedback>> QueryAsync(UpdateFeedbackQuery query = null);

        [OperationContract]
        Task<int> CountAsync(UpdateFeedbackQuery query = null);

        [OperationContract]
        Task<UpdateFeedback> UpdateAsync(UpdateFeedback entity);
    }
}