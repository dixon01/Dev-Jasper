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
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DataScope))] //here
        [KnownType(typeof(Permission))] //here
        [KnownType(typeof(DataScopePropertyValueFilter))]
        [KnownType(typeof(PermissionPropertyValueFilter))]
        public partial class AuthorizationQuery : AuthorizationFilter, IQuery
        {
            public AuthorizationQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new AuthorizationQuery Create()
            {
                return new AuthorizationQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                DataScope = 0,

                [EnumMember]
                Permission = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UserRoleQuery : UserRoleFilter, IQuery
        {
            public UserRoleQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UserRoleQuery Create()
            {
                return new UserRoleQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Name = 0,

                [EnumMember]
                Description = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        public partial class MediaConfigurationQuery : MediaConfigurationFilter, IQuery
        {
            public MediaConfigurationQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new MediaConfigurationQuery Create()
            {
                return new MediaConfigurationQuery();
            }

            [DataContract]
            public enum SortingProperties
            {
                [EnumMember]
                CreatedOn = 0,

                [EnumMember]
                LastModifiedOn = 1,

                [EnumMember]
                Version = 2
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        public partial class UnitConfigurationQuery : UnitConfigurationFilter, IQuery
        {
            public UnitConfigurationQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UnitConfigurationQuery Create()
            {
                return new UnitConfigurationQuery();
            }

            [DataContract]
            public enum SortingProperties
            {
                [EnumMember]
                CreatedOn = 0,

                [EnumMember]
                LastModifiedOn = 1,

                [EnumMember]
                Version = 2
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class DocumentQuery : DocumentFilter, IQuery
        {
            public DocumentQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new DocumentQuery Create()
            {
                return new DocumentQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Name = 0,

                [EnumMember]
                Description = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(MajorPropertyValueFilter))]
        [KnownType(typeof(MinorPropertyValueFilter))]
        public partial class DocumentVersionQuery : DocumentVersionFilter, IQuery
        {
            public DocumentVersionQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new DocumentVersionQuery Create()
            {
                return new DocumentVersionQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Major = 0,

                [EnumMember]
                Minor = 1,

                [EnumMember]
                Content = 2,

                [EnumMember]
                Description = 3,
                [EnumMember]
                CreatedOn = 4,

                [EnumMember]
                LastModifiedOn = 5,

                [EnumMember]
                Version = 6
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(Level))] //here
        [KnownType(typeof(LevelPropertyValueFilter))]
        [KnownType(typeof(AdditionalDataPropertyValueFilter))]
        [KnownType(typeof(ApplicationPropertyValueFilter))]
        [KnownType(typeof(LoggerPropertyValueFilter))]
        [KnownType(typeof(MessagePropertyValueFilter))]
        [KnownType(typeof(TimestampPropertyValueFilter))]
        public partial class LogEntryQuery : LogEntryFilter, IQuery
        {
            public LogEntryQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new LogEntryQuery Create()
            {
                return new LogEntryQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Application = 0,

                [EnumMember]
                Timestamp = 1,

                [EnumMember]
                Level = 2,

                [EnumMember]
                Logger = 3,

                [EnumMember]
                Message = 4,

                [EnumMember]
                AdditionalData = 5,
                [EnumMember]
                CreatedOn = 6,

                [EnumMember]
                LastModifiedOn = 7,

                [EnumMember]
                Version = 8
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        public partial class AssociationTenantUserUserRoleQuery : AssociationTenantUserUserRoleFilter, IQuery
        {
            public AssociationTenantUserUserRoleQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new AssociationTenantUserUserRoleQuery Create()
            {
                return new AssociationTenantUserUserRoleQuery();
            }

            [DataContract]
            public enum SortingProperties
            {
                [EnumMember]
                CreatedOn = 0,

                [EnumMember]
                LastModifiedOn = 1,

                [EnumMember]
                Version = 2
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class TenantQuery : TenantFilter, IQuery
        {
            public TenantQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new TenantQuery Create()
            {
                return new TenantQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Name = 0,

                [EnumMember]
                Description = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
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
        public partial class UserQuery : UserFilter, IQuery
        {
            public UserQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UserQuery Create()
            {
                return new UserQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Username = 0,

                [EnumMember]
                Domain = 1,

                [EnumMember]
                HashedPassword = 2,

                [EnumMember]
                FirstName = 3,

                [EnumMember]
                LastName = 4,

                [EnumMember]
                Email = 5,

                [EnumMember]
                Culture = 6,

                [EnumMember]
                TimeZone = 7,

                [EnumMember]
                Description = 8,

                [EnumMember]
                LastLoginAttempt = 9,

                [EnumMember]
                LastSuccessfulLogin = 10,

                [EnumMember]
                ConsecutiveLoginFailures = 11,

                [EnumMember]
                IsEnabled = 12,
                [EnumMember]
                CreatedOn = 13,

                [EnumMember]
                LastModifiedOn = 14,

                [EnumMember]
                Version = 15
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(SystemIdPropertyValueFilter))]
        public partial class SystemConfigQuery : SystemConfigFilter, IQuery
        {
            public SystemConfigQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new SystemConfigQuery Create()
            {
                return new SystemConfigQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                SystemId = 0,

                [EnumMember]
                Settings = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(UserDefinedPropertyEnabledEntity))] //here
        [KnownType(typeof(OwnerEntityPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UserDefinedPropertyQuery : UserDefinedPropertyFilter, IQuery
        {
            public UserDefinedPropertyQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UserDefinedPropertyQuery Create()
            {
                return new UserDefinedPropertyQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                OwnerEntity = 0,

                [EnumMember]
                Name = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(HashAlgorithmTypes))] //here
        [KnownType(typeof(HashAlgorithmTypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(HashPropertyValueFilter))]
        [KnownType(typeof(LengthPropertyValueFilter))]
        [KnownType(typeof(MimeTypePropertyValueFilter))]
        [KnownType(typeof(OriginalFilenamePropertyValueFilter))]
        [KnownType(typeof(ThumbnailHashPropertyValueFilter))]
        public partial class ContentResourceQuery : ContentResourceFilter, IQuery
        {
            public ContentResourceQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new ContentResourceQuery Create()
            {
                return new ContentResourceQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                OriginalFilename = 0,

                [EnumMember]
                Description = 1,

                [EnumMember]
                ThumbnailHash = 2,

                [EnumMember]
                Hash = 3,

                [EnumMember]
                HashAlgorithmType = 4,

                [EnumMember]
                MimeType = 5,

                [EnumMember]
                Length = 6,
                [EnumMember]
                CreatedOn = 7,

                [EnumMember]
                LastModifiedOn = 8,

                [EnumMember]
                Version = 9
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(HashPropertyValueFilter))]
        [KnownType(typeof(LengthPropertyValueFilter))]
        [KnownType(typeof(MimeTypePropertyValueFilter))]
        [KnownType(typeof(OriginalFilenamePropertyValueFilter))]
        [KnownType(typeof(ThumbnailHashPropertyValueFilter))]
        public partial class ResourceQuery : ResourceFilter, IQuery
        {
            public ResourceQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new ResourceQuery Create()
            {
                return new ResourceQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                OriginalFilename = 0,

                [EnumMember]
                Description = 1,

                [EnumMember]
                Hash = 2,

                [EnumMember]
                ThumbnailHash = 3,

                [EnumMember]
                MimeType = 4,

                [EnumMember]
                Length = 5,
                [EnumMember]
                CreatedOn = 6,

                [EnumMember]
                LastModifiedOn = 7,

                [EnumMember]
                Version = 8
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(PackageIdPropertyValueFilter))]
        [KnownType(typeof(ProductNamePropertyValueFilter))]
        public partial class PackageQuery : PackageFilter, IQuery
        {
            public PackageQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new PackageQuery Create()
            {
                return new PackageQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                PackageId = 0,

                [EnumMember]
                ProductName = 1,

                [EnumMember]
                Description = 2,
                [EnumMember]
                CreatedOn = 3,

                [EnumMember]
                LastModifiedOn = 4,

                [EnumMember]
                Version = 5
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(SoftwareVersionPropertyValueFilter))]
        public partial class PackageVersionQuery : PackageVersionFilter, IQuery
        {
            public PackageVersionQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new PackageVersionQuery Create()
            {
                return new PackageVersionQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                SoftwareVersion = 0,

                [EnumMember]
                Structure = 1,

                [EnumMember]
                Description = 2,
                [EnumMember]
                CreatedOn = 3,

                [EnumMember]
                LastModifiedOn = 4,

                [EnumMember]
                Version = 5
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(UnitTypes))] //here
        [KnownType(typeof(UnitTypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class ProductTypeQuery : ProductTypeFilter, IQuery
        {
            public ProductTypeQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new ProductTypeQuery Create()
            {
                return new ProductTypeQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                UnitType = 0,

                [EnumMember]
                Name = 1,

                [EnumMember]
                Description = 2,

                [EnumMember]
                HardwareDescriptor = 3,
                [EnumMember]
                CreatedOn = 4,

                [EnumMember]
                LastModifiedOn = 5,

                [EnumMember]
                Version = 6
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(IsConnectedPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        [KnownType(typeof(NetworkAddressPropertyValueFilter))]
        public partial class UnitQuery : UnitFilter, IQuery
        {
            public UnitQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UnitQuery Create()
            {
                return new UnitQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Name = 0,

                [EnumMember]
                NetworkAddress = 1,

                [EnumMember]
                Description = 2,

                [EnumMember]
                IsConnected = 3,
                [EnumMember]
                CreatedOn = 4,

                [EnumMember]
                LastModifiedOn = 5,

                [EnumMember]
                Version = 6
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(UpdateIndexPropertyValueFilter))]
        [KnownType(typeof(WasInstalledPropertyValueFilter))]
        [KnownType(typeof(WasTransferredPropertyValueFilter))]
        public partial class UpdateCommandQuery : UpdateCommandFilter, IQuery
        {
            public UpdateCommandQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UpdateCommandQuery Create()
            {
                return new UpdateCommandQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                UpdateIndex = 0,

                [EnumMember]
                Command = 1,

                [EnumMember]
                WasTransferred = 2,

                [EnumMember]
                WasInstalled = 3,
                [EnumMember]
                CreatedOn = 4,

                [EnumMember]
                LastModifiedOn = 5,

                [EnumMember]
                Version = 6
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(UpdateState))] //here
        [KnownType(typeof(StatePropertyValueFilter))]
        [KnownType(typeof(TimestampPropertyValueFilter))]
        public partial class UpdateFeedbackQuery : UpdateFeedbackFilter, IQuery
        {
            public UpdateFeedbackQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UpdateFeedbackQuery Create()
            {
                return new UpdateFeedbackQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Timestamp = 0,

                [EnumMember]
                State = 1,

                [EnumMember]
                Feedback = 2,
                [EnumMember]
                CreatedOn = 3,

                [EnumMember]
                LastModifiedOn = 4,

                [EnumMember]
                Version = 5
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(NamePropertyValueFilter))]
        public partial class UpdateGroupQuery : UpdateGroupFilter, IQuery
        {
            public UpdateGroupQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UpdateGroupQuery Create()
            {
                return new UpdateGroupQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Name = 0,

                [EnumMember]
                Description = 1,
                [EnumMember]
                CreatedOn = 2,

                [EnumMember]
                LastModifiedOn = 3,

                [EnumMember]
                Version = 4
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }

        [DataContract]
        [DebuggerDisplay("Skip {Skip}, Take {Take}, {Sorting.Count} sorting value(s)")]
        [KnownType(typeof(UpdatePartType))] //here
        [KnownType(typeof(TypePropertyValueFilter))]
        [KnownType(typeof(DescriptionPropertyValueFilter))]
        [KnownType(typeof(EndPropertyValueFilter))]
        [KnownType(typeof(StartPropertyValueFilter))]
        public partial class UpdatePartQuery : UpdatePartFilter, IQuery
        {
            public UpdatePartQuery()
            {
                this.Sorting = new List<OrderClause>();
            }
            
            public int Skip { get; set; }

            public int? Take { get; set; }

            [DataMember]
            public ICollection<OrderClause> Sorting { get; set; }

            public static new UpdatePartQuery Create()
            {
                return new UpdatePartQuery();
            }

            [DataContract]
            public enum SortingProperties
            {

                [EnumMember]
                Type = 0,

                [EnumMember]
                Start = 1,

                [EnumMember]
                End = 2,

                [EnumMember]
                Description = 3,

                [EnumMember]
                Structure = 4,

                [EnumMember]
                InstallInstructions = 5,

                [EnumMember]
                DynamicContent = 6,
                [EnumMember]
                CreatedOn = 7,

                [EnumMember]
                LastModifiedOn = 8,

                [EnumMember]
                Version = 9
            }

            [DataContract]
            [DebuggerDisplay("{Property} {Direction}")]
            public class OrderClause
            {
                public OrderClause(SortingProperties property, SortDirection direction)
                {
                    this.Property = property;
                    this.Direction = direction;
                }

                public OrderClause()
                {
                }

                [DataMember]
                public SortDirection Direction { get; set; }

                [DataMember]
                public SortingProperties Property { get; set; }
            }

        }
    }
}
