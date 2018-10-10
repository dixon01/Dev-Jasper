namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;

	public partial interface IChangeTrackingManagersSet
	{
		IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; set; }

		IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; set; }

		ITenantChangeTrackingManager TenantChangeTrackingManager { get; set; }

		IUserChangeTrackingManager UserChangeTrackingManager { get; set; }

		IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; set; }

		IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; set; }

		IUnitChangeTrackingManager UnitChangeTrackingManager { get; set; }

		IResourceChangeTrackingManager ResourceChangeTrackingManager { get; set; }

		IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; set; }

		IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; set; }

		IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; set; }

		IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; set; }

		IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; set; }

		IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; set; }

		IPackageChangeTrackingManager PackageChangeTrackingManager { get; set; }

		IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; set; }

		IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; set; }

		IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; set; }

		IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; set; }

		ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; set; }
	}

	public partial class ChangeTrackingManagersSet : IChangeTrackingManagersSet
	{
		public IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; set; }

		public IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; set; }

		public ITenantChangeTrackingManager TenantChangeTrackingManager { get; set; }

		public IUserChangeTrackingManager UserChangeTrackingManager { get; set; }

		public IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; set; }

		public IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; set; }

		public IUnitChangeTrackingManager UnitChangeTrackingManager { get; set; }

		public IResourceChangeTrackingManager ResourceChangeTrackingManager { get; set; }

		public IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; set; }

		public IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; set; }

		public IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; set; }

		public IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; set; }

		public IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; set; }

		public IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; set; }

		public IPackageChangeTrackingManager PackageChangeTrackingManager { get; set; }

		public IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; set; }

		public IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; set; }

		public IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; set; }

		public IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; set; }

		public ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; set; }
	}

	public static class ChangeTrackingManagersSetExtensions
	{
		public static IChangeTrackingManagersSet ToSet(this ChangeTrackingManagementBootstrapperResult result)
		{
			var set = new ChangeTrackingManagersSet();
			set.UserRoleChangeTrackingManager = result.UserRoleChangeTrackingManager;
			set.AuthorizationChangeTrackingManager = result.AuthorizationChangeTrackingManager;
			set.TenantChangeTrackingManager = result.TenantChangeTrackingManager;
			set.UserChangeTrackingManager = result.UserChangeTrackingManager;
			set.AssociationTenantUserUserRoleChangeTrackingManager = result.AssociationTenantUserUserRoleChangeTrackingManager;
			set.ProductTypeChangeTrackingManager = result.ProductTypeChangeTrackingManager;
			set.UnitChangeTrackingManager = result.UnitChangeTrackingManager;
			set.ResourceChangeTrackingManager = result.ResourceChangeTrackingManager;
			set.UpdateGroupChangeTrackingManager = result.UpdateGroupChangeTrackingManager;
			set.UpdatePartChangeTrackingManager = result.UpdatePartChangeTrackingManager;
			set.UpdateCommandChangeTrackingManager = result.UpdateCommandChangeTrackingManager;
			set.UpdateFeedbackChangeTrackingManager = result.UpdateFeedbackChangeTrackingManager;
			set.DocumentChangeTrackingManager = result.DocumentChangeTrackingManager;
			set.DocumentVersionChangeTrackingManager = result.DocumentVersionChangeTrackingManager;
			set.PackageChangeTrackingManager = result.PackageChangeTrackingManager;
			set.PackageVersionChangeTrackingManager = result.PackageVersionChangeTrackingManager;
			set.UnitConfigurationChangeTrackingManager = result.UnitConfigurationChangeTrackingManager;
			set.MediaConfigurationChangeTrackingManager = result.MediaConfigurationChangeTrackingManager;
			set.UserDefinedPropertyChangeTrackingManager = result.UserDefinedPropertyChangeTrackingManager;
			set.SystemConfigChangeTrackingManager = result.SystemConfigChangeTrackingManager;
			return set;
		}
	}
}
