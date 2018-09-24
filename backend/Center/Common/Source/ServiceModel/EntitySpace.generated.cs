namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    namespace AccessControl
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum DataScope
        {
            [EnumMember]
            AccessControl  = 10,

            [EnumMember]
            Tenant  = 0,

            [EnumMember]
            User  = 1,

            [EnumMember]
            ProductType  = 20,

            [EnumMember]
            Unit  = 2,

            [EnumMember]
            Resource  = 30,

            [EnumMember]
            Update  = 40,

            [EnumMember]
            Software  = 50,

            [EnumMember]
            UnitConfiguration  = 80,

            [EnumMember]
            MediaConfiguration  = 100,

            [EnumMember]
            Meta  = 200,

            [EnumMember]
            CenterAdmin  = 1000,

            [EnumMember]
            CenterDiag  = 1001,

            [EnumMember]
            CenterMedia  = 1002
        }

        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum Permission
        {
            [EnumMember]
            Create  = 0,

            [EnumMember]
            Read  = 1,

            [EnumMember]
            Write  = 2,

            [EnumMember]
            Delete  = 3,

            [EnumMember]
            Interact  = 4,

            [EnumMember]
            Abort  = 5
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UserRole : ICloneable
        {
            public static readonly UserRole Null = new UserRole();
                        
            public UserRole()
            {
                this.UserDefinedProperties = new Dictionary<string, string>();
            }

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public ICollection<Authorization> Authorizations { get; set; }
        
            [DataMember]
            public IDictionary<string, string> UserDefinedProperties { get; set; }		

            public object Clone()
            {
                var clone = (UserRole)this.MemberwiseClone();
                clone.Authorizations =
                    this.Authorizations == null ?
                        null : new List<Authorization>(this.Authorizations);
                clone.UserDefinedProperties = new Dictionary<string, string>(this.UserDefinedProperties);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Authorization : ICloneable
        {
            public static readonly Authorization Null = new Authorization();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public UserRole UserRole { get; set; }

            [DataMember]
            public DataScope DataScope { get; set; }

            [DataMember]
            public Permission Permission { get; set; }

            public object Clone()
            {
                var clone = (Authorization)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Membership
    {
        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Tenant : ICloneable
        {
            public static readonly Tenant Null = new Tenant();
                        
            public Tenant()
            {
                this.UserDefinedProperties = new Dictionary<string, string>();
            }

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public ICollection<User> Users { get; set; }

            [DataMember]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }
        
            [DataMember]
            public IDictionary<string, string> UserDefinedProperties { get; set; }		

            public object Clone()
            {
                var clone = (Tenant)this.MemberwiseClone();
                clone.Users =
                    this.Users == null ?
                        null : new List<User>(this.Users);
                clone.UpdateGroups =
                    this.UpdateGroups == null ?
                        null : new List<Update.UpdateGroup>(this.UpdateGroups);
                clone.UserDefinedProperties = new Dictionary<string, string>(this.UserDefinedProperties);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class User : ICloneable
        {
            public static readonly User Null = new User();
                        
            public User()
            {
                this.UserDefinedProperties = new Dictionary<string, string>();
            }

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Tenant OwnerTenant { get; set; }

            [DataMember]
            public ICollection<AssociationTenantUserUserRole> AssociationTenantUserUserRoles { get; set; }

            [DataMember]
            public string Username { get; set; }

            [DataMember]
            public string Domain { get; set; }

            [DataMember]
            public string HashedPassword { get; set; }

            [DataMember]
            public string FirstName { get; set; }

            [DataMember]
            public string LastName { get; set; }

            [DataMember]
            public string Email { get; set; }

            [DataMember]
            public string Culture { get; set; }

            [DataMember]
            public string TimeZone { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public DateTime? LastLoginAttempt { get; set; }

            [DataMember]
            public DateTime? LastSuccessfulLogin { get; set; }

            [DataMember]
            public int ConsecutiveLoginFailures { get; set; }

            [DataMember]
            public bool IsEnabled { get; set; }
        
            [DataMember]
            public IDictionary<string, string> UserDefinedProperties { get; set; }		

            public object Clone()
            {
                var clone = (User)this.MemberwiseClone();
                clone.AssociationTenantUserUserRoles =
                    this.AssociationTenantUserUserRoles == null ?
                        null : new List<AssociationTenantUserUserRole>(this.AssociationTenantUserUserRoles);
                clone.UserDefinedProperties = new Dictionary<string, string>(this.UserDefinedProperties);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class AssociationTenantUserUserRole : ICloneable
        {
            public static readonly AssociationTenantUserUserRole Null = new AssociationTenantUserUserRole();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Tenant Tenant { get; set; }

            [DataMember]
            public User User { get; set; }

            [DataMember]
            public AccessControl.UserRole UserRole { get; set; }

            public object Clone()
            {
                var clone = (AssociationTenantUserUserRole)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Units
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum UnitTypes
        {
            [EnumMember]
            [Description("imotion TFT")]
            Tft  = 0,

            [EnumMember]
            [Description("imotion OBU")]
            Obu  = 1,

            [EnumMember]
            [Description("iqube E-Paper")]
            EPaper  = 2
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class ProductType : ICloneable
        {
            public static readonly ProductType Null = new ProductType();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public UnitTypes UnitType { get; set; }

            [DataMember]
            public ICollection<Unit> Units { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string HardwareDescriptorXml { get; set; }

            [DataMember]
            public string HardwareDescriptorXmlType { get; set; }

            public XmlData HardwareDescriptor
            {
                get
                {
                    return new XmlData(this.HardwareDescriptorXml, this.HardwareDescriptorXmlType);
                }

                set
                {
                    this.HardwareDescriptorXml = value.Xml;
                    this.HardwareDescriptorXmlType = value.Type;
                }
            }

            public object Clone()
            {
                var clone = (ProductType)this.MemberwiseClone();
                clone.Units =
                    this.Units == null ?
                        null : new List<Unit>(this.Units);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Unit : ICloneable
        {
            public static readonly Unit Null = new Unit();
                        
            public Unit()
            {
                this.UserDefinedProperties = new Dictionary<string, string>();
            }

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Membership.Tenant Tenant { get; set; }

            [DataMember]
            public ProductType ProductType { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string NetworkAddress { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public bool IsConnected { get; set; }

            [DataMember]
            public ICollection<Update.UpdateCommand> UpdateCommands { get; set; }

            [DataMember]
            public Update.UpdateGroup UpdateGroup { get; set; }
        
            [DataMember]
            public IDictionary<string, string> UserDefinedProperties { get; set; }		

            public object Clone()
            {
                var clone = (Unit)this.MemberwiseClone();
                clone.UpdateCommands =
                    this.UpdateCommands == null ?
                        null : new List<Update.UpdateCommand>(this.UpdateCommands);
                clone.UserDefinedProperties = new Dictionary<string, string>(this.UserDefinedProperties);
                return clone;
            }
        }
    }

    namespace Resources
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum HashAlgorithmTypes
        {
            [EnumMember]
            MD5  = 0,

            [EnumMember]
            xxHash64  = 1,

            [EnumMember]
            xxHash32  = 2
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Resource : ICloneable
        {
            public static readonly Resource Null = new Resource();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Membership.User UploadingUser { get; set; }

            [DataMember]
            public string OriginalFilename { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string Hash { get; set; }

            [DataMember]
            public string ThumbnailHash { get; set; }

            [DataMember]
            public string MimeType { get; set; }

            [DataMember]
            public long Length { get; set; }

            public object Clone()
            {
                var clone = (Resource)this.MemberwiseClone();
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class ContentResource : ICloneable
        {
            public static readonly ContentResource Null = new ContentResource();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Membership.User UploadingUser { get; set; }

            [DataMember]
            public string OriginalFilename { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string ThumbnailHash { get; set; }

            [DataMember]
            public string Hash { get; set; }

            [DataMember]
            public HashAlgorithmTypes HashAlgorithmType { get; set; }

            [DataMember]
            public string MimeType { get; set; }

            [DataMember]
            public long Length { get; set; }

            public object Clone()
            {
                var clone = (ContentResource)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Update
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum UpdatePartType
        {
            [EnumMember]
            Setup  = 0,

            [EnumMember]
            Presentation  = 1,

            [EnumMember]
            AutoPresentation  = 2
        }

        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum UpdateState
        {
            [EnumMember]
            Unknown  = 0,

            [EnumMember]
            Created  = 1,

            [EnumMember]
            Transferring  = 2,

            [EnumMember]
            Transferred  = 3,

            [EnumMember]
            Installing  = 4,

            [EnumMember]
            Installed  = 10,

            [EnumMember]
            Ignored  = 11,

            [EnumMember]
            PartiallyInstalled  = 12,

            [EnumMember]
            TransferFailed  = 20,

            [EnumMember]
            InstallationFailed  = 21
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UpdateGroup : ICloneable
        {
            public static readonly UpdateGroup Null = new UpdateGroup();
                        
            public UpdateGroup()
            {
                this.UserDefinedProperties = new Dictionary<string, string>();
            }

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public Membership.Tenant Tenant { get; set; }

            [DataMember]
            public ICollection<Units.Unit> Units { get; set; }

            [DataMember]
            public ICollection<UpdatePart> UpdateParts { get; set; }

            [DataMember]
            public Configurations.UnitConfiguration UnitConfiguration { get; set; }

            [DataMember]
            public Configurations.MediaConfiguration MediaConfiguration { get; set; }
        
            [DataMember]
            public IDictionary<string, string> UserDefinedProperties { get; set; }		

            public object Clone()
            {
                var clone = (UpdateGroup)this.MemberwiseClone();
                clone.Units =
                    this.Units == null ?
                        null : new List<Units.Unit>(this.Units);
                clone.UpdateParts =
                    this.UpdateParts == null ?
                        null : new List<UpdatePart>(this.UpdateParts);
                clone.UserDefinedProperties = new Dictionary<string, string>(this.UserDefinedProperties);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UpdatePart : ICloneable
        {
            public static readonly UpdatePart Null = new UpdatePart();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public UpdateGroup UpdateGroup { get; set; }

            [DataMember]
            public UpdatePartType Type { get; set; }

            [DataMember]
            public DateTime Start { get; set; }

            [DataMember]
            public DateTime End { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string StructureXml { get; set; }

            [DataMember]
            public string StructureXmlType { get; set; }

            public XmlData Structure
            {
                get
                {
                    return new XmlData(this.StructureXml, this.StructureXmlType);
                }

                set
                {
                    this.StructureXml = value.Xml;
                    this.StructureXmlType = value.Type;
                }
            }

            [DataMember]
            public string InstallInstructionsXml { get; set; }

            [DataMember]
            public string InstallInstructionsXmlType { get; set; }

            public XmlData InstallInstructions
            {
                get
                {
                    return new XmlData(this.InstallInstructionsXml, this.InstallInstructionsXmlType);
                }

                set
                {
                    this.InstallInstructionsXml = value.Xml;
                    this.InstallInstructionsXmlType = value.Type;
                }
            }

            [DataMember]
            public string DynamicContentXml { get; set; }

            [DataMember]
            public string DynamicContentXmlType { get; set; }

            public XmlData DynamicContent
            {
                get
                {
                    return new XmlData(this.DynamicContentXml, this.DynamicContentXmlType);
                }

                set
                {
                    this.DynamicContentXml = value.Xml;
                    this.DynamicContentXmlType = value.Type;
                }
            }

            [DataMember]
            public ICollection<UpdateCommand> RelatedCommands { get; set; }

            public object Clone()
            {
                var clone = (UpdatePart)this.MemberwiseClone();
                clone.RelatedCommands =
                    this.RelatedCommands == null ?
                        null : new List<UpdateCommand>(this.RelatedCommands);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UpdateCommand : ICloneable
        {
            public static readonly UpdateCommand Null = new UpdateCommand();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public int UpdateIndex { get; set; }

            [DataMember]
            public Units.Unit Unit { get; set; }

            [DataMember]
            public string CommandXml { get; set; }

            [DataMember]
            public string CommandXmlType { get; set; }

            public XmlData Command
            {
                get
                {
                    return new XmlData(this.CommandXml, this.CommandXmlType);
                }

                set
                {
                    this.CommandXml = value.Xml;
                    this.CommandXmlType = value.Type;
                }
            }

            [DataMember]
            public bool WasTransferred { get; set; }

            [DataMember]
            public bool WasInstalled { get; set; }

            [DataMember]
            public ICollection<UpdatePart> IncludedParts { get; set; }

            [DataMember]
            public ICollection<UpdateFeedback> Feedbacks { get; set; }

            public object Clone()
            {
                var clone = (UpdateCommand)this.MemberwiseClone();
                clone.IncludedParts =
                    this.IncludedParts == null ?
                        null : new List<UpdatePart>(this.IncludedParts);
                clone.Feedbacks =
                    this.Feedbacks == null ?
                        null : new List<UpdateFeedback>(this.Feedbacks);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UpdateFeedback : ICloneable
        {
            public static readonly UpdateFeedback Null = new UpdateFeedback();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public UpdateCommand UpdateCommand { get; set; }

            [DataMember]
            public DateTime Timestamp { get; set; }

            [DataMember]
            public UpdateState State { get; set; }

            [DataMember]
            public string FeedbackXml { get; set; }

            [DataMember]
            public string FeedbackXmlType { get; set; }

            public XmlData Feedback
            {
                get
                {
                    return new XmlData(this.FeedbackXml, this.FeedbackXmlType);
                }

                set
                {
                    this.FeedbackXml = value.Xml;
                    this.FeedbackXmlType = value.Type;
                }
            }

            public object Clone()
            {
                var clone = (UpdateFeedback)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Documents
    {
        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Document : ICloneable
        {
            public static readonly Document Null = new Document();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public Membership.Tenant Tenant { get; set; }

            [DataMember]
            public ICollection<DocumentVersion> Versions { get; set; }

            public object Clone()
            {
                var clone = (Document)this.MemberwiseClone();
                clone.Versions =
                    this.Versions == null ?
                        null : new List<DocumentVersion>(this.Versions);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class DocumentVersion : ICloneable
        {
            public static readonly DocumentVersion Null = new DocumentVersion();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Document Document { get; set; }

            [DataMember]
            public Membership.User CreatingUser { get; set; }

            [DataMember]
            public int Major { get; set; }

            [DataMember]
            public int Minor { get; set; }

            [DataMember]
            public string ContentXml { get; set; }

            [DataMember]
            public string ContentXmlType { get; set; }

            public XmlData Content
            {
                get
                {
                    return new XmlData(this.ContentXml, this.ContentXmlType);
                }

                set
                {
                    this.ContentXml = value.Xml;
                    this.ContentXmlType = value.Type;
                }
            }

            [DataMember]
            public string Description { get; set; }

            public object Clone()
            {
                var clone = (DocumentVersion)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Software
    {
        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class Package : ICloneable
        {
            public static readonly Package Null = new Package();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public string PackageId { get; set; }

            [DataMember]
            public string ProductName { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public ICollection<PackageVersion> Versions { get; set; }

            public object Clone()
            {
                var clone = (Package)this.MemberwiseClone();
                clone.Versions =
                    this.Versions == null ?
                        null : new List<PackageVersion>(this.Versions);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class PackageVersion : ICloneable
        {
            public static readonly PackageVersion Null = new PackageVersion();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Package Package { get; set; }

            [DataMember]
            public string SoftwareVersion { get; set; }

            [DataMember]
            public string StructureXml { get; set; }

            [DataMember]
            public string StructureXmlType { get; set; }

            public XmlData Structure
            {
                get
                {
                    return new XmlData(this.StructureXml, this.StructureXmlType);
                }

                set
                {
                    this.StructureXml = value.Xml;
                    this.StructureXmlType = value.Type;
                }
            }

            [DataMember]
            public string Description { get; set; }

            public object Clone()
            {
                var clone = (PackageVersion)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Configurations
    {
        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UnitConfiguration : ICloneable
        {
            public static readonly UnitConfiguration Null = new UnitConfiguration();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }

            [DataMember]
            public Documents.Document Document { get; set; }

            [DataMember]
            public Units.ProductType ProductType { get; set; }

            public object Clone()
            {
                var clone = (UnitConfiguration)this.MemberwiseClone();
                clone.UpdateGroups =
                    this.UpdateGroups == null ?
                        null : new List<Update.UpdateGroup>(this.UpdateGroups);
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class MediaConfiguration : ICloneable
        {
            public static readonly MediaConfiguration Null = new MediaConfiguration();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public ICollection<Update.UpdateGroup> UpdateGroups { get; set; }

            [DataMember]
            public Documents.Document Document { get; set; }

            public object Clone()
            {
                var clone = (MediaConfiguration)this.MemberwiseClone();
                clone.UpdateGroups =
                    this.UpdateGroups == null ?
                        null : new List<Update.UpdateGroup>(this.UpdateGroups);
                return clone;
            }
        }
    }

    namespace Log
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum Level
        {
            [EnumMember]
            Trace  = 0,

            [EnumMember]
            Debug  = 1,

            [EnumMember]
            Info  = 2,

            [EnumMember]
            Warn  = 3,

            [EnumMember]
            Error  = 4,

            [EnumMember]
            Fatal  = 5
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class LogEntry : ICloneable
        {
            public static readonly LogEntry Null = new LogEntry();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public Units.Unit Unit { get; set; }

            [DataMember]
            public string Application { get; set; }

            [DataMember]
            public DateTime Timestamp { get; set; }

            [DataMember]
            public Level Level { get; set; }

            [DataMember]
            public string Logger { get; set; }

            [DataMember]
            public string Message { get; set; }

            [DataMember]
            public string AdditionalData { get; set; }

            public object Clone()
            {
                var clone = (LogEntry)this.MemberwiseClone();
                return clone;
            }
        }
    }

    namespace Meta
    {
        [DataContract(Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public enum UserDefinedPropertyEnabledEntity
        {
            [EnumMember]
            Unit  = 0,

            [EnumMember]
            User  = 1,

            [EnumMember]
            Tenant  = 2,

            [EnumMember]
            UserRole  = 3,

            [EnumMember]
            UpdateGroup  = 4
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class UserDefinedProperty : ICloneable
        {
            public static readonly UserDefinedProperty Null = new UserDefinedProperty();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public UserDefinedPropertyEnabledEntity OwnerEntity { get; set; }

            [DataMember]
            public Membership.Tenant Tenant { get; set; }

            [DataMember]
            public string Name { get; set; }

            public object Clone()
            {
                var clone = (UserDefinedProperty)this.MemberwiseClone();
                return clone;
            }
        }

        [DataContract(IsReference = true, Namespace = "http://schema.gorba.com/Center/Entities/Dto")]
        public partial class SystemConfig : ICloneable
        {
            public static readonly SystemConfig Null = new SystemConfig();
            
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public DateTime CreatedOn { get; set; }

            [DataMember]
            public DateTime? LastModifiedOn { get; set; }

            [DataMember]
            public int Version { get; set; }

            [DataMember]
            public Guid SystemId { get; set; }

            [DataMember]
            public string SettingsXml { get; set; }

            [DataMember]
            public string SettingsXmlType { get; set; }

            public XmlData Settings
            {
                get
                {
                    return new XmlData(this.SettingsXml, this.SettingsXmlType);
                }

                set
                {
                    this.SettingsXml = value.Xml;
                    this.SettingsXmlType = value.Type;
                }
            }

            public object Clone()
            {
                var clone = (SystemConfig)this.MemberwiseClone();
                return clone;
            }
        }
    }
}