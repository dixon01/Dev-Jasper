namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    
    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    
    using Gorba.Center.Admin.Core.DataViewModels;

    namespace AccessControl
    {
        using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
        using Gorba.Center.Admin.Core.ViewModels.Stages.AccessControl;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        
        public partial class UserRoleStageController : EntityStageControllerBase
        {
            private readonly UserRoleDataController dataController;
            private readonly UserRoleStageViewModel stage;

            public UserRoleStageController(UserRoleDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UserRole";
                this.PartitionName = "AccessControl";
                this.StageViewModel = this.stage = new UserRoleStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserRoleReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UserRoleDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UserRoles == null)
                {
                    this.stage.UserRoles = this.dataController.All;
                }
            }
        }

        
        public partial class AuthorizationStageController : EntityStageControllerBase
        {
            private readonly AuthorizationDataController dataController;
            private readonly AuthorizationStageViewModel stage;

            public AuthorizationStageController(AuthorizationDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Authorization";
                this.PartitionName = "AccessControl";
                this.StageViewModel = this.stage = new AuthorizationStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is AuthorizationReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is AuthorizationDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Authorizations == null)
                {
                    this.stage.Authorizations = this.dataController.All;
                }
            }
        }

            }

    namespace Membership
    {
        using Gorba.Center.Admin.Core.DataViewModels.Membership;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        
        public partial class TenantStageController : EntityStageControllerBase
        {
            private readonly TenantDataController dataController;
            private readonly TenantStageViewModel stage;

            public TenantStageController(TenantDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Tenant";
                this.PartitionName = "Membership";
                this.StageViewModel = this.stage = new TenantStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is TenantReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is TenantDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Tenants == null)
                {
                    this.stage.Tenants = this.dataController.All;
                }
            }
        }

        
        public partial class UserStageController : EntityStageControllerBase
        {
            private readonly UserDataController dataController;
            private readonly UserStageViewModel stage;

            public UserStageController(UserDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "User";
                this.PartitionName = "Membership";
                this.StageViewModel = this.stage = new UserStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UserDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Users == null)
                {
                    this.stage.Users = this.dataController.All;
                }
            }
        }

        
        public partial class AssociationTenantUserUserRoleStageController : EntityStageControllerBase
        {
            private readonly AssociationTenantUserUserRoleDataController dataController;
            private readonly AssociationTenantUserUserRoleStageViewModel stage;

            public AssociationTenantUserUserRoleStageController(AssociationTenantUserUserRoleDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "AssociationTenantUserUserRole";
                this.PartitionName = "Membership";
                this.StageViewModel = this.stage = new AssociationTenantUserUserRoleStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is AssociationTenantUserUserRoleReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is AssociationTenantUserUserRoleDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.AssociationTenantUserUserRoles == null)
                {
                    this.stage.AssociationTenantUserUserRoles = this.dataController.All;
                }
            }
        }

            }

    namespace Units
    {
        using Gorba.Center.Admin.Core.DataViewModels.Units;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        
        public partial class ProductTypeStageController : EntityStageControllerBase
        {
            private readonly ProductTypeDataController dataController;
            private readonly ProductTypeStageViewModel stage;

            public ProductTypeStageController(ProductTypeDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "ProductType";
                this.PartitionName = "Units";
                this.StageViewModel = this.stage = new ProductTypeStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is ProductTypeReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is ProductTypeDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.ProductTypes == null)
                {
                    this.stage.ProductTypes = this.dataController.All;
                }
            }
        }

        
        public partial class UnitStageController : EntityStageControllerBase
        {
            private readonly UnitDataController dataController;
            private readonly UnitStageViewModel stage;

            public UnitStageController(UnitDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Unit";
                this.PartitionName = "Units";
                this.StageViewModel = this.stage = new UnitStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Units == null)
                {
                    this.stage.Units = this.dataController.All;
                }
            }
        }

            }

    namespace Resources
    {
        using Gorba.Center.Admin.Core.DataViewModels.Resources;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        
        public partial class ResourceStageController : EntityStageControllerBase
        {
            private readonly ResourceDataController dataController;
            private readonly ResourceStageViewModel stage;

            public ResourceStageController(ResourceDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Resource";
                this.PartitionName = "Resources";
                this.StageViewModel = this.stage = new ResourceStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is ResourceReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is ResourceDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Resources == null)
                {
                    this.stage.Resources = this.dataController.All;
                }
            }
        }

            }

    namespace Update
    {
        using Gorba.Center.Admin.Core.DataViewModels.Update;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        
        public partial class UpdateGroupStageController : EntityStageControllerBase
        {
            private readonly UpdateGroupDataController dataController;
            private readonly UpdateGroupStageViewModel stage;

            public UpdateGroupStageController(UpdateGroupDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UpdateGroup";
                this.PartitionName = "Update";
                this.StageViewModel = this.stage = new UpdateGroupStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateGroupReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateGroupDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UpdateGroups == null)
                {
                    this.stage.UpdateGroups = this.dataController.All;
                }
            }
        }

        
        public partial class UpdatePartStageController : EntityStageControllerBase
        {
            private readonly UpdatePartDataController dataController;
            private readonly UpdatePartStageViewModel stage;

            public UpdatePartStageController(UpdatePartDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UpdatePart";
                this.PartitionName = "Update";
                this.StageViewModel = this.stage = new UpdatePartStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdatePartReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdatePartDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UpdateParts == null)
                {
                    this.stage.UpdateParts = this.dataController.All;
                }
            }
        }

        
        public partial class UpdateCommandStageController : EntityStageControllerBase
        {
            private readonly UpdateCommandDataController dataController;
            private readonly UpdateCommandStageViewModel stage;

            public UpdateCommandStageController(UpdateCommandDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UpdateCommand";
                this.PartitionName = "Update";
                this.StageViewModel = this.stage = new UpdateCommandStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateCommandReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateCommandDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UpdateCommands == null)
                {
                    this.stage.UpdateCommands = this.dataController.All;
                }
            }
        }

        
        public partial class UpdateFeedbackStageController : EntityStageControllerBase
        {
            private readonly UpdateFeedbackDataController dataController;
            private readonly UpdateFeedbackStageViewModel stage;

            public UpdateFeedbackStageController(UpdateFeedbackDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UpdateFeedback";
                this.PartitionName = "Update";
                this.StageViewModel = this.stage = new UpdateFeedbackStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateFeedbackReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateFeedbackDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UpdateFeedbacks == null)
                {
                    this.stage.UpdateFeedbacks = this.dataController.All;
                }
            }
        }

            }

    namespace Documents
    {
        using Gorba.Center.Admin.Core.DataViewModels.Documents;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        
        public partial class DocumentStageController : EntityStageControllerBase
        {
            private readonly DocumentDataController dataController;
            private readonly DocumentStageViewModel stage;

            public DocumentStageController(DocumentDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Document";
                this.PartitionName = "Documents";
                this.StageViewModel = this.stage = new DocumentStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Documents == null)
                {
                    this.stage.Documents = this.dataController.All;
                }
            }
        }

        
        public partial class DocumentVersionStageController : EntityStageControllerBase
        {
            private readonly DocumentVersionDataController dataController;
            private readonly DocumentVersionStageViewModel stage;

            public DocumentVersionStageController(DocumentVersionDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "DocumentVersion";
                this.PartitionName = "Documents";
                this.StageViewModel = this.stage = new DocumentVersionStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentVersionReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentVersionDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.DocumentVersions == null)
                {
                    this.stage.DocumentVersions = this.dataController.All;
                }
            }
        }

            }

    namespace Software
    {
        using Gorba.Center.Admin.Core.DataViewModels.Software;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        
        public partial class PackageStageController : EntityStageControllerBase
        {
            private readonly PackageDataController dataController;
            private readonly PackageStageViewModel stage;

            public PackageStageController(PackageDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "Package";
                this.PartitionName = "Software";
                this.StageViewModel = this.stage = new PackageStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.Packages == null)
                {
                    this.stage.Packages = this.dataController.All;
                }
            }
        }

        
        public partial class PackageVersionStageController : EntityStageControllerBase
        {
            private readonly PackageVersionDataController dataController;
            private readonly PackageVersionStageViewModel stage;

            public PackageVersionStageController(PackageVersionDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "PackageVersion";
                this.PartitionName = "Software";
                this.StageViewModel = this.stage = new PackageVersionStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageVersionReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageVersionDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.PackageVersions == null)
                {
                    this.stage.PackageVersions = this.dataController.All;
                }
            }
        }

            }

    namespace Configurations
    {
        using Gorba.Center.Admin.Core.DataViewModels.Configurations;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        
        public partial class UnitConfigurationStageController : EntityStageControllerBase
        {
            private readonly UnitConfigurationDataController dataController;
            private readonly UnitConfigurationStageViewModel stage;

            public UnitConfigurationStageController(UnitConfigurationDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UnitConfiguration";
                this.PartitionName = "Configurations";
                this.StageViewModel = this.stage = new UnitConfigurationStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitConfigurationReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitConfigurationDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UnitConfigurations == null)
                {
                    this.stage.UnitConfigurations = this.dataController.All;
                }
            }
        }

        
        public partial class MediaConfigurationStageController : EntityStageControllerBase
        {
            private readonly MediaConfigurationDataController dataController;
            private readonly MediaConfigurationStageViewModel stage;

            public MediaConfigurationStageController(MediaConfigurationDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "MediaConfiguration";
                this.PartitionName = "Configurations";
                this.StageViewModel = this.stage = new MediaConfigurationStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is MediaConfigurationReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is MediaConfigurationDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.MediaConfigurations == null)
                {
                    this.stage.MediaConfigurations = this.dataController.All;
                }
            }
        }

            }

    namespace Log
    {
        using Gorba.Center.Admin.Core.DataViewModels.Log;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Log;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;

            }

    namespace Meta
    {
        using Gorba.Center.Admin.Core.DataViewModels.Meta;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Meta;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        
        public partial class UserDefinedPropertyStageController : EntityStageControllerBase
        {
            private readonly UserDefinedPropertyDataController dataController;
            private readonly UserDefinedPropertyStageViewModel stage;

            public UserDefinedPropertyStageController(UserDefinedPropertyDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "UserDefinedProperty";
                this.PartitionName = "Meta";
                this.StageViewModel = this.stage = new UserDefinedPropertyStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserDefinedPropertyReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is UserDefinedPropertyDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.UserDefinedProperties == null)
                {
                    this.stage.UserDefinedProperties = this.dataController.All;
                }
            }
        }

        
        public partial class SystemConfigStageController : EntityStageControllerBase
        {
            private readonly SystemConfigDataController dataController;
            private readonly SystemConfigStageViewModel stage;

            public SystemConfigStageController(SystemConfigDataController dataController)
                : base(dataController)
            {
                this.dataController = dataController;
                this.Name = "SystemConfig";
                this.PartitionName = "Meta";
                this.StageViewModel = this.stage = new SystemConfigStageViewModel(dataController.Factory.CommandRegistry);
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is SystemConfigReadOnlyDataViewModel;
            }

            public override bool SupportsEntity(DataViewModelBase dataViewModel)
            {
                return dataViewModel is SystemConfigDataViewModel;
            }
            
            public override void LoadData()
            {
                if (this.stage.SystemConfigs == null)
                {
                    this.stage.SystemConfigs = this.dataController.All;
                }
            }
        }

            }
}