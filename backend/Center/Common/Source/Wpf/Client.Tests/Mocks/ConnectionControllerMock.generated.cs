
namespace Gorba.Center.Common.Wpf.Client.Tests.Mocks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;	
    
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;
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

    public partial class ConnectionControllerMock
    {

        public virtual IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; private set; }

        public virtual IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; private set; }

        public virtual ITenantChangeTrackingManager TenantChangeTrackingManager { get; private set; }

        public virtual IUserChangeTrackingManager UserChangeTrackingManager { get; private set; }

        public virtual IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; private set; }

        public virtual IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; private set; }

        public virtual IUnitChangeTrackingManager UnitChangeTrackingManager { get; private set; }

        public virtual IResourceChangeTrackingManager ResourceChangeTrackingManager { get; private set; }

        public virtual IContentResourceChangeTrackingManager ContentResourceChangeTrackingManager { get; private set; }

        public virtual IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; private set; }

        public virtual IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; private set; }

        public virtual IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; private set; }

        public virtual IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; private set; }

        public virtual IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; private set; }

        public virtual IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; private set; }

        public virtual IPackageChangeTrackingManager PackageChangeTrackingManager { get; private set; }

        public virtual IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; private set; }

        public virtual IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; private set; }

        public virtual IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; private set; }

        public virtual IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; private set; }

        public virtual ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; private set; }


        private void ConfigureChangeTrackingManagers()
        {
            this.UserRoleChangeTrackingManager = new UserRoleChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UserRoleChangeTrackingManager);
            this.AuthorizationChangeTrackingManager = new AuthorizationChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.AuthorizationChangeTrackingManager);
            this.TenantChangeTrackingManager = new TenantChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.TenantChangeTrackingManager);
            this.UserChangeTrackingManager = new UserChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UserChangeTrackingManager);
            this.AssociationTenantUserUserRoleChangeTrackingManager = new AssociationTenantUserUserRoleChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.AssociationTenantUserUserRoleChangeTrackingManager);
            this.ProductTypeChangeTrackingManager = new ProductTypeChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.ProductTypeChangeTrackingManager);
            this.UnitChangeTrackingManager = new UnitChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UnitChangeTrackingManager);
            this.ResourceChangeTrackingManager = new ResourceChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.ResourceChangeTrackingManager);
            this.ContentResourceChangeTrackingManager = new ContentResourceChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.ContentResourceChangeTrackingManager);
            this.UpdateGroupChangeTrackingManager = new UpdateGroupChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UpdateGroupChangeTrackingManager);
            this.UpdatePartChangeTrackingManager = new UpdatePartChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UpdatePartChangeTrackingManager);
            this.UpdateCommandChangeTrackingManager = new UpdateCommandChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UpdateCommandChangeTrackingManager);
            this.UpdateFeedbackChangeTrackingManager = new UpdateFeedbackChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UpdateFeedbackChangeTrackingManager);
            this.DocumentChangeTrackingManager = new DocumentChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.DocumentChangeTrackingManager);
            this.DocumentVersionChangeTrackingManager = new DocumentVersionChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.DocumentVersionChangeTrackingManager);
            this.PackageChangeTrackingManager = new PackageChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.PackageChangeTrackingManager);
            this.PackageVersionChangeTrackingManager = new PackageVersionChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.PackageVersionChangeTrackingManager);
            this.UnitConfigurationChangeTrackingManager = new UnitConfigurationChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UnitConfigurationChangeTrackingManager);
            this.MediaConfigurationChangeTrackingManager = new MediaConfigurationChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.MediaConfigurationChangeTrackingManager);
            this.UserDefinedPropertyChangeTrackingManager = new UserDefinedPropertyChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.UserDefinedPropertyChangeTrackingManager);
            this.SystemConfigChangeTrackingManager = new SystemConfigChangeTrackingManagerMock();
            DependencyResolver.Current.Register(this.SystemConfigChangeTrackingManager);

        }
    }
}

