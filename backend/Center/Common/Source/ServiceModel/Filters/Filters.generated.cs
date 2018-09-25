namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    using Filters;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;

        [DataContract]
        [KnownType(typeof(DataScope))] //here
        [KnownType(typeof(Permission))] //here
        [KnownType(typeof(DataScopePropertyValueFilter))]
        [KnownType(typeof(PermissionPropertyValueFilter))]
        public partial class AuthorizationFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public UserRoleFilter UserRole { get; set; }

            [DataMember]
            public DataScopePropertyValueFilter DataScope { get; set; }

            [DataMember]
            public PermissionPropertyValueFilter Permission { get; set; }

            public static AuthorizationFilter Create()
            {
                return new AuthorizationFilter();
            }

            [DataContract]
            public class DataScopePropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class PermissionPropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UserRoleFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public AuthorizationFilter Authorizations { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            public static UserRoleFilter Create()
            {
                return new UserRoleFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;

        [DataContract]
        public partial class MediaConfigurationFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Documents.DocumentFilter Document { get; set; }

            [DataMember]
            public Update.UpdateGroupFilter UpdateGroups { get; set; }

            public static MediaConfigurationFilter Create()
            {
                return new MediaConfigurationFilter();
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        public partial class UnitConfigurationFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Documents.DocumentFilter Document { get; set; }

            [DataMember]
            public Units.ProductTypeFilter ProductType { get; set; }

            [DataMember]
            public Update.UpdateGroupFilter UpdateGroups { get; set; }

            public static UnitConfigurationFilter Create()
            {
                return new UnitConfigurationFilter();
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class DocumentFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Membership.TenantFilter Tenant { get; set; }

            [DataMember]
            public DocumentVersionFilter Versions { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            public static DocumentFilter Create()
            {
                return new DocumentFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(MajorPropertyValueFilter))]
        [KnownType(typeof(MinorPropertyValueFilter))]
        public partial class DocumentVersionFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Membership.UserFilter CreatingUser { get; set; }

            [DataMember]
            public DocumentFilter Document { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public MajorPropertyValueFilter Major { get; set; }

            [DataMember]
            public MinorPropertyValueFilter Minor { get; set; }

            [DataMember]
            public bool IncludeContent { get; set; }

            public static DocumentVersionFilter Create()
            {
                return new DocumentVersionFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class MajorPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class MinorPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;

        [DataContract]
        [KnownType(typeof(Level))] //here
        [KnownType(typeof(LevelPropertyValueFilter))]
        [KnownType(typeof(AdditionalDataPropertyValueFilter))]
        [KnownType(typeof(ApplicationPropertyValueFilter))]
        [KnownType(typeof(LoggerPropertyValueFilter))]
        [KnownType(typeof(MessagePropertyValueFilter))]
        [KnownType(typeof(TimestampPropertyValueFilter))]
        public partial class LogEntryFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public Units.UnitFilter Unit { get; set; }

            [DataMember]
            public AdditionalDataPropertyValueFilter AdditionalData { get; set; }

            [DataMember]
            public ApplicationPropertyValueFilter Application { get; set; }

            [DataMember]
            public LoggerPropertyValueFilter Logger { get; set; }

            [DataMember]
            public MessagePropertyValueFilter Message { get; set; }

            [DataMember]
            public TimestampPropertyValueFilter Timestamp { get; set; }

            [DataMember]
            public LevelPropertyValueFilter Level { get; set; }

            public static LogEntryFilter Create()
            {
                return new LogEntryFilter();
            }

            [DataContract]
            public class AdditionalDataPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class ApplicationPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class LoggerPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class MessagePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class TimestampPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LevelPropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;

        [DataContract]
        public partial class AssociationTenantUserUserRoleFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public TenantFilter Tenant { get; set; }

            [DataMember]
            public UserFilter User { get; set; }

            [DataMember]
            public AccessControl.UserRoleFilter UserRole { get; set; }

            public static AssociationTenantUserUserRoleFilter Create()
            {
                return new AssociationTenantUserUserRoleFilter();
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class TenantFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Update.UpdateGroupFilter UpdateGroups { get; set; }

            [DataMember]
            public UserFilter Users { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            public static TenantFilter Create()
            {
                return new TenantFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(ConsecutiveLoginFailuresPropertyValueFilter))]
        [KnownType(typeof(CulturePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(DomainPropertyValueFilter))]
        [KnownType(typeof(EmailPropertyValueFilter))]
        [KnownType(typeof(FirstNamePropertyValueFilter))]
        [KnownType(typeof(HashedPasswordPropertyValueFilter))]
        [KnownType(typeof(IsEnabledPropertyValueFilter))]
        [KnownType(typeof(LastLoginAttemptPropertyValueFilter))]
        [KnownType(typeof(LastNamePropertyValueFilter))]
        [KnownType(typeof(LastSuccessfulLoginPropertyValueFilter))]
        [KnownType(typeof(TimeZonePropertyValueFilter))]
        [KnownType(typeof(UsernamePropertyValueFilter))]
        public partial class UserFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public TenantFilter OwnerTenant { get; set; }

            [DataMember]
            public AssociationTenantUserUserRoleFilter AssociationTenantUserUserRoles { get; set; }

            [DataMember]
            public ConsecutiveLoginFailuresPropertyValueFilter ConsecutiveLoginFailures { get; set; }

            [DataMember]
            public CulturePropertyValueFilter Culture { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public DomainPropertyValueFilter Domain { get; set; }

            [DataMember]
            public EmailPropertyValueFilter Email { get; set; }

            [DataMember]
            public FirstNamePropertyValueFilter FirstName { get; set; }

            [DataMember]
            public HashedPasswordPropertyValueFilter HashedPassword { get; set; }

            [DataMember]
            public IsEnabledPropertyValueFilter IsEnabled { get; set; }

            [DataMember]
            public LastLoginAttemptPropertyValueFilter LastLoginAttempt { get; set; }

            [DataMember]
            public LastNamePropertyValueFilter LastName { get; set; }

            [DataMember]
            public LastSuccessfulLoginPropertyValueFilter LastSuccessfulLogin { get; set; }

            [DataMember]
            public TimeZonePropertyValueFilter TimeZone { get; set; }

            [DataMember]
            public UsernamePropertyValueFilter Username { get; set; }

            public static UserFilter Create()
            {
                return new UserFilter();
            }

            [DataContract]
            public class ConsecutiveLoginFailuresPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CulturePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class DomainPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class EmailPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class FirstNamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class HashedPasswordPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IsEnabledPropertyValueFilter : BooleanPropertyValueFilter
            {
            }

            [DataContract]
            public class LastLoginAttemptPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastNamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class LastSuccessfulLoginPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class TimeZonePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class UsernamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;

        [DataContract]
        [KnownType(typeof(SystemIdPropertyValueFilter))]
        public partial class SystemConfigFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public SystemIdPropertyValueFilter SystemId { get; set; }

            [DataMember]
            public bool IncludeSettings { get; set; }

            public static SystemConfigFilter Create()
            {
                return new SystemConfigFilter();
            }

            [DataContract]
            public class SystemIdPropertyValueFilter : GuidPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(UserDefinedPropertyEnabledEntity))] //here
        [KnownType(typeof(OwnerEntityPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UserDefinedPropertyFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Membership.TenantFilter Tenant { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            [DataMember]
            public OwnerEntityPropertyValueFilter OwnerEntity { get; set; }

            public static UserDefinedPropertyFilter Create()
            {
                return new UserDefinedPropertyFilter();
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class OwnerEntityPropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;

        [DataContract]
        [KnownType(typeof(HashAlgorithmTypes))] //here
        [KnownType(typeof(HashAlgorithmTypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(HashPropertyValueFilter))]
        [KnownType(typeof(LengthPropertyValueFilter))]
        [KnownType(typeof(MimeTypePropertyValueFilter))]
        [KnownType(typeof(OriginalFilenamePropertyValueFilter))]
        [KnownType(typeof(ThumbnailHashPropertyValueFilter))]
        public partial class ContentResourceFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Membership.UserFilter UploadingUser { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public HashPropertyValueFilter Hash { get; set; }

            [DataMember]
            public LengthPropertyValueFilter Length { get; set; }

            [DataMember]
            public MimeTypePropertyValueFilter MimeType { get; set; }

            [DataMember]
            public OriginalFilenamePropertyValueFilter OriginalFilename { get; set; }

            [DataMember]
            public ThumbnailHashPropertyValueFilter ThumbnailHash { get; set; }

            [DataMember]
            public HashAlgorithmTypePropertyValueFilter HashAlgorithmType { get; set; }

            public static ContentResourceFilter Create()
            {
                return new ContentResourceFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class HashPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class LengthPropertyValueFilter : Int64PropertyValueFilter
            {
            }

            [DataContract]
            public class MimeTypePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class OriginalFilenamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class ThumbnailHashPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class HashAlgorithmTypePropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(HashPropertyValueFilter))]
        [KnownType(typeof(LengthPropertyValueFilter))]
        [KnownType(typeof(MimeTypePropertyValueFilter))]
        [KnownType(typeof(OriginalFilenamePropertyValueFilter))]
        [KnownType(typeof(ThumbnailHashPropertyValueFilter))]
        public partial class ResourceFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Membership.UserFilter UploadingUser { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public HashPropertyValueFilter Hash { get; set; }

            [DataMember]
            public LengthPropertyValueFilter Length { get; set; }

            [DataMember]
            public MimeTypePropertyValueFilter MimeType { get; set; }

            [DataMember]
            public OriginalFilenamePropertyValueFilter OriginalFilename { get; set; }

            [DataMember]
            public ThumbnailHashPropertyValueFilter ThumbnailHash { get; set; }

            public static ResourceFilter Create()
            {
                return new ResourceFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class HashPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class LengthPropertyValueFilter : Int64PropertyValueFilter
            {
            }

            [DataContract]
            public class MimeTypePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class OriginalFilenamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class ThumbnailHashPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(PackageIdPropertyValueFilter))]
        [KnownType(typeof(ProductNamePropertyValueFilter))]
        public partial class PackageFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public PackageVersionFilter Versions { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public PackageIdPropertyValueFilter PackageId { get; set; }

            [DataMember]
            public ProductNamePropertyValueFilter ProductName { get; set; }

            public static PackageFilter Create()
            {
                return new PackageFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class PackageIdPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class ProductNamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(SoftwareVersionPropertyValueFilter))]
        public partial class PackageVersionFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public PackageFilter Package { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public SoftwareVersionPropertyValueFilter SoftwareVersion { get; set; }

            [DataMember]
            public bool IncludeStructure { get; set; }

            public static PackageVersionFilter Create()
            {
                return new PackageVersionFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class SoftwareVersionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;

        [DataContract]
        [KnownType(typeof(UnitTypes))] //here
        [KnownType(typeof(UnitTypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class ProductTypeFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public UnitFilter Units { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            [DataMember]
            public UnitTypePropertyValueFilter UnitType { get; set; }

            [DataMember]
            public bool IncludeHardwareDescriptor { get; set; }

            public static ProductTypeFilter Create()
            {
                return new ProductTypeFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class UnitTypePropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(IsConnectedPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        [KnownType(typeof(NetworkAddressPropertyValueFilter))]
        public partial class UnitFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public ProductTypeFilter ProductType { get; set; }

            [DataMember]
            public Membership.TenantFilter Tenant { get; set; }

            [DataMember]
            public Update.UpdateGroupFilter UpdateGroup { get; set; }

            [DataMember]
            public Update.UpdateCommandFilter UpdateCommands { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public IsConnectedPropertyValueFilter IsConnected { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            [DataMember]
            public NetworkAddressPropertyValueFilter NetworkAddress { get; set; }

            public static UnitFilter Create()
            {
                return new UnitFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IsConnectedPropertyValueFilter : BooleanPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NetworkAddressPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;

        [DataContract]
        [KnownType(typeof(UpdateIndexPropertyValueFilter))]
        [KnownType(typeof(WasInstalledPropertyValueFilter))]
        [KnownType(typeof(WasTransferredPropertyValueFilter))]
        public partial class UpdateCommandFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Units.UnitFilter Unit { get; set; }

            [DataMember]
            public UpdateFeedbackFilter Feedbacks { get; set; }

            [DataMember]
            public UpdatePartFilter IncludedParts { get; set; }

            [DataMember]
            public UpdateIndexPropertyValueFilter UpdateIndex { get; set; }

            [DataMember]
            public WasInstalledPropertyValueFilter WasInstalled { get; set; }

            [DataMember]
            public WasTransferredPropertyValueFilter WasTransferred { get; set; }

            [DataMember]
            public bool IncludeCommand { get; set; }

            public static UpdateCommandFilter Create()
            {
                return new UpdateCommandFilter();
            }

            [DataContract]
            public class UpdateIndexPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class WasInstalledPropertyValueFilter : BooleanPropertyValueFilter
            {
            }

            [DataContract]
            public class WasTransferredPropertyValueFilter : BooleanPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(UpdateState))] //here
        [KnownType(typeof(StatePropertyValueFilter))]
        [KnownType(typeof(TimestampPropertyValueFilter))]
        public partial class UpdateFeedbackFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public UpdateCommandFilter UpdateCommand { get; set; }

            [DataMember]
            public TimestampPropertyValueFilter Timestamp { get; set; }

            [DataMember]
            public StatePropertyValueFilter State { get; set; }

            [DataMember]
            public bool IncludeFeedback { get; set; }

            public static UpdateFeedbackFilter Create()
            {
                return new UpdateFeedbackFilter();
            }

            [DataContract]
            public class TimestampPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class StatePropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UpdateGroupFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public Configurations.MediaConfigurationFilter MediaConfiguration { get; set; }

            [DataMember]
            public Membership.TenantFilter Tenant { get; set; }

            [DataMember]
            public Configurations.UnitConfigurationFilter UnitConfiguration { get; set; }

            [DataMember]
            public Units.UnitFilter Units { get; set; }

            [DataMember]
            public UpdatePartFilter UpdateParts { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public NamePropertyValueFilter Name { get; set; }

            public static UpdateGroupFilter Create()
            {
                return new UpdateGroupFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class NamePropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }

        [DataContract]
        [KnownType(typeof(UpdatePartType))] //here
        [KnownType(typeof(TypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(EndPropertyValueFilter))]
        [KnownType(typeof(StartPropertyValueFilter))]
        public partial class UpdatePartFilter
        {
            [DataMember]
            public IdPropertyValueFilter Id { get; set; }

            [DataMember]
            public CreatedOnPropertyValueFilter CreatedOn { get; set; }

            [DataMember]
            public LastModifiedOnPropertyValueFilter LastModifiedOn { get; set; }

            [DataMember]
            public VersionPropertyValueFilter Version { get; set; }

            [DataMember]
            public UpdateGroupFilter UpdateGroup { get; set; }

            [DataMember]
            public UpdateCommandFilter RelatedCommands { get; set; }

            [DataMember]
            public DescriptionPropertyValueFilter Description { get; set; }

            [DataMember]
            public EndPropertyValueFilter End { get; set; }

            [DataMember]
            public StartPropertyValueFilter Start { get; set; }

            [DataMember]
            public TypePropertyValueFilter Type { get; set; }

            [DataMember]
            public bool IncludeDynamicContent { get; set; }

            [DataMember]
            public bool IncludeInstallInstructions { get; set; }

            [DataMember]
            public bool IncludeStructure { get; set; }

            public static UpdatePartFilter Create()
            {
                return new UpdatePartFilter();
            }

            [DataContract]
            public class DescriptionPropertyValueFilter : StringPropertyValueFilter
            {
            }

            [DataContract]
            public class EndPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class StartPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class TypePropertyValueFilter : EnumPropertyValueFilter
            {
            }

            [DataContract]
            public class IdPropertyValueFilter : Int32PropertyValueFilter
            {
            }

            [DataContract]
            public class CreatedOnPropertyValueFilter : DateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class LastModifiedOnPropertyValueFilter : NullableDateTimePropertyValueFilter
            {
            }

            [DataContract]
            public class VersionPropertyValueFilter : Int32PropertyValueFilter
            {
            }

        }
    }
}
