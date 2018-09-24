namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;

    public partial class ChangeTrackingManagementBootstrapperResult
    {
        //public Task<IUserRoleChangeTrackingManager> UserRoleChangeTrackingManager { get; set; }
		public IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; set; }

        //public Task<IAuthorizationChangeTrackingManager> AuthorizationChangeTrackingManager { get; set; }
		public IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; set; }

        //public Task<ITenantChangeTrackingManager> TenantChangeTrackingManager { get; set; }
		public ITenantChangeTrackingManager TenantChangeTrackingManager { get; set; }

        //public Task<IUserChangeTrackingManager> UserChangeTrackingManager { get; set; }
		public IUserChangeTrackingManager UserChangeTrackingManager { get; set; }

        //public Task<IAssociationTenantUserUserRoleChangeTrackingManager> AssociationTenantUserUserRoleChangeTrackingManager { get; set; }
		public IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; set; }

        //public Task<IProductTypeChangeTrackingManager> ProductTypeChangeTrackingManager { get; set; }
		public IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; set; }

        //public Task<IUnitChangeTrackingManager> UnitChangeTrackingManager { get; set; }
		public IUnitChangeTrackingManager UnitChangeTrackingManager { get; set; }

        //public Task<IResourceChangeTrackingManager> ResourceChangeTrackingManager { get; set; }
		public IResourceChangeTrackingManager ResourceChangeTrackingManager { get; set; }

        //public Task<IContentResourceChangeTrackingManager> ContentResourceChangeTrackingManager { get; set; }
		public IContentResourceChangeTrackingManager ContentResourceChangeTrackingManager { get; set; }

        //public Task<IUpdateGroupChangeTrackingManager> UpdateGroupChangeTrackingManager { get; set; }
		public IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; set; }

        //public Task<IUpdatePartChangeTrackingManager> UpdatePartChangeTrackingManager { get; set; }
		public IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; set; }

        //public Task<IUpdateCommandChangeTrackingManager> UpdateCommandChangeTrackingManager { get; set; }
		public IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; set; }

        //public Task<IUpdateFeedbackChangeTrackingManager> UpdateFeedbackChangeTrackingManager { get; set; }
		public IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; set; }

        //public Task<IDocumentChangeTrackingManager> DocumentChangeTrackingManager { get; set; }
		public IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; set; }

        //public Task<IDocumentVersionChangeTrackingManager> DocumentVersionChangeTrackingManager { get; set; }
		public IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; set; }

        //public Task<IPackageChangeTrackingManager> PackageChangeTrackingManager { get; set; }
		public IPackageChangeTrackingManager PackageChangeTrackingManager { get; set; }

        //public Task<IPackageVersionChangeTrackingManager> PackageVersionChangeTrackingManager { get; set; }
		public IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; set; }

        //public Task<IUnitConfigurationChangeTrackingManager> UnitConfigurationChangeTrackingManager { get; set; }
		public IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; set; }

        //public Task<IMediaConfigurationChangeTrackingManager> MediaConfigurationChangeTrackingManager { get; set; }
		public IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; set; }

        //public Task<IUserDefinedPropertyChangeTrackingManager> UserDefinedPropertyChangeTrackingManager { get; set; }
		public IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; set; }

        //public Task<ISystemConfigChangeTrackingManager> SystemConfigChangeTrackingManager { get; set; }
		public ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; set; }
    }
}