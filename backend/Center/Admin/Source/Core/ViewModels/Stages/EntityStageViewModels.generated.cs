namespace Gorba.Center.Admin.Core.ViewModels.Stages
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;
    
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    
    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;

    namespace AccessControl
    {
        using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
        
        public class UserRoleStageViewModel : EntityStageViewModelBase
        {
            private UserRoleReadOnlyDataViewModel selectedUserRole;

            private IReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel> allUserRoles;

            public UserRoleStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UserRole";
                this.PluralDisplayName = "User Roles";
                this.SingularDisplayName = "User Role";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel> UserRoles
            {
                get
                {
                    return this.allUserRoles;
                }

                set
                {
                    var old = this.allUserRoles;
                    if (!this.SetProperty(ref this.allUserRoles, value, () => this.UserRoles))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UserRoleReadOnlyDataViewModel SelectedUserRole
            {
                get
                {
                    return this.selectedUserRole;
                }

                set
                {
                    var model = this.UserRoles.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUserRole, model, () => this.SelectedUserRole))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UserRoles;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUserRole;
                }
                
                set
                {
                    this.SelectedUserRole = (UserRoleReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUserRoles != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUserRoles.IsLoading;
                }
            }
        }
        
        public class AuthorizationStageViewModel : EntityStageViewModelBase
        {
            private AuthorizationReadOnlyDataViewModel selectedAuthorization;

            private IReadOnlyEntityCollection<AuthorizationReadOnlyDataViewModel> allAuthorizations;

            public AuthorizationStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Authorization";
                this.PluralDisplayName = "Authorizations";
                this.SingularDisplayName = "Authorization";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<AuthorizationReadOnlyDataViewModel> Authorizations
            {
                get
                {
                    return this.allAuthorizations;
                }

                set
                {
                    var old = this.allAuthorizations;
                    if (!this.SetProperty(ref this.allAuthorizations, value, () => this.Authorizations))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public AuthorizationReadOnlyDataViewModel SelectedAuthorization
            {
                get
                {
                    return this.selectedAuthorization;
                }

                set
                {
                    var model = this.Authorizations.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedAuthorization, model, () => this.SelectedAuthorization))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Authorizations;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedAuthorization;
                }
                
                set
                {
                    this.SelectedAuthorization = (AuthorizationReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allAuthorizations != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allAuthorizations.IsLoading;
                }
            }
        }
        
    }

    namespace Membership
    {
        using Gorba.Center.Admin.Core.DataViewModels.Membership;
        
        public class TenantStageViewModel : EntityStageViewModelBase
        {
            private TenantReadOnlyDataViewModel selectedTenant;

            private IReadOnlyEntityCollection<TenantReadOnlyDataViewModel> allTenants;

            public TenantStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Tenant";
                this.PluralDisplayName = "Tenants";
                this.SingularDisplayName = "Tenant";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<TenantReadOnlyDataViewModel> Tenants
            {
                get
                {
                    return this.allTenants;
                }

                set
                {
                    var old = this.allTenants;
                    if (!this.SetProperty(ref this.allTenants, value, () => this.Tenants))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public TenantReadOnlyDataViewModel SelectedTenant
            {
                get
                {
                    return this.selectedTenant;
                }

                set
                {
                    var model = this.Tenants.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedTenant, model, () => this.SelectedTenant))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Tenants;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedTenant;
                }
                
                set
                {
                    this.SelectedTenant = (TenantReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allTenants != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allTenants.IsLoading;
                }
            }
        }
        
        public class UserStageViewModel : EntityStageViewModelBase
        {
            private UserReadOnlyDataViewModel selectedUser;

            private IReadOnlyEntityCollection<UserReadOnlyDataViewModel> allUsers;

            public UserStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "User";
                this.PluralDisplayName = "Users";
                this.SingularDisplayName = "User";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UserReadOnlyDataViewModel> Users
            {
                get
                {
                    return this.allUsers;
                }

                set
                {
                    var old = this.allUsers;
                    if (!this.SetProperty(ref this.allUsers, value, () => this.Users))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UserReadOnlyDataViewModel SelectedUser
            {
                get
                {
                    return this.selectedUser;
                }

                set
                {
                    var model = this.Users.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUser, model, () => this.SelectedUser))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Users;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUser;
                }
                
                set
                {
                    this.SelectedUser = (UserReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUsers != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUsers.IsLoading;
                }
            }
        }
        
        public class AssociationTenantUserUserRoleStageViewModel : EntityStageViewModelBase
        {
            private AssociationTenantUserUserRoleReadOnlyDataViewModel selectedAssociationTenantUserUserRole;

            private IReadOnlyEntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> allAssociationTenantUserUserRoles;

            public AssociationTenantUserUserRoleStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "AssociationTenantUserUserRole";
                this.PluralDisplayName = "Association Tenant User User Roles";
                this.SingularDisplayName = "Association Tenant User User Role";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> AssociationTenantUserUserRoles
            {
                get
                {
                    return this.allAssociationTenantUserUserRoles;
                }

                set
                {
                    var old = this.allAssociationTenantUserUserRoles;
                    if (!this.SetProperty(ref this.allAssociationTenantUserUserRoles, value, () => this.AssociationTenantUserUserRoles))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public AssociationTenantUserUserRoleReadOnlyDataViewModel SelectedAssociationTenantUserUserRole
            {
                get
                {
                    return this.selectedAssociationTenantUserUserRole;
                }

                set
                {
                    var model = this.AssociationTenantUserUserRoles.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedAssociationTenantUserUserRole, model, () => this.SelectedAssociationTenantUserUserRole))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.AssociationTenantUserUserRoles;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedAssociationTenantUserUserRole;
                }
                
                set
                {
                    this.SelectedAssociationTenantUserUserRole = (AssociationTenantUserUserRoleReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allAssociationTenantUserUserRoles != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allAssociationTenantUserUserRoles.IsLoading;
                }
            }
        }
        
    }

    namespace Units
    {
        using Gorba.Center.Admin.Core.DataViewModels.Units;
        
        public class ProductTypeStageViewModel : EntityStageViewModelBase
        {
            private ProductTypeReadOnlyDataViewModel selectedProductType;

            private IReadOnlyEntityCollection<ProductTypeReadOnlyDataViewModel> allProductTypes;

            public ProductTypeStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "ProductType";
                this.PluralDisplayName = "Product Types";
                this.SingularDisplayName = "Product Type";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<ProductTypeReadOnlyDataViewModel> ProductTypes
            {
                get
                {
                    return this.allProductTypes;
                }

                set
                {
                    var old = this.allProductTypes;
                    if (!this.SetProperty(ref this.allProductTypes, value, () => this.ProductTypes))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public ProductTypeReadOnlyDataViewModel SelectedProductType
            {
                get
                {
                    return this.selectedProductType;
                }

                set
                {
                    var model = this.ProductTypes.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedProductType, model, () => this.SelectedProductType))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.ProductTypes;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedProductType;
                }
                
                set
                {
                    this.SelectedProductType = (ProductTypeReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allProductTypes != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allProductTypes.IsLoading;
                }
            }
        }
        
        public class UnitStageViewModel : EntityStageViewModelBase
        {
            private UnitReadOnlyDataViewModel selectedUnit;

            private IReadOnlyEntityCollection<UnitReadOnlyDataViewModel> allUnits;

            public UnitStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Unit";
                this.PluralDisplayName = "Units";
                this.SingularDisplayName = "Unit";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UnitReadOnlyDataViewModel> Units
            {
                get
                {
                    return this.allUnits;
                }

                set
                {
                    var old = this.allUnits;
                    if (!this.SetProperty(ref this.allUnits, value, () => this.Units))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UnitReadOnlyDataViewModel SelectedUnit
            {
                get
                {
                    return this.selectedUnit;
                }

                set
                {
                    var model = this.Units.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUnit, model, () => this.SelectedUnit))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Units;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUnit;
                }
                
                set
                {
                    this.SelectedUnit = (UnitReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUnits != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUnits.IsLoading;
                }
            }
        }
        
    }

    namespace Resources
    {
        using Gorba.Center.Admin.Core.DataViewModels.Resources;
        
        public class ResourceStageViewModel : EntityStageViewModelBase
        {
            private ResourceReadOnlyDataViewModel selectedResource;

            private IReadOnlyEntityCollection<ResourceReadOnlyDataViewModel> allResources;

            public ResourceStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Resource";
                this.PluralDisplayName = "Resources";
                this.SingularDisplayName = "Resource";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<ResourceReadOnlyDataViewModel> Resources
            {
                get
                {
                    return this.allResources;
                }

                set
                {
                    var old = this.allResources;
                    if (!this.SetProperty(ref this.allResources, value, () => this.Resources))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public ResourceReadOnlyDataViewModel SelectedResource
            {
                get
                {
                    return this.selectedResource;
                }

                set
                {
                    var model = this.Resources.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedResource, model, () => this.SelectedResource))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Resources;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedResource;
                }
                
                set
                {
                    this.SelectedResource = (ResourceReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allResources != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allResources.IsLoading;
                }
            }
        }
        
    }

    namespace Update
    {
        using Gorba.Center.Admin.Core.DataViewModels.Update;
        
        public class UpdateGroupStageViewModel : EntityStageViewModelBase
        {
            private UpdateGroupReadOnlyDataViewModel selectedUpdateGroup;

            private IReadOnlyEntityCollection<UpdateGroupReadOnlyDataViewModel> allUpdateGroups;

            public UpdateGroupStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UpdateGroup";
                this.PluralDisplayName = "Update Groups";
                this.SingularDisplayName = "Update Group";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UpdateGroupReadOnlyDataViewModel> UpdateGroups
            {
                get
                {
                    return this.allUpdateGroups;
                }

                set
                {
                    var old = this.allUpdateGroups;
                    if (!this.SetProperty(ref this.allUpdateGroups, value, () => this.UpdateGroups))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UpdateGroupReadOnlyDataViewModel SelectedUpdateGroup
            {
                get
                {
                    return this.selectedUpdateGroup;
                }

                set
                {
                    var model = this.UpdateGroups.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUpdateGroup, model, () => this.SelectedUpdateGroup))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UpdateGroups;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUpdateGroup;
                }
                
                set
                {
                    this.SelectedUpdateGroup = (UpdateGroupReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUpdateGroups != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUpdateGroups.IsLoading;
                }
            }
        }
        
        public class UpdatePartStageViewModel : EntityStageViewModelBase
        {
            private UpdatePartReadOnlyDataViewModel selectedUpdatePart;

            private IReadOnlyEntityCollection<UpdatePartReadOnlyDataViewModel> allUpdateParts;

            public UpdatePartStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UpdatePart";
                this.PluralDisplayName = "Update Parts";
                this.SingularDisplayName = "Update Part";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UpdatePartReadOnlyDataViewModel> UpdateParts
            {
                get
                {
                    return this.allUpdateParts;
                }

                set
                {
                    var old = this.allUpdateParts;
                    if (!this.SetProperty(ref this.allUpdateParts, value, () => this.UpdateParts))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UpdatePartReadOnlyDataViewModel SelectedUpdatePart
            {
                get
                {
                    return this.selectedUpdatePart;
                }

                set
                {
                    var model = this.UpdateParts.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUpdatePart, model, () => this.SelectedUpdatePart))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UpdateParts;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUpdatePart;
                }
                
                set
                {
                    this.SelectedUpdatePart = (UpdatePartReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUpdateParts != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUpdateParts.IsLoading;
                }
            }
        }
        
        public class UpdateCommandStageViewModel : EntityStageViewModelBase
        {
            private UpdateCommandReadOnlyDataViewModel selectedUpdateCommand;

            private IReadOnlyEntityCollection<UpdateCommandReadOnlyDataViewModel> allUpdateCommands;

            public UpdateCommandStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UpdateCommand";
                this.PluralDisplayName = "Update Commands";
                this.SingularDisplayName = "Update Command";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UpdateCommandReadOnlyDataViewModel> UpdateCommands
            {
                get
                {
                    return this.allUpdateCommands;
                }

                set
                {
                    var old = this.allUpdateCommands;
                    if (!this.SetProperty(ref this.allUpdateCommands, value, () => this.UpdateCommands))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UpdateCommandReadOnlyDataViewModel SelectedUpdateCommand
            {
                get
                {
                    return this.selectedUpdateCommand;
                }

                set
                {
                    var model = this.UpdateCommands.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUpdateCommand, model, () => this.SelectedUpdateCommand))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UpdateCommands;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUpdateCommand;
                }
                
                set
                {
                    this.SelectedUpdateCommand = (UpdateCommandReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUpdateCommands != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUpdateCommands.IsLoading;
                }
            }
        }
        
        public class UpdateFeedbackStageViewModel : EntityStageViewModelBase
        {
            private UpdateFeedbackReadOnlyDataViewModel selectedUpdateFeedback;

            private IReadOnlyEntityCollection<UpdateFeedbackReadOnlyDataViewModel> allUpdateFeedbacks;

            public UpdateFeedbackStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UpdateFeedback";
                this.PluralDisplayName = "Update Feedbacks";
                this.SingularDisplayName = "Update Feedback";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UpdateFeedbackReadOnlyDataViewModel> UpdateFeedbacks
            {
                get
                {
                    return this.allUpdateFeedbacks;
                }

                set
                {
                    var old = this.allUpdateFeedbacks;
                    if (!this.SetProperty(ref this.allUpdateFeedbacks, value, () => this.UpdateFeedbacks))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UpdateFeedbackReadOnlyDataViewModel SelectedUpdateFeedback
            {
                get
                {
                    return this.selectedUpdateFeedback;
                }

                set
                {
                    var model = this.UpdateFeedbacks.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUpdateFeedback, model, () => this.SelectedUpdateFeedback))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UpdateFeedbacks;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUpdateFeedback;
                }
                
                set
                {
                    this.SelectedUpdateFeedback = (UpdateFeedbackReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUpdateFeedbacks != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUpdateFeedbacks.IsLoading;
                }
            }
        }
        
    }

    namespace Documents
    {
        using Gorba.Center.Admin.Core.DataViewModels.Documents;
        
        public class DocumentStageViewModel : EntityStageViewModelBase
        {
            private DocumentReadOnlyDataViewModel selectedDocument;

            private IReadOnlyEntityCollection<DocumentReadOnlyDataViewModel> allDocuments;

            public DocumentStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Document";
                this.PluralDisplayName = "Documents";
                this.SingularDisplayName = "Document";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<DocumentReadOnlyDataViewModel> Documents
            {
                get
                {
                    return this.allDocuments;
                }

                set
                {
                    var old = this.allDocuments;
                    if (!this.SetProperty(ref this.allDocuments, value, () => this.Documents))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public DocumentReadOnlyDataViewModel SelectedDocument
            {
                get
                {
                    return this.selectedDocument;
                }

                set
                {
                    var model = this.Documents.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedDocument, model, () => this.SelectedDocument))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Documents;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedDocument;
                }
                
                set
                {
                    this.SelectedDocument = (DocumentReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allDocuments != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allDocuments.IsLoading;
                }
            }
        }
        
        public class DocumentVersionStageViewModel : EntityStageViewModelBase
        {
            private DocumentVersionReadOnlyDataViewModel selectedDocumentVersion;

            private IReadOnlyEntityCollection<DocumentVersionReadOnlyDataViewModel> allDocumentVersions;

            public DocumentVersionStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "DocumentVersion";
                this.PluralDisplayName = "Document Versions";
                this.SingularDisplayName = "Document Version";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<DocumentVersionReadOnlyDataViewModel> DocumentVersions
            {
                get
                {
                    return this.allDocumentVersions;
                }

                set
                {
                    var old = this.allDocumentVersions;
                    if (!this.SetProperty(ref this.allDocumentVersions, value, () => this.DocumentVersions))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public DocumentVersionReadOnlyDataViewModel SelectedDocumentVersion
            {
                get
                {
                    return this.selectedDocumentVersion;
                }

                set
                {
                    var model = this.DocumentVersions.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedDocumentVersion, model, () => this.SelectedDocumentVersion))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.DocumentVersions;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedDocumentVersion;
                }
                
                set
                {
                    this.SelectedDocumentVersion = (DocumentVersionReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allDocumentVersions != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allDocumentVersions.IsLoading;
                }
            }
        }
        
    }

    namespace Software
    {
        using Gorba.Center.Admin.Core.DataViewModels.Software;
        
        public class PackageStageViewModel : EntityStageViewModelBase
        {
            private PackageReadOnlyDataViewModel selectedPackage;

            private IReadOnlyEntityCollection<PackageReadOnlyDataViewModel> allPackages;

            public PackageStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "Package";
                this.PluralDisplayName = "Packages";
                this.SingularDisplayName = "Package";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<PackageReadOnlyDataViewModel> Packages
            {
                get
                {
                    return this.allPackages;
                }

                set
                {
                    var old = this.allPackages;
                    if (!this.SetProperty(ref this.allPackages, value, () => this.Packages))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public PackageReadOnlyDataViewModel SelectedPackage
            {
                get
                {
                    return this.selectedPackage;
                }

                set
                {
                    var model = this.Packages.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedPackage, model, () => this.SelectedPackage))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.Packages;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedPackage;
                }
                
                set
                {
                    this.SelectedPackage = (PackageReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allPackages != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allPackages.IsLoading;
                }
            }
        }
        
        public class PackageVersionStageViewModel : EntityStageViewModelBase
        {
            private PackageVersionReadOnlyDataViewModel selectedPackageVersion;

            private IReadOnlyEntityCollection<PackageVersionReadOnlyDataViewModel> allPackageVersions;

            public PackageVersionStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "PackageVersion";
                this.PluralDisplayName = "Package Versions";
                this.SingularDisplayName = "Package Version";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<PackageVersionReadOnlyDataViewModel> PackageVersions
            {
                get
                {
                    return this.allPackageVersions;
                }

                set
                {
                    var old = this.allPackageVersions;
                    if (!this.SetProperty(ref this.allPackageVersions, value, () => this.PackageVersions))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public PackageVersionReadOnlyDataViewModel SelectedPackageVersion
            {
                get
                {
                    return this.selectedPackageVersion;
                }

                set
                {
                    var model = this.PackageVersions.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedPackageVersion, model, () => this.SelectedPackageVersion))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.PackageVersions;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedPackageVersion;
                }
                
                set
                {
                    this.SelectedPackageVersion = (PackageVersionReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allPackageVersions != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allPackageVersions.IsLoading;
                }
            }
        }
        
    }

    namespace Configurations
    {
        using Gorba.Center.Admin.Core.DataViewModels.Configurations;
        
        public class UnitConfigurationStageViewModel : EntityStageViewModelBase
        {
            private UnitConfigurationReadOnlyDataViewModel selectedUnitConfiguration;

            private IReadOnlyEntityCollection<UnitConfigurationReadOnlyDataViewModel> allUnitConfigurations;

            public UnitConfigurationStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UnitConfiguration";
                this.PluralDisplayName = "Unit Configurations";
                this.SingularDisplayName = "Unit Configuration";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UnitConfigurationReadOnlyDataViewModel> UnitConfigurations
            {
                get
                {
                    return this.allUnitConfigurations;
                }

                set
                {
                    var old = this.allUnitConfigurations;
                    if (!this.SetProperty(ref this.allUnitConfigurations, value, () => this.UnitConfigurations))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UnitConfigurationReadOnlyDataViewModel SelectedUnitConfiguration
            {
                get
                {
                    return this.selectedUnitConfiguration;
                }

                set
                {
                    var model = this.UnitConfigurations.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUnitConfiguration, model, () => this.SelectedUnitConfiguration))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UnitConfigurations;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUnitConfiguration;
                }
                
                set
                {
                    this.SelectedUnitConfiguration = (UnitConfigurationReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUnitConfigurations != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUnitConfigurations.IsLoading;
                }
            }
        }
        
        public class MediaConfigurationStageViewModel : EntityStageViewModelBase
        {
            private MediaConfigurationReadOnlyDataViewModel selectedMediaConfiguration;

            private IReadOnlyEntityCollection<MediaConfigurationReadOnlyDataViewModel> allMediaConfigurations;

            public MediaConfigurationStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "MediaConfiguration";
                this.PluralDisplayName = "Media Configurations";
                this.SingularDisplayName = "Media Configuration";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<MediaConfigurationReadOnlyDataViewModel> MediaConfigurations
            {
                get
                {
                    return this.allMediaConfigurations;
                }

                set
                {
                    var old = this.allMediaConfigurations;
                    if (!this.SetProperty(ref this.allMediaConfigurations, value, () => this.MediaConfigurations))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public MediaConfigurationReadOnlyDataViewModel SelectedMediaConfiguration
            {
                get
                {
                    return this.selectedMediaConfiguration;
                }

                set
                {
                    var model = this.MediaConfigurations.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedMediaConfiguration, model, () => this.SelectedMediaConfiguration))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.MediaConfigurations;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedMediaConfiguration;
                }
                
                set
                {
                    this.SelectedMediaConfiguration = (MediaConfigurationReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return true;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allMediaConfigurations != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allMediaConfigurations.IsLoading;
                }
            }
        }
        
    }

    namespace Log
    {
        using Gorba.Center.Admin.Core.DataViewModels.Log;
        
    }

    namespace Meta
    {
        using Gorba.Center.Admin.Core.DataViewModels.Meta;
        
        public class UserDefinedPropertyStageViewModel : EntityStageViewModelBase
        {
            private UserDefinedPropertyReadOnlyDataViewModel selectedUserDefinedProperty;

            private IReadOnlyEntityCollection<UserDefinedPropertyReadOnlyDataViewModel> allUserDefinedProperties;

            public UserDefinedPropertyStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "UserDefinedProperty";
                this.PluralDisplayName = "User Defined Properties";
                this.SingularDisplayName = "User Defined Property";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<UserDefinedPropertyReadOnlyDataViewModel> UserDefinedProperties
            {
                get
                {
                    return this.allUserDefinedProperties;
                }

                set
                {
                    var old = this.allUserDefinedProperties;
                    if (!this.SetProperty(ref this.allUserDefinedProperties, value, () => this.UserDefinedProperties))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public UserDefinedPropertyReadOnlyDataViewModel SelectedUserDefinedProperty
            {
                get
                {
                    return this.selectedUserDefinedProperty;
                }

                set
                {
                    var model = this.UserDefinedProperties.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedUserDefinedProperty, model, () => this.SelectedUserDefinedProperty))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.UserDefinedProperties;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedUserDefinedProperty;
                }
                
                set
                {
                    this.SelectedUserDefinedProperty = (UserDefinedPropertyReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allUserDefinedProperties != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allUserDefinedProperties.IsLoading;
                }
            }
        }
        
        public class SystemConfigStageViewModel : EntityStageViewModelBase
        {
            private SystemConfigReadOnlyDataViewModel selectedSystemConfig;

            private IReadOnlyEntityCollection<SystemConfigReadOnlyDataViewModel> allSystemConfigs;

            public SystemConfigStageViewModel(ICommandRegistry commandRegistry)
                : base(commandRegistry)
            {
                this.Name = this.EntityName = "SystemConfig";
                this.PluralDisplayName = "System Configs";
                this.SingularDisplayName = "System Config";
                
                this.IsLoading = true;
            }

            public IReadOnlyEntityCollection<SystemConfigReadOnlyDataViewModel> SystemConfigs
            {
                get
                {
                    return this.allSystemConfigs;
                }

                set
                {
                    var old = this.allSystemConfigs;
                    if (!this.SetProperty(ref this.allSystemConfigs, value, () => this.SystemConfigs))
                    {
                        return;
                    }

                    if (old != null)
                    {
                        old.PropertyChanged -= this.AllOnPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.AllOnPropertyChanged;
                        this.IsLoading = value.IsLoading;
                    }
                    else
                    {
                        this.IsLoading = true;
                    }

                    this.RaisePropertyChanged(() => this.Instances);
                }
            }
            
            public SystemConfigReadOnlyDataViewModel SelectedSystemConfig
            {
                get
                {
                    return this.selectedSystemConfig;
                }

                set
                {
                    var model = this.SystemConfigs.FirstOrDefault(m => m.Equals(value));
                    if (this.SetProperty(ref this.selectedSystemConfig, model, () => this.SelectedSystemConfig))
                    {
                        this.RaisePropertyChanged(() => this.SelectedInstance);
                    }
                }
            }

            public override IList Instances
            {
                get
                {
                    return this.SystemConfigs;
                }
            }
            
            public override ReadOnlyDataViewModelBase SelectedInstance
            {
                get
                {
                    return this.SelectedSystemConfig;
                }
                
                set
                {
                    this.SelectedSystemConfig = (SystemConfigReadOnlyDataViewModel)value;
                }
            }

            public override bool HasDetails
            {
                get
                {
                    return false;
                }
            }

            private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (this.allSystemConfigs != null && e.PropertyName == "IsLoading")
                {
                    this.IsLoading = this.allSystemConfigs.IsLoading;
                }
            }
        }
        
    }
}