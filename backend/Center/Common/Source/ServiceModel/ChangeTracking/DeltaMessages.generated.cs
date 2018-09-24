namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.Collections;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;

        public partial class AuthorizationDeltaMessage : DeltaMessageBase
        {
            public AuthorizationDeltaMessage()
            {
            }

            public AuthorizationDeltaMessage(AuthorizationDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.UserRole != null)
                {
                    this.UserRoleId = delta.UserRole.ReferenceId;
                    this.UserRoleChanged = true;
                }

                if (delta.DataScope != null)
                {
                    this.DataScope = delta.DataScope.Value;
                    this.DataScopeChanged = true;
                }

                if (delta.Permission != null)
                {
                    this.Permission = delta.Permission.Value;
                    this.PermissionChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UserRoleId { get; set; }
    
            public bool UserRoleChanged { get; set; }
    
            public DataScope DataScope { get; set; }
    
            public bool DataScopeChanged { get; set; }
    
            public Permission Permission { get; set; }
    
            public bool PermissionChanged { get; set; }

            public AuthorizationDelta CreateDelta()
            {
                var delta = new AuthorizationDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UserRoleChanged)
                {
                    delta.ChangeUserRole(this.UserRoleId);
                }
    
                if (this.DataScopeChanged)
                {
                    delta.ChangeDataScope(this.DataScope);
                }
    
                if (this.PermissionChanged)
                {
                    delta.ChangePermission(this.Permission);
                }

                return delta;
            }
        }

        public partial class UserRoleDeltaMessage : DeltaMessageBase
        {
            public UserRoleDeltaMessage()
            {
            }

            public UserRoleDeltaMessage(UserRoleDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                this.UserDefinedProperties = delta.UserDefinedProperties.Select(
                    udp => new Property { Name = udp.Key, Value = udp.Value }).ToList();
            }

            public List<Property> UserDefinedProperties { get; set; }

            public int Id { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }

            public UserRoleDelta CreateDelta()
            {
                var delta = new UserRoleDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }

				if (this.UserDefinedProperties != null)
				{
					foreach (var udp in this.UserDefinedProperties)
					{
						delta.UserDefinedPropertiesDelta[udp.Name] = udp.Value;
					}
				}

                return delta;
            }
        }
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;

        public partial class AssociationTenantUserUserRoleDeltaMessage : DeltaMessageBase
        {
            public AssociationTenantUserUserRoleDeltaMessage()
            {
            }

            public AssociationTenantUserUserRoleDeltaMessage(AssociationTenantUserUserRoleDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Tenant != null)
                {
                    this.TenantId = delta.Tenant.ReferenceId;
                    this.TenantChanged = true;
                }
                if (delta.User != null)
                {
                    this.UserId = delta.User.ReferenceId;
                    this.UserChanged = true;
                }
                if (delta.UserRole != null)
                {
                    this.UserRoleId = delta.UserRole.ReferenceId;
                    this.UserRoleChanged = true;
                }
            }

            public int Id { get; set; }

            public int? TenantId { get; set; }
    
            public bool TenantChanged { get; set; }

            public int? UserId { get; set; }
    
            public bool UserChanged { get; set; }

            public int? UserRoleId { get; set; }
    
            public bool UserRoleChanged { get; set; }

            public AssociationTenantUserUserRoleDelta CreateDelta()
            {
                var delta = new AssociationTenantUserUserRoleDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.TenantChanged)
                {
                    delta.ChangeTenant(this.TenantId);
                }
    
                if (this.UserChanged)
                {
                    delta.ChangeUser(this.UserId);
                }
    
                if (this.UserRoleChanged)
                {
                    delta.ChangeUserRole(this.UserRoleId);
                }

                return delta;
            }
        }

        public partial class TenantDeltaMessage : DeltaMessageBase
        {
            public TenantDeltaMessage()
            {
            }

            public TenantDeltaMessage(TenantDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                this.UserDefinedProperties = delta.UserDefinedProperties.Select(
                    udp => new Property { Name = udp.Key, Value = udp.Value }).ToList();
            }

            public List<Property> UserDefinedProperties { get; set; }

            public int Id { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }

            public TenantDelta CreateDelta()
            {
                var delta = new TenantDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }

				if (this.UserDefinedProperties != null)
				{
					foreach (var udp in this.UserDefinedProperties)
					{
						delta.UserDefinedPropertiesDelta[udp.Name] = udp.Value;
					}
				}

                return delta;
            }
        }

        public partial class UserDeltaMessage : DeltaMessageBase
        {
            public UserDeltaMessage()
            {
            }

            public UserDeltaMessage(UserDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.OwnerTenant != null)
                {
                    this.OwnerTenantId = delta.OwnerTenant.ReferenceId;
                    this.OwnerTenantChanged = true;
                }

                if (delta.Username != null)
                {
                    this.Username = delta.Username.Value;
                    this.UsernameChanged = true;
                }

                if (delta.Domain != null)
                {
                    this.Domain = delta.Domain.Value;
                    this.DomainChanged = true;
                }

                if (delta.HashedPassword != null)
                {
                    this.HashedPassword = delta.HashedPassword.Value;
                    this.HashedPasswordChanged = true;
                }

                if (delta.FirstName != null)
                {
                    this.FirstName = delta.FirstName.Value;
                    this.FirstNameChanged = true;
                }

                if (delta.LastName != null)
                {
                    this.LastName = delta.LastName.Value;
                    this.LastNameChanged = true;
                }

                if (delta.Email != null)
                {
                    this.Email = delta.Email.Value;
                    this.EmailChanged = true;
                }

                if (delta.Culture != null)
                {
                    this.Culture = delta.Culture.Value;
                    this.CultureChanged = true;
                }

                if (delta.TimeZone != null)
                {
                    this.TimeZone = delta.TimeZone.Value;
                    this.TimeZoneChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.LastLoginAttempt != null)
                {
                    this.LastLoginAttempt = delta.LastLoginAttempt.Value;
                    this.LastLoginAttemptChanged = true;
                }

                if (delta.LastSuccessfulLogin != null)
                {
                    this.LastSuccessfulLogin = delta.LastSuccessfulLogin.Value;
                    this.LastSuccessfulLoginChanged = true;
                }

                if (delta.ConsecutiveLoginFailures != null)
                {
                    this.ConsecutiveLoginFailures = delta.ConsecutiveLoginFailures.Value;
                    this.ConsecutiveLoginFailuresChanged = true;
                }

                if (delta.IsEnabled != null)
                {
                    this.IsEnabled = delta.IsEnabled.Value;
                    this.IsEnabledChanged = true;
                }

                this.UserDefinedProperties = delta.UserDefinedProperties.Select(
                    udp => new Property { Name = udp.Key, Value = udp.Value }).ToList();
            }

            public List<Property> UserDefinedProperties { get; set; }

            public int Id { get; set; }

            public int? OwnerTenantId { get; set; }
    
            public bool OwnerTenantChanged { get; set; }
    
            public string Username { get; set; }
    
            public bool UsernameChanged { get; set; }
    
            public string Domain { get; set; }
    
            public bool DomainChanged { get; set; }
    
            public string HashedPassword { get; set; }
    
            public bool HashedPasswordChanged { get; set; }
    
            public string FirstName { get; set; }
    
            public bool FirstNameChanged { get; set; }
    
            public string LastName { get; set; }
    
            public bool LastNameChanged { get; set; }
    
            public string Email { get; set; }
    
            public bool EmailChanged { get; set; }
    
            public string Culture { get; set; }
    
            public bool CultureChanged { get; set; }
    
            public string TimeZone { get; set; }
    
            public bool TimeZoneChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public DateTime? LastLoginAttempt { get; set; }
    
            public bool LastLoginAttemptChanged { get; set; }
    
            public DateTime? LastSuccessfulLogin { get; set; }
    
            public bool LastSuccessfulLoginChanged { get; set; }
    
            public int ConsecutiveLoginFailures { get; set; }
    
            public bool ConsecutiveLoginFailuresChanged { get; set; }
    
            public bool IsEnabled { get; set; }
    
            public bool IsEnabledChanged { get; set; }

            public UserDelta CreateDelta()
            {
                var delta = new UserDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.OwnerTenantChanged)
                {
                    delta.ChangeOwnerTenant(this.OwnerTenantId);
                }
    
                if (this.UsernameChanged)
                {
                    delta.ChangeUsername(this.Username);
                }
    
                if (this.DomainChanged)
                {
                    delta.ChangeDomain(this.Domain);
                }
    
                if (this.HashedPasswordChanged)
                {
                    delta.ChangeHashedPassword(this.HashedPassword);
                }
    
                if (this.FirstNameChanged)
                {
                    delta.ChangeFirstName(this.FirstName);
                }
    
                if (this.LastNameChanged)
                {
                    delta.ChangeLastName(this.LastName);
                }
    
                if (this.EmailChanged)
                {
                    delta.ChangeEmail(this.Email);
                }
    
                if (this.CultureChanged)
                {
                    delta.ChangeCulture(this.Culture);
                }
    
                if (this.TimeZoneChanged)
                {
                    delta.ChangeTimeZone(this.TimeZone);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.LastLoginAttemptChanged)
                {
                    delta.ChangeLastLoginAttempt(this.LastLoginAttempt);
                }
    
                if (this.LastSuccessfulLoginChanged)
                {
                    delta.ChangeLastSuccessfulLogin(this.LastSuccessfulLogin);
                }
    
                if (this.ConsecutiveLoginFailuresChanged)
                {
                    delta.ChangeConsecutiveLoginFailures(this.ConsecutiveLoginFailures);
                }
    
                if (this.IsEnabledChanged)
                {
                    delta.ChangeIsEnabled(this.IsEnabled);
                }

				if (this.UserDefinedProperties != null)
				{
					foreach (var udp in this.UserDefinedProperties)
					{
						delta.UserDefinedPropertiesDelta[udp.Name] = udp.Value;
					}
				}

                return delta;
            }
        }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;

        public partial class ProductTypeDeltaMessage : DeltaMessageBase
        {
            public ProductTypeDeltaMessage()
            {
            }

            public ProductTypeDeltaMessage(ProductTypeDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;

                if (delta.UnitType != null)
                {
                    this.UnitType = delta.UnitType.Value;
                    this.UnitTypeChanged = true;
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.HardwareDescriptor != null)
                {
                    this.HardwareDescriptorXml = delta.HardwareDescriptor.Value.Xml;
                    this.HardwareDescriptorType = delta.HardwareDescriptor.Value.Type;
                    this.HardwareDescriptorChanged = true;
                }
            }

            public int Id { get; set; }
    
            public UnitTypes UnitType { get; set; }
    
            public bool UnitTypeChanged { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string HardwareDescriptorXml { get; set; }
    
            public string HardwareDescriptorType { get; set; }
    
            public bool HardwareDescriptorChanged { get; set; }

            public ProductTypeDelta CreateDelta()
            {
                var delta = new ProductTypeDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UnitTypeChanged)
                {
                    delta.ChangeUnitType(this.UnitType);
                }
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.HardwareDescriptorChanged)
                {
                    delta.ChangeHardwareDescriptor(new XmlData(this.HardwareDescriptorXml, this.HardwareDescriptorType));
                }

                return delta;
            }
        }

        public partial class UnitDeltaMessage : DeltaMessageBase
        {
            public UnitDeltaMessage()
            {
            }

            public UnitDeltaMessage(UnitDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Tenant != null)
                {
                    this.TenantId = delta.Tenant.ReferenceId;
                    this.TenantChanged = true;
                }
                if (delta.ProductType != null)
                {
                    this.ProductTypeId = delta.ProductType.ReferenceId;
                    this.ProductTypeChanged = true;
                }
                if (delta.UpdateGroup != null)
                {
                    this.UpdateGroupId = delta.UpdateGroup.ReferenceId;
                    this.UpdateGroupChanged = true;
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.NetworkAddress != null)
                {
                    this.NetworkAddress = delta.NetworkAddress.Value;
                    this.NetworkAddressChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.IsConnected != null)
                {
                    this.IsConnected = delta.IsConnected.Value;
                    this.IsConnectedChanged = true;
                }

                this.UserDefinedProperties = delta.UserDefinedProperties.Select(
                    udp => new Property { Name = udp.Key, Value = udp.Value }).ToList();
            }

            public List<Property> UserDefinedProperties { get; set; }

            public int Id { get; set; }

            public int? TenantId { get; set; }
    
            public bool TenantChanged { get; set; }

            public int? ProductTypeId { get; set; }
    
            public bool ProductTypeChanged { get; set; }

            public int? UpdateGroupId { get; set; }
    
            public bool UpdateGroupChanged { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string NetworkAddress { get; set; }
    
            public bool NetworkAddressChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public bool IsConnected { get; set; }
    
            public bool IsConnectedChanged { get; set; }

            public UnitDelta CreateDelta()
            {
                var delta = new UnitDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.TenantChanged)
                {
                    delta.ChangeTenant(this.TenantId);
                }
    
                if (this.ProductTypeChanged)
                {
                    delta.ChangeProductType(this.ProductTypeId);
                }
    
                if (this.UpdateGroupChanged)
                {
                    delta.ChangeUpdateGroup(this.UpdateGroupId);
                }
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.NetworkAddressChanged)
                {
                    delta.ChangeNetworkAddress(this.NetworkAddress);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.IsConnectedChanged)
                {
                    delta.ChangeIsConnected(this.IsConnected);
                }

				if (this.UserDefinedProperties != null)
				{
					foreach (var udp in this.UserDefinedProperties)
					{
						delta.UserDefinedPropertiesDelta[udp.Name] = udp.Value;
					}
				}

                return delta;
            }
        }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;

        public partial class ContentResourceDeltaMessage : DeltaMessageBase
        {
            public ContentResourceDeltaMessage()
            {
            }

            public ContentResourceDeltaMessage(ContentResourceDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.UploadingUser != null)
                {
                    this.UploadingUserId = delta.UploadingUser.ReferenceId;
                    this.UploadingUserChanged = true;
                }

                if (delta.OriginalFilename != null)
                {
                    this.OriginalFilename = delta.OriginalFilename.Value;
                    this.OriginalFilenameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.ThumbnailHash != null)
                {
                    this.ThumbnailHash = delta.ThumbnailHash.Value;
                    this.ThumbnailHashChanged = true;
                }

                if (delta.Hash != null)
                {
                    this.Hash = delta.Hash.Value;
                    this.HashChanged = true;
                }

                if (delta.HashAlgorithmType != null)
                {
                    this.HashAlgorithmType = delta.HashAlgorithmType.Value;
                    this.HashAlgorithmTypeChanged = true;
                }

                if (delta.MimeType != null)
                {
                    this.MimeType = delta.MimeType.Value;
                    this.MimeTypeChanged = true;
                }

                if (delta.Length != null)
                {
                    this.Length = delta.Length.Value;
                    this.LengthChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UploadingUserId { get; set; }
    
            public bool UploadingUserChanged { get; set; }
    
            public string OriginalFilename { get; set; }
    
            public bool OriginalFilenameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string ThumbnailHash { get; set; }
    
            public bool ThumbnailHashChanged { get; set; }
    
            public string Hash { get; set; }
    
            public bool HashChanged { get; set; }
    
            public HashAlgorithmTypes HashAlgorithmType { get; set; }
    
            public bool HashAlgorithmTypeChanged { get; set; }
    
            public string MimeType { get; set; }
    
            public bool MimeTypeChanged { get; set; }
    
            public long Length { get; set; }
    
            public bool LengthChanged { get; set; }

            public ContentResourceDelta CreateDelta()
            {
                var delta = new ContentResourceDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UploadingUserChanged)
                {
                    delta.ChangeUploadingUser(this.UploadingUserId);
                }
    
                if (this.OriginalFilenameChanged)
                {
                    delta.ChangeOriginalFilename(this.OriginalFilename);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.ThumbnailHashChanged)
                {
                    delta.ChangeThumbnailHash(this.ThumbnailHash);
                }
    
                if (this.HashChanged)
                {
                    delta.ChangeHash(this.Hash);
                }
    
                if (this.HashAlgorithmTypeChanged)
                {
                    delta.ChangeHashAlgorithmType(this.HashAlgorithmType);
                }
    
                if (this.MimeTypeChanged)
                {
                    delta.ChangeMimeType(this.MimeType);
                }
    
                if (this.LengthChanged)
                {
                    delta.ChangeLength(this.Length);
                }

                return delta;
            }
        }

        public partial class ResourceDeltaMessage : DeltaMessageBase
        {
            public ResourceDeltaMessage()
            {
            }

            public ResourceDeltaMessage(ResourceDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.UploadingUser != null)
                {
                    this.UploadingUserId = delta.UploadingUser.ReferenceId;
                    this.UploadingUserChanged = true;
                }

                if (delta.OriginalFilename != null)
                {
                    this.OriginalFilename = delta.OriginalFilename.Value;
                    this.OriginalFilenameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.Hash != null)
                {
                    this.Hash = delta.Hash.Value;
                    this.HashChanged = true;
                }

                if (delta.ThumbnailHash != null)
                {
                    this.ThumbnailHash = delta.ThumbnailHash.Value;
                    this.ThumbnailHashChanged = true;
                }

                if (delta.MimeType != null)
                {
                    this.MimeType = delta.MimeType.Value;
                    this.MimeTypeChanged = true;
                }

                if (delta.Length != null)
                {
                    this.Length = delta.Length.Value;
                    this.LengthChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UploadingUserId { get; set; }
    
            public bool UploadingUserChanged { get; set; }
    
            public string OriginalFilename { get; set; }
    
            public bool OriginalFilenameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string Hash { get; set; }
    
            public bool HashChanged { get; set; }
    
            public string ThumbnailHash { get; set; }
    
            public bool ThumbnailHashChanged { get; set; }
    
            public string MimeType { get; set; }
    
            public bool MimeTypeChanged { get; set; }
    
            public long Length { get; set; }
    
            public bool LengthChanged { get; set; }

            public ResourceDelta CreateDelta()
            {
                var delta = new ResourceDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UploadingUserChanged)
                {
                    delta.ChangeUploadingUser(this.UploadingUserId);
                }
    
                if (this.OriginalFilenameChanged)
                {
                    delta.ChangeOriginalFilename(this.OriginalFilename);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.HashChanged)
                {
                    delta.ChangeHash(this.Hash);
                }
    
                if (this.ThumbnailHashChanged)
                {
                    delta.ChangeThumbnailHash(this.ThumbnailHash);
                }
    
                if (this.MimeTypeChanged)
                {
                    delta.ChangeMimeType(this.MimeType);
                }
    
                if (this.LengthChanged)
                {
                    delta.ChangeLength(this.Length);
                }

                return delta;
            }
        }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;

        public partial class UpdateCommandDeltaMessage : DeltaMessageBase
        {
            public UpdateCommandDeltaMessage()
            {
            }

            public UpdateCommandDeltaMessage(UpdateCommandDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Unit != null)
                {
                    this.UnitId = delta.Unit.ReferenceId;
                    this.UnitChanged = true;
                }

                if (delta.UpdateIndex != null)
                {
                    this.UpdateIndex = delta.UpdateIndex.Value;
                    this.UpdateIndexChanged = true;
                }

                if (delta.WasTransferred != null)
                {
                    this.WasTransferred = delta.WasTransferred.Value;
                    this.WasTransferredChanged = true;
                }

                if (delta.WasInstalled != null)
                {
                    this.WasInstalled = delta.WasInstalled.Value;
                    this.WasInstalledChanged = true;
                }

                if (delta.Command != null)
                {
                    this.CommandXml = delta.Command.Value.Xml;
                    this.CommandType = delta.Command.Value.Type;
                    this.CommandChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UnitId { get; set; }
    
            public bool UnitChanged { get; set; }
    
            public int UpdateIndex { get; set; }
    
            public bool UpdateIndexChanged { get; set; }
    
            public bool WasTransferred { get; set; }
    
            public bool WasTransferredChanged { get; set; }
    
            public bool WasInstalled { get; set; }
    
            public bool WasInstalledChanged { get; set; }
    
            public string CommandXml { get; set; }
    
            public string CommandType { get; set; }
    
            public bool CommandChanged { get; set; }

            public UpdateCommandDelta CreateDelta()
            {
                var delta = new UpdateCommandDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UnitChanged)
                {
                    delta.ChangeUnit(this.UnitId);
                }
    
                if (this.UpdateIndexChanged)
                {
                    delta.ChangeUpdateIndex(this.UpdateIndex);
                }
    
                if (this.WasTransferredChanged)
                {
                    delta.ChangeWasTransferred(this.WasTransferred);
                }
    
                if (this.WasInstalledChanged)
                {
                    delta.ChangeWasInstalled(this.WasInstalled);
                }
    
                if (this.CommandChanged)
                {
                    delta.ChangeCommand(new XmlData(this.CommandXml, this.CommandType));
                }

                return delta;
            }
        }

        public partial class UpdateFeedbackDeltaMessage : DeltaMessageBase
        {
            public UpdateFeedbackDeltaMessage()
            {
            }

            public UpdateFeedbackDeltaMessage(UpdateFeedbackDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.UpdateCommand != null)
                {
                    this.UpdateCommandId = delta.UpdateCommand.ReferenceId;
                    this.UpdateCommandChanged = true;
                }

                if (delta.Timestamp != null)
                {
                    this.Timestamp = delta.Timestamp.Value;
                    this.TimestampChanged = true;
                }

                if (delta.State != null)
                {
                    this.State = delta.State.Value;
                    this.StateChanged = true;
                }

                if (delta.Feedback != null)
                {
                    this.FeedbackXml = delta.Feedback.Value.Xml;
                    this.FeedbackType = delta.Feedback.Value.Type;
                    this.FeedbackChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UpdateCommandId { get; set; }
    
            public bool UpdateCommandChanged { get; set; }
    
            public DateTime Timestamp { get; set; }
    
            public bool TimestampChanged { get; set; }
    
            public UpdateState State { get; set; }
    
            public bool StateChanged { get; set; }
    
            public string FeedbackXml { get; set; }
    
            public string FeedbackType { get; set; }
    
            public bool FeedbackChanged { get; set; }

            public UpdateFeedbackDelta CreateDelta()
            {
                var delta = new UpdateFeedbackDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UpdateCommandChanged)
                {
                    delta.ChangeUpdateCommand(this.UpdateCommandId);
                }
    
                if (this.TimestampChanged)
                {
                    delta.ChangeTimestamp(this.Timestamp);
                }
    
                if (this.StateChanged)
                {
                    delta.ChangeState(this.State);
                }
    
                if (this.FeedbackChanged)
                {
                    delta.ChangeFeedback(new XmlData(this.FeedbackXml, this.FeedbackType));
                }

                return delta;
            }
        }

        public partial class UpdateGroupDeltaMessage : DeltaMessageBase
        {
            public UpdateGroupDeltaMessage()
            {
            }

            public UpdateGroupDeltaMessage(UpdateGroupDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Tenant != null)
                {
                    this.TenantId = delta.Tenant.ReferenceId;
                    this.TenantChanged = true;
                }
                if (delta.UnitConfiguration != null)
                {
                    this.UnitConfigurationId = delta.UnitConfiguration.ReferenceId;
                    this.UnitConfigurationChanged = true;
                }
                if (delta.MediaConfiguration != null)
                {
                    this.MediaConfigurationId = delta.MediaConfiguration.ReferenceId;
                    this.MediaConfigurationChanged = true;
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                this.UserDefinedProperties = delta.UserDefinedProperties.Select(
                    udp => new Property { Name = udp.Key, Value = udp.Value }).ToList();
            }

            public List<Property> UserDefinedProperties { get; set; }

            public int Id { get; set; }

            public int? TenantId { get; set; }
    
            public bool TenantChanged { get; set; }

            public int? UnitConfigurationId { get; set; }
    
            public bool UnitConfigurationChanged { get; set; }

            public int? MediaConfigurationId { get; set; }
    
            public bool MediaConfigurationChanged { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }

            public UpdateGroupDelta CreateDelta()
            {
                var delta = new UpdateGroupDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.TenantChanged)
                {
                    delta.ChangeTenant(this.TenantId);
                }
    
                if (this.UnitConfigurationChanged)
                {
                    delta.ChangeUnitConfiguration(this.UnitConfigurationId);
                }
    
                if (this.MediaConfigurationChanged)
                {
                    delta.ChangeMediaConfiguration(this.MediaConfigurationId);
                }
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }

				if (this.UserDefinedProperties != null)
				{
					foreach (var udp in this.UserDefinedProperties)
					{
						delta.UserDefinedPropertiesDelta[udp.Name] = udp.Value;
					}
				}

                return delta;
            }
        }

        public partial class UpdatePartDeltaMessage : DeltaMessageBase
        {
            public UpdatePartDeltaMessage()
            {
            }

            public UpdatePartDeltaMessage(UpdatePartDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.UpdateGroup != null)
                {
                    this.UpdateGroupId = delta.UpdateGroup.ReferenceId;
                    this.UpdateGroupChanged = true;
                }

                if (delta.Type != null)
                {
                    this.Type = delta.Type.Value;
                    this.TypeChanged = true;
                }

                if (delta.Start != null)
                {
                    this.Start = delta.Start.Value;
                    this.StartChanged = true;
                }

                if (delta.End != null)
                {
                    this.End = delta.End.Value;
                    this.EndChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.Structure != null)
                {
                    this.StructureXml = delta.Structure.Value.Xml;
                    this.StructureType = delta.Structure.Value.Type;
                    this.StructureChanged = true;
                }

                if (delta.InstallInstructions != null)
                {
                    this.InstallInstructionsXml = delta.InstallInstructions.Value.Xml;
                    this.InstallInstructionsType = delta.InstallInstructions.Value.Type;
                    this.InstallInstructionsChanged = true;
                }

                if (delta.DynamicContent != null)
                {
                    this.DynamicContentXml = delta.DynamicContent.Value.Xml;
                    this.DynamicContentType = delta.DynamicContent.Value.Type;
                    this.DynamicContentChanged = true;
                }
            }

            public int Id { get; set; }

            public int? UpdateGroupId { get; set; }
    
            public bool UpdateGroupChanged { get; set; }
    
            public UpdatePartType Type { get; set; }
    
            public bool TypeChanged { get; set; }
    
            public DateTime Start { get; set; }
    
            public bool StartChanged { get; set; }
    
            public DateTime End { get; set; }
    
            public bool EndChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string StructureXml { get; set; }
    
            public string StructureType { get; set; }
    
            public bool StructureChanged { get; set; }
    
            public string InstallInstructionsXml { get; set; }
    
            public string InstallInstructionsType { get; set; }
    
            public bool InstallInstructionsChanged { get; set; }
    
            public string DynamicContentXml { get; set; }
    
            public string DynamicContentType { get; set; }
    
            public bool DynamicContentChanged { get; set; }

            public UpdatePartDelta CreateDelta()
            {
                var delta = new UpdatePartDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.UpdateGroupChanged)
                {
                    delta.ChangeUpdateGroup(this.UpdateGroupId);
                }
    
                if (this.TypeChanged)
                {
                    delta.ChangeType(this.Type);
                }
    
                if (this.StartChanged)
                {
                    delta.ChangeStart(this.Start);
                }
    
                if (this.EndChanged)
                {
                    delta.ChangeEnd(this.End);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.StructureChanged)
                {
                    delta.ChangeStructure(new XmlData(this.StructureXml, this.StructureType));
                }
    
                if (this.InstallInstructionsChanged)
                {
                    delta.ChangeInstallInstructions(new XmlData(this.InstallInstructionsXml, this.InstallInstructionsType));
                }
    
                if (this.DynamicContentChanged)
                {
                    delta.ChangeDynamicContent(new XmlData(this.DynamicContentXml, this.DynamicContentType));
                }

                return delta;
            }
        }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;

        public partial class DocumentDeltaMessage : DeltaMessageBase
        {
            public DocumentDeltaMessage()
            {
            }

            public DocumentDeltaMessage(DocumentDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Tenant != null)
                {
                    this.TenantId = delta.Tenant.ReferenceId;
                    this.TenantChanged = true;
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }
            }

            public int Id { get; set; }

            public int? TenantId { get; set; }
    
            public bool TenantChanged { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }

            public DocumentDelta CreateDelta()
            {
                var delta = new DocumentDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.TenantChanged)
                {
                    delta.ChangeTenant(this.TenantId);
                }
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }

                return delta;
            }
        }

        public partial class DocumentVersionDeltaMessage : DeltaMessageBase
        {
            public DocumentVersionDeltaMessage()
            {
            }

            public DocumentVersionDeltaMessage(DocumentVersionDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Document != null)
                {
                    this.DocumentId = delta.Document.ReferenceId;
                    this.DocumentChanged = true;
                }
                if (delta.CreatingUser != null)
                {
                    this.CreatingUserId = delta.CreatingUser.ReferenceId;
                    this.CreatingUserChanged = true;
                }

                if (delta.Major != null)
                {
                    this.Major = delta.Major.Value;
                    this.MajorChanged = true;
                }

                if (delta.Minor != null)
                {
                    this.Minor = delta.Minor.Value;
                    this.MinorChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.Content != null)
                {
                    this.ContentXml = delta.Content.Value.Xml;
                    this.ContentType = delta.Content.Value.Type;
                    this.ContentChanged = true;
                }
            }

            public int Id { get; set; }

            public int? DocumentId { get; set; }
    
            public bool DocumentChanged { get; set; }

            public int? CreatingUserId { get; set; }
    
            public bool CreatingUserChanged { get; set; }
    
            public int Major { get; set; }
    
            public bool MajorChanged { get; set; }
    
            public int Minor { get; set; }
    
            public bool MinorChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string ContentXml { get; set; }
    
            public string ContentType { get; set; }
    
            public bool ContentChanged { get; set; }

            public DocumentVersionDelta CreateDelta()
            {
                var delta = new DocumentVersionDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.DocumentChanged)
                {
                    delta.ChangeDocument(this.DocumentId);
                }
    
                if (this.CreatingUserChanged)
                {
                    delta.ChangeCreatingUser(this.CreatingUserId);
                }
    
                if (this.MajorChanged)
                {
                    delta.ChangeMajor(this.Major);
                }
    
                if (this.MinorChanged)
                {
                    delta.ChangeMinor(this.Minor);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.ContentChanged)
                {
                    delta.ChangeContent(new XmlData(this.ContentXml, this.ContentType));
                }

                return delta;
            }
        }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;

        public partial class PackageDeltaMessage : DeltaMessageBase
        {
            public PackageDeltaMessage()
            {
            }

            public PackageDeltaMessage(PackageDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;

                if (delta.PackageId != null)
                {
                    this.PackageId = delta.PackageId.Value;
                    this.PackageIdChanged = true;
                }

                if (delta.ProductName != null)
                {
                    this.ProductName = delta.ProductName.Value;
                    this.ProductNameChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }
            }

            public int Id { get; set; }
    
            public string PackageId { get; set; }
    
            public bool PackageIdChanged { get; set; }
    
            public string ProductName { get; set; }
    
            public bool ProductNameChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }

            public PackageDelta CreateDelta()
            {
                var delta = new PackageDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.PackageIdChanged)
                {
                    delta.ChangePackageId(this.PackageId);
                }
    
                if (this.ProductNameChanged)
                {
                    delta.ChangeProductName(this.ProductName);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }

                return delta;
            }
        }

        public partial class PackageVersionDeltaMessage : DeltaMessageBase
        {
            public PackageVersionDeltaMessage()
            {
            }

            public PackageVersionDeltaMessage(PackageVersionDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Package != null)
                {
                    this.PackageId = delta.Package.ReferenceId;
                    this.PackageChanged = true;
                }

                if (delta.SoftwareVersion != null)
                {
                    this.SoftwareVersion = delta.SoftwareVersion.Value;
                    this.SoftwareVersionChanged = true;
                }

                if (delta.Description != null)
                {
                    this.Description = delta.Description.Value;
                    this.DescriptionChanged = true;
                }

                if (delta.Structure != null)
                {
                    this.StructureXml = delta.Structure.Value.Xml;
                    this.StructureType = delta.Structure.Value.Type;
                    this.StructureChanged = true;
                }
            }

            public int Id { get; set; }

            public int? PackageId { get; set; }
    
            public bool PackageChanged { get; set; }
    
            public string SoftwareVersion { get; set; }
    
            public bool SoftwareVersionChanged { get; set; }
    
            public string Description { get; set; }
    
            public bool DescriptionChanged { get; set; }
    
            public string StructureXml { get; set; }
    
            public string StructureType { get; set; }
    
            public bool StructureChanged { get; set; }

            public PackageVersionDelta CreateDelta()
            {
                var delta = new PackageVersionDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.PackageChanged)
                {
                    delta.ChangePackage(this.PackageId);
                }
    
                if (this.SoftwareVersionChanged)
                {
                    delta.ChangeSoftwareVersion(this.SoftwareVersion);
                }
    
                if (this.DescriptionChanged)
                {
                    delta.ChangeDescription(this.Description);
                }
    
                if (this.StructureChanged)
                {
                    delta.ChangeStructure(new XmlData(this.StructureXml, this.StructureType));
                }

                return delta;
            }
        }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;

        public partial class MediaConfigurationDeltaMessage : DeltaMessageBase
        {
            public MediaConfigurationDeltaMessage()
            {
            }

            public MediaConfigurationDeltaMessage(MediaConfigurationDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Document != null)
                {
                    this.DocumentId = delta.Document.ReferenceId;
                    this.DocumentChanged = true;
                }
            }

            public int Id { get; set; }

            public int? DocumentId { get; set; }
    
            public bool DocumentChanged { get; set; }

            public MediaConfigurationDelta CreateDelta()
            {
                var delta = new MediaConfigurationDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.DocumentChanged)
                {
                    delta.ChangeDocument(this.DocumentId);
                }

                return delta;
            }
        }

        public partial class UnitConfigurationDeltaMessage : DeltaMessageBase
        {
            public UnitConfigurationDeltaMessage()
            {
            }

            public UnitConfigurationDeltaMessage(UnitConfigurationDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Document != null)
                {
                    this.DocumentId = delta.Document.ReferenceId;
                    this.DocumentChanged = true;
                }
                if (delta.ProductType != null)
                {
                    this.ProductTypeId = delta.ProductType.ReferenceId;
                    this.ProductTypeChanged = true;
                }
            }

            public int Id { get; set; }

            public int? DocumentId { get; set; }
    
            public bool DocumentChanged { get; set; }

            public int? ProductTypeId { get; set; }
    
            public bool ProductTypeChanged { get; set; }

            public UnitConfigurationDelta CreateDelta()
            {
                var delta = new UnitConfigurationDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.DocumentChanged)
                {
                    delta.ChangeDocument(this.DocumentId);
                }
    
                if (this.ProductTypeChanged)
                {
                    delta.ChangeProductType(this.ProductTypeId);
                }

                return delta;
            }
        }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;

        public partial class SystemConfigDeltaMessage : DeltaMessageBase
        {
            public SystemConfigDeltaMessage()
            {
            }

            public SystemConfigDeltaMessage(SystemConfigDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;

                if (delta.SystemId != null)
                {
                    this.SystemId = delta.SystemId.Value;
                    this.SystemIdChanged = true;
                }

                if (delta.Settings != null)
                {
                    this.SettingsXml = delta.Settings.Value.Xml;
                    this.SettingsType = delta.Settings.Value.Type;
                    this.SettingsChanged = true;
                }
            }

            public int Id { get; set; }
    
            public Guid SystemId { get; set; }
    
            public bool SystemIdChanged { get; set; }
    
            public string SettingsXml { get; set; }
    
            public string SettingsType { get; set; }
    
            public bool SettingsChanged { get; set; }

            public SystemConfigDelta CreateDelta()
            {
                var delta = new SystemConfigDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.SystemIdChanged)
                {
                    delta.ChangeSystemId(this.SystemId);
                }
    
                if (this.SettingsChanged)
                {
                    delta.ChangeSettings(new XmlData(this.SettingsXml, this.SettingsType));
                }

                return delta;
            }
        }

        public partial class UserDefinedPropertyDeltaMessage : DeltaMessageBase
        {
            public UserDefinedPropertyDeltaMessage()
            {
            }

            public UserDefinedPropertyDeltaMessage(UserDefinedPropertyDelta delta)
                : base (delta)
            {
                this.Id = delta.Id;
                if (delta.Tenant != null)
                {
                    this.TenantId = delta.Tenant.ReferenceId;
                    this.TenantChanged = true;
                }

                if (delta.OwnerEntity != null)
                {
                    this.OwnerEntity = delta.OwnerEntity.Value;
                    this.OwnerEntityChanged = true;
                }

                if (delta.Name != null)
                {
                    this.Name = delta.Name.Value;
                    this.NameChanged = true;
                }
            }

            public int Id { get; set; }

            public int? TenantId { get; set; }
    
            public bool TenantChanged { get; set; }
    
            public UserDefinedPropertyEnabledEntity OwnerEntity { get; set; }
    
            public bool OwnerEntityChanged { get; set; }
    
            public string Name { get; set; }
    
            public bool NameChanged { get; set; }

            public UserDefinedPropertyDelta CreateDelta()
            {
                var delta = new UserDefinedPropertyDelta(this.Id, this.DeltaOperation);
                this.FillDelta(delta);
    
                if (this.TenantChanged)
                {
                    delta.ChangeTenant(this.TenantId);
                }
    
                if (this.OwnerEntityChanged)
                {
                    delta.ChangeOwnerEntity(this.OwnerEntity);
                }
    
                if (this.NameChanged)
                {
                    delta.ChangeName(this.Name);
                }

                return delta;
            }
        }
    }
}
