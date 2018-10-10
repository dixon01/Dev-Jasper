namespace Gorba.Center.BackgroundSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    using NLog;

    namespace AccessControl
    {
        public enum DataScope
        {
            AccessControl  = 10,

            Tenant  = 0,

            User  = 1,

            ProductType  = 20,

            Unit  = 2,

            Resource  = 30,

            Update  = 40,

            Software  = 50,

            UnitConfiguration  = 80,

            MediaConfiguration  = 100,

            Meta  = 200,

            CenterAdmin  = 1000,

            CenterDiag  = 1001,

            CenterMedia  = 1002
        }

        public enum Permission
        {
            Create  = 0,

            Read  = 1,

            Write  = 2,

            Delete  = 3,

            Interact  = 4,

            Abort  = 5
        }

        public partial class Authorization : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Authorization Null = new Authorization();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public DataScope DataScope { get; set; }

            [Required]
            public UserRole UserRole { get; set; }

            public Permission Permission { get; set; }

            public Authorization Clone()
            {
                var clone = (Authorization)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UserRole : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UserRole Null = new UserRole();
            
            public UserRole()
            {
                this.UserDefinedProperties = new List<UserRoleUserDefinedProperty>();
                this.RawUserDefinedProperties = new Dictionary<string, string>();
            }

            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string Name { get; set; }

            public string Description { get; set; }

            [InverseProperty("UserRole")]
            public ICollection<Authorization> Authorizations { get; set; }
    
            public ICollection<UserRoleUserDefinedProperty> UserDefinedProperties { get; set; }
            
            [NotMapped]
            public IDictionary<string, string> RawUserDefinedProperties { get; set; }

            public UserRole Clone()
            {
                var clone = (UserRole)this.MemberwiseClone();
                clone.RawUserDefinedProperties = new Dictionary<string, string>(this.RawUserDefinedProperties);
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UserRoleUserDefinedProperty : ICloneable
        {
            public UserRoleUserDefinedProperty(Meta.UserDefinedProperty propertyDefinition, string value)
            {
                this.PropertyDefinition = propertyDefinition;
                this.Value = value;
            }

            public UserRoleUserDefinedProperty()
            {
            }

            public int Id { get; set; }
            
            [Index("Index_UserRoleUserDefinedProperty", IsUnique = true)]
            [Required]
            public Meta.UserDefinedProperty PropertyDefinition { get; set; }

            public string Value { get; set; }

            public UserRoleUserDefinedProperty Clone()
            {
                var clone = ((ICloneable)this).Clone();
                return (UserRoleUserDefinedProperty)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }

    namespace Membership
    {
        public partial class Tenant : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Tenant Null = new Tenant();
            
            public Tenant()
            {
                this.UserDefinedProperties = new List<TenantUserDefinedProperty>();
                this.RawUserDefinedProperties = new Dictionary<string, string>();
            }

            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string Name { get; set; }

            public string Description { get; set; }

            [InverseProperty("OwnerTenant")]
            public ICollection<User> Users { get; set; }

            [InverseProperty("Tenant")]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }
    
            public ICollection<TenantUserDefinedProperty> UserDefinedProperties { get; set; }
            
            [NotMapped]
            public IDictionary<string, string> RawUserDefinedProperties { get; set; }

            public Tenant Clone()
            {
                var clone = (Tenant)this.MemberwiseClone();
                clone.RawUserDefinedProperties = new Dictionary<string, string>(this.RawUserDefinedProperties);
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class TenantUserDefinedProperty : ICloneable
        {
            public TenantUserDefinedProperty(Meta.UserDefinedProperty propertyDefinition, string value)
            {
                this.PropertyDefinition = propertyDefinition;
                this.Value = value;
            }

            public TenantUserDefinedProperty()
            {
            }

            public int Id { get; set; }
            
            [Index("Index_TenantUserDefinedProperty", IsUnique = true)]
            [Required]
            public Meta.UserDefinedProperty PropertyDefinition { get; set; }

            public string Value { get; set; }

            public TenantUserDefinedProperty Clone()
            {
                var clone = ((ICloneable)this).Clone();
                return (TenantUserDefinedProperty)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public partial class User : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly User Null = new User();
            
            public User()
            {
                this.UserDefinedProperties = new List<UserUserDefinedProperty>();
                this.RawUserDefinedProperties = new Dictionary<string, string>();
            }

            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public Tenant OwnerTenant { get; set; }

            [InverseProperty("User")]
            public ICollection<AssociationTenantUserUserRole> AssociationTenantUserUserRoles { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string Username { get; set; }

            public string Domain { get; set; }

            public string HashedPassword { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string Culture { get; set; }

            public string TimeZone { get; set; }

            public string Description { get; set; }

            private DateTime? fieldLastLoginAttempt;

            [Column(TypeName = "datetime2")]
            public DateTime? LastLoginAttempt
            {
                get
                {
                    return this.fieldLastLoginAttempt;
                }

                set
                {
                    SetDateTime(ref this.fieldLastLoginAttempt, value);
                }
            }

            private DateTime? fieldLastSuccessfulLogin;

            [Column(TypeName = "datetime2")]
            public DateTime? LastSuccessfulLogin
            {
                get
                {
                    return this.fieldLastSuccessfulLogin;
                }

                set
                {
                    SetDateTime(ref this.fieldLastSuccessfulLogin, value);
                }
            }

            public int ConsecutiveLoginFailures { get; set; }

            public bool IsEnabled { get; set; }
    
            public ICollection<UserUserDefinedProperty> UserDefinedProperties { get; set; }
            
            [NotMapped]
            public IDictionary<string, string> RawUserDefinedProperties { get; set; }

            public User Clone()
            {
                var clone = (User)this.MemberwiseClone();
                clone.RawUserDefinedProperties = new Dictionary<string, string>(this.RawUserDefinedProperties);
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UserUserDefinedProperty : ICloneable
        {
            public UserUserDefinedProperty(Meta.UserDefinedProperty propertyDefinition, string value)
            {
                this.PropertyDefinition = propertyDefinition;
                this.Value = value;
            }

            public UserUserDefinedProperty()
            {
            }

            public int Id { get; set; }
            
            [Index("Index_UserUserDefinedProperty", IsUnique = true)]
            [Required]
            public Meta.UserDefinedProperty PropertyDefinition { get; set; }

            public string Value { get; set; }

            public UserUserDefinedProperty Clone()
            {
                var clone = ((ICloneable)this).Clone();
                return (UserUserDefinedProperty)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public partial class AssociationTenantUserUserRole : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly AssociationTenantUserUserRole Null = new AssociationTenantUserUserRole();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public Tenant Tenant { get; set; }

            [Required]
            public User User { get; set; }

            [Required]
            public AccessControl.UserRole UserRole { get; set; }

            public AssociationTenantUserUserRole Clone()
            {
                var clone = (AssociationTenantUserUserRole)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Units
    {
        public enum UnitTypes
        {
            Tft  = 0,

            Obu  = 1,

            EPaper  = 2
        }

        public partial class ProductType : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly ProductType Null = new ProductType();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public UnitTypes UnitType { get; set; }

            [InverseProperty("ProductType")]
            public ICollection<Unit> Units { get; set; }

            [Required]
            public string Name { get; set; }

            public string Description { get; set; }

            public XmlData HardwareDescriptor { get; set; }

            public ProductType Clone()
            {
                var clone = (ProductType)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class Unit : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Unit Null = new Unit();
            
            public Unit()
            {
                this.UserDefinedProperties = new List<UnitUserDefinedProperty>();
                this.RawUserDefinedProperties = new Dictionary<string, string>();
            }

            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public Membership.Tenant Tenant { get; set; }

            [Required]
            public ProductType ProductType { get; set; }

            [Required]
            public string Name { get; set; }

            public string NetworkAddress { get; set; }

            public string Description { get; set; }

            [Required]
            public bool IsConnected { get; set; }

            [InverseProperty("Unit")]
            public ICollection<Update.UpdateCommand> UpdateCommands { get; set; }

            public Update.UpdateGroup UpdateGroup { get; set; }
    
            public ICollection<UnitUserDefinedProperty> UserDefinedProperties { get; set; }
            
            [NotMapped]
            public IDictionary<string, string> RawUserDefinedProperties { get; set; }

            public Unit Clone()
            {
                var clone = (Unit)this.MemberwiseClone();
                clone.RawUserDefinedProperties = new Dictionary<string, string>(this.RawUserDefinedProperties);
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UnitUserDefinedProperty : ICloneable
        {
            public UnitUserDefinedProperty(Meta.UserDefinedProperty propertyDefinition, string value)
            {
                this.PropertyDefinition = propertyDefinition;
                this.Value = value;
            }

            public UnitUserDefinedProperty()
            {
            }

            public int Id { get; set; }
            
            [Index("Index_UnitUserDefinedProperty", IsUnique = true)]
            [Required]
            public Meta.UserDefinedProperty PropertyDefinition { get; set; }

            public string Value { get; set; }

            public UnitUserDefinedProperty Clone()
            {
                var clone = ((ICloneable)this).Clone();
                return (UnitUserDefinedProperty)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }

    namespace Resources
    {
        public enum HashAlgorithmTypes
        {
            MD5  = 0,

            xxHash64  = 1
        }

        public partial class Resource : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Resource Null = new Resource();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public Membership.User UploadingUser { get; set; }

            public string OriginalFilename { get; set; }

            public string Description { get; set; }

            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string Hash { get; set; }

            public string ThumbnailHash { get; set; }

            public string MimeType { get; set; }

            public long Length { get; set; }

            public Resource Clone()
            {
                var clone = (Resource)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class ContentResource : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly ContentResource Null = new ContentResource();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public Membership.User UploadingUser { get; set; }

            public string OriginalFilename { get; set; }

            public string Description { get; set; }

            public string ThumbnailHash { get; set; }

			[Index("IX_HashAndHashType", 0, IsUnique=true)]
            [MaxLength(100)]
            public string Hash { get; set; }

			[Index("IX_HashAndHashType", 1, IsUnique=true)]
            public HashAlgorithmTypes HashAlgorithmType { get; set; }

            public string MimeType { get; set; }

            public long Length { get; set; }

            public ContentResource Clone()
            {
                var clone = (ContentResource)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Update
    {
        public enum UpdatePartType
        {
            Setup  = 0,

            Presentation  = 1,

            AutoPresentation  = 2
        }

        public enum UpdateState
        {
            Unknown  = 0,

            Created  = 1,

            Transferring  = 2,

            Transferred  = 3,

            Installing  = 4,

            Installed  = 10,

            Ignored  = 11,

            PartiallyInstalled  = 12,

            TransferFailed  = 20,

            InstallationFailed  = 21
        }

        public partial class UpdateGroup : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UpdateGroup Null = new UpdateGroup();
            
            public UpdateGroup()
            {
                this.UserDefinedProperties = new List<UpdateGroupUserDefinedProperty>();
                this.RawUserDefinedProperties = new Dictionary<string, string>();
            }

            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public string Name { get; set; }

            public string Description { get; set; }

            [Required]
            public Membership.Tenant Tenant { get; set; }

            [InverseProperty("UpdateGroup")]
            public ICollection<Units.Unit> Units { get; set; }

            [InverseProperty("UpdateGroup")]
            public ICollection<UpdatePart> UpdateParts { get; set; }

            public Configurations.UnitConfiguration UnitConfiguration { get; set; }

            public Configurations.MediaConfiguration MediaConfiguration { get; set; }
    
            public ICollection<UpdateGroupUserDefinedProperty> UserDefinedProperties { get; set; }
            
            [NotMapped]
            public IDictionary<string, string> RawUserDefinedProperties { get; set; }

            public UpdateGroup Clone()
            {
                var clone = (UpdateGroup)this.MemberwiseClone();
                clone.RawUserDefinedProperties = new Dictionary<string, string>(this.RawUserDefinedProperties);
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UpdateGroupUserDefinedProperty : ICloneable
        {
            public UpdateGroupUserDefinedProperty(Meta.UserDefinedProperty propertyDefinition, string value)
            {
                this.PropertyDefinition = propertyDefinition;
                this.Value = value;
            }

            public UpdateGroupUserDefinedProperty()
            {
            }

            public int Id { get; set; }
            
            [Index("Index_UpdateGroupUserDefinedProperty", IsUnique = true)]
            [Required]
            public Meta.UserDefinedProperty PropertyDefinition { get; set; }

            public string Value { get; set; }

            public UpdateGroupUserDefinedProperty Clone()
            {
                var clone = ((ICloneable)this).Clone();
                return (UpdateGroupUserDefinedProperty)clone;
            }

            object ICloneable.Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public partial class UpdatePart : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UpdatePart Null = new UpdatePart();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public UpdateGroup UpdateGroup { get; set; }

            [Required]
            public UpdatePartType Type { get; set; }

            private DateTime fieldStart;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime Start
            {
                get
                {
                    return this.fieldStart;
                }

                set
                {
                    SetDateTime(ref this.fieldStart, value);
                }
            }

            private DateTime fieldEnd;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime End
            {
                get
                {
                    return this.fieldEnd;
                }

                set
                {
                    SetDateTime(ref this.fieldEnd, value);
                }
            }

            public string Description { get; set; }

            public XmlData Structure { get; set; }

            public XmlData InstallInstructions { get; set; }

            public XmlData DynamicContent { get; set; }
            public ICollection<UpdateCommand> RelatedCommands { get; set; }

            public UpdatePart Clone()
            {
                var clone = (UpdatePart)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UpdateCommand : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UpdateCommand Null = new UpdateCommand();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public int UpdateIndex { get; set; }

            [Required]
            public Units.Unit Unit { get; set; }

            public XmlData Command { get; set; }

            [Required]
            public bool WasTransferred { get; set; }

            [Required]
            public bool WasInstalled { get; set; }
            public ICollection<UpdatePart> IncludedParts { get; set; }

            [InverseProperty("UpdateCommand")]
            public ICollection<UpdateFeedback> Feedbacks { get; set; }

            public UpdateCommand Clone()
            {
                var clone = (UpdateCommand)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class UpdateFeedback : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UpdateFeedback Null = new UpdateFeedback();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public UpdateCommand UpdateCommand { get; set; }

            private DateTime fieldTimestamp;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime Timestamp
            {
                get
                {
                    return this.fieldTimestamp;
                }

                set
                {
                    SetDateTime(ref this.fieldTimestamp, value);
                }
            }

            [Required]
            public UpdateState State { get; set; }

            public XmlData Feedback { get; set; }

            public UpdateFeedback Clone()
            {
                var clone = (UpdateFeedback)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Documents
    {
        public partial class Document : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Document Null = new Document();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public string Name { get; set; }

            public string Description { get; set; }

            [Required]
            public Membership.Tenant Tenant { get; set; }

            [InverseProperty("Document")]
            public ICollection<DocumentVersion> Versions { get; set; }

            public Document Clone()
            {
                var clone = (Document)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class DocumentVersion : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly DocumentVersion Null = new DocumentVersion();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public Document Document { get; set; }

            public Membership.User CreatingUser { get; set; }

            public int Major { get; set; }

            public int Minor { get; set; }

            public XmlData Content { get; set; }

            public string Description { get; set; }

            public DocumentVersion Clone()
            {
                var clone = (DocumentVersion)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Software
    {
        public partial class Package : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly Package Null = new Package();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string PackageId { get; set; }

            [Required]
            [Index(IsUnique = true)]
            [MaxLength(100)]
            public string ProductName { get; set; }

            public string Description { get; set; }

            [InverseProperty("Package")]
            public ICollection<PackageVersion> Versions { get; set; }

            public Package Clone()
            {
                var clone = (Package)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class PackageVersion : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly PackageVersion Null = new PackageVersion();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [Required]
            public Package Package { get; set; }

            public string SoftwareVersion { get; set; }

            public XmlData Structure { get; set; }

            public string Description { get; set; }

            public PackageVersion Clone()
            {
                var clone = (PackageVersion)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Configurations
    {
        public partial class UnitConfiguration : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UnitConfiguration Null = new UnitConfiguration();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [InverseProperty("UnitConfiguration")]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }

            [Required]
            public Documents.Document Document { get; set; }

            [Required]
            public Units.ProductType ProductType { get; set; }

            public UnitConfiguration Clone()
            {
                var clone = (UnitConfiguration)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class MediaConfiguration : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly MediaConfiguration Null = new MediaConfiguration();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            [InverseProperty("MediaConfiguration")]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }

            [Required]
            public Documents.Document Document { get; set; }

            public MediaConfiguration Clone()
            {
                var clone = (MediaConfiguration)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Log
    {
        public enum Level
        {
            Trace  = 0,

            Debug  = 1,

            Info  = 2,

            Warn  = 3,

            Error  = 4,

            Fatal  = 5
        }

        public partial class LogEntry : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly LogEntry Null = new LogEntry();
            
            public int Id { get; set; }

            public System.Nullable<int> Unit_Id { get; set; }

            [ForeignKey("Unit_Id")]
            public Units.Unit Unit { get; set; }

            [Required]
            public string Application { get; set; }

            private DateTime fieldTimestamp;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime Timestamp
            {
                get
                {
                    return this.fieldTimestamp;
                }

                set
                {
                    SetDateTime(ref this.fieldTimestamp, value);
                }
            }

            [Required]
            public Level Level { get; set; }

            [Required]
            public string Logger { get; set; }

            [Required]
            public string Message { get; set; }

            public string AdditionalData { get; set; }

            public LogEntry Clone()
            {
                var clone = (LogEntry)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    namespace Meta
    {
        public enum UserDefinedPropertyEnabledEntity
        {
            Unit  = 0,

            User  = 1,

            Tenant  = 2,

            UserRole  = 3,

            UpdateGroup  = 4
        }

        public partial class UserDefinedProperty : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly UserDefinedProperty Null = new UserDefinedProperty();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public UserDefinedPropertyEnabledEntity OwnerEntity { get; set; }

            public Membership.Tenant Tenant { get; set; }

            public string Name { get; set; }

            public UserDefinedProperty Clone()
            {
                var clone = (UserDefinedProperty)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public partial class SystemConfig : ICloneable
        {
            private static readonly Logger EntityLogger = LogManager.GetCurrentClassLogger();

            public static readonly SystemConfig Null = new SystemConfig();
            
            public int Id { get; set; }

            private DateTime createdOn;

            [Column(TypeName = "datetime2")]
            [Required]
            public DateTime CreatedOn
            {
                get
                {
                    return this.createdOn;
                }

                set
                {
                    SetDateTime(ref this.createdOn, value);
                }
            }

            private DateTime? lastModifiedOn;

            [Column(TypeName = "datetime2")]
            public DateTime? LastModifiedOn
            {
                get
                {
                    return this.lastModifiedOn;
                }

                set
                {
                    SetDateTime(ref this.lastModifiedOn, value);
                }
            }

            public int Version { get; set; }

            public Guid SystemId { get; set; }

            public XmlData Settings { get; set; }

            public SystemConfig Clone()
            {
                var clone = (SystemConfig)this.MemberwiseClone();
                return clone;
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            private static void SetDateTime(
                ref DateTime field, DateTime value, [CallerMemberName] string propertyName = null)
            {
                switch(value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private static void SetDateTime(
                ref DateTime? field, DateTime? value, [CallerMemberName] string propertyName = null)
            {
                if (!value.HasValue)
                {
                    field = null;
                    return;
                }
                
                switch(value.Value.Kind)
                {
                    case DateTimeKind.Unspecified:
                        field = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                        break;
                    case DateTimeKind.Utc:
                        field = value;
                        break;
                    case DateTimeKind.Local:
                        field = value.Value.ToUniversalTime();
                        EntityLogger.Debug("Local date value converted to Utc for property '{0}'", propertyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}