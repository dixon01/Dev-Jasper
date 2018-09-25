namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    
    using Gorba.Center.Common.ServiceModel;
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
        
        public partial class UserRoleValidator : EntityValidatorBase
        {
            public UserRoleValidator(UserRoleDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UserRoleDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateName(UserRoleDataViewModel dvm);
            
            partial void ValidateDescription(UserRoleDataViewModel dvm);
            
            partial void Validate(string propertyName, UserRoleDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class AuthorizationValidator : EntityValidatorBase
        {
            public AuthorizationValidator(AuthorizationDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public AuthorizationDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUserRole(AuthorizationDataViewModel dvm);
            
            partial void ValidateDataScope(AuthorizationDataViewModel dvm);
            
            partial void ValidatePermission(AuthorizationDataViewModel dvm);
            
            partial void Validate(string propertyName, AuthorizationDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UserRole" || propertyName == null)
                {            
                    this.ValidateUserRole(this.DataViewModel);
                }

                if (propertyName == "DataScope" || propertyName == null)
                {            
                    this.ValidateDataScope(this.DataViewModel);
                }

                if (propertyName == "Permission" || propertyName == null)
                {            
                    this.ValidatePermission(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Membership
    {
        using Gorba.Center.Admin.Core.DataViewModels.Membership;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;
        
        public partial class TenantValidator : EntityValidatorBase
        {
            public TenantValidator(TenantDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public TenantDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateName(TenantDataViewModel dvm);
            
            partial void ValidateDescription(TenantDataViewModel dvm);
            
            partial void Validate(string propertyName, TenantDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class UserValidator : EntityValidatorBase
        {
            public UserValidator(UserDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UserDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateOwnerTenant(UserDataViewModel dvm);
            
            partial void ValidateUsername(UserDataViewModel dvm);
            
            partial void ValidateDomain(UserDataViewModel dvm);
            
            partial void ValidateHashedPassword(UserDataViewModel dvm);
            
            partial void ValidateFirstName(UserDataViewModel dvm);
            
            partial void ValidateLastName(UserDataViewModel dvm);
            
            partial void ValidateEmail(UserDataViewModel dvm);
            
            partial void ValidateCulture(UserDataViewModel dvm);
            
            partial void ValidateTimeZone(UserDataViewModel dvm);
            
            partial void ValidateDescription(UserDataViewModel dvm);
            
            partial void ValidateLastLoginAttempt(UserDataViewModel dvm);
            
            partial void ValidateLastSuccessfulLogin(UserDataViewModel dvm);
            
            partial void ValidateConsecutiveLoginFailures(UserDataViewModel dvm);
            
            partial void ValidateIsEnabled(UserDataViewModel dvm);
            
            partial void Validate(string propertyName, UserDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "OwnerTenant" || propertyName == null)
                {            
                    this.ValidateOwnerTenant(this.DataViewModel);
                }

                if (propertyName == "Username" || propertyName == null)
                {            
                    this.ValidateUsername(this.DataViewModel);
                }

                if (propertyName == "Domain" || propertyName == null)
                {            
                    this.ValidateDomain(this.DataViewModel);
                }

                if (propertyName == "HashedPassword" || propertyName == null)
                {            
                    this.ValidateHashedPassword(this.DataViewModel);
                }

                if (propertyName == "FirstName" || propertyName == null)
                {            
                    this.ValidateFirstName(this.DataViewModel);
                }

                if (propertyName == "LastName" || propertyName == null)
                {            
                    this.ValidateLastName(this.DataViewModel);
                }

                if (propertyName == "Email" || propertyName == null)
                {            
                    this.ValidateEmail(this.DataViewModel);
                }

                if (propertyName == "Culture" || propertyName == null)
                {            
                    this.ValidateCulture(this.DataViewModel);
                }

                if (propertyName == "TimeZone" || propertyName == null)
                {            
                    this.ValidateTimeZone(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "LastLoginAttempt" || propertyName == null)
                {            
                    this.ValidateLastLoginAttempt(this.DataViewModel);
                }

                if (propertyName == "LastSuccessfulLogin" || propertyName == null)
                {            
                    this.ValidateLastSuccessfulLogin(this.DataViewModel);
                }

                if (propertyName == "ConsecutiveLoginFailures" || propertyName == null)
                {            
                    this.ValidateConsecutiveLoginFailures(this.DataViewModel);
                }

                if (propertyName == "IsEnabled" || propertyName == null)
                {            
                    this.ValidateIsEnabled(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class AssociationTenantUserUserRoleValidator : EntityValidatorBase
        {
            public AssociationTenantUserUserRoleValidator(AssociationTenantUserUserRoleDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public AssociationTenantUserUserRoleDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateTenant(AssociationTenantUserUserRoleDataViewModel dvm);
            
            partial void ValidateUser(AssociationTenantUserUserRoleDataViewModel dvm);
            
            partial void ValidateUserRole(AssociationTenantUserUserRoleDataViewModel dvm);
            
            partial void Validate(string propertyName, AssociationTenantUserUserRoleDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Tenant" || propertyName == null)
                {            
                    this.ValidateTenant(this.DataViewModel);
                }

                if (propertyName == "User" || propertyName == null)
                {            
                    this.ValidateUser(this.DataViewModel);
                }

                if (propertyName == "UserRole" || propertyName == null)
                {            
                    this.ValidateUserRole(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Units
    {
        using Gorba.Center.Admin.Core.DataViewModels.Units;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;
        
        public partial class ProductTypeValidator : EntityValidatorBase
        {
            public ProductTypeValidator(ProductTypeDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public ProductTypeDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUnitType(ProductTypeDataViewModel dvm);
            
            partial void ValidateName(ProductTypeDataViewModel dvm);
            
            partial void ValidateDescription(ProductTypeDataViewModel dvm);
            
            partial void ValidateHardwareDescriptor(ProductTypeDataViewModel dvm);
            
            partial void Validate(string propertyName, ProductTypeDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UnitType" || propertyName == null)
                {            
                    this.ValidateUnitType(this.DataViewModel);
                }

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "HardwareDescriptor" || propertyName == null)
                {
                    this.ValidateXml("HardwareDescriptor", this.DataViewModel.HardwareDescriptor);            
                    this.ValidateHardwareDescriptor(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class UnitValidator : EntityValidatorBase
        {
            public UnitValidator(UnitDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UnitDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateTenant(UnitDataViewModel dvm);
            
            partial void ValidateProductType(UnitDataViewModel dvm);
            
            partial void ValidateName(UnitDataViewModel dvm);
            
            partial void ValidateNetworkAddress(UnitDataViewModel dvm);
            
            partial void ValidateDescription(UnitDataViewModel dvm);
            
            partial void ValidateIsConnected(UnitDataViewModel dvm);
            
            partial void ValidateUpdateGroup(UnitDataViewModel dvm);
            
            partial void Validate(string propertyName, UnitDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Tenant" || propertyName == null)
                {            
                    this.ValidateTenant(this.DataViewModel);
                }

                if (propertyName == "ProductType" || propertyName == null)
                {            
                    this.ValidateProductType(this.DataViewModel);
                }

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "NetworkAddress" || propertyName == null)
                {            
                    this.ValidateNetworkAddress(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "IsConnected" || propertyName == null)
                {            
                    this.ValidateIsConnected(this.DataViewModel);
                }

                if (propertyName == "UpdateGroup" || propertyName == null)
                {            
                    this.ValidateUpdateGroup(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Resources
    {
        using Gorba.Center.Admin.Core.DataViewModels.Resources;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;
        
        public partial class ResourceValidator : EntityValidatorBase
        {
            public ResourceValidator(ResourceDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public ResourceDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUploadingUser(ResourceDataViewModel dvm);
            
            partial void ValidateOriginalFilename(ResourceDataViewModel dvm);
            
            partial void ValidateDescription(ResourceDataViewModel dvm);
            
            partial void ValidateHash(ResourceDataViewModel dvm);
            
            partial void ValidateThumbnailHash(ResourceDataViewModel dvm);
            
            partial void ValidateMimeType(ResourceDataViewModel dvm);
            
            partial void ValidateLength(ResourceDataViewModel dvm);
            
            partial void Validate(string propertyName, ResourceDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UploadingUser" || propertyName == null)
                {            
                    this.ValidateUploadingUser(this.DataViewModel);
                }

                if (propertyName == "OriginalFilename" || propertyName == null)
                {            
                    this.ValidateOriginalFilename(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "Hash" || propertyName == null)
                {            
                    this.ValidateHash(this.DataViewModel);
                }

                if (propertyName == "ThumbnailHash" || propertyName == null)
                {            
                    this.ValidateThumbnailHash(this.DataViewModel);
                }

                if (propertyName == "MimeType" || propertyName == null)
                {            
                    this.ValidateMimeType(this.DataViewModel);
                }

                if (propertyName == "Length" || propertyName == null)
                {            
                    this.ValidateLength(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Update
    {
        using Gorba.Center.Admin.Core.DataViewModels.Update;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;
        
        public partial class UpdateGroupValidator : EntityValidatorBase
        {
            public UpdateGroupValidator(UpdateGroupDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UpdateGroupDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateName(UpdateGroupDataViewModel dvm);
            
            partial void ValidateDescription(UpdateGroupDataViewModel dvm);
            
            partial void ValidateTenant(UpdateGroupDataViewModel dvm);
            
            partial void ValidateUnitConfiguration(UpdateGroupDataViewModel dvm);
            
            partial void ValidateMediaConfiguration(UpdateGroupDataViewModel dvm);
            
            partial void Validate(string propertyName, UpdateGroupDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "Tenant" || propertyName == null)
                {            
                    this.ValidateTenant(this.DataViewModel);
                }

                if (propertyName == "UnitConfiguration" || propertyName == null)
                {            
                    this.ValidateUnitConfiguration(this.DataViewModel);
                }

                if (propertyName == "MediaConfiguration" || propertyName == null)
                {            
                    this.ValidateMediaConfiguration(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class UpdatePartValidator : EntityValidatorBase
        {
            public UpdatePartValidator(UpdatePartDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UpdatePartDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUpdateGroup(UpdatePartDataViewModel dvm);
            
            partial void ValidateType(UpdatePartDataViewModel dvm);
            
            partial void ValidateStart(UpdatePartDataViewModel dvm);
            
            partial void ValidateEnd(UpdatePartDataViewModel dvm);
            
            partial void ValidateDescription(UpdatePartDataViewModel dvm);
            
            partial void ValidateStructure(UpdatePartDataViewModel dvm);
            
            partial void ValidateInstallInstructions(UpdatePartDataViewModel dvm);
            
            partial void ValidateDynamicContent(UpdatePartDataViewModel dvm);
            
            partial void Validate(string propertyName, UpdatePartDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UpdateGroup" || propertyName == null)
                {            
                    this.ValidateUpdateGroup(this.DataViewModel);
                }

                if (propertyName == "Type" || propertyName == null)
                {            
                    this.ValidateType(this.DataViewModel);
                }

                if (propertyName == "Start" || propertyName == null)
                {            
                    this.ValidateStart(this.DataViewModel);
                }

                if (propertyName == "End" || propertyName == null)
                {            
                    this.ValidateEnd(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "Structure" || propertyName == null)
                {
                    this.ValidateXml("Structure", this.DataViewModel.Structure);            
                    this.ValidateStructure(this.DataViewModel);
                }

                if (propertyName == "InstallInstructions" || propertyName == null)
                {
                    this.ValidateXml("InstallInstructions", this.DataViewModel.InstallInstructions);            
                    this.ValidateInstallInstructions(this.DataViewModel);
                }

                if (propertyName == "DynamicContent" || propertyName == null)
                {
                    this.ValidateXml("DynamicContent", this.DataViewModel.DynamicContent);            
                    this.ValidateDynamicContent(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class UpdateCommandValidator : EntityValidatorBase
        {
            public UpdateCommandValidator(UpdateCommandDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UpdateCommandDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUpdateIndex(UpdateCommandDataViewModel dvm);
            
            partial void ValidateUnit(UpdateCommandDataViewModel dvm);
            
            partial void ValidateCommand(UpdateCommandDataViewModel dvm);
            
            partial void ValidateWasTransferred(UpdateCommandDataViewModel dvm);
            
            partial void ValidateWasInstalled(UpdateCommandDataViewModel dvm);
            
            partial void Validate(string propertyName, UpdateCommandDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UpdateIndex" || propertyName == null)
                {            
                    this.ValidateUpdateIndex(this.DataViewModel);
                }

                if (propertyName == "Unit" || propertyName == null)
                {            
                    this.ValidateUnit(this.DataViewModel);
                }

                if (propertyName == "Command" || propertyName == null)
                {
                    this.ValidateXml("Command", this.DataViewModel.Command);            
                    this.ValidateCommand(this.DataViewModel);
                }

                if (propertyName == "WasTransferred" || propertyName == null)
                {            
                    this.ValidateWasTransferred(this.DataViewModel);
                }

                if (propertyName == "WasInstalled" || propertyName == null)
                {            
                    this.ValidateWasInstalled(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class UpdateFeedbackValidator : EntityValidatorBase
        {
            public UpdateFeedbackValidator(UpdateFeedbackDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UpdateFeedbackDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateUpdateCommand(UpdateFeedbackDataViewModel dvm);
            
            partial void ValidateTimestamp(UpdateFeedbackDataViewModel dvm);
            
            partial void ValidateState(UpdateFeedbackDataViewModel dvm);
            
            partial void ValidateFeedback(UpdateFeedbackDataViewModel dvm);
            
            partial void Validate(string propertyName, UpdateFeedbackDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "UpdateCommand" || propertyName == null)
                {            
                    this.ValidateUpdateCommand(this.DataViewModel);
                }

                if (propertyName == "Timestamp" || propertyName == null)
                {            
                    this.ValidateTimestamp(this.DataViewModel);
                }

                if (propertyName == "State" || propertyName == null)
                {            
                    this.ValidateState(this.DataViewModel);
                }

                if (propertyName == "Feedback" || propertyName == null)
                {
                    this.ValidateXml("Feedback", this.DataViewModel.Feedback);            
                    this.ValidateFeedback(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Documents
    {
        using Gorba.Center.Admin.Core.DataViewModels.Documents;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;
        
        public partial class DocumentValidator : EntityValidatorBase
        {
            public DocumentValidator(DocumentDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public DocumentDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateName(DocumentDataViewModel dvm);
            
            partial void ValidateDescription(DocumentDataViewModel dvm);
            
            partial void ValidateTenant(DocumentDataViewModel dvm);
            
            partial void Validate(string propertyName, DocumentDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }

                if (propertyName == "Tenant" || propertyName == null)
                {            
                    this.ValidateTenant(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class DocumentVersionValidator : EntityValidatorBase
        {
            public DocumentVersionValidator(DocumentVersionDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public DocumentVersionDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateDocument(DocumentVersionDataViewModel dvm);
            
            partial void ValidateCreatingUser(DocumentVersionDataViewModel dvm);
            
            partial void ValidateMajor(DocumentVersionDataViewModel dvm);
            
            partial void ValidateMinor(DocumentVersionDataViewModel dvm);
            
            partial void ValidateContent(DocumentVersionDataViewModel dvm);
            
            partial void ValidateDescription(DocumentVersionDataViewModel dvm);
            
            partial void Validate(string propertyName, DocumentVersionDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Document" || propertyName == null)
                {            
                    this.ValidateDocument(this.DataViewModel);
                }

                if (propertyName == "CreatingUser" || propertyName == null)
                {            
                    this.ValidateCreatingUser(this.DataViewModel);
                }

                if (propertyName == "Major" || propertyName == null)
                {            
                    this.ValidateMajor(this.DataViewModel);
                }

                if (propertyName == "Minor" || propertyName == null)
                {            
                    this.ValidateMinor(this.DataViewModel);
                }

                if (propertyName == "Content" || propertyName == null)
                {
                    this.ValidateXml("Content", this.DataViewModel.Content);            
                    this.ValidateContent(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Software
    {
        using Gorba.Center.Admin.Core.DataViewModels.Software;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;
        
        public partial class PackageValidator : EntityValidatorBase
        {
            public PackageValidator(PackageDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public PackageDataViewModel DataViewModel { get; private set; }
            
            partial void ValidatePackageId(PackageDataViewModel dvm);
            
            partial void ValidateProductName(PackageDataViewModel dvm);
            
            partial void ValidateDescription(PackageDataViewModel dvm);
            
            partial void Validate(string propertyName, PackageDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "PackageId" || propertyName == null)
                {            
                    this.ValidatePackageId(this.DataViewModel);
                }

                if (propertyName == "ProductName" || propertyName == null)
                {            
                    this.ValidateProductName(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class PackageVersionValidator : EntityValidatorBase
        {
            public PackageVersionValidator(PackageVersionDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public PackageVersionDataViewModel DataViewModel { get; private set; }
            
            partial void ValidatePackage(PackageVersionDataViewModel dvm);
            
            partial void ValidateSoftwareVersion(PackageVersionDataViewModel dvm);
            
            partial void ValidateStructure(PackageVersionDataViewModel dvm);
            
            partial void ValidateDescription(PackageVersionDataViewModel dvm);
            
            partial void Validate(string propertyName, PackageVersionDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Package" || propertyName == null)
                {            
                    this.ValidatePackage(this.DataViewModel);
                }

                if (propertyName == "SoftwareVersion" || propertyName == null)
                {            
                    this.ValidateSoftwareVersion(this.DataViewModel);
                }

                if (propertyName == "Structure" || propertyName == null)
                {
                    this.ValidateXml("Structure", this.DataViewModel.Structure);            
                    this.ValidateStructure(this.DataViewModel);
                }

                if (propertyName == "Description" || propertyName == null)
                {            
                    this.ValidateDescription(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }

    namespace Configurations
    {
        using Gorba.Center.Admin.Core.DataViewModels.Configurations;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;
        
        public partial class UnitConfigurationValidator : EntityValidatorBase
        {
            public UnitConfigurationValidator(UnitConfigurationDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UnitConfigurationDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateDocument(UnitConfigurationDataViewModel dvm);
            
            partial void ValidateProductType(UnitConfigurationDataViewModel dvm);
            
            partial void Validate(string propertyName, UnitConfigurationDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Document" || propertyName == null)
                {            
                    this.ValidateDocument(this.DataViewModel);
                }

                if (propertyName == "ProductType" || propertyName == null)
                {            
                    this.ValidateProductType(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class MediaConfigurationValidator : EntityValidatorBase
        {
            public MediaConfigurationValidator(MediaConfigurationDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public MediaConfigurationDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateDocument(MediaConfigurationDataViewModel dvm);
            
            partial void Validate(string propertyName, MediaConfigurationDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "Document" || propertyName == null)
                {            
                    this.ValidateDocument(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
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
        
        public partial class UserDefinedPropertyValidator : EntityValidatorBase
        {
            public UserDefinedPropertyValidator(UserDefinedPropertyDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public UserDefinedPropertyDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateOwnerEntity(UserDefinedPropertyDataViewModel dvm);
            
            partial void ValidateTenant(UserDefinedPropertyDataViewModel dvm);
            
            partial void ValidateName(UserDefinedPropertyDataViewModel dvm);
            
            partial void Validate(string propertyName, UserDefinedPropertyDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "OwnerEntity" || propertyName == null)
                {            
                    this.ValidateOwnerEntity(this.DataViewModel);
                }

                if (propertyName == "Tenant" || propertyName == null)
                {            
                    this.ValidateTenant(this.DataViewModel);
                }

                if (propertyName == "Name" || propertyName == null)
                {            
                    this.ValidateName(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
        public partial class SystemConfigValidator : EntityValidatorBase
        {
            public SystemConfigValidator(SystemConfigDataViewModel dataViewModel, DataController dataController)
                : base(dataViewModel, dataController)
            {
                this.DataViewModel = dataViewModel;
            }

            public SystemConfigDataViewModel DataViewModel { get; private set; }
            
            partial void ValidateSystemId(SystemConfigDataViewModel dvm);
            
            partial void ValidateSettings(SystemConfigDataViewModel dvm);
            
            partial void Validate(string propertyName, SystemConfigDataViewModel dvm);
            
            protected override void HandleDataViewModelChange(string propertyName)
            {
                base.HandleDataViewModelChange(propertyName);

                if (propertyName == "SystemId" || propertyName == null)
                {            
                    this.ValidateSystemId(this.DataViewModel);
                }

                if (propertyName == "Settings" || propertyName == null)
                {
                    this.ValidateXml("Settings", this.DataViewModel.Settings);            
                    this.ValidateSettings(this.DataViewModel);
                }
                this.Validate(propertyName, this.DataViewModel);
            }
        }
        
    }
}