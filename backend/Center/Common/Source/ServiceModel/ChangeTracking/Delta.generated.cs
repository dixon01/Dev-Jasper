namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.Collections;

    namespace AccessControl
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        public partial class AuthorizationDelta : DeltaBase
        {
            private readonly int? originalUserRoleId;

            private readonly DataScope originalDataScope;

            private readonly Permission originalPermission;
 
            public AuthorizationDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public AuthorizationDelta(AuthorizationReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUserRoleId = model.UserRole == null ? null : (int?)model.UserRole.Id;
                this.originalDataScope = model.DataScope;
                this.originalPermission = model.Permission;
                this.Version = new Version(model.Version.Value);
            }
 
            public AuthorizationDelta(Authorization model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUserRoleId = model.UserRole == null ? null : (int?)model.UserRole.Id;
                this.originalDataScope = model.DataScope;
                this.originalPermission = model.Permission;
                this.Version = new Version(model.Version);
            }
 
            public AuthorizationDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> UserRole { get; private set; }
    
            public PropertyChange<DataScope> DataScope { get; private set; }
    
            public PropertyChange<Permission> Permission { get; private set; }

            public void ChangeUserRole(int? id)
            {
                this.UserRole = new ReferenceChange<int>(this.originalUserRoleId).ChangeReference(id);
            }

            public void ChangeUserRole(UserRoleReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UserRole = new ReferenceChange<int>(this.originalUserRoleId).ChangeReference(id);
            }

            public void ChangeDataScope(DataScope value)
            {
                this.DataScope = new PropertyChange<DataScope>(this.originalDataScope).ChangeValue(value);
            }

            public void ChangePermission(Permission value)
            {
                this.Permission = new PropertyChange<Permission>(this.originalPermission).ChangeValue(value);
            }

            public AuthorizationDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UserRole != null && this.UserRole.OriginalReferenceId != this.UserRole.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UserRole = null;
                }
    
                if (this.DataScope != null && this.DataScope.OriginalValue != this.DataScope.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.DataScope = null;
                }
    
                if (this.Permission != null && this.Permission.OriginalValue != this.Permission.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Permission = null;
                }

                return hasChanges ? clone : null;
            }

            public AuthorizationDelta Clone()
            {
                return (AuthorizationDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UserRoleDelta : DeltaBase
        {            private UserDefinedPropertiesDictionary userDefinedPropertiesDelta;

            private readonly string originalName;

            private readonly string originalDescription;
 
            public UserRoleDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserRoleDelta(UserRoleReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version.Value);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserRoleDelta(UserRole model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserRoleDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }

            public IDictionary<string, string> UserDefinedPropertiesDelta
            {
                get
                {
                    return this.userDefinedPropertiesDelta;
                }
            }

            public IReadOnlyDictionary<string, string> UserDefinedProperties { get; private set; }

            public int Id { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public UserRoleDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = true;
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }

                return hasChanges ? clone : null;
            }

            public UserRoleDelta Clone()
            {
                return (UserRoleDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Membership
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        public partial class AssociationTenantUserUserRoleDelta : DeltaBase
        {
            private readonly int? originalTenantId;

            private readonly int? originalUserId;

            private readonly int? originalUserRoleId;
 
            public AssociationTenantUserUserRoleDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public AssociationTenantUserUserRoleDelta(AssociationTenantUserUserRoleReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalUserId = model.User == null ? null : (int?)model.User.Id;
                this.originalUserRoleId = model.UserRole == null ? null : (int?)model.UserRole.Id;
                this.Version = new Version(model.Version.Value);
            }
 
            public AssociationTenantUserUserRoleDelta(AssociationTenantUserUserRole model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalUserId = model.User == null ? null : (int?)model.User.Id;
                this.originalUserRoleId = model.UserRole == null ? null : (int?)model.UserRole.Id;
                this.Version = new Version(model.Version);
            }
 
            public AssociationTenantUserUserRoleDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Tenant { get; private set; }
    
            public ReferenceChange<int> User { get; private set; }
    
            public ReferenceChange<int> UserRole { get; private set; }

            public void ChangeTenant(int? id)
            {
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeTenant(TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeUser(int? id)
            {
                this.User = new ReferenceChange<int>(this.originalUserId).ChangeReference(id);
            }

            public void ChangeUser(UserReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.User = new ReferenceChange<int>(this.originalUserId).ChangeReference(id);
            }

            public void ChangeUserRole(int? id)
            {
                this.UserRole = new ReferenceChange<int>(this.originalUserRoleId).ChangeReference(id);
            }

            public void ChangeUserRole(AccessControl.UserRoleReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UserRole = new ReferenceChange<int>(this.originalUserRoleId).ChangeReference(id);
            }

            public AssociationTenantUserUserRoleDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Tenant != null && this.Tenant.OriginalReferenceId != this.Tenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Tenant = null;
                }
    
                if (this.User != null && this.User.OriginalReferenceId != this.User.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.User = null;
                }
    
                if (this.UserRole != null && this.UserRole.OriginalReferenceId != this.UserRole.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UserRole = null;
                }

                return hasChanges ? clone : null;
            }

            public AssociationTenantUserUserRoleDelta Clone()
            {
                return (AssociationTenantUserUserRoleDelta)(this as ICloneable).Clone();
            }
        }

        public partial class TenantDelta : DeltaBase
        {            private UserDefinedPropertiesDictionary userDefinedPropertiesDelta;

            private readonly string originalName;

            private readonly string originalDescription;
 
            public TenantDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public TenantDelta(TenantReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version.Value);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public TenantDelta(Tenant model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public TenantDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }

            public IDictionary<string, string> UserDefinedPropertiesDelta
            {
                get
                {
                    return this.userDefinedPropertiesDelta;
                }
            }

            public IReadOnlyDictionary<string, string> UserDefinedProperties { get; private set; }

            public int Id { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public TenantDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = true;
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }

                return hasChanges ? clone : null;
            }

            public TenantDelta Clone()
            {
                return (TenantDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UserDelta : DeltaBase
        {            private UserDefinedPropertiesDictionary userDefinedPropertiesDelta;

            private readonly int? originalOwnerTenantId;

            private readonly string originalUsername;

            private readonly string originalDomain;

            private readonly string originalHashedPassword;

            private readonly string originalFirstName;

            private readonly string originalLastName;

            private readonly string originalEmail;

            private readonly string originalCulture;

            private readonly string originalTimeZone;

            private readonly string originalDescription;

            private readonly DateTime? originalLastLoginAttempt;

            private readonly DateTime? originalLastSuccessfulLogin;

            private readonly int originalConsecutiveLoginFailures;

            private readonly bool originalIsEnabled;
 
            public UserDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserDelta(UserReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalOwnerTenantId = model.OwnerTenant == null ? null : (int?)model.OwnerTenant.Id;
                this.originalUsername = model.Username;
                this.originalDomain = model.Domain;
                this.originalHashedPassword = model.HashedPassword;
                this.originalFirstName = model.FirstName;
                this.originalLastName = model.LastName;
                this.originalEmail = model.Email;
                this.originalCulture = model.Culture;
                this.originalTimeZone = model.TimeZone;
                this.originalDescription = model.Description;
                this.originalLastLoginAttempt = model.LastLoginAttempt;
                this.originalLastSuccessfulLogin = model.LastSuccessfulLogin;
                this.originalConsecutiveLoginFailures = model.ConsecutiveLoginFailures;
                this.originalIsEnabled = model.IsEnabled;
                this.Version = new Version(model.Version.Value);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserDelta(User model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalOwnerTenantId = model.OwnerTenant == null ? null : (int?)model.OwnerTenant.Id;
                this.originalUsername = model.Username;
                this.originalDomain = model.Domain;
                this.originalHashedPassword = model.HashedPassword;
                this.originalFirstName = model.FirstName;
                this.originalLastName = model.LastName;
                this.originalEmail = model.Email;
                this.originalCulture = model.Culture;
                this.originalTimeZone = model.TimeZone;
                this.originalDescription = model.Description;
                this.originalLastLoginAttempt = model.LastLoginAttempt;
                this.originalLastSuccessfulLogin = model.LastSuccessfulLogin;
                this.originalConsecutiveLoginFailures = model.ConsecutiveLoginFailures;
                this.originalIsEnabled = model.IsEnabled;
                this.Version = new Version(model.Version);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UserDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }

            public IDictionary<string, string> UserDefinedPropertiesDelta
            {
                get
                {
                    return this.userDefinedPropertiesDelta;
                }
            }

            public IReadOnlyDictionary<string, string> UserDefinedProperties { get; private set; }

            public int Id { get; private set; }
    
            public ReferenceChange<int> OwnerTenant { get; private set; }
    
            public PropertyChange<string> Username { get; private set; }
    
            public PropertyChange<string> Domain { get; private set; }
    
            public PropertyChange<string> HashedPassword { get; private set; }
    
            public PropertyChange<string> FirstName { get; private set; }
    
            public PropertyChange<string> LastName { get; private set; }
    
            public PropertyChange<string> Email { get; private set; }
    
            public PropertyChange<string> Culture { get; private set; }
    
            public PropertyChange<string> TimeZone { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<DateTime?> LastLoginAttempt { get; private set; }
    
            public PropertyChange<DateTime?> LastSuccessfulLogin { get; private set; }
    
            public PropertyChange<int> ConsecutiveLoginFailures { get; private set; }
    
            public PropertyChange<bool> IsEnabled { get; private set; }

            public void ChangeOwnerTenant(int? id)
            {
                this.OwnerTenant = new ReferenceChange<int>(this.originalOwnerTenantId).ChangeReference(id);
            }

            public void ChangeOwnerTenant(TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.OwnerTenant = new ReferenceChange<int>(this.originalOwnerTenantId).ChangeReference(id);
            }

            public void ChangeUsername(string value)
            {
                this.Username = new PropertyChange<string>(this.originalUsername).ChangeValue(value);
            }

            public void ChangeDomain(string value)
            {
                this.Domain = new PropertyChange<string>(this.originalDomain).ChangeValue(value);
            }

            public void ChangeHashedPassword(string value)
            {
                this.HashedPassword = new PropertyChange<string>(this.originalHashedPassword).ChangeValue(value);
            }

            public void ChangeFirstName(string value)
            {
                this.FirstName = new PropertyChange<string>(this.originalFirstName).ChangeValue(value);
            }

            public void ChangeLastName(string value)
            {
                this.LastName = new PropertyChange<string>(this.originalLastName).ChangeValue(value);
            }

            public void ChangeEmail(string value)
            {
                this.Email = new PropertyChange<string>(this.originalEmail).ChangeValue(value);
            }

            public void ChangeCulture(string value)
            {
                this.Culture = new PropertyChange<string>(this.originalCulture).ChangeValue(value);
            }

            public void ChangeTimeZone(string value)
            {
                this.TimeZone = new PropertyChange<string>(this.originalTimeZone).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeLastLoginAttempt(DateTime? value)
            {
                this.LastLoginAttempt = new PropertyChange<DateTime?>(this.originalLastLoginAttempt).ChangeValue(value);
            }

            public void ChangeLastSuccessfulLogin(DateTime? value)
            {
                this.LastSuccessfulLogin = new PropertyChange<DateTime?>(this.originalLastSuccessfulLogin).ChangeValue(value);
            }

            public void ChangeConsecutiveLoginFailures(int value)
            {
                this.ConsecutiveLoginFailures = new PropertyChange<int>(this.originalConsecutiveLoginFailures).ChangeValue(value);
            }

            public void ChangeIsEnabled(bool value)
            {
                this.IsEnabled = new PropertyChange<bool>(this.originalIsEnabled).ChangeValue(value);
            }

            public UserDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = true;
    
                if (this.OwnerTenant != null && this.OwnerTenant.OriginalReferenceId != this.OwnerTenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.OwnerTenant = null;
                }
    
                if (this.Username != null && this.Username.OriginalValue != this.Username.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Username = null;
                }
    
                if (this.Domain != null && this.Domain.OriginalValue != this.Domain.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Domain = null;
                }
    
                if (this.HashedPassword != null && this.HashedPassword.OriginalValue != this.HashedPassword.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.HashedPassword = null;
                }
    
                if (this.FirstName != null && this.FirstName.OriginalValue != this.FirstName.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.FirstName = null;
                }
    
                if (this.LastName != null && this.LastName.OriginalValue != this.LastName.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.LastName = null;
                }
    
                if (this.Email != null && this.Email.OriginalValue != this.Email.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Email = null;
                }
    
                if (this.Culture != null && this.Culture.OriginalValue != this.Culture.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Culture = null;
                }
    
                if (this.TimeZone != null && this.TimeZone.OriginalValue != this.TimeZone.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.TimeZone = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.LastLoginAttempt != null && this.LastLoginAttempt.OriginalValue != this.LastLoginAttempt.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.LastLoginAttempt = null;
                }
    
                if (this.LastSuccessfulLogin != null && this.LastSuccessfulLogin.OriginalValue != this.LastSuccessfulLogin.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.LastSuccessfulLogin = null;
                }
    
                if (this.ConsecutiveLoginFailures != null && this.ConsecutiveLoginFailures.OriginalValue != this.ConsecutiveLoginFailures.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ConsecutiveLoginFailures = null;
                }
    
                if (this.IsEnabled != null && this.IsEnabled.OriginalValue != this.IsEnabled.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.IsEnabled = null;
                }

                return hasChanges ? clone : null;
            }

            public UserDelta Clone()
            {
                return (UserDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Units
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        public partial class ProductTypeDelta : DeltaBase
        {
            private readonly UnitTypes originalUnitType;

            private readonly string originalName;

            private readonly string originalDescription;

            private readonly XmlData originalHardwareDescriptor;
 
            public ProductTypeDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public ProductTypeDelta(ProductTypeReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUnitType = model.UnitType;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.originalHardwareDescriptor = model.HardwareDescriptor;
                this.Version = new Version(model.Version.Value);
            }
 
            public ProductTypeDelta(ProductType model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUnitType = model.UnitType;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.originalHardwareDescriptor = model.HardwareDescriptor;
                this.Version = new Version(model.Version);
            }
 
            public ProductTypeDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public PropertyChange<UnitTypes> UnitType { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<XmlData> HardwareDescriptor { get; private set; }

            public void ChangeUnitType(UnitTypes value)
            {
                this.UnitType = new PropertyChange<UnitTypes>(this.originalUnitType).ChangeValue(value);
            }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeHardwareDescriptor(XmlData value)
            {
                this.HardwareDescriptor = new PropertyChange<XmlData>(this.originalHardwareDescriptor).ChangeValue(value);
            }

            public ProductTypeDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UnitType != null && this.UnitType.OriginalValue != this.UnitType.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UnitType = null;
                }
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.HardwareDescriptor != null && this.HardwareDescriptor.OriginalValue != this.HardwareDescriptor.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.HardwareDescriptor = null;
                }

                return hasChanges ? clone : null;
            }

            public ProductTypeDelta Clone()
            {
                return (ProductTypeDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UnitDelta : DeltaBase
        {            private UserDefinedPropertiesDictionary userDefinedPropertiesDelta;

            private readonly int? originalTenantId;

            private readonly int? originalProductTypeId;

            private readonly int? originalUpdateGroupId;

            private readonly string originalName;

            private readonly string originalNetworkAddress;

            private readonly string originalDescription;

            private readonly bool originalIsConnected;
 
            public UnitDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UnitDelta(UnitReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalProductTypeId = model.ProductType == null ? null : (int?)model.ProductType.Id;
                this.originalUpdateGroupId = model.UpdateGroup == null ? null : (int?)model.UpdateGroup.Id;
                this.originalName = model.Name;
                this.originalNetworkAddress = model.NetworkAddress;
                this.originalDescription = model.Description;
                this.originalIsConnected = model.IsConnected;
                this.Version = new Version(model.Version.Value);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UnitDelta(Unit model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalProductTypeId = model.ProductType == null ? null : (int?)model.ProductType.Id;
                this.originalUpdateGroupId = model.UpdateGroup == null ? null : (int?)model.UpdateGroup.Id;
                this.originalName = model.Name;
                this.originalNetworkAddress = model.NetworkAddress;
                this.originalDescription = model.Description;
                this.originalIsConnected = model.IsConnected;
                this.Version = new Version(model.Version);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UnitDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }

            public IDictionary<string, string> UserDefinedPropertiesDelta
            {
                get
                {
                    return this.userDefinedPropertiesDelta;
                }
            }

            public IReadOnlyDictionary<string, string> UserDefinedProperties { get; private set; }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Tenant { get; private set; }
    
            public ReferenceChange<int> ProductType { get; private set; }
    
            public ReferenceChange<int> UpdateGroup { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> NetworkAddress { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<bool> IsConnected { get; private set; }

            public void ChangeTenant(int? id)
            {
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeTenant(Membership.TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeProductType(int? id)
            {
                this.ProductType = new ReferenceChange<int>(this.originalProductTypeId).ChangeReference(id);
            }

            public void ChangeProductType(ProductTypeReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.ProductType = new ReferenceChange<int>(this.originalProductTypeId).ChangeReference(id);
            }

            public void ChangeUpdateGroup(int? id)
            {
                this.UpdateGroup = new ReferenceChange<int>(this.originalUpdateGroupId).ChangeReference(id);
            }

            public void ChangeUpdateGroup(Update.UpdateGroupReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UpdateGroup = new ReferenceChange<int>(this.originalUpdateGroupId).ChangeReference(id);
            }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeNetworkAddress(string value)
            {
                this.NetworkAddress = new PropertyChange<string>(this.originalNetworkAddress).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeIsConnected(bool value)
            {
                this.IsConnected = new PropertyChange<bool>(this.originalIsConnected).ChangeValue(value);
            }

            public UnitDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = true;
    
                if (this.Tenant != null && this.Tenant.OriginalReferenceId != this.Tenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Tenant = null;
                }
    
                if (this.ProductType != null && this.ProductType.OriginalReferenceId != this.ProductType.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ProductType = null;
                }
    
                if (this.UpdateGroup != null && this.UpdateGroup.OriginalReferenceId != this.UpdateGroup.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UpdateGroup = null;
                }
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.NetworkAddress != null && this.NetworkAddress.OriginalValue != this.NetworkAddress.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.NetworkAddress = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.IsConnected != null && this.IsConnected.OriginalValue != this.IsConnected.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.IsConnected = null;
                }

                return hasChanges ? clone : null;
            }

            public UnitDelta Clone()
            {
                return (UnitDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Resources
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        public partial class ContentResourceDelta : DeltaBase
        {
            private readonly int? originalUploadingUserId;

            private readonly string originalOriginalFilename;

            private readonly string originalDescription;

            private readonly string originalThumbnailHash;

            private readonly string originalHash;

            private readonly HashAlgorithmTypes originalHashAlgorithmType;

            private readonly string originalMimeType;

            private readonly long originalLength;
 
            public ContentResourceDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public ContentResourceDelta(ContentResourceReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUploadingUserId = model.UploadingUser == null ? null : (int?)model.UploadingUser.Id;
                this.originalOriginalFilename = model.OriginalFilename;
                this.originalDescription = model.Description;
                this.originalThumbnailHash = model.ThumbnailHash;
                this.originalHash = model.Hash;
                this.originalHashAlgorithmType = model.HashAlgorithmType;
                this.originalMimeType = model.MimeType;
                this.originalLength = model.Length;
                this.Version = new Version(model.Version.Value);
            }
 
            public ContentResourceDelta(ContentResource model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUploadingUserId = model.UploadingUser == null ? null : (int?)model.UploadingUser.Id;
                this.originalOriginalFilename = model.OriginalFilename;
                this.originalDescription = model.Description;
                this.originalThumbnailHash = model.ThumbnailHash;
                this.originalHash = model.Hash;
                this.originalHashAlgorithmType = model.HashAlgorithmType;
                this.originalMimeType = model.MimeType;
                this.originalLength = model.Length;
                this.Version = new Version(model.Version);
            }
 
            public ContentResourceDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> UploadingUser { get; private set; }
    
            public PropertyChange<string> OriginalFilename { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<string> ThumbnailHash { get; private set; }
    
            public PropertyChange<string> Hash { get; private set; }
    
            public PropertyChange<HashAlgorithmTypes> HashAlgorithmType { get; private set; }
    
            public PropertyChange<string> MimeType { get; private set; }
    
            public PropertyChange<long> Length { get; private set; }

            public void ChangeUploadingUser(int? id)
            {
                this.UploadingUser = new ReferenceChange<int>(this.originalUploadingUserId).ChangeReference(id);
            }

            public void ChangeUploadingUser(Membership.UserReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UploadingUser = new ReferenceChange<int>(this.originalUploadingUserId).ChangeReference(id);
            }

            public void ChangeOriginalFilename(string value)
            {
                this.OriginalFilename = new PropertyChange<string>(this.originalOriginalFilename).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeThumbnailHash(string value)
            {
                this.ThumbnailHash = new PropertyChange<string>(this.originalThumbnailHash).ChangeValue(value);
            }

            public void ChangeHash(string value)
            {
                this.Hash = new PropertyChange<string>(this.originalHash).ChangeValue(value);
            }

            public void ChangeHashAlgorithmType(HashAlgorithmTypes value)
            {
                this.HashAlgorithmType = new PropertyChange<HashAlgorithmTypes>(this.originalHashAlgorithmType).ChangeValue(value);
            }

            public void ChangeMimeType(string value)
            {
                this.MimeType = new PropertyChange<string>(this.originalMimeType).ChangeValue(value);
            }

            public void ChangeLength(long value)
            {
                this.Length = new PropertyChange<long>(this.originalLength).ChangeValue(value);
            }

            public ContentResourceDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UploadingUser != null && this.UploadingUser.OriginalReferenceId != this.UploadingUser.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UploadingUser = null;
                }
    
                if (this.OriginalFilename != null && this.OriginalFilename.OriginalValue != this.OriginalFilename.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.OriginalFilename = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.ThumbnailHash != null && this.ThumbnailHash.OriginalValue != this.ThumbnailHash.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ThumbnailHash = null;
                }
    
                if (this.Hash != null && this.Hash.OriginalValue != this.Hash.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Hash = null;
                }
    
                if (this.HashAlgorithmType != null && this.HashAlgorithmType.OriginalValue != this.HashAlgorithmType.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.HashAlgorithmType = null;
                }
    
                if (this.MimeType != null && this.MimeType.OriginalValue != this.MimeType.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.MimeType = null;
                }
    
                if (this.Length != null && this.Length.OriginalValue != this.Length.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Length = null;
                }

                return hasChanges ? clone : null;
            }

            public ContentResourceDelta Clone()
            {
                return (ContentResourceDelta)(this as ICloneable).Clone();
            }
        }

        public partial class ResourceDelta : DeltaBase
        {
            private readonly int? originalUploadingUserId;

            private readonly string originalOriginalFilename;

            private readonly string originalDescription;

            private readonly string originalHash;

            private readonly string originalThumbnailHash;

            private readonly string originalMimeType;

            private readonly long originalLength;
 
            public ResourceDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public ResourceDelta(ResourceReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUploadingUserId = model.UploadingUser == null ? null : (int?)model.UploadingUser.Id;
                this.originalOriginalFilename = model.OriginalFilename;
                this.originalDescription = model.Description;
                this.originalHash = model.Hash;
                this.originalThumbnailHash = model.ThumbnailHash;
                this.originalMimeType = model.MimeType;
                this.originalLength = model.Length;
                this.Version = new Version(model.Version.Value);
            }
 
            public ResourceDelta(Resource model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUploadingUserId = model.UploadingUser == null ? null : (int?)model.UploadingUser.Id;
                this.originalOriginalFilename = model.OriginalFilename;
                this.originalDescription = model.Description;
                this.originalHash = model.Hash;
                this.originalThumbnailHash = model.ThumbnailHash;
                this.originalMimeType = model.MimeType;
                this.originalLength = model.Length;
                this.Version = new Version(model.Version);
            }
 
            public ResourceDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> UploadingUser { get; private set; }
    
            public PropertyChange<string> OriginalFilename { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<string> Hash { get; private set; }
    
            public PropertyChange<string> ThumbnailHash { get; private set; }
    
            public PropertyChange<string> MimeType { get; private set; }
    
            public PropertyChange<long> Length { get; private set; }

            public void ChangeUploadingUser(int? id)
            {
                this.UploadingUser = new ReferenceChange<int>(this.originalUploadingUserId).ChangeReference(id);
            }

            public void ChangeUploadingUser(Membership.UserReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UploadingUser = new ReferenceChange<int>(this.originalUploadingUserId).ChangeReference(id);
            }

            public void ChangeOriginalFilename(string value)
            {
                this.OriginalFilename = new PropertyChange<string>(this.originalOriginalFilename).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeHash(string value)
            {
                this.Hash = new PropertyChange<string>(this.originalHash).ChangeValue(value);
            }

            public void ChangeThumbnailHash(string value)
            {
                this.ThumbnailHash = new PropertyChange<string>(this.originalThumbnailHash).ChangeValue(value);
            }

            public void ChangeMimeType(string value)
            {
                this.MimeType = new PropertyChange<string>(this.originalMimeType).ChangeValue(value);
            }

            public void ChangeLength(long value)
            {
                this.Length = new PropertyChange<long>(this.originalLength).ChangeValue(value);
            }

            public ResourceDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UploadingUser != null && this.UploadingUser.OriginalReferenceId != this.UploadingUser.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UploadingUser = null;
                }
    
                if (this.OriginalFilename != null && this.OriginalFilename.OriginalValue != this.OriginalFilename.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.OriginalFilename = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.Hash != null && this.Hash.OriginalValue != this.Hash.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Hash = null;
                }
    
                if (this.ThumbnailHash != null && this.ThumbnailHash.OriginalValue != this.ThumbnailHash.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ThumbnailHash = null;
                }
    
                if (this.MimeType != null && this.MimeType.OriginalValue != this.MimeType.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.MimeType = null;
                }
    
                if (this.Length != null && this.Length.OriginalValue != this.Length.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Length = null;
                }

                return hasChanges ? clone : null;
            }

            public ResourceDelta Clone()
            {
                return (ResourceDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Update
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        public partial class UpdateCommandDelta : DeltaBase
        {
            private readonly int? originalUnitId;

            private readonly int originalUpdateIndex;

            private readonly bool originalWasTransferred;

            private readonly bool originalWasInstalled;

            private readonly XmlData originalCommand;
 
            public UpdateCommandDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public UpdateCommandDelta(UpdateCommandReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUnitId = model.Unit == null ? null : (int?)model.Unit.Id;
                this.originalUpdateIndex = model.UpdateIndex;
                this.originalWasTransferred = model.WasTransferred;
                this.originalWasInstalled = model.WasInstalled;
                this.originalCommand = model.Command;
                this.Version = new Version(model.Version.Value);
            }
 
            public UpdateCommandDelta(UpdateCommand model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUnitId = model.Unit == null ? null : (int?)model.Unit.Id;
                this.originalUpdateIndex = model.UpdateIndex;
                this.originalWasTransferred = model.WasTransferred;
                this.originalWasInstalled = model.WasInstalled;
                this.originalCommand = model.Command;
                this.Version = new Version(model.Version);
            }
 
            public UpdateCommandDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Unit { get; private set; }
    
            public PropertyChange<int> UpdateIndex { get; private set; }
    
            public PropertyChange<bool> WasTransferred { get; private set; }
    
            public PropertyChange<bool> WasInstalled { get; private set; }
    
            public PropertyChange<XmlData> Command { get; private set; }

            public void ChangeUnit(int? id)
            {
                this.Unit = new ReferenceChange<int>(this.originalUnitId).ChangeReference(id);
            }

            public void ChangeUnit(Units.UnitReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Unit = new ReferenceChange<int>(this.originalUnitId).ChangeReference(id);
            }

            public void ChangeUpdateIndex(int value)
            {
                this.UpdateIndex = new PropertyChange<int>(this.originalUpdateIndex).ChangeValue(value);
            }

            public void ChangeWasTransferred(bool value)
            {
                this.WasTransferred = new PropertyChange<bool>(this.originalWasTransferred).ChangeValue(value);
            }

            public void ChangeWasInstalled(bool value)
            {
                this.WasInstalled = new PropertyChange<bool>(this.originalWasInstalled).ChangeValue(value);
            }

            public void ChangeCommand(XmlData value)
            {
                this.Command = new PropertyChange<XmlData>(this.originalCommand).ChangeValue(value);
            }

            public UpdateCommandDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Unit != null && this.Unit.OriginalReferenceId != this.Unit.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Unit = null;
                }
    
                if (this.UpdateIndex != null && this.UpdateIndex.OriginalValue != this.UpdateIndex.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UpdateIndex = null;
                }
    
                if (this.WasTransferred != null && this.WasTransferred.OriginalValue != this.WasTransferred.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.WasTransferred = null;
                }
    
                if (this.WasInstalled != null && this.WasInstalled.OriginalValue != this.WasInstalled.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.WasInstalled = null;
                }
    
                if (this.Command != null && this.Command.OriginalValue != this.Command.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Command = null;
                }

                return hasChanges ? clone : null;
            }

            public UpdateCommandDelta Clone()
            {
                return (UpdateCommandDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UpdateFeedbackDelta : DeltaBase
        {
            private readonly int? originalUpdateCommandId;

            private readonly DateTime originalTimestamp;

            private readonly UpdateState originalState;

            private readonly XmlData originalFeedback;
 
            public UpdateFeedbackDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public UpdateFeedbackDelta(UpdateFeedbackReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUpdateCommandId = model.UpdateCommand == null ? null : (int?)model.UpdateCommand.Id;
                this.originalTimestamp = model.Timestamp;
                this.originalState = model.State;
                this.originalFeedback = model.Feedback;
                this.Version = new Version(model.Version.Value);
            }
 
            public UpdateFeedbackDelta(UpdateFeedback model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUpdateCommandId = model.UpdateCommand == null ? null : (int?)model.UpdateCommand.Id;
                this.originalTimestamp = model.Timestamp;
                this.originalState = model.State;
                this.originalFeedback = model.Feedback;
                this.Version = new Version(model.Version);
            }
 
            public UpdateFeedbackDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> UpdateCommand { get; private set; }
    
            public PropertyChange<DateTime> Timestamp { get; private set; }
    
            public PropertyChange<UpdateState> State { get; private set; }
    
            public PropertyChange<XmlData> Feedback { get; private set; }

            public void ChangeUpdateCommand(int? id)
            {
                this.UpdateCommand = new ReferenceChange<int>(this.originalUpdateCommandId).ChangeReference(id);
            }

            public void ChangeUpdateCommand(UpdateCommandReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UpdateCommand = new ReferenceChange<int>(this.originalUpdateCommandId).ChangeReference(id);
            }

            public void ChangeTimestamp(DateTime value)
            {
                this.Timestamp = new PropertyChange<DateTime>(this.originalTimestamp).ChangeValue(value);
            }

            public void ChangeState(UpdateState value)
            {
                this.State = new PropertyChange<UpdateState>(this.originalState).ChangeValue(value);
            }

            public void ChangeFeedback(XmlData value)
            {
                this.Feedback = new PropertyChange<XmlData>(this.originalFeedback).ChangeValue(value);
            }

            public UpdateFeedbackDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UpdateCommand != null && this.UpdateCommand.OriginalReferenceId != this.UpdateCommand.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UpdateCommand = null;
                }
    
                if (this.Timestamp != null && this.Timestamp.OriginalValue != this.Timestamp.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Timestamp = null;
                }
    
                if (this.State != null && this.State.OriginalValue != this.State.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.State = null;
                }
    
                if (this.Feedback != null && this.Feedback.OriginalValue != this.Feedback.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Feedback = null;
                }

                return hasChanges ? clone : null;
            }

            public UpdateFeedbackDelta Clone()
            {
                return (UpdateFeedbackDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UpdateGroupDelta : DeltaBase
        {            private UserDefinedPropertiesDictionary userDefinedPropertiesDelta;

            private readonly int? originalTenantId;

            private readonly int? originalUnitConfigurationId;

            private readonly int? originalMediaConfigurationId;

            private readonly string originalName;

            private readonly string originalDescription;
 
            public UpdateGroupDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UpdateGroupDelta(UpdateGroupReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalUnitConfigurationId = model.UnitConfiguration == null ? null : (int?)model.UnitConfiguration.Id;
                this.originalMediaConfigurationId = model.MediaConfiguration == null ? null : (int?)model.MediaConfiguration.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version.Value);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UpdateGroupDelta(UpdateGroup model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalUnitConfigurationId = model.UnitConfiguration == null ? null : (int?)model.UnitConfiguration.Id;
                this.originalMediaConfigurationId = model.MediaConfiguration == null ? null : (int?)model.MediaConfiguration.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }
 
            public UpdateGroupDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
                this.UserDefinedProperties =
                    this.userDefinedPropertiesDelta = new UserDefinedPropertiesDictionary();
            }

            public IDictionary<string, string> UserDefinedPropertiesDelta
            {
                get
                {
                    return this.userDefinedPropertiesDelta;
                }
            }

            public IReadOnlyDictionary<string, string> UserDefinedProperties { get; private set; }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Tenant { get; private set; }
    
            public ReferenceChange<int> UnitConfiguration { get; private set; }
    
            public ReferenceChange<int> MediaConfiguration { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }

            public void ChangeTenant(int? id)
            {
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeTenant(Membership.TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeUnitConfiguration(int? id)
            {
                this.UnitConfiguration = new ReferenceChange<int>(this.originalUnitConfigurationId).ChangeReference(id);
            }

            public void ChangeUnitConfiguration(Configurations.UnitConfigurationReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UnitConfiguration = new ReferenceChange<int>(this.originalUnitConfigurationId).ChangeReference(id);
            }

            public void ChangeMediaConfiguration(int? id)
            {
                this.MediaConfiguration = new ReferenceChange<int>(this.originalMediaConfigurationId).ChangeReference(id);
            }

            public void ChangeMediaConfiguration(Configurations.MediaConfigurationReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.MediaConfiguration = new ReferenceChange<int>(this.originalMediaConfigurationId).ChangeReference(id);
            }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public UpdateGroupDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = true;
    
                if (this.Tenant != null && this.Tenant.OriginalReferenceId != this.Tenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Tenant = null;
                }
    
                if (this.UnitConfiguration != null && this.UnitConfiguration.OriginalReferenceId != this.UnitConfiguration.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UnitConfiguration = null;
                }
    
                if (this.MediaConfiguration != null && this.MediaConfiguration.OriginalReferenceId != this.MediaConfiguration.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.MediaConfiguration = null;
                }
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }

                return hasChanges ? clone : null;
            }

            public UpdateGroupDelta Clone()
            {
                return (UpdateGroupDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UpdatePartDelta : DeltaBase
        {
            private readonly int? originalUpdateGroupId;

            private readonly UpdatePartType originalType;

            private readonly DateTime originalStart;

            private readonly DateTime originalEnd;

            private readonly string originalDescription;

            private readonly XmlData originalStructure;

            private readonly XmlData originalInstallInstructions;

            private readonly XmlData originalDynamicContent;
 
            public UpdatePartDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public UpdatePartDelta(UpdatePartReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUpdateGroupId = model.UpdateGroup == null ? null : (int?)model.UpdateGroup.Id;
                this.originalType = model.Type;
                this.originalStart = model.Start;
                this.originalEnd = model.End;
                this.originalDescription = model.Description;
                this.originalStructure = model.Structure;
                this.originalInstallInstructions = model.InstallInstructions;
                this.originalDynamicContent = model.DynamicContent;
                this.Version = new Version(model.Version.Value);
            }
 
            public UpdatePartDelta(UpdatePart model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalUpdateGroupId = model.UpdateGroup == null ? null : (int?)model.UpdateGroup.Id;
                this.originalType = model.Type;
                this.originalStart = model.Start;
                this.originalEnd = model.End;
                this.originalDescription = model.Description;
                this.originalStructure = model.Structure;
                this.originalInstallInstructions = model.InstallInstructions;
                this.originalDynamicContent = model.DynamicContent;
                this.Version = new Version(model.Version);
            }
 
            public UpdatePartDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> UpdateGroup { get; private set; }
    
            public PropertyChange<UpdatePartType> Type { get; private set; }
    
            public PropertyChange<DateTime> Start { get; private set; }
    
            public PropertyChange<DateTime> End { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<XmlData> Structure { get; private set; }
    
            public PropertyChange<XmlData> InstallInstructions { get; private set; }
    
            public PropertyChange<XmlData> DynamicContent { get; private set; }

            public void ChangeUpdateGroup(int? id)
            {
                this.UpdateGroup = new ReferenceChange<int>(this.originalUpdateGroupId).ChangeReference(id);
            }

            public void ChangeUpdateGroup(UpdateGroupReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.UpdateGroup = new ReferenceChange<int>(this.originalUpdateGroupId).ChangeReference(id);
            }

            public void ChangeType(UpdatePartType value)
            {
                this.Type = new PropertyChange<UpdatePartType>(this.originalType).ChangeValue(value);
            }

            public void ChangeStart(DateTime value)
            {
                this.Start = new PropertyChange<DateTime>(this.originalStart).ChangeValue(value);
            }

            public void ChangeEnd(DateTime value)
            {
                this.End = new PropertyChange<DateTime>(this.originalEnd).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeStructure(XmlData value)
            {
                this.Structure = new PropertyChange<XmlData>(this.originalStructure).ChangeValue(value);
            }

            public void ChangeInstallInstructions(XmlData value)
            {
                this.InstallInstructions = new PropertyChange<XmlData>(this.originalInstallInstructions).ChangeValue(value);
            }

            public void ChangeDynamicContent(XmlData value)
            {
                this.DynamicContent = new PropertyChange<XmlData>(this.originalDynamicContent).ChangeValue(value);
            }

            public UpdatePartDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.UpdateGroup != null && this.UpdateGroup.OriginalReferenceId != this.UpdateGroup.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.UpdateGroup = null;
                }
    
                if (this.Type != null && this.Type.OriginalValue != this.Type.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Type = null;
                }
    
                if (this.Start != null && this.Start.OriginalValue != this.Start.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Start = null;
                }
    
                if (this.End != null && this.End.OriginalValue != this.End.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.End = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.Structure != null && this.Structure.OriginalValue != this.Structure.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Structure = null;
                }
    
                if (this.InstallInstructions != null && this.InstallInstructions.OriginalValue != this.InstallInstructions.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.InstallInstructions = null;
                }
    
                if (this.DynamicContent != null && this.DynamicContent.OriginalValue != this.DynamicContent.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.DynamicContent = null;
                }

                return hasChanges ? clone : null;
            }

            public UpdatePartDelta Clone()
            {
                return (UpdatePartDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Documents
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        public partial class DocumentDelta : DeltaBase
        {
            private readonly int? originalTenantId;

            private readonly string originalName;

            private readonly string originalDescription;
 
            public DocumentDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public DocumentDelta(DocumentReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version.Value);
            }
 
            public DocumentDelta(Document model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalName = model.Name;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version);
            }
 
            public DocumentDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Tenant { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }

            public void ChangeTenant(int? id)
            {
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeTenant(Membership.TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public DocumentDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Tenant != null && this.Tenant.OriginalReferenceId != this.Tenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Tenant = null;
                }
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }

                return hasChanges ? clone : null;
            }

            public DocumentDelta Clone()
            {
                return (DocumentDelta)(this as ICloneable).Clone();
            }
        }

        public partial class DocumentVersionDelta : DeltaBase
        {
            private readonly int? originalDocumentId;

            private readonly int? originalCreatingUserId;

            private readonly int originalMajor;

            private readonly int originalMinor;

            private readonly string originalDescription;

            private readonly XmlData originalContent;
 
            public DocumentVersionDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public DocumentVersionDelta(DocumentVersionReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.originalCreatingUserId = model.CreatingUser == null ? null : (int?)model.CreatingUser.Id;
                this.originalMajor = model.Major;
                this.originalMinor = model.Minor;
                this.originalDescription = model.Description;
                this.originalContent = model.Content;
                this.Version = new Version(model.Version.Value);
            }
 
            public DocumentVersionDelta(DocumentVersion model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.originalCreatingUserId = model.CreatingUser == null ? null : (int?)model.CreatingUser.Id;
                this.originalMajor = model.Major;
                this.originalMinor = model.Minor;
                this.originalDescription = model.Description;
                this.originalContent = model.Content;
                this.Version = new Version(model.Version);
            }
 
            public DocumentVersionDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Document { get; private set; }
    
            public ReferenceChange<int> CreatingUser { get; private set; }
    
            public PropertyChange<int> Major { get; private set; }
    
            public PropertyChange<int> Minor { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<XmlData> Content { get; private set; }

            public void ChangeDocument(int? id)
            {
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public void ChangeDocument(DocumentReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public void ChangeCreatingUser(int? id)
            {
                this.CreatingUser = new ReferenceChange<int>(this.originalCreatingUserId).ChangeReference(id);
            }

            public void ChangeCreatingUser(Membership.UserReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.CreatingUser = new ReferenceChange<int>(this.originalCreatingUserId).ChangeReference(id);
            }

            public void ChangeMajor(int value)
            {
                this.Major = new PropertyChange<int>(this.originalMajor).ChangeValue(value);
            }

            public void ChangeMinor(int value)
            {
                this.Minor = new PropertyChange<int>(this.originalMinor).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeContent(XmlData value)
            {
                this.Content = new PropertyChange<XmlData>(this.originalContent).ChangeValue(value);
            }

            public DocumentVersionDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Document != null && this.Document.OriginalReferenceId != this.Document.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Document = null;
                }
    
                if (this.CreatingUser != null && this.CreatingUser.OriginalReferenceId != this.CreatingUser.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.CreatingUser = null;
                }
    
                if (this.Major != null && this.Major.OriginalValue != this.Major.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Major = null;
                }
    
                if (this.Minor != null && this.Minor.OriginalValue != this.Minor.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Minor = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.Content != null && this.Content.OriginalValue != this.Content.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Content = null;
                }

                return hasChanges ? clone : null;
            }

            public DocumentVersionDelta Clone()
            {
                return (DocumentVersionDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Software
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        public partial class PackageDelta : DeltaBase
        {
            private readonly string originalPackageId;

            private readonly string originalProductName;

            private readonly string originalDescription;
 
            public PackageDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public PackageDelta(PackageReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalPackageId = model.PackageId;
                this.originalProductName = model.ProductName;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version.Value);
            }
 
            public PackageDelta(Package model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalPackageId = model.PackageId;
                this.originalProductName = model.ProductName;
                this.originalDescription = model.Description;
                this.Version = new Version(model.Version);
            }
 
            public PackageDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public PropertyChange<string> PackageId { get; private set; }
    
            public PropertyChange<string> ProductName { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }

            public void ChangePackageId(string value)
            {
                this.PackageId = new PropertyChange<string>(this.originalPackageId).ChangeValue(value);
            }

            public void ChangeProductName(string value)
            {
                this.ProductName = new PropertyChange<string>(this.originalProductName).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public PackageDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.PackageId != null && this.PackageId.OriginalValue != this.PackageId.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.PackageId = null;
                }
    
                if (this.ProductName != null && this.ProductName.OriginalValue != this.ProductName.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ProductName = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }

                return hasChanges ? clone : null;
            }

            public PackageDelta Clone()
            {
                return (PackageDelta)(this as ICloneable).Clone();
            }
        }

        public partial class PackageVersionDelta : DeltaBase
        {
            private readonly int? originalPackageId;

            private readonly string originalSoftwareVersion;

            private readonly string originalDescription;

            private readonly XmlData originalStructure;
 
            public PackageVersionDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public PackageVersionDelta(PackageVersionReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalPackageId = model.Package == null ? null : (int?)model.Package.Id;
                this.originalSoftwareVersion = model.SoftwareVersion;
                this.originalDescription = model.Description;
                this.originalStructure = model.Structure;
                this.Version = new Version(model.Version.Value);
            }
 
            public PackageVersionDelta(PackageVersion model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalPackageId = model.Package == null ? null : (int?)model.Package.Id;
                this.originalSoftwareVersion = model.SoftwareVersion;
                this.originalDescription = model.Description;
                this.originalStructure = model.Structure;
                this.Version = new Version(model.Version);
            }
 
            public PackageVersionDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Package { get; private set; }
    
            public PropertyChange<string> SoftwareVersion { get; private set; }
    
            public PropertyChange<string> Description { get; private set; }
    
            public PropertyChange<XmlData> Structure { get; private set; }

            public void ChangePackage(int? id)
            {
                this.Package = new ReferenceChange<int>(this.originalPackageId).ChangeReference(id);
            }

            public void ChangePackage(PackageReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Package = new ReferenceChange<int>(this.originalPackageId).ChangeReference(id);
            }

            public void ChangeSoftwareVersion(string value)
            {
                this.SoftwareVersion = new PropertyChange<string>(this.originalSoftwareVersion).ChangeValue(value);
            }

            public void ChangeDescription(string value)
            {
                this.Description = new PropertyChange<string>(this.originalDescription).ChangeValue(value);
            }

            public void ChangeStructure(XmlData value)
            {
                this.Structure = new PropertyChange<XmlData>(this.originalStructure).ChangeValue(value);
            }

            public PackageVersionDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Package != null && this.Package.OriginalReferenceId != this.Package.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Package = null;
                }
    
                if (this.SoftwareVersion != null && this.SoftwareVersion.OriginalValue != this.SoftwareVersion.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.SoftwareVersion = null;
                }
    
                if (this.Description != null && this.Description.OriginalValue != this.Description.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Description = null;
                }
    
                if (this.Structure != null && this.Structure.OriginalValue != this.Structure.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Structure = null;
                }

                return hasChanges ? clone : null;
            }

            public PackageVersionDelta Clone()
            {
                return (PackageVersionDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Configurations
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        public partial class MediaConfigurationDelta : DeltaBase
        {
            private readonly int? originalDocumentId;
 
            public MediaConfigurationDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public MediaConfigurationDelta(MediaConfigurationReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.Version = new Version(model.Version.Value);
            }
 
            public MediaConfigurationDelta(MediaConfiguration model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.Version = new Version(model.Version);
            }
 
            public MediaConfigurationDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Document { get; private set; }

            public void ChangeDocument(int? id)
            {
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public void ChangeDocument(Documents.DocumentReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public MediaConfigurationDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Document != null && this.Document.OriginalReferenceId != this.Document.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Document = null;
                }

                return hasChanges ? clone : null;
            }

            public MediaConfigurationDelta Clone()
            {
                return (MediaConfigurationDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UnitConfigurationDelta : DeltaBase
        {
            private readonly int? originalDocumentId;

            private readonly int? originalProductTypeId;
 
            public UnitConfigurationDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public UnitConfigurationDelta(UnitConfigurationReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.originalProductTypeId = model.ProductType == null ? null : (int?)model.ProductType.Id;
                this.Version = new Version(model.Version.Value);
            }
 
            public UnitConfigurationDelta(UnitConfiguration model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalDocumentId = model.Document == null ? null : (int?)model.Document.Id;
                this.originalProductTypeId = model.ProductType == null ? null : (int?)model.ProductType.Id;
                this.Version = new Version(model.Version);
            }
 
            public UnitConfigurationDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Document { get; private set; }
    
            public ReferenceChange<int> ProductType { get; private set; }

            public void ChangeDocument(int? id)
            {
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public void ChangeDocument(Documents.DocumentReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Document = new ReferenceChange<int>(this.originalDocumentId).ChangeReference(id);
            }

            public void ChangeProductType(int? id)
            {
                this.ProductType = new ReferenceChange<int>(this.originalProductTypeId).ChangeReference(id);
            }

            public void ChangeProductType(Units.ProductTypeReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.ProductType = new ReferenceChange<int>(this.originalProductTypeId).ChangeReference(id);
            }

            public UnitConfigurationDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Document != null && this.Document.OriginalReferenceId != this.Document.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Document = null;
                }
    
                if (this.ProductType != null && this.ProductType.OriginalReferenceId != this.ProductType.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.ProductType = null;
                }

                return hasChanges ? clone : null;
            }

            public UnitConfigurationDelta Clone()
            {
                return (UnitConfigurationDelta)(this as ICloneable).Clone();
            }
        }
    }

    namespace Log
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;
    }

    namespace Meta
    {
        using System.IO;
        using System.Threading.Tasks;
        using System.Xml.Serialization;
        
        using Gorba.Center.Common.ServiceModel.Collections;
        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        public partial class SystemConfigDelta : DeltaBase
        {
            private readonly Guid originalSystemId;

            private readonly XmlData originalSettings;
 
            public SystemConfigDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public SystemConfigDelta(SystemConfigReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalSystemId = model.SystemId;
                this.originalSettings = model.Settings;
                this.Version = new Version(model.Version.Value);
            }
 
            public SystemConfigDelta(SystemConfig model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalSystemId = model.SystemId;
                this.originalSettings = model.Settings;
                this.Version = new Version(model.Version);
            }
 
            public SystemConfigDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public PropertyChange<Guid> SystemId { get; private set; }
    
            public PropertyChange<XmlData> Settings { get; private set; }

            public void ChangeSystemId(Guid value)
            {
                this.SystemId = new PropertyChange<Guid>(this.originalSystemId).ChangeValue(value);
            }

            public void ChangeSettings(XmlData value)
            {
                this.Settings = new PropertyChange<XmlData>(this.originalSettings).ChangeValue(value);
            }

            public SystemConfigDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.SystemId != null && this.SystemId.OriginalValue != this.SystemId.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.SystemId = null;
                }
    
                if (this.Settings != null && this.Settings.OriginalValue != this.Settings.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Settings = null;
                }

                return hasChanges ? clone : null;
            }

            public SystemConfigDelta Clone()
            {
                return (SystemConfigDelta)(this as ICloneable).Clone();
            }
        }

        public partial class UserDefinedPropertyDelta : DeltaBase
        {
            private readonly int? originalTenantId;

            private readonly UserDefinedPropertyEnabledEntity originalOwnerEntity;

            private readonly string originalName;
 
            public UserDefinedPropertyDelta()
                : base(DeltaOperation.Created)
            {
                this.Version = new Version(1);
            }
 
            public UserDefinedPropertyDelta(UserDefinedPropertyReadableModel model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalOwnerEntity = model.OwnerEntity;
                this.originalName = model.Name;
                this.Version = new Version(model.Version.Value);
            }
 
            public UserDefinedPropertyDelta(UserDefinedProperty model)
                : base(DeltaOperation.Updated)
            {
                this.Id = model.Id;
                this.originalTenantId = model.Tenant == null ? null : (int?)model.Tenant.Id;
                this.originalOwnerEntity = model.OwnerEntity;
                this.originalName = model.Name;
                this.Version = new Version(model.Version);
            }
 
            public UserDefinedPropertyDelta(int id, DeltaOperation operation)
                : base(operation)
            {
                this.Id = id;
                this.Version = new Version(1);
            }

            public int Id { get; private set; }
    
            public ReferenceChange<int> Tenant { get; private set; }
    
            public PropertyChange<UserDefinedPropertyEnabledEntity> OwnerEntity { get; private set; }
    
            public PropertyChange<string> Name { get; private set; }

            public void ChangeTenant(int? id)
            {
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeTenant(Membership.TenantReadableModel value)
            {
                var id = value == null ? (int?)null : value.Id;
                this.Tenant = new ReferenceChange<int>(this.originalTenantId).ChangeReference(id);
            }

            public void ChangeOwnerEntity(UserDefinedPropertyEnabledEntity value)
            {
                this.OwnerEntity = new PropertyChange<UserDefinedPropertyEnabledEntity>(this.originalOwnerEntity).ChangeValue(value);
            }

            public void ChangeName(string value)
            {
                this.Name = new PropertyChange<string>(this.originalName).ChangeValue(value);
            }

            public UserDefinedPropertyDelta GetChangedDelta()
            {
                var clone = this.Clone();
                if (this.DeltaOperation == DeltaOperation.Deleted || this.DeltaOperation == DeltaOperation.Created)
                {
                    return clone;
                }

                var hasChanges = false;
    
                if (this.Tenant != null && this.Tenant.OriginalReferenceId != this.Tenant.ReferenceId)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Tenant = null;
                }
    
                if (this.OwnerEntity != null && this.OwnerEntity.OriginalValue != this.OwnerEntity.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.OwnerEntity = null;
                }
    
                if (this.Name != null && this.Name.OriginalValue != this.Name.Value)
                {
                    hasChanges = true;
                }
                else
                {
                    clone.Name = null;
                }

                return hasChanges ? clone : null;
            }

            public UserDefinedPropertyDelta Clone()
            {
                return (UserDefinedPropertyDelta)(this as ICloneable).Clone();
            }
        }
    }
}
