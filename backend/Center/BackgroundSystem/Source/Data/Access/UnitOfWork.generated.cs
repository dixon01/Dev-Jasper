namespace Gorba.Center.BackgroundSystem.Data.Access
{
	using System;
	using System.Transactions;

	using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
	using Gorba.Center.BackgroundSystem.Data.Model.Configurations;
	using Gorba.Center.BackgroundSystem.Data.Model.Documents;
	using Gorba.Center.BackgroundSystem.Data.Model.Log;
	using Gorba.Center.BackgroundSystem.Data.Model.Membership;
	using Gorba.Center.BackgroundSystem.Data.Model.Meta;
	using Gorba.Center.BackgroundSystem.Data.Model.Resources;
	using Gorba.Center.BackgroundSystem.Data.Model.Software;
	using Gorba.Center.BackgroundSystem.Data.Model.Units;
	using Gorba.Center.BackgroundSystem.Data.Model.Update;

	public interface IUnitOfWork : IDisposable
    {
        IRepository<Authorization> Authorizations { get; set; }

        IRepository<UserRole> UserRoles { get; set; }

        IRepository<UnitConfiguration> UnitConfigurations { get; set; }

        IRepository<MediaConfiguration> MediaConfigurations { get; set; }

        IRepository<Document> Documents { get; set; }

        IRepository<DocumentVersion> DocumentVersions { get; set; }

        IRepository<LogEntry> LogEntries { get; set; }

        IRepository<Tenant> Tenants { get; set; }

        IRepository<User> Users { get; set; }

        IRepository<AssociationTenantUserUserRole> AssociationTenantUserUserRoles { get; set; }

        IRepository<UserDefinedProperty> UserDefinedProperties { get; set; }

        IRepository<SystemConfig> SystemConfigs { get; set; }

        IRepository<Resource> Resources { get; set; }

        IRepository<ContentResource> ContentResources { get; set; }

        IRepository<Package> Packages { get; set; }

        IRepository<PackageVersion> PackageVersions { get; set; }

        IRepository<ProductType> ProductTypes { get; set; }

        IRepository<Unit> Units { get; set; }

        IRepository<UpdateGroup> UpdateGroups { get; set; }

        IRepository<UpdatePart> UpdateParts { get; set; }

        IRepository<UpdateCommand> UpdateCommands { get; set; }

        IRepository<UpdateFeedback> UpdateFeedbacks { get; set; }

        void Commit();

        void Rollback();

        void Initialize(CenterDataContext dataContext);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope transactionScope;

        public IRepository<Authorization> Authorizations { get; set; }

        public IRepository<UserRole> UserRoles { get; set; }

        public IRepository<UnitConfiguration> UnitConfigurations { get; set; }

        public IRepository<MediaConfiguration> MediaConfigurations { get; set; }

        public IRepository<Document> Documents { get; set; }

        public IRepository<DocumentVersion> DocumentVersions { get; set; }

        public IRepository<LogEntry> LogEntries { get; set; }

        public IRepository<Tenant> Tenants { get; set; }

        public IRepository<User> Users { get; set; }

        public IRepository<AssociationTenantUserUserRole> AssociationTenantUserUserRoles { get; set; }

        public IRepository<UserDefinedProperty> UserDefinedProperties { get; set; }

        public IRepository<SystemConfig> SystemConfigs { get; set; }

        public IRepository<Resource> Resources { get; set; }

        public IRepository<ContentResource> ContentResources { get; set; }

        public IRepository<Package> Packages { get; set; }

        public IRepository<PackageVersion> PackageVersions { get; set; }

        public IRepository<ProductType> ProductTypes { get; set; }

        public IRepository<Unit> Units { get; set; }

        public IRepository<UpdateGroup> UpdateGroups { get; set; }

        public IRepository<UpdatePart> UpdateParts { get; set; }

        public IRepository<UpdateCommand> UpdateCommands { get; set; }

        public IRepository<UpdateFeedback> UpdateFeedbacks { get; set; }

        public void Initialize(CenterDataContext dataContext)
        {
            this.transactionScope = Transaction.Current == null
                                        ? new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)
                                        : new TransactionScope(Transaction.Current, TransactionScopeAsyncFlowOption.Enabled);
			this.Authorizations = new AuthorizationRepository(dataContext);
			this.UserRoles = new UserRoleRepository(dataContext);
			this.UnitConfigurations = new UnitConfigurationRepository(dataContext);
			this.MediaConfigurations = new MediaConfigurationRepository(dataContext);
			this.Documents = new DocumentRepository(dataContext);
			this.DocumentVersions = new DocumentVersionRepository(dataContext);
			this.LogEntries = new LogEntryRepository(dataContext);
			this.Tenants = new TenantRepository(dataContext);
			this.Users = new UserRepository(dataContext);
			this.AssociationTenantUserUserRoles = new AssociationTenantUserUserRoleRepository(dataContext);
			this.UserDefinedProperties = new UserDefinedPropertyRepository(dataContext);
			this.SystemConfigs = new SystemConfigRepository(dataContext);
			this.Resources = new ResourceRepository(dataContext);
			this.ContentResources = new ContentResourceRepository(dataContext);
			this.Packages = new PackageRepository(dataContext);
			this.PackageVersions = new PackageVersionRepository(dataContext);
			this.ProductTypes = new ProductTypeRepository(dataContext);
			this.Units = new UnitRepository(dataContext);
			this.UpdateGroups = new UpdateGroupRepository(dataContext);
			this.UpdateParts = new UpdatePartRepository(dataContext);
			this.UpdateCommands = new UpdateCommandRepository(dataContext);
			this.UpdateFeedbacks = new UpdateFeedbackRepository(dataContext);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.transactionScope != null)
                {
                    this.transactionScope.Dispose();
                }
            }
        }

        public void Commit()
        {
            if (this.transactionScope == null)
            {
                return;
            }

            this.transactionScope.Complete();
        }

        public void Rollback()
        {
            if (this.transactionScope == null)
            {
                return;
            }

            this.transactionScope.Dispose();
        }
    }
}