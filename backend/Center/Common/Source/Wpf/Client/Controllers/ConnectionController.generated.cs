
namespace Gorba.Center.Common.Wpf.Client.Controllers
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

    public partial interface IConnectionController : IDisposable
    {
	
        IUserRoleChangeTrackingManager UserRoleChangeTrackingManager { get; }

	
        IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager { get; }

	
        ITenantChangeTrackingManager TenantChangeTrackingManager { get; }

	
        IUserChangeTrackingManager UserChangeTrackingManager { get; }

	
        IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager { get; }

	
        IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager { get; }

	
        IUnitChangeTrackingManager UnitChangeTrackingManager { get; }

	
        IResourceChangeTrackingManager ResourceChangeTrackingManager { get; }

	
        IContentResourceChangeTrackingManager ContentResourceChangeTrackingManager { get; }

	
        IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager { get; }

	
        IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager { get; }

	
        IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager { get; }

	
        IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager { get; }

	
        IDocumentChangeTrackingManager DocumentChangeTrackingManager { get; }

	
        IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager { get; }

	
        IPackageChangeTrackingManager PackageChangeTrackingManager { get; }

	
        IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager { get; }

	
        IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager { get; }

	
        IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager { get; }

	
        IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager { get; }

	
        ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager { get; }

    }
    
    public partial class ConnectionController
    {
        private IUserRoleChangeTrackingManager ctmUserRole;

        public IUserRoleChangeTrackingManager UserRoleChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UserRoleChangeTrackingManager when not connected to a server");
                }
                
                return ctmUserRole;
            }
            
            private set
            {
                this.ctmUserRole = value;
            }
        }

        private IAuthorizationChangeTrackingManager ctmAuthorization;

        public IAuthorizationChangeTrackingManager AuthorizationChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the AuthorizationChangeTrackingManager when not connected to a server");
                }
                
                return ctmAuthorization;
            }
            
            private set
            {
                this.ctmAuthorization = value;
            }
        }

        private ITenantChangeTrackingManager ctmTenant;

        public ITenantChangeTrackingManager TenantChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the TenantChangeTrackingManager when not connected to a server");
                }
                
                return ctmTenant;
            }
            
            private set
            {
                this.ctmTenant = value;
            }
        }

        private IUserChangeTrackingManager ctmUser;

        public IUserChangeTrackingManager UserChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UserChangeTrackingManager when not connected to a server");
                }
                
                return ctmUser;
            }
            
            private set
            {
                this.ctmUser = value;
            }
        }

        private IAssociationTenantUserUserRoleChangeTrackingManager ctmAssociationTenantUserUserRole;

        public IAssociationTenantUserUserRoleChangeTrackingManager AssociationTenantUserUserRoleChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the AssociationTenantUserUserRoleChangeTrackingManager when not connected to a server");
                }
                
                return ctmAssociationTenantUserUserRole;
            }
            
            private set
            {
                this.ctmAssociationTenantUserUserRole = value;
            }
        }

        private IProductTypeChangeTrackingManager ctmProductType;

        public IProductTypeChangeTrackingManager ProductTypeChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the ProductTypeChangeTrackingManager when not connected to a server");
                }
                
                return ctmProductType;
            }
            
            private set
            {
                this.ctmProductType = value;
            }
        }

        private IUnitChangeTrackingManager ctmUnit;

        public IUnitChangeTrackingManager UnitChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UnitChangeTrackingManager when not connected to a server");
                }
                
                return ctmUnit;
            }
            
            private set
            {
                this.ctmUnit = value;
            }
        }

        private IResourceChangeTrackingManager ctmResource;

        public IResourceChangeTrackingManager ResourceChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the ResourceChangeTrackingManager when not connected to a server");
                }
                
                return ctmResource;
            }
            
            private set
            {
                this.ctmResource = value;
            }
        }

        private IContentResourceChangeTrackingManager ctmContentResource;

        public IContentResourceChangeTrackingManager ContentResourceChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the ContentResourceChangeTrackingManager when not connected to a server");
                }
                
                return ctmContentResource;
            }
            
            private set
            {
                this.ctmContentResource = value;
            }
        }

        private IUpdateGroupChangeTrackingManager ctmUpdateGroup;

        public IUpdateGroupChangeTrackingManager UpdateGroupChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UpdateGroupChangeTrackingManager when not connected to a server");
                }
                
                return ctmUpdateGroup;
            }
            
            private set
            {
                this.ctmUpdateGroup = value;
            }
        }

        private IUpdatePartChangeTrackingManager ctmUpdatePart;

        public IUpdatePartChangeTrackingManager UpdatePartChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UpdatePartChangeTrackingManager when not connected to a server");
                }
                
                return ctmUpdatePart;
            }
            
            private set
            {
                this.ctmUpdatePart = value;
            }
        }

        private IUpdateCommandChangeTrackingManager ctmUpdateCommand;

        public IUpdateCommandChangeTrackingManager UpdateCommandChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UpdateCommandChangeTrackingManager when not connected to a server");
                }
                
                return ctmUpdateCommand;
            }
            
            private set
            {
                this.ctmUpdateCommand = value;
            }
        }

        private IUpdateFeedbackChangeTrackingManager ctmUpdateFeedback;

        public IUpdateFeedbackChangeTrackingManager UpdateFeedbackChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UpdateFeedbackChangeTrackingManager when not connected to a server");
                }
                
                return ctmUpdateFeedback;
            }
            
            private set
            {
                this.ctmUpdateFeedback = value;
            }
        }

        private IDocumentChangeTrackingManager ctmDocument;

        public IDocumentChangeTrackingManager DocumentChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the DocumentChangeTrackingManager when not connected to a server");
                }
                
                return ctmDocument;
            }
            
            private set
            {
                this.ctmDocument = value;
            }
        }

        private IDocumentVersionChangeTrackingManager ctmDocumentVersion;

        public IDocumentVersionChangeTrackingManager DocumentVersionChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the DocumentVersionChangeTrackingManager when not connected to a server");
                }
                
                return ctmDocumentVersion;
            }
            
            private set
            {
                this.ctmDocumentVersion = value;
            }
        }

        private IPackageChangeTrackingManager ctmPackage;

        public IPackageChangeTrackingManager PackageChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the PackageChangeTrackingManager when not connected to a server");
                }
                
                return ctmPackage;
            }
            
            private set
            {
                this.ctmPackage = value;
            }
        }

        private IPackageVersionChangeTrackingManager ctmPackageVersion;

        public IPackageVersionChangeTrackingManager PackageVersionChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the PackageVersionChangeTrackingManager when not connected to a server");
                }
                
                return ctmPackageVersion;
            }
            
            private set
            {
                this.ctmPackageVersion = value;
            }
        }

        private IUnitConfigurationChangeTrackingManager ctmUnitConfiguration;

        public IUnitConfigurationChangeTrackingManager UnitConfigurationChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UnitConfigurationChangeTrackingManager when not connected to a server");
                }
                
                return ctmUnitConfiguration;
            }
            
            private set
            {
                this.ctmUnitConfiguration = value;
            }
        }

        private IMediaConfigurationChangeTrackingManager ctmMediaConfiguration;

        public IMediaConfigurationChangeTrackingManager MediaConfigurationChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the MediaConfigurationChangeTrackingManager when not connected to a server");
                }
                
                return ctmMediaConfiguration;
            }
            
            private set
            {
                this.ctmMediaConfiguration = value;
            }
        }

        private IUserDefinedPropertyChangeTrackingManager ctmUserDefinedProperty;

        public IUserDefinedPropertyChangeTrackingManager UserDefinedPropertyChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the UserDefinedPropertyChangeTrackingManager when not connected to a server");
                }
                
                return ctmUserDefinedProperty;
            }
            
            private set
            {
                this.ctmUserDefinedProperty = value;
            }
        }

        private ISystemConfigChangeTrackingManager ctmSystemConfig;

        public ISystemConfigChangeTrackingManager SystemConfigChangeTrackingManager
        {
            get
            {
                if (!this.IsConfigured)
                {
                    throw new NotSupportedException("Can't get the SystemConfigChangeTrackingManager when not connected to a server");
                }
                
                return ctmSystemConfig;
            }
            
            private set
            {
                this.ctmSystemConfig = value;
            }
        }


        private async Task ConfigureChangeTrackingManagersAsync(
            BackgroundSystemConfiguration configuration, UserCredentials userCredentials, Action<Exception[]> errorSummaryCallback = null)
        {
            var sessionId = NotificationManagerUtility.GenerateUniqueSessionId();
            var bootstrapper = new RemoteChangeTrackingManagementBootstrapper(
                configuration,
                NotificationManagerUtility.GetApplicationName(),
                sessionId);

            var result = await bootstrapper.RunAsync(userCredentials);

            // assign managers to this controller change trackers

            this.UserRoleChangeTrackingManager = result.UserRoleChangeTrackingManager;
            
            this.AuthorizationChangeTrackingManager = result.AuthorizationChangeTrackingManager;
            
            this.TenantChangeTrackingManager = result.TenantChangeTrackingManager;
            
            this.UserChangeTrackingManager = result.UserChangeTrackingManager;
            
            this.AssociationTenantUserUserRoleChangeTrackingManager = result.AssociationTenantUserUserRoleChangeTrackingManager;
            
            this.ProductTypeChangeTrackingManager = result.ProductTypeChangeTrackingManager;
            
            this.UnitChangeTrackingManager = result.UnitChangeTrackingManager;
            
            this.ResourceChangeTrackingManager = result.ResourceChangeTrackingManager;
            
            this.ContentResourceChangeTrackingManager = result.ContentResourceChangeTrackingManager;
            
            this.UpdateGroupChangeTrackingManager = result.UpdateGroupChangeTrackingManager;
            
            this.UpdatePartChangeTrackingManager = result.UpdatePartChangeTrackingManager;
            
            this.UpdateCommandChangeTrackingManager = result.UpdateCommandChangeTrackingManager;
            
            this.UpdateFeedbackChangeTrackingManager = result.UpdateFeedbackChangeTrackingManager;
            
            this.DocumentChangeTrackingManager = result.DocumentChangeTrackingManager;
            
            this.DocumentVersionChangeTrackingManager = result.DocumentVersionChangeTrackingManager;
            
            this.PackageChangeTrackingManager = result.PackageChangeTrackingManager;
            
            this.PackageVersionChangeTrackingManager = result.PackageVersionChangeTrackingManager;
            
            this.UnitConfigurationChangeTrackingManager = result.UnitConfigurationChangeTrackingManager;
            
            this.MediaConfigurationChangeTrackingManager = result.MediaConfigurationChangeTrackingManager;
            
            ChannelScopeFactoryUtility<ILogEntryDataService>.ConfigureAsDataService(configuration.DataServices, "LogEntry");
            this.UserDefinedPropertyChangeTrackingManager = result.UserDefinedPropertyChangeTrackingManager;
            
            this.SystemConfigChangeTrackingManager = result.SystemConfigChangeTrackingManager;
            
            // process errors from bootstrapper
            if (result.Exceptions.Count > 0)
            {
                errorSummaryCallback(result.Exceptions.ToArray());
            }

            this.ConfigureNotGenerated();
        }

        private void UpdateChangeTrackingCredentials()
        {
            this.UserRoleChangeTrackingManager.ChangeCredentials(this.credentials);
            this.AuthorizationChangeTrackingManager.ChangeCredentials(this.credentials);
            this.TenantChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UserChangeTrackingManager.ChangeCredentials(this.credentials);
            this.AssociationTenantUserUserRoleChangeTrackingManager.ChangeCredentials(this.credentials);
            this.ProductTypeChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UnitChangeTrackingManager.ChangeCredentials(this.credentials);
            this.ResourceChangeTrackingManager.ChangeCredentials(this.credentials);
            this.ContentResourceChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UpdateGroupChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UpdatePartChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UpdateCommandChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UpdateFeedbackChangeTrackingManager.ChangeCredentials(this.credentials);
            this.DocumentChangeTrackingManager.ChangeCredentials(this.credentials);
            this.DocumentVersionChangeTrackingManager.ChangeCredentials(this.credentials);
            this.PackageChangeTrackingManager.ChangeCredentials(this.credentials);
            this.PackageVersionChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UnitConfigurationChangeTrackingManager.ChangeCredentials(this.credentials);
            this.MediaConfigurationChangeTrackingManager.ChangeCredentials(this.credentials);
            this.UserDefinedPropertyChangeTrackingManager.ChangeCredentials(this.credentials);
            this.SystemConfigChangeTrackingManager.ChangeCredentials(this.credentials);
        }

        private void DisposeChangeTrackingManagers()
        {
            var actions = new Action[]
                              {
                                () => this.DisposeChangeTrackingManager(this.UserRoleChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.AuthorizationChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.TenantChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UserChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.AssociationTenantUserUserRoleChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.ProductTypeChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UnitChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.ResourceChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.ContentResourceChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UpdateGroupChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UpdatePartChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UpdateCommandChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UpdateFeedbackChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.DocumentChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.DocumentVersionChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.PackageChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.PackageVersionChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UnitConfigurationChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.MediaConfigurationChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.UserDefinedPropertyChangeTrackingManager),
                                () => this.DisposeChangeTrackingManager(this.SystemConfigChangeTrackingManager),
                            };
            actions.AsParallel().ForAll(action => action());

        }

        private void DisposeChangeTrackingManager(IChangeTrackingManager changeTrackingManager)
        {
            if (changeTrackingManager == null)
            {
                return;
            }
            
            try
            {
                changeTrackingManager.Dispose();
				changeTrackingManager = null;
            }
            catch (Exception exception)
            {
                Logger.DebugException("Error while disposing changeTrackingManager", exception);
            }
        }

        partial void ConfigureNotGenerated();
    }
}

