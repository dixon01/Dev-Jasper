namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        
        
        public partial class UserRoleReadOnlyDataViewModel : ReadOnlyDataViewModelWithUdpBase
        {
            public UserRoleReadOnlyDataViewModel(
                UserRoleReadableModel readableModel, IUserRoleUdpContext udpContext, DataViewModelFactory factory)
                : base(factory, udpContext != null ? udpContext.GetAdditionalUserRoleProperties() : null)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Authorizations = new EntityCollection<AuthorizationReadOnlyDataViewModel>(
                    "Authorizations", "Authorization", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public EntityCollection<AuthorizationReadOnlyDataViewModel> Authorizations { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UserRoleReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UserRoleReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.ReadableModel.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }
            
            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UserRoleDataViewModel : DataViewModelWithUdpBase
        {
            public UserRoleDataViewModel(
                UserRoleReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory, readOnlyDataViewModel.GetUserDefinedPropertyNames())
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UserRoleDataViewModel(
                UserRoleWritableModel writableModel, IUserRoleUdpContext udpContext, DataViewModelFactory factory)
                : base(null, factory, udpContext != null ? udpContext.GetAdditionalUserRoleProperties() : null)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public UserRoleWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UserRoleDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.Model.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }

            protected override void SetUserDefinedPropertyValue(string name, string value)
            {
                this.Model.UserDefinedProperties[name] = value;
                this.RaisePropertyChanged(name);
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UserRoleWritableModel writableModel)
            {
                this.Model = writableModel;
            }
        }

        public partial class AuthorizationReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private UserRoleReadOnlyDataViewModel referenceUserRole;

            public AuthorizationReadOnlyDataViewModel(
                AuthorizationReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public DataScope DataScope
            {
                get
                {
                    return this.ReadableModel.DataScope;
                }
            }

            public Permission Permission
            {
                get
                {
                    return this.ReadableModel.Permission;
                }
            }

            public string UserRoleDisplayText
            {
                get
                {
                    var reference = this.UserRole;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public UserRoleReadOnlyDataViewModel UserRole
            {
                get
                {
                    var currentId = this.referenceUserRole == null ? 0 : this.referenceUserRole.Id;
                    var modelId = this.ReadableModel.UserRole == null ? 0 : this.ReadableModel.UserRole.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUserRole =
                            this.ReadableModel.UserRole == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UserRole);
                    }

                    return this.referenceUserRole;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.DataScope);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public AuthorizationReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as AuthorizationReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "DataScope")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class AuthorizationDataViewModel : DataViewModelBase
        {
            public AuthorizationDataViewModel(
                AuthorizationReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public AuthorizationDataViewModel(
                AuthorizationWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public DataScope DataScope
            {
                get
                {
                    return this.Model.DataScope;
                }

                set
                {
                    if (object.Equals(this.Model.DataScope, value))
                    {
                        return;
                    }

                    this.Model.DataScope = value;
                    this.RaisePropertyChanged(() => this.DataScope);
                }
            }

            public Permission Permission
            {
                get
                {
                    return this.Model.Permission;
                }

                set
                {
                    if (object.Equals(this.Model.Permission, value))
                    {
                        return;
                    }

                    this.Model.Permission = value;
                    this.RaisePropertyChanged(() => this.Permission);
                }
            }

            public EntityReference<UserRoleReadOnlyDataViewModel> UserRole { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.DataScope);
                }
            }

            public AuthorizationWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as AuthorizationDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(AuthorizationWritableModel writableModel)
            {
                this.Model = writableModel;

                this.UserRole = new EntityReference<UserRoleReadOnlyDataViewModel>();
                if (writableModel.UserRole != null)
                {
                    this.UserRole.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UserRole);
                }

                this.UserRole.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UserRole = this.UserRole.SelectedEntity == null
                                                         ? null
                                                         : this.UserRole.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UserRole");
                        }
                    };
            }
        }

    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        
        
        public partial class TenantReadOnlyDataViewModel : ReadOnlyDataViewModelWithUdpBase
        {
            public TenantReadOnlyDataViewModel(
                TenantReadableModel readableModel, ITenantUdpContext udpContext, DataViewModelFactory factory)
                : base(factory, udpContext != null ? udpContext.GetAdditionalTenantProperties() : null)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Users = new EntityCollection<UserReadOnlyDataViewModel>(
                    "Users", "User", this.Factory.CommandRegistry);
                this.UpdateGroups = new EntityCollection<Update.UpdateGroupReadOnlyDataViewModel>(
                    "UpdateGroups", "UpdateGroup", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public EntityCollection<UserReadOnlyDataViewModel> Users { get; private set; }

            public EntityCollection<Update.UpdateGroupReadOnlyDataViewModel> UpdateGroups { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public TenantReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as TenantReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.ReadableModel.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }
            
            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class TenantDataViewModel : DataViewModelWithUdpBase
        {
            public TenantDataViewModel(
                TenantReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory, readOnlyDataViewModel.GetUserDefinedPropertyNames())
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public TenantDataViewModel(
                TenantWritableModel writableModel, ITenantUdpContext udpContext, DataViewModelFactory factory)
                : base(null, factory, udpContext != null ? udpContext.GetAdditionalTenantProperties() : null)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public TenantWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as TenantDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.Model.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }

            protected override void SetUserDefinedPropertyValue(string name, string value)
            {
                this.Model.UserDefinedProperties[name] = value;
                this.RaisePropertyChanged(name);
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(TenantWritableModel writableModel)
            {
                this.Model = writableModel;
            }
        }

        public partial class UserReadOnlyDataViewModel : ReadOnlyDataViewModelWithUdpBase
        {
            private TenantReadOnlyDataViewModel referenceOwnerTenant;

            public UserReadOnlyDataViewModel(
                UserReadableModel readableModel, IUserUdpContext udpContext, DataViewModelFactory factory)
                : base(factory, udpContext != null ? udpContext.GetAdditionalUserProperties() : null)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.AssociationTenantUserUserRoles = new EntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel>(
                    "AssociationTenantUserUserRoles", "AssociationTenantUserUserRole", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Username
            {
                get
                {
                    return this.ReadableModel.Username;
                }
            }

            public string Domain
            {
                get
                {
                    return this.ReadableModel.Domain;
                }
            }

            public string HashedPassword
            {
                get
                {
                    return this.ReadableModel.HashedPassword;
                }
            }

            public string FirstName
            {
                get
                {
                    return this.ReadableModel.FirstName;
                }
            }

            public string LastName
            {
                get
                {
                    return this.ReadableModel.LastName;
                }
            }

            public string Email
            {
                get
                {
                    return this.ReadableModel.Email;
                }
            }

            public string Culture
            {
                get
                {
                    return this.ReadableModel.Culture;
                }
            }

            public string TimeZone
            {
                get
                {
                    return this.ReadableModel.TimeZone;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public DateTime? LastLoginAttempt
            {
                get
                {
                    return this.ReadableModel.LastLoginAttempt;
                }
            }

            public DateTime? LastSuccessfulLogin
            {
                get
                {
                    return this.ReadableModel.LastSuccessfulLogin;
                }
            }

            public int ConsecutiveLoginFailures
            {
                get
                {
                    return this.ReadableModel.ConsecutiveLoginFailures;
                }
            }

            public bool IsEnabled
            {
                get
                {
                    return this.ReadableModel.IsEnabled;
                }
            }

            public string OwnerTenantDisplayText
            {
                get
                {
                    var reference = this.OwnerTenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public TenantReadOnlyDataViewModel OwnerTenant
            {
                get
                {
                    var currentId = this.referenceOwnerTenant == null ? 0 : this.referenceOwnerTenant.Id;
                    var modelId = this.ReadableModel.OwnerTenant == null ? 0 : this.ReadableModel.OwnerTenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceOwnerTenant =
                            this.ReadableModel.OwnerTenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.OwnerTenant);
                    }

                    return this.referenceOwnerTenant;
                }
            }

            public EntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> AssociationTenantUserUserRoles { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Username);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UserReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UserReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.ReadableModel.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }
            
            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Username")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UserDataViewModel : DataViewModelWithUdpBase
        {
            public UserDataViewModel(
                UserReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory, readOnlyDataViewModel.GetUserDefinedPropertyNames())
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UserDataViewModel(
                UserWritableModel writableModel, IUserUdpContext udpContext, DataViewModelFactory factory)
                : base(null, factory, udpContext != null ? udpContext.GetAdditionalUserProperties() : null)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Username
            {
                get
                {
                    return this.Model.Username;
                }

                set
                {
                    if (object.Equals(this.Model.Username, value))
                    {
                        return;
                    }

                    this.Model.Username = value;
                    this.RaisePropertyChanged(() => this.Username);
                }
            }

            public string Domain
            {
                get
                {
                    return this.Model.Domain;
                }

                set
                {
                    if (object.Equals(this.Model.Domain, value))
                    {
                        return;
                    }

                    this.Model.Domain = value;
                    this.RaisePropertyChanged(() => this.Domain);
                }
            }

            public string HashedPassword
            {
                get
                {
                    return this.Model.HashedPassword;
                }

                set
                {
                    if (object.Equals(this.Model.HashedPassword, value))
                    {
                        return;
                    }

                    this.Model.HashedPassword = value;
                    this.RaisePropertyChanged(() => this.HashedPassword);
                }
            }

            public string FirstName
            {
                get
                {
                    return this.Model.FirstName;
                }

                set
                {
                    if (object.Equals(this.Model.FirstName, value))
                    {
                        return;
                    }

                    this.Model.FirstName = value;
                    this.RaisePropertyChanged(() => this.FirstName);
                }
            }

            public string LastName
            {
                get
                {
                    return this.Model.LastName;
                }

                set
                {
                    if (object.Equals(this.Model.LastName, value))
                    {
                        return;
                    }

                    this.Model.LastName = value;
                    this.RaisePropertyChanged(() => this.LastName);
                }
            }

            public string Email
            {
                get
                {
                    return this.Model.Email;
                }

                set
                {
                    if (object.Equals(this.Model.Email, value))
                    {
                        return;
                    }

                    this.Model.Email = value;
                    this.RaisePropertyChanged(() => this.Email);
                }
            }

            public string Culture
            {
                get
                {
                    return this.Model.Culture;
                }

                set
                {
                    if (object.Equals(this.Model.Culture, value))
                    {
                        return;
                    }

                    this.Model.Culture = value;
                    this.RaisePropertyChanged(() => this.Culture);
                }
            }

            public string TimeZone
            {
                get
                {
                    return this.Model.TimeZone;
                }

                set
                {
                    if (object.Equals(this.Model.TimeZone, value))
                    {
                        return;
                    }

                    this.Model.TimeZone = value;
                    this.RaisePropertyChanged(() => this.TimeZone);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public DateTime? LastLoginAttempt
            {
                get
                {
                    return this.Model.LastLoginAttempt;
                }

                set
                {
                    if (object.Equals(this.Model.LastLoginAttempt, value))
                    {
                        return;
                    }

                    this.Model.LastLoginAttempt = value;
                    this.RaisePropertyChanged(() => this.LastLoginAttempt);
                }
            }

            public DateTime? LastSuccessfulLogin
            {
                get
                {
                    return this.Model.LastSuccessfulLogin;
                }

                set
                {
                    if (object.Equals(this.Model.LastSuccessfulLogin, value))
                    {
                        return;
                    }

                    this.Model.LastSuccessfulLogin = value;
                    this.RaisePropertyChanged(() => this.LastSuccessfulLogin);
                }
            }

            public int ConsecutiveLoginFailures
            {
                get
                {
                    return this.Model.ConsecutiveLoginFailures;
                }

                set
                {
                    if (object.Equals(this.Model.ConsecutiveLoginFailures, value))
                    {
                        return;
                    }

                    this.Model.ConsecutiveLoginFailures = value;
                    this.RaisePropertyChanged(() => this.ConsecutiveLoginFailures);
                }
            }

            public bool IsEnabled
            {
                get
                {
                    return this.Model.IsEnabled;
                }

                set
                {
                    if (object.Equals(this.Model.IsEnabled, value))
                    {
                        return;
                    }

                    this.Model.IsEnabled = value;
                    this.RaisePropertyChanged(() => this.IsEnabled);
                }
            }

            public EntityReference<TenantReadOnlyDataViewModel> OwnerTenant { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Username);
                }
            }

            public UserWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UserDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.Model.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }

            protected override void SetUserDefinedPropertyValue(string name, string value)
            {
                this.Model.UserDefinedProperties[name] = value;
                this.RaisePropertyChanged(name);
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UserWritableModel writableModel)
            {
                this.Model = writableModel;

                this.OwnerTenant = new EntityReference<TenantReadOnlyDataViewModel>();
                if (writableModel.OwnerTenant != null)
                {
                    this.OwnerTenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.OwnerTenant);
                }

                this.OwnerTenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.OwnerTenant = this.OwnerTenant.SelectedEntity == null
                                                         ? null
                                                         : this.OwnerTenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("OwnerTenant");
                        }
                    };
            }
        }

        public partial class AssociationTenantUserUserRoleReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private TenantReadOnlyDataViewModel referenceTenant;

            private UserReadOnlyDataViewModel referenceUser;

            private AccessControl.UserRoleReadOnlyDataViewModel referenceUserRole;

            public AssociationTenantUserUserRoleReadOnlyDataViewModel(
                AssociationTenantUserUserRoleReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string TenantDisplayText
            {
                get
                {
                    var reference = this.Tenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public TenantReadOnlyDataViewModel Tenant
            {
                get
                {
                    var currentId = this.referenceTenant == null ? 0 : this.referenceTenant.Id;
                    var modelId = this.ReadableModel.Tenant == null ? 0 : this.ReadableModel.Tenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceTenant =
                            this.ReadableModel.Tenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Tenant);
                    }

                    return this.referenceTenant;
                }
            }

            public string UserDisplayText
            {
                get
                {
                    var reference = this.User;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public UserReadOnlyDataViewModel User
            {
                get
                {
                    var currentId = this.referenceUser == null ? 0 : this.referenceUser.Id;
                    var modelId = this.ReadableModel.User == null ? 0 : this.ReadableModel.User.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUser =
                            this.ReadableModel.User == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.User);
                    }

                    return this.referenceUser;
                }
            }

            public string UserRoleDisplayText
            {
                get
                {
                    var reference = this.UserRole;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public AccessControl.UserRoleReadOnlyDataViewModel UserRole
            {
                get
                {
                    var currentId = this.referenceUserRole == null ? 0 : this.referenceUserRole.Id;
                    var modelId = this.ReadableModel.UserRole == null ? 0 : this.ReadableModel.UserRole.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUserRole =
                            this.ReadableModel.UserRole == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UserRole);
                    }

                    return this.referenceUserRole;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.UserRole);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public AssociationTenantUserUserRoleReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as AssociationTenantUserUserRoleReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "UserRole")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class AssociationTenantUserUserRoleDataViewModel : DataViewModelBase
        {
            public AssociationTenantUserUserRoleDataViewModel(
                AssociationTenantUserUserRoleReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public AssociationTenantUserUserRoleDataViewModel(
                AssociationTenantUserUserRoleWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public EntityReference<TenantReadOnlyDataViewModel> Tenant { get; private set; }

            public EntityReference<UserReadOnlyDataViewModel> User { get; private set; }

            public EntityReference<AccessControl.UserRoleReadOnlyDataViewModel> UserRole { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.UserRole);
                }
            }

            public AssociationTenantUserUserRoleWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as AssociationTenantUserUserRoleDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(AssociationTenantUserUserRoleWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Tenant = new EntityReference<TenantReadOnlyDataViewModel>();
                if (writableModel.Tenant != null)
                {
                    this.Tenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Tenant);
                }

                this.Tenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Tenant = this.Tenant.SelectedEntity == null
                                                         ? null
                                                         : this.Tenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Tenant");
                        }
                    };

                this.User = new EntityReference<UserReadOnlyDataViewModel>();
                if (writableModel.User != null)
                {
                    this.User.SelectedEntity = this.Factory.CreateReadOnly(writableModel.User);
                }

                this.User.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.User = this.User.SelectedEntity == null
                                                         ? null
                                                         : this.User.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("User");
                        }
                    };

                this.UserRole = new EntityReference<AccessControl.UserRoleReadOnlyDataViewModel>();
                if (writableModel.UserRole != null)
                {
                    this.UserRole.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UserRole);
                }

                this.UserRole.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UserRole = this.UserRole.SelectedEntity == null
                                                         ? null
                                                         : this.UserRole.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UserRole");
                        }
                    };
            }
        }

    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        
        
        public partial class ProductTypeReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            public ProductTypeReadOnlyDataViewModel(
                ProductTypeReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Units = new EntityCollection<UnitReadOnlyDataViewModel>(
                    "Units", "Unit", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public UnitTypes UnitType
            {
                get
                {
                    return this.ReadableModel.UnitType;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public EntityCollection<UnitReadOnlyDataViewModel> Units { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public ProductTypeReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as ProductTypeReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class ProductTypeDataViewModel : DataViewModelBase
        {
            public ProductTypeDataViewModel(
                ProductTypeReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public ProductTypeDataViewModel(
                ProductTypeWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public UnitTypes UnitType
            {
                get
                {
                    return this.Model.UnitType;
                }

                set
                {
                    if (object.Equals(this.Model.UnitType, value))
                    {
                        return;
                    }

                    this.Model.UnitType = value;
                    this.RaisePropertyChanged(() => this.UnitType);
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public XmlDataViewModel HardwareDescriptor { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public ProductTypeWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as ProductTypeDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(ProductTypeWritableModel writableModel)
            {
                this.Model = writableModel;

                this.HardwareDescriptor =
                    new XmlDataViewModel(
                        () => this.Model.HardwareDescriptor,
                        value =>
                            {
                                if (object.Equals(this.Model.HardwareDescriptor, value))
                                {
                                    return;
                                }

                                this.Model.HardwareDescriptor = value;
                                this.RaisePropertyChanged(() => this.HardwareDescriptor);
                            });
            }
        }

        public partial class UnitReadOnlyDataViewModel : ReadOnlyDataViewModelWithUdpBase
        {
            private Membership.TenantReadOnlyDataViewModel referenceTenant;

            private ProductTypeReadOnlyDataViewModel referenceProductType;

            private Update.UpdateGroupReadOnlyDataViewModel referenceUpdateGroup;

            public UnitReadOnlyDataViewModel(
                UnitReadableModel readableModel, IUnitUdpContext udpContext, DataViewModelFactory factory)
                : base(factory, udpContext != null ? udpContext.GetAdditionalUnitProperties() : null)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.UpdateCommands = new EntityCollection<Update.UpdateCommandReadOnlyDataViewModel>(
                    "UpdateCommands", "UpdateCommand", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string NetworkAddress
            {
                get
                {
                    return this.ReadableModel.NetworkAddress;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public bool IsConnected
            {
                get
                {
                    return this.ReadableModel.IsConnected;
                }
            }

            public string TenantDisplayText
            {
                get
                {
                    var reference = this.Tenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.TenantReadOnlyDataViewModel Tenant
            {
                get
                {
                    var currentId = this.referenceTenant == null ? 0 : this.referenceTenant.Id;
                    var modelId = this.ReadableModel.Tenant == null ? 0 : this.ReadableModel.Tenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceTenant =
                            this.ReadableModel.Tenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Tenant);
                    }

                    return this.referenceTenant;
                }
            }

            public string ProductTypeDisplayText
            {
                get
                {
                    var reference = this.ProductType;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public ProductTypeReadOnlyDataViewModel ProductType
            {
                get
                {
                    var currentId = this.referenceProductType == null ? 0 : this.referenceProductType.Id;
                    var modelId = this.ReadableModel.ProductType == null ? 0 : this.ReadableModel.ProductType.Id;
                    if (currentId != modelId)
                    {
                        this.referenceProductType =
                            this.ReadableModel.ProductType == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.ProductType);
                    }

                    return this.referenceProductType;
                }
            }

            public string UpdateGroupDisplayText
            {
                get
                {
                    var reference = this.UpdateGroup;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Update.UpdateGroupReadOnlyDataViewModel UpdateGroup
            {
                get
                {
                    var currentId = this.referenceUpdateGroup == null ? 0 : this.referenceUpdateGroup.Id;
                    var modelId = this.ReadableModel.UpdateGroup == null ? 0 : this.ReadableModel.UpdateGroup.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUpdateGroup =
                            this.ReadableModel.UpdateGroup == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UpdateGroup);
                    }

                    return this.referenceUpdateGroup;
                }
            }

            public EntityCollection<Update.UpdateCommandReadOnlyDataViewModel> UpdateCommands { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UnitReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UnitReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.ReadableModel.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }
            
            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UnitDataViewModel : DataViewModelWithUdpBase
        {
            public UnitDataViewModel(
                UnitReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory, readOnlyDataViewModel.GetUserDefinedPropertyNames())
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UnitDataViewModel(
                UnitWritableModel writableModel, IUnitUdpContext udpContext, DataViewModelFactory factory)
                : base(null, factory, udpContext != null ? udpContext.GetAdditionalUnitProperties() : null)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string NetworkAddress
            {
                get
                {
                    return this.Model.NetworkAddress;
                }

                set
                {
                    if (object.Equals(this.Model.NetworkAddress, value))
                    {
                        return;
                    }

                    this.Model.NetworkAddress = value;
                    this.RaisePropertyChanged(() => this.NetworkAddress);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public bool IsConnected
            {
                get
                {
                    return this.Model.IsConnected;
                }

                set
                {
                    if (object.Equals(this.Model.IsConnected, value))
                    {
                        return;
                    }

                    this.Model.IsConnected = value;
                    this.RaisePropertyChanged(() => this.IsConnected);
                }
            }

            public EntityReference<Membership.TenantReadOnlyDataViewModel> Tenant { get; private set; }

            public EntityReference<ProductTypeReadOnlyDataViewModel> ProductType { get; private set; }

            public EntityReference<Update.UpdateGroupReadOnlyDataViewModel> UpdateGroup { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public UnitWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UnitDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.Model.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }

            protected override void SetUserDefinedPropertyValue(string name, string value)
            {
                this.Model.UserDefinedProperties[name] = value;
                this.RaisePropertyChanged(name);
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UnitWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Tenant = new EntityReference<Membership.TenantReadOnlyDataViewModel>();
                if (writableModel.Tenant != null)
                {
                    this.Tenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Tenant);
                }

                this.Tenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Tenant = this.Tenant.SelectedEntity == null
                                                         ? null
                                                         : this.Tenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Tenant");
                        }
                    };

                this.ProductType = new EntityReference<ProductTypeReadOnlyDataViewModel>();
                if (writableModel.ProductType != null)
                {
                    this.ProductType.SelectedEntity = this.Factory.CreateReadOnly(writableModel.ProductType);
                }

                this.ProductType.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.ProductType = this.ProductType.SelectedEntity == null
                                                         ? null
                                                         : this.ProductType.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("ProductType");
                        }
                    };

                this.UpdateGroup = new EntityReference<Update.UpdateGroupReadOnlyDataViewModel>();
                if (writableModel.UpdateGroup != null)
                {
                    this.UpdateGroup.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UpdateGroup);
                }

                this.UpdateGroup.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UpdateGroup = this.UpdateGroup.SelectedEntity == null
                                                         ? null
                                                         : this.UpdateGroup.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UpdateGroup");
                        }
                    };
            }
        }

    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        
        
        public partial class ResourceReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Membership.UserReadOnlyDataViewModel referenceUploadingUser;

            public ResourceReadOnlyDataViewModel(
                ResourceReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string OriginalFilename
            {
                get
                {
                    return this.ReadableModel.OriginalFilename;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string Hash
            {
                get
                {
                    return this.ReadableModel.Hash;
                }
            }

            public string ThumbnailHash
            {
                get
                {
                    return this.ReadableModel.ThumbnailHash;
                }
            }

            public string MimeType
            {
                get
                {
                    return this.ReadableModel.MimeType;
                }
            }

            public long Length
            {
                get
                {
                    return this.ReadableModel.Length;
                }
            }

            public string UploadingUserDisplayText
            {
                get
                {
                    var reference = this.UploadingUser;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.UserReadOnlyDataViewModel UploadingUser
            {
                get
                {
                    var currentId = this.referenceUploadingUser == null ? 0 : this.referenceUploadingUser.Id;
                    var modelId = this.ReadableModel.UploadingUser == null ? 0 : this.ReadableModel.UploadingUser.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUploadingUser =
                            this.ReadableModel.UploadingUser == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UploadingUser);
                    }

                    return this.referenceUploadingUser;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Hash);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public ResourceReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as ResourceReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Hash")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class ResourceDataViewModel : DataViewModelBase
        {
            public ResourceDataViewModel(
                ResourceReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public ResourceDataViewModel(
                ResourceWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string OriginalFilename
            {
                get
                {
                    return this.Model.OriginalFilename;
                }

                set
                {
                    if (object.Equals(this.Model.OriginalFilename, value))
                    {
                        return;
                    }

                    this.Model.OriginalFilename = value;
                    this.RaisePropertyChanged(() => this.OriginalFilename);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public string Hash
            {
                get
                {
                    return this.Model.Hash;
                }

                set
                {
                    if (object.Equals(this.Model.Hash, value))
                    {
                        return;
                    }

                    this.Model.Hash = value;
                    this.RaisePropertyChanged(() => this.Hash);
                }
            }

            public string ThumbnailHash
            {
                get
                {
                    return this.Model.ThumbnailHash;
                }

                set
                {
                    if (object.Equals(this.Model.ThumbnailHash, value))
                    {
                        return;
                    }

                    this.Model.ThumbnailHash = value;
                    this.RaisePropertyChanged(() => this.ThumbnailHash);
                }
            }

            public string MimeType
            {
                get
                {
                    return this.Model.MimeType;
                }

                set
                {
                    if (object.Equals(this.Model.MimeType, value))
                    {
                        return;
                    }

                    this.Model.MimeType = value;
                    this.RaisePropertyChanged(() => this.MimeType);
                }
            }

            public long Length
            {
                get
                {
                    return this.Model.Length;
                }

                set
                {
                    if (object.Equals(this.Model.Length, value))
                    {
                        return;
                    }

                    this.Model.Length = value;
                    this.RaisePropertyChanged(() => this.Length);
                }
            }

            public EntityReference<Membership.UserReadOnlyDataViewModel> UploadingUser { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Hash);
                }
            }

            public ResourceWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as ResourceDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(ResourceWritableModel writableModel)
            {
                this.Model = writableModel;

                this.UploadingUser = new EntityReference<Membership.UserReadOnlyDataViewModel>();
                if (writableModel.UploadingUser != null)
                {
                    this.UploadingUser.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UploadingUser);
                }

                this.UploadingUser.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UploadingUser = this.UploadingUser.SelectedEntity == null
                                                         ? null
                                                         : this.UploadingUser.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UploadingUser");
                        }
                    };
            }
        }

    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        
        
        public partial class UpdateGroupReadOnlyDataViewModel : ReadOnlyDataViewModelWithUdpBase
        {
            private Membership.TenantReadOnlyDataViewModel referenceTenant;

            private Configurations.UnitConfigurationReadOnlyDataViewModel referenceUnitConfiguration;

            private Configurations.MediaConfigurationReadOnlyDataViewModel referenceMediaConfiguration;

            public UpdateGroupReadOnlyDataViewModel(
                UpdateGroupReadableModel readableModel, IUpdateGroupUdpContext udpContext, DataViewModelFactory factory)
                : base(factory, udpContext != null ? udpContext.GetAdditionalUpdateGroupProperties() : null)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Units = new EntityCollection<Units.UnitReadOnlyDataViewModel>(
                    "Units", "Unit", this.Factory.CommandRegistry);
                this.UpdateParts = new EntityCollection<UpdatePartReadOnlyDataViewModel>(
                    "UpdateParts", "UpdatePart", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string TenantDisplayText
            {
                get
                {
                    var reference = this.Tenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.TenantReadOnlyDataViewModel Tenant
            {
                get
                {
                    var currentId = this.referenceTenant == null ? 0 : this.referenceTenant.Id;
                    var modelId = this.ReadableModel.Tenant == null ? 0 : this.ReadableModel.Tenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceTenant =
                            this.ReadableModel.Tenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Tenant);
                    }

                    return this.referenceTenant;
                }
            }

            public string UnitConfigurationDisplayText
            {
                get
                {
                    var reference = this.UnitConfiguration;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Configurations.UnitConfigurationReadOnlyDataViewModel UnitConfiguration
            {
                get
                {
                    var currentId = this.referenceUnitConfiguration == null ? 0 : this.referenceUnitConfiguration.Id;
                    var modelId = this.ReadableModel.UnitConfiguration == null ? 0 : this.ReadableModel.UnitConfiguration.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUnitConfiguration =
                            this.ReadableModel.UnitConfiguration == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UnitConfiguration);
                    }

                    return this.referenceUnitConfiguration;
                }
            }

            public string MediaConfigurationDisplayText
            {
                get
                {
                    var reference = this.MediaConfiguration;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Configurations.MediaConfigurationReadOnlyDataViewModel MediaConfiguration
            {
                get
                {
                    var currentId = this.referenceMediaConfiguration == null ? 0 : this.referenceMediaConfiguration.Id;
                    var modelId = this.ReadableModel.MediaConfiguration == null ? 0 : this.ReadableModel.MediaConfiguration.Id;
                    if (currentId != modelId)
                    {
                        this.referenceMediaConfiguration =
                            this.ReadableModel.MediaConfiguration == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.MediaConfiguration);
                    }

                    return this.referenceMediaConfiguration;
                }
            }

            public EntityCollection<Units.UnitReadOnlyDataViewModel> Units { get; private set; }

            public EntityCollection<UpdatePartReadOnlyDataViewModel> UpdateParts { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UpdateGroupReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateGroupReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.ReadableModel.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }
            
            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UpdateGroupDataViewModel : DataViewModelWithUdpBase
        {
            public UpdateGroupDataViewModel(
                UpdateGroupReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory, readOnlyDataViewModel.GetUserDefinedPropertyNames())
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UpdateGroupDataViewModel(
                UpdateGroupWritableModel writableModel, IUpdateGroupUdpContext udpContext, DataViewModelFactory factory)
                : base(null, factory, udpContext != null ? udpContext.GetAdditionalUpdateGroupProperties() : null)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public EntityReference<Membership.TenantReadOnlyDataViewModel> Tenant { get; private set; }

            public EntityReference<Configurations.UnitConfigurationReadOnlyDataViewModel> UnitConfiguration { get; private set; }

            public EntityReference<Configurations.MediaConfigurationReadOnlyDataViewModel> MediaConfiguration { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public UpdateGroupWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateGroupDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override string GetUserDefinedPropertyValue(string name)
            {
                string value;
                this.Model.UserDefinedProperties.TryGetValue(name, out value);
                return value;
            }

            protected override void SetUserDefinedPropertyValue(string name, string value)
            {
                this.Model.UserDefinedProperties[name] = value;
                this.RaisePropertyChanged(name);
            }
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UpdateGroupWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Tenant = new EntityReference<Membership.TenantReadOnlyDataViewModel>();
                if (writableModel.Tenant != null)
                {
                    this.Tenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Tenant);
                }

                this.Tenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Tenant = this.Tenant.SelectedEntity == null
                                                         ? null
                                                         : this.Tenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Tenant");
                        }
                    };

                this.UnitConfiguration = new EntityReference<Configurations.UnitConfigurationReadOnlyDataViewModel>();
                if (writableModel.UnitConfiguration != null)
                {
                    this.UnitConfiguration.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UnitConfiguration);
                }

                this.UnitConfiguration.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UnitConfiguration = this.UnitConfiguration.SelectedEntity == null
                                                         ? null
                                                         : this.UnitConfiguration.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UnitConfiguration");
                        }
                    };

                this.MediaConfiguration = new EntityReference<Configurations.MediaConfigurationReadOnlyDataViewModel>();
                if (writableModel.MediaConfiguration != null)
                {
                    this.MediaConfiguration.SelectedEntity = this.Factory.CreateReadOnly(writableModel.MediaConfiguration);
                }

                this.MediaConfiguration.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.MediaConfiguration = this.MediaConfiguration.SelectedEntity == null
                                                         ? null
                                                         : this.MediaConfiguration.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("MediaConfiguration");
                        }
                    };
            }
        }

        public partial class UpdatePartReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private UpdateGroupReadOnlyDataViewModel referenceUpdateGroup;

            public UpdatePartReadOnlyDataViewModel(
                UpdatePartReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.RelatedCommands = new EntityCollection<UpdateCommandReadOnlyDataViewModel>(
                    "RelatedCommands", "UpdateCommand", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public UpdatePartType Type
            {
                get
                {
                    return this.ReadableModel.Type;
                }
            }

            public DateTime Start
            {
                get
                {
                    return this.ReadableModel.Start;
                }
            }

            public DateTime End
            {
                get
                {
                    return this.ReadableModel.End;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string UpdateGroupDisplayText
            {
                get
                {
                    var reference = this.UpdateGroup;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public UpdateGroupReadOnlyDataViewModel UpdateGroup
            {
                get
                {
                    var currentId = this.referenceUpdateGroup == null ? 0 : this.referenceUpdateGroup.Id;
                    var modelId = this.ReadableModel.UpdateGroup == null ? 0 : this.ReadableModel.UpdateGroup.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUpdateGroup =
                            this.ReadableModel.UpdateGroup == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UpdateGroup);
                    }

                    return this.referenceUpdateGroup;
                }
            }

            public EntityCollection<UpdateCommandReadOnlyDataViewModel> RelatedCommands { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Type);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UpdatePartReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UpdatePartReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Type")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UpdatePartDataViewModel : DataViewModelBase
        {
            public UpdatePartDataViewModel(
                UpdatePartReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UpdatePartDataViewModel(
                UpdatePartWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public UpdatePartType Type
            {
                get
                {
                    return this.Model.Type;
                }

                set
                {
                    if (object.Equals(this.Model.Type, value))
                    {
                        return;
                    }

                    this.Model.Type = value;
                    this.RaisePropertyChanged(() => this.Type);
                }
            }

            public DateTime Start
            {
                get
                {
                    return this.Model.Start;
                }

                set
                {
                    if (object.Equals(this.Model.Start, value))
                    {
                        return;
                    }

                    this.Model.Start = value;
                    this.RaisePropertyChanged(() => this.Start);
                }
            }

            public DateTime End
            {
                get
                {
                    return this.Model.End;
                }

                set
                {
                    if (object.Equals(this.Model.End, value))
                    {
                        return;
                    }

                    this.Model.End = value;
                    this.RaisePropertyChanged(() => this.End);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public XmlDataViewModel Structure { get; private set; }

            public XmlDataViewModel InstallInstructions { get; private set; }

            public XmlDataViewModel DynamicContent { get; private set; }

            public EntityReference<UpdateGroupReadOnlyDataViewModel> UpdateGroup { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Type);
                }
            }

            public UpdatePartWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UpdatePartDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UpdatePartWritableModel writableModel)
            {
                this.Model = writableModel;

                this.UpdateGroup = new EntityReference<UpdateGroupReadOnlyDataViewModel>();
                if (writableModel.UpdateGroup != null)
                {
                    this.UpdateGroup.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UpdateGroup);
                }

                this.UpdateGroup.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UpdateGroup = this.UpdateGroup.SelectedEntity == null
                                                         ? null
                                                         : this.UpdateGroup.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UpdateGroup");
                        }
                    };

                this.Structure =
                    new XmlDataViewModel(
                        () => this.Model.Structure,
                        value =>
                            {
                                if (object.Equals(this.Model.Structure, value))
                                {
                                    return;
                                }

                                this.Model.Structure = value;
                                this.RaisePropertyChanged(() => this.Structure);
                            });

                this.InstallInstructions =
                    new XmlDataViewModel(
                        () => this.Model.InstallInstructions,
                        value =>
                            {
                                if (object.Equals(this.Model.InstallInstructions, value))
                                {
                                    return;
                                }

                                this.Model.InstallInstructions = value;
                                this.RaisePropertyChanged(() => this.InstallInstructions);
                            });

                this.DynamicContent =
                    new XmlDataViewModel(
                        () => this.Model.DynamicContent,
                        value =>
                            {
                                if (object.Equals(this.Model.DynamicContent, value))
                                {
                                    return;
                                }

                                this.Model.DynamicContent = value;
                                this.RaisePropertyChanged(() => this.DynamicContent);
                            });
            }
        }

        public partial class UpdateCommandReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Units.UnitReadOnlyDataViewModel referenceUnit;

            public UpdateCommandReadOnlyDataViewModel(
                UpdateCommandReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.IncludedParts = new EntityCollection<UpdatePartReadOnlyDataViewModel>(
                    "IncludedParts", "UpdatePart", this.Factory.CommandRegistry);
                this.Feedbacks = new EntityCollection<UpdateFeedbackReadOnlyDataViewModel>(
                    "Feedbacks", "UpdateFeedback", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public int UpdateIndex
            {
                get
                {
                    return this.ReadableModel.UpdateIndex;
                }
            }

            public bool WasTransferred
            {
                get
                {
                    return this.ReadableModel.WasTransferred;
                }
            }

            public bool WasInstalled
            {
                get
                {
                    return this.ReadableModel.WasInstalled;
                }
            }

            public string UnitDisplayText
            {
                get
                {
                    var reference = this.Unit;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Units.UnitReadOnlyDataViewModel Unit
            {
                get
                {
                    var currentId = this.referenceUnit == null ? 0 : this.referenceUnit.Id;
                    var modelId = this.ReadableModel.Unit == null ? 0 : this.ReadableModel.Unit.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUnit =
                            this.ReadableModel.Unit == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Unit);
                    }

                    return this.referenceUnit;
                }
            }

            public EntityCollection<UpdatePartReadOnlyDataViewModel> IncludedParts { get; private set; }

            public EntityCollection<UpdateFeedbackReadOnlyDataViewModel> Feedbacks { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.UpdateIndex);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UpdateCommandReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateCommandReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "UpdateIndex")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UpdateCommandDataViewModel : DataViewModelBase
        {
            public UpdateCommandDataViewModel(
                UpdateCommandReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UpdateCommandDataViewModel(
                UpdateCommandWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public int UpdateIndex
            {
                get
                {
                    return this.Model.UpdateIndex;
                }

                set
                {
                    if (object.Equals(this.Model.UpdateIndex, value))
                    {
                        return;
                    }

                    this.Model.UpdateIndex = value;
                    this.RaisePropertyChanged(() => this.UpdateIndex);
                }
            }

            public bool WasTransferred
            {
                get
                {
                    return this.Model.WasTransferred;
                }

                set
                {
                    if (object.Equals(this.Model.WasTransferred, value))
                    {
                        return;
                    }

                    this.Model.WasTransferred = value;
                    this.RaisePropertyChanged(() => this.WasTransferred);
                }
            }

            public bool WasInstalled
            {
                get
                {
                    return this.Model.WasInstalled;
                }

                set
                {
                    if (object.Equals(this.Model.WasInstalled, value))
                    {
                        return;
                    }

                    this.Model.WasInstalled = value;
                    this.RaisePropertyChanged(() => this.WasInstalled);
                }
            }

            public XmlDataViewModel Command { get; private set; }

            public EntityReference<Units.UnitReadOnlyDataViewModel> Unit { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.UpdateIndex);
                }
            }

            public UpdateCommandWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateCommandDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UpdateCommandWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Unit = new EntityReference<Units.UnitReadOnlyDataViewModel>();
                if (writableModel.Unit != null)
                {
                    this.Unit.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Unit);
                }

                this.Unit.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Unit = this.Unit.SelectedEntity == null
                                                         ? null
                                                         : this.Unit.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Unit");
                        }
                    };

                this.Command =
                    new XmlDataViewModel(
                        () => this.Model.Command,
                        value =>
                            {
                                if (object.Equals(this.Model.Command, value))
                                {
                                    return;
                                }

                                this.Model.Command = value;
                                this.RaisePropertyChanged(() => this.Command);
                            });
            }
        }

        public partial class UpdateFeedbackReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private UpdateCommandReadOnlyDataViewModel referenceUpdateCommand;

            public UpdateFeedbackReadOnlyDataViewModel(
                UpdateFeedbackReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public DateTime Timestamp
            {
                get
                {
                    return this.ReadableModel.Timestamp;
                }
            }

            public UpdateState State
            {
                get
                {
                    return this.ReadableModel.State;
                }
            }

            public string UpdateCommandDisplayText
            {
                get
                {
                    var reference = this.UpdateCommand;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public UpdateCommandReadOnlyDataViewModel UpdateCommand
            {
                get
                {
                    var currentId = this.referenceUpdateCommand == null ? 0 : this.referenceUpdateCommand.Id;
                    var modelId = this.ReadableModel.UpdateCommand == null ? 0 : this.ReadableModel.UpdateCommand.Id;
                    if (currentId != modelId)
                    {
                        this.referenceUpdateCommand =
                            this.ReadableModel.UpdateCommand == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.UpdateCommand);
                    }

                    return this.referenceUpdateCommand;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Timestamp);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UpdateFeedbackReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateFeedbackReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Timestamp")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UpdateFeedbackDataViewModel : DataViewModelBase
        {
            public UpdateFeedbackDataViewModel(
                UpdateFeedbackReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UpdateFeedbackDataViewModel(
                UpdateFeedbackWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public DateTime Timestamp
            {
                get
                {
                    return this.Model.Timestamp;
                }

                set
                {
                    if (object.Equals(this.Model.Timestamp, value))
                    {
                        return;
                    }

                    this.Model.Timestamp = value;
                    this.RaisePropertyChanged(() => this.Timestamp);
                }
            }

            public UpdateState State
            {
                get
                {
                    return this.Model.State;
                }

                set
                {
                    if (object.Equals(this.Model.State, value))
                    {
                        return;
                    }

                    this.Model.State = value;
                    this.RaisePropertyChanged(() => this.State);
                }
            }

            public XmlDataViewModel Feedback { get; private set; }

            public EntityReference<UpdateCommandReadOnlyDataViewModel> UpdateCommand { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Timestamp);
                }
            }

            public UpdateFeedbackWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UpdateFeedbackDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UpdateFeedbackWritableModel writableModel)
            {
                this.Model = writableModel;

                this.UpdateCommand = new EntityReference<UpdateCommandReadOnlyDataViewModel>();
                if (writableModel.UpdateCommand != null)
                {
                    this.UpdateCommand.SelectedEntity = this.Factory.CreateReadOnly(writableModel.UpdateCommand);
                }

                this.UpdateCommand.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.UpdateCommand = this.UpdateCommand.SelectedEntity == null
                                                         ? null
                                                         : this.UpdateCommand.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("UpdateCommand");
                        }
                    };

                this.Feedback =
                    new XmlDataViewModel(
                        () => this.Model.Feedback,
                        value =>
                            {
                                if (object.Equals(this.Model.Feedback, value))
                                {
                                    return;
                                }

                                this.Model.Feedback = value;
                                this.RaisePropertyChanged(() => this.Feedback);
                            });
            }
        }

    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        
        
        public partial class DocumentReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Membership.TenantReadOnlyDataViewModel referenceTenant;

            public DocumentReadOnlyDataViewModel(
                DocumentReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Versions = new EntityCollection<DocumentVersionReadOnlyDataViewModel>(
                    "Versions", "DocumentVersion", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string TenantDisplayText
            {
                get
                {
                    var reference = this.Tenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.TenantReadOnlyDataViewModel Tenant
            {
                get
                {
                    var currentId = this.referenceTenant == null ? 0 : this.referenceTenant.Id;
                    var modelId = this.ReadableModel.Tenant == null ? 0 : this.ReadableModel.Tenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceTenant =
                            this.ReadableModel.Tenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Tenant);
                    }

                    return this.referenceTenant;
                }
            }

            public EntityCollection<DocumentVersionReadOnlyDataViewModel> Versions { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public DocumentReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as DocumentReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class DocumentDataViewModel : DataViewModelBase
        {
            public DocumentDataViewModel(
                DocumentReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public DocumentDataViewModel(
                DocumentWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public EntityReference<Membership.TenantReadOnlyDataViewModel> Tenant { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public DocumentWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as DocumentDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(DocumentWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Tenant = new EntityReference<Membership.TenantReadOnlyDataViewModel>();
                if (writableModel.Tenant != null)
                {
                    this.Tenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Tenant);
                }

                this.Tenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Tenant = this.Tenant.SelectedEntity == null
                                                         ? null
                                                         : this.Tenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Tenant");
                        }
                    };
            }
        }

        public partial class DocumentVersionReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private DocumentReadOnlyDataViewModel referenceDocument;

            private Membership.UserReadOnlyDataViewModel referenceCreatingUser;

            public DocumentVersionReadOnlyDataViewModel(
                DocumentVersionReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public int Major
            {
                get
                {
                    return this.ReadableModel.Major;
                }
            }

            public int Minor
            {
                get
                {
                    return this.ReadableModel.Minor;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string DocumentDisplayText
            {
                get
                {
                    var reference = this.Document;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public DocumentReadOnlyDataViewModel Document
            {
                get
                {
                    var currentId = this.referenceDocument == null ? 0 : this.referenceDocument.Id;
                    var modelId = this.ReadableModel.Document == null ? 0 : this.ReadableModel.Document.Id;
                    if (currentId != modelId)
                    {
                        this.referenceDocument =
                            this.ReadableModel.Document == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Document);
                    }

                    return this.referenceDocument;
                }
            }

            public string CreatingUserDisplayText
            {
                get
                {
                    var reference = this.CreatingUser;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.UserReadOnlyDataViewModel CreatingUser
            {
                get
                {
                    var currentId = this.referenceCreatingUser == null ? 0 : this.referenceCreatingUser.Id;
                    var modelId = this.ReadableModel.CreatingUser == null ? 0 : this.ReadableModel.CreatingUser.Id;
                    if (currentId != modelId)
                    {
                        this.referenceCreatingUser =
                            this.ReadableModel.CreatingUser == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.CreatingUser);
                    }

                    return this.referenceCreatingUser;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Description);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public DocumentVersionReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as DocumentVersionReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Description")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class DocumentVersionDataViewModel : DataViewModelBase
        {
            public DocumentVersionDataViewModel(
                DocumentVersionReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public DocumentVersionDataViewModel(
                DocumentVersionWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public int Major
            {
                get
                {
                    return this.Model.Major;
                }

                set
                {
                    if (object.Equals(this.Model.Major, value))
                    {
                        return;
                    }

                    this.Model.Major = value;
                    this.RaisePropertyChanged(() => this.Major);
                }
            }

            public int Minor
            {
                get
                {
                    return this.Model.Minor;
                }

                set
                {
                    if (object.Equals(this.Model.Minor, value))
                    {
                        return;
                    }

                    this.Model.Minor = value;
                    this.RaisePropertyChanged(() => this.Minor);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public XmlDataViewModel Content { get; private set; }

            public EntityReference<DocumentReadOnlyDataViewModel> Document { get; private set; }

            public EntityReference<Membership.UserReadOnlyDataViewModel> CreatingUser { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Description);
                }
            }

            public DocumentVersionWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as DocumentVersionDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(DocumentVersionWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Document = new EntityReference<DocumentReadOnlyDataViewModel>();
                if (writableModel.Document != null)
                {
                    this.Document.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Document);
                }

                this.Document.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Document = this.Document.SelectedEntity == null
                                                         ? null
                                                         : this.Document.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Document");
                        }
                    };

                this.CreatingUser = new EntityReference<Membership.UserReadOnlyDataViewModel>();
                if (writableModel.CreatingUser != null)
                {
                    this.CreatingUser.SelectedEntity = this.Factory.CreateReadOnly(writableModel.CreatingUser);
                }

                this.CreatingUser.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.CreatingUser = this.CreatingUser.SelectedEntity == null
                                                         ? null
                                                         : this.CreatingUser.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("CreatingUser");
                        }
                    };

                this.Content =
                    new XmlDataViewModel(
                        () => this.Model.Content,
                        value =>
                            {
                                if (object.Equals(this.Model.Content, value))
                                {
                                    return;
                                }

                                this.Model.Content = value;
                                this.RaisePropertyChanged(() => this.Content);
                            });
            }
        }

    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        
        
        public partial class PackageReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            public PackageReadOnlyDataViewModel(
                PackageReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.Versions = new EntityCollection<PackageVersionReadOnlyDataViewModel>(
                    "Versions", "PackageVersion", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string PackageId
            {
                get
                {
                    return this.ReadableModel.PackageId;
                }
            }

            public string ProductName
            {
                get
                {
                    return this.ReadableModel.ProductName;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public EntityCollection<PackageVersionReadOnlyDataViewModel> Versions { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.ProductName);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public PackageReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as PackageReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "ProductName")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class PackageDataViewModel : DataViewModelBase
        {
            public PackageDataViewModel(
                PackageReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public PackageDataViewModel(
                PackageWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string PackageId
            {
                get
                {
                    return this.Model.PackageId;
                }

                set
                {
                    if (object.Equals(this.Model.PackageId, value))
                    {
                        return;
                    }

                    this.Model.PackageId = value;
                    this.RaisePropertyChanged(() => this.PackageId);
                }
            }

            public string ProductName
            {
                get
                {
                    return this.Model.ProductName;
                }

                set
                {
                    if (object.Equals(this.Model.ProductName, value))
                    {
                        return;
                    }

                    this.Model.ProductName = value;
                    this.RaisePropertyChanged(() => this.ProductName);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.ProductName);
                }
            }

            public PackageWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as PackageDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(PackageWritableModel writableModel)
            {
                this.Model = writableModel;
            }
        }

        public partial class PackageVersionReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private PackageReadOnlyDataViewModel referencePackage;

            public PackageVersionReadOnlyDataViewModel(
                PackageVersionReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string SoftwareVersion
            {
                get
                {
                    return this.ReadableModel.SoftwareVersion;
                }
            }

            public string Description
            {
                get
                {
                    return this.ReadableModel.Description;
                }
            }

            public string PackageDisplayText
            {
                get
                {
                    var reference = this.Package;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public PackageReadOnlyDataViewModel Package
            {
                get
                {
                    var currentId = this.referencePackage == null ? 0 : this.referencePackage.Id;
                    var modelId = this.ReadableModel.Package == null ? 0 : this.ReadableModel.Package.Id;
                    if (currentId != modelId)
                    {
                        this.referencePackage =
                            this.ReadableModel.Package == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Package);
                    }

                    return this.referencePackage;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.SoftwareVersion);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public PackageVersionReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as PackageVersionReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "SoftwareVersion")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class PackageVersionDataViewModel : DataViewModelBase
        {
            public PackageVersionDataViewModel(
                PackageVersionReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public PackageVersionDataViewModel(
                PackageVersionWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public string SoftwareVersion
            {
                get
                {
                    return this.Model.SoftwareVersion;
                }

                set
                {
                    if (object.Equals(this.Model.SoftwareVersion, value))
                    {
                        return;
                    }

                    this.Model.SoftwareVersion = value;
                    this.RaisePropertyChanged(() => this.SoftwareVersion);
                }
            }

            public string Description
            {
                get
                {
                    return this.Model.Description;
                }

                set
                {
                    if (object.Equals(this.Model.Description, value))
                    {
                        return;
                    }

                    this.Model.Description = value;
                    this.RaisePropertyChanged(() => this.Description);
                }
            }

            public XmlDataViewModel Structure { get; private set; }

            public EntityReference<PackageReadOnlyDataViewModel> Package { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.SoftwareVersion);
                }
            }

            public PackageVersionWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as PackageVersionDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(PackageVersionWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Package = new EntityReference<PackageReadOnlyDataViewModel>();
                if (writableModel.Package != null)
                {
                    this.Package.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Package);
                }

                this.Package.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Package = this.Package.SelectedEntity == null
                                                         ? null
                                                         : this.Package.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Package");
                        }
                    };

                this.Structure =
                    new XmlDataViewModel(
                        () => this.Model.Structure,
                        value =>
                            {
                                if (object.Equals(this.Model.Structure, value))
                                {
                                    return;
                                }

                                this.Model.Structure = value;
                                this.RaisePropertyChanged(() => this.Structure);
                            });
            }
        }

    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        
        
        public partial class UnitConfigurationReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Documents.DocumentReadOnlyDataViewModel referenceDocument;

            private Units.ProductTypeReadOnlyDataViewModel referenceProductType;

            public UnitConfigurationReadOnlyDataViewModel(
                UnitConfigurationReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.UpdateGroups = new EntityCollection<Update.UpdateGroupReadOnlyDataViewModel>(
                    "UpdateGroups", "UpdateGroup", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string DocumentDisplayText
            {
                get
                {
                    var reference = this.Document;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Documents.DocumentReadOnlyDataViewModel Document
            {
                get
                {
                    var currentId = this.referenceDocument == null ? 0 : this.referenceDocument.Id;
                    var modelId = this.ReadableModel.Document == null ? 0 : this.ReadableModel.Document.Id;
                    if (currentId != modelId)
                    {
                        this.referenceDocument =
                            this.ReadableModel.Document == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Document);
                    }

                    return this.referenceDocument;
                }
            }

            public string ProductTypeDisplayText
            {
                get
                {
                    var reference = this.ProductType;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Units.ProductTypeReadOnlyDataViewModel ProductType
            {
                get
                {
                    var currentId = this.referenceProductType == null ? 0 : this.referenceProductType.Id;
                    var modelId = this.ReadableModel.ProductType == null ? 0 : this.ReadableModel.ProductType.Id;
                    if (currentId != modelId)
                    {
                        this.referenceProductType =
                            this.ReadableModel.ProductType == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.ProductType);
                    }

                    return this.referenceProductType;
                }
            }

            public EntityCollection<Update.UpdateGroupReadOnlyDataViewModel> UpdateGroups { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Document);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UnitConfigurationReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UnitConfigurationReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Document")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UnitConfigurationDataViewModel : DataViewModelBase
        {
            public UnitConfigurationDataViewModel(
                UnitConfigurationReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UnitConfigurationDataViewModel(
                UnitConfigurationWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public EntityReference<Documents.DocumentReadOnlyDataViewModel> Document { get; private set; }

            public EntityReference<Units.ProductTypeReadOnlyDataViewModel> ProductType { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Document);
                }
            }

            public UnitConfigurationWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UnitConfigurationDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UnitConfigurationWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Document = new EntityReference<Documents.DocumentReadOnlyDataViewModel>();
                if (writableModel.Document != null)
                {
                    this.Document.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Document);
                }

                this.Document.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Document = this.Document.SelectedEntity == null
                                                         ? null
                                                         : this.Document.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Document");
                        }
                    };

                this.ProductType = new EntityReference<Units.ProductTypeReadOnlyDataViewModel>();
                if (writableModel.ProductType != null)
                {
                    this.ProductType.SelectedEntity = this.Factory.CreateReadOnly(writableModel.ProductType);
                }

                this.ProductType.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.ProductType = this.ProductType.SelectedEntity == null
                                                         ? null
                                                         : this.ProductType.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("ProductType");
                        }
                    };
            }
        }

        public partial class MediaConfigurationReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Documents.DocumentReadOnlyDataViewModel referenceDocument;

            public MediaConfigurationReadOnlyDataViewModel(
                MediaConfigurationReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.UpdateGroups = new EntityCollection<Update.UpdateGroupReadOnlyDataViewModel>(
                    "UpdateGroups", "UpdateGroup", this.Factory.CommandRegistry);
                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public string DocumentDisplayText
            {
                get
                {
                    var reference = this.Document;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Documents.DocumentReadOnlyDataViewModel Document
            {
                get
                {
                    var currentId = this.referenceDocument == null ? 0 : this.referenceDocument.Id;
                    var modelId = this.ReadableModel.Document == null ? 0 : this.ReadableModel.Document.Id;
                    if (currentId != modelId)
                    {
                        this.referenceDocument =
                            this.ReadableModel.Document == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Document);
                    }

                    return this.referenceDocument;
                }
            }

            public EntityCollection<Update.UpdateGroupReadOnlyDataViewModel> UpdateGroups { get; private set; }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Document);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public MediaConfigurationReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as MediaConfigurationReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Document")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class MediaConfigurationDataViewModel : DataViewModelBase
        {
            public MediaConfigurationDataViewModel(
                MediaConfigurationReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public MediaConfigurationDataViewModel(
                MediaConfigurationWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public EntityReference<Documents.DocumentReadOnlyDataViewModel> Document { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Document);
                }
            }

            public MediaConfigurationWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as MediaConfigurationDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(MediaConfigurationWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Document = new EntityReference<Documents.DocumentReadOnlyDataViewModel>();
                if (writableModel.Document != null)
                {
                    this.Document.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Document);
                }

                this.Document.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Document = this.Document.SelectedEntity == null
                                                         ? null
                                                         : this.Document.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Document");
                        }
                    };
            }
        }

    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        
        
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        
        
        public partial class UserDefinedPropertyReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            private Membership.TenantReadOnlyDataViewModel referenceTenant;

            public UserDefinedPropertyReadOnlyDataViewModel(
                UserDefinedPropertyReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public UserDefinedPropertyEnabledEntity OwnerEntity
            {
                get
                {
                    return this.ReadableModel.OwnerEntity;
                }
            }

            public string Name
            {
                get
                {
                    return this.ReadableModel.Name;
                }
            }

            public string TenantDisplayText
            {
                get
                {
                    var reference = this.Tenant;
                    return reference == null ? null : reference.DisplayText;
                }
            }

            public Membership.TenantReadOnlyDataViewModel Tenant
            {
                get
                {
                    var currentId = this.referenceTenant == null ? 0 : this.referenceTenant.Id;
                    var modelId = this.ReadableModel.Tenant == null ? 0 : this.ReadableModel.Tenant.Id;
                    if (currentId != modelId)
                    {
                        this.referenceTenant =
                            this.ReadableModel.Tenant == null
                                ? null
                                : this.Factory.CreateReadOnly(this.ReadableModel.Tenant);
                    }

                    return this.referenceTenant;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.Name);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public UserDefinedPropertyReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as UserDefinedPropertyReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "Name")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class UserDefinedPropertyDataViewModel : DataViewModelBase
        {
            public UserDefinedPropertyDataViewModel(
                UserDefinedPropertyReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public UserDefinedPropertyDataViewModel(
                UserDefinedPropertyWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public UserDefinedPropertyEnabledEntity OwnerEntity
            {
                get
                {
                    return this.Model.OwnerEntity;
                }

                set
                {
                    if (object.Equals(this.Model.OwnerEntity, value))
                    {
                        return;
                    }

                    this.Model.OwnerEntity = value;
                    this.RaisePropertyChanged(() => this.OwnerEntity);
                }
            }

            public string Name
            {
                get
                {
                    return this.Model.Name;
                }

                set
                {
                    if (object.Equals(this.Model.Name, value))
                    {
                        return;
                    }

                    this.Model.Name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }

            public EntityReference<Membership.TenantReadOnlyDataViewModel> Tenant { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.Name);
                }
            }

            public UserDefinedPropertyWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as UserDefinedPropertyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(UserDefinedPropertyWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Tenant = new EntityReference<Membership.TenantReadOnlyDataViewModel>();
                if (writableModel.Tenant != null)
                {
                    this.Tenant.SelectedEntity = this.Factory.CreateReadOnly(writableModel.Tenant);
                }

                this.Tenant.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "SelectedEntity")
                        {
                            this.Model.Tenant = this.Tenant.SelectedEntity == null
                                                         ? null
                                                         : this.Tenant.SelectedEntity.ReadableModel;
                            this.RaisePropertyChanged("Tenant");
                        }
                    };
            }
        }

        public partial class SystemConfigReadOnlyDataViewModel : ReadOnlyDataViewModelBase
        {
            public SystemConfigReadOnlyDataViewModel(
                SystemConfigReadableModel readableModel, DataViewModelFactory factory)
                : base(factory)
            {
                if (readableModel == null)
                {
                    throw new ArgumentNullException("readableModel");
                }

                this.ReadableModel = readableModel;
                this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            }

            public int Id
            {
                get
                {
                    return this.ReadableModel.Id;
                }
            }

            public DateTime CreatedOn
            {
                get
                {
                    return this.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.ReadableModel.LastModifiedOn;
                }
            }

            public Guid SystemId
            {
                get
                {
                    return this.ReadableModel.SystemId;
                }
            }

            public override string DisplayText
            {
                get
                {
                    var displayText = Convert.ToString(this.SystemId);
                    this.GetDisplayText(ref displayText);
                    return displayText;
                }
            }

            public SystemConfigReadableModel ReadableModel { get; private set; }

            public override string GetIdString()
            {
                return Convert.ToString(this.Id);
            }

            public override bool Equals(object obj)
            {
                var other = obj as SystemConfigReadOnlyDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            partial void GetDisplayText(ref string displayText);

            private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == "SystemId")
                {
                    this.RaisePropertyChanged(() => this.DisplayText);
                }
            }
        }

        public partial class SystemConfigDataViewModel : DataViewModelBase
        {
            public SystemConfigDataViewModel(
                SystemConfigReadOnlyDataViewModel readOnlyDataViewModel, DataViewModelFactory factory)
                : base(readOnlyDataViewModel, factory)
            {
                if (readOnlyDataViewModel == null)
                {
                    throw new ArgumentNullException("readOnlyDataViewModel");
                }

                this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
            }

            public SystemConfigDataViewModel(
                SystemConfigWritableModel writableModel, DataViewModelFactory factory)
                : base(null, factory)
            {
                if (writableModel == null)
                {
                    throw new ArgumentNullException("writableModel");
                }

                this.Initialize(writableModel);
            }

            public int Id
            {
                get
                {
                    return this.Model.Id;
                }
            }

            public DateTime? CreatedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? (DateTime?)null : this.Model.ReadableModel.CreatedOn;
                }
            }

            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.Model.ReadableModel == null ? null : this.Model.ReadableModel.LastModifiedOn;
                }
            }

            public Guid SystemId
            {
                get
                {
                    return this.Model.SystemId;
                }

                set
                {
                    if (object.Equals(this.Model.SystemId, value))
                    {
                        return;
                    }

                    this.Model.SystemId = value;
                    this.RaisePropertyChanged(() => this.SystemId);
                }
            }

            public XmlDataViewModel Settings { get; private set; }

            public override string DisplayText
            {
                get
                {
                    return Convert.ToString(this.SystemId);
                }
            }

            public SystemConfigWritableModel Model { get; private set; }

            public override bool Equals(object obj)
            {
                var other = obj as SystemConfigDataViewModel;
                return other != null && this.Id.Equals(other.Id);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Model.Dispose();
                }
            }

            private void Initialize(SystemConfigWritableModel writableModel)
            {
                this.Model = writableModel;

                this.Settings =
                    new XmlDataViewModel(
                        () => this.Model.Settings,
                        value =>
                            {
                                if (object.Equals(this.Model.Settings, value))
                                {
                                    return;
                                }

                                this.Model.Settings = value;
                                this.RaisePropertyChanged(() => this.Settings);
                            });
            }
        }

    }
}