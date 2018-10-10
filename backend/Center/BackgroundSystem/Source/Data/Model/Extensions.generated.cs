namespace Gorba.Center.BackgroundSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    using DatabaseAccessControlDataScope = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.DataScope;
    using DatabaseResourcesHashAlgorithmTypes = Gorba.Center.BackgroundSystem.Data.Model.Resources.HashAlgorithmTypes;
    using DatabaseLogLevel = Gorba.Center.BackgroundSystem.Data.Model.Log.Level;
    using DatabaseAccessControlPermission = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.Permission;
    using DatabaseUnitsUnitTypes = Gorba.Center.BackgroundSystem.Data.Model.Units.UnitTypes;
    using DatabaseUpdateUpdatePartType = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdatePartType;
    using DatabaseUpdateUpdateState = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateState;
    using DatabaseMetaUserDefinedPropertyEnabledEntity = Gorba.Center.BackgroundSystem.Data.Model.Meta.UserDefinedPropertyEnabledEntity;

    using DtoAccessControlDataScope = Gorba.Center.Common.ServiceModel.AccessControl.DataScope;
    using DtoResourcesHashAlgorithmTypes = Gorba.Center.Common.ServiceModel.Resources.HashAlgorithmTypes;
    using DtoLogLevel = Gorba.Center.Common.ServiceModel.Log.Level;
    using DtoAccessControlPermission = Gorba.Center.Common.ServiceModel.AccessControl.Permission;
    using DtoUnitsUnitTypes = Gorba.Center.Common.ServiceModel.Units.UnitTypes;
    using DtoUpdateUpdatePartType = Gorba.Center.Common.ServiceModel.Update.UpdatePartType;
    using DtoUpdateUpdateState = Gorba.Center.Common.ServiceModel.Update.UpdateState;
    using DtoMetaUserDefinedPropertyEnabledEntity = Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity;

    using DatabaseXmlData = Gorba.Center.BackgroundSystem.Data.Model.XmlData;
    using DatabaseMembershipAssociationTenantUserUserRole = Gorba.Center.BackgroundSystem.Data.Model.Membership.AssociationTenantUserUserRole;
    using DatabaseAccessControlAuthorization = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.Authorization;
    using DatabaseResourcesContentResource = Gorba.Center.BackgroundSystem.Data.Model.Resources.ContentResource;
    using DatabaseDocumentsDocument = Gorba.Center.BackgroundSystem.Data.Model.Documents.Document;
    using DatabaseDocumentsDocumentVersion = Gorba.Center.BackgroundSystem.Data.Model.Documents.DocumentVersion;
    using DatabaseLogLogEntry = Gorba.Center.BackgroundSystem.Data.Model.Log.LogEntry;
    using DatabaseConfigurationsMediaConfiguration = Gorba.Center.BackgroundSystem.Data.Model.Configurations.MediaConfiguration;
    using DatabaseSoftwarePackage = Gorba.Center.BackgroundSystem.Data.Model.Software.Package;
    using DatabaseSoftwarePackageVersion = Gorba.Center.BackgroundSystem.Data.Model.Software.PackageVersion;
    using DatabaseUnitsProductType = Gorba.Center.BackgroundSystem.Data.Model.Units.ProductType;
    using DatabaseResourcesResource = Gorba.Center.BackgroundSystem.Data.Model.Resources.Resource;
    using DatabaseMetaSystemConfig = Gorba.Center.BackgroundSystem.Data.Model.Meta.SystemConfig;
    using DatabaseMembershipTenant = Gorba.Center.BackgroundSystem.Data.Model.Membership.Tenant;
    using DatabaseUnitsUnit = Gorba.Center.BackgroundSystem.Data.Model.Units.Unit;
    using DatabaseConfigurationsUnitConfiguration = Gorba.Center.BackgroundSystem.Data.Model.Configurations.UnitConfiguration;
    using DatabaseUpdateUpdateCommand = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateCommand;
    using DatabaseUpdateUpdateFeedback = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateFeedback;
    using DatabaseUpdateUpdateGroup = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateGroup;
    using DatabaseUpdateUpdatePart = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdatePart;
    using DatabaseMembershipUser = Gorba.Center.BackgroundSystem.Data.Model.Membership.User;
    using DatabaseMetaUserDefinedProperty = Gorba.Center.BackgroundSystem.Data.Model.Meta.UserDefinedProperty;
    using DatabaseAccessControlUserRole = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.UserRole;

    using DtoXmlData = Gorba.Center.Common.ServiceModel.XmlData;
    using DtoMembershipAssociationTenantUserUserRole = Gorba.Center.Common.ServiceModel.Membership.AssociationTenantUserUserRole;
    using DtoAccessControlAuthorization = Gorba.Center.Common.ServiceModel.AccessControl.Authorization;
    using DtoResourcesContentResource = Gorba.Center.Common.ServiceModel.Resources.ContentResource;
    using DtoDocumentsDocument = Gorba.Center.Common.ServiceModel.Documents.Document;
    using DtoDocumentsDocumentVersion = Gorba.Center.Common.ServiceModel.Documents.DocumentVersion;
    using DtoLogLogEntry = Gorba.Center.Common.ServiceModel.Log.LogEntry;
    using DtoConfigurationsMediaConfiguration = Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration;
    using DtoSoftwarePackage = Gorba.Center.Common.ServiceModel.Software.Package;
    using DtoSoftwarePackageVersion = Gorba.Center.Common.ServiceModel.Software.PackageVersion;
    using DtoUnitsProductType = Gorba.Center.Common.ServiceModel.Units.ProductType;
    using DtoResourcesResource = Gorba.Center.Common.ServiceModel.Resources.Resource;
    using DtoMetaSystemConfig = Gorba.Center.Common.ServiceModel.Meta.SystemConfig;
    using DtoMembershipTenant = Gorba.Center.Common.ServiceModel.Membership.Tenant;
    using DtoUnitsUnit = Gorba.Center.Common.ServiceModel.Units.Unit;
    using DtoConfigurationsUnitConfiguration = Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration;
    using DtoUpdateUpdateCommand = Gorba.Center.Common.ServiceModel.Update.UpdateCommand;
    using DtoUpdateUpdateFeedback = Gorba.Center.Common.ServiceModel.Update.UpdateFeedback;
    using DtoUpdateUpdateGroup = Gorba.Center.Common.ServiceModel.Update.UpdateGroup;
    using DtoUpdateUpdatePart = Gorba.Center.Common.ServiceModel.Update.UpdatePart;
    using DtoMembershipUser = Gorba.Center.Common.ServiceModel.Membership.User;
    using DtoMetaUserDefinedProperty = Gorba.Center.Common.ServiceModel.Meta.UserDefinedProperty;
    using DtoAccessControlUserRole = Gorba.Center.Common.ServiceModel.AccessControl.UserRole;

    internal enum ReferenceTypes
    {
        DatabaseAccessControlDataScope = 0,
        DatabaseResourcesHashAlgorithmTypes = 1,
        DatabaseLogLevel = 2,
        DatabaseAccessControlPermission = 3,
        DatabaseUnitsUnitTypes = 4,
        DatabaseUpdateUpdatePartType = 5,
        DatabaseUpdateUpdateState = 6,
        DatabaseMetaUserDefinedPropertyEnabledEntity = 7,
        DtoAccessControlDataScope = 8,
        DtoResourcesHashAlgorithmTypes = 9,
        DtoLogLevel = 10,
        DtoAccessControlPermission = 11,
        DtoUnitsUnitTypes = 12,
        DtoUpdateUpdatePartType = 13,
        DtoUpdateUpdateState = 14,
        DtoMetaUserDefinedPropertyEnabledEntity = 15,
        DatabaseMembershipAssociationTenantUserUserRole = 16,
        DatabaseAccessControlAuthorization = 17,
        DatabaseResourcesContentResource = 18,
        DatabaseDocumentsDocument = 19,
        DatabaseDocumentsDocumentVersion = 20,
        DatabaseLogLogEntry = 21,
        DatabaseConfigurationsMediaConfiguration = 22,
        DatabaseSoftwarePackage = 23,
        DatabaseSoftwarePackageVersion = 24,
        DatabaseUnitsProductType = 25,
        DatabaseResourcesResource = 26,
        DatabaseMetaSystemConfig = 27,
        DatabaseMembershipTenant = 28,
        DatabaseUnitsUnit = 29,
        DatabaseConfigurationsUnitConfiguration = 30,
        DatabaseUpdateUpdateCommand = 31,
        DatabaseUpdateUpdateFeedback = 32,
        DatabaseUpdateUpdateGroup = 33,
        DatabaseUpdateUpdatePart = 34,
        DatabaseMembershipUser = 35,
        DatabaseMetaUserDefinedProperty = 36,
        DatabaseAccessControlUserRole = 37,
        DtoMembershipAssociationTenantUserUserRole = 24,
        DtoAccessControlAuthorization = 25,
        DtoResourcesContentResource = 26,
        DtoDocumentsDocument = 27,
        DtoDocumentsDocumentVersion = 28,
        DtoLogLogEntry = 29,
        DtoConfigurationsMediaConfiguration = 30,
        DtoSoftwarePackage = 31,
        DtoSoftwarePackageVersion = 32,
        DtoUnitsProductType = 33,
        DtoResourcesResource = 34,
        DtoMetaSystemConfig = 35,
        DtoMembershipTenant = 36,
        DtoUnitsUnit = 37,
        DtoConfigurationsUnitConfiguration = 38,
        DtoUpdateUpdateCommand = 39,
        DtoUpdateUpdateFeedback = 40,
        DtoUpdateUpdateGroup = 41,
        DtoUpdateUpdatePart = 42,
        DtoMembershipUser = 43,
        DtoMetaUserDefinedProperty = 44,
        DtoAccessControlUserRole = 45
    }

    public static partial class Extensions
    {
        public static DatabaseMembershipAssociationTenantUserUserRole ToDatabase(
            this DtoMembershipAssociationTenantUserUserRole source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseMembershipAssociationTenantUserUserRole ToDatabaseInternal(
            this DtoMembershipAssociationTenantUserUserRole source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseMembershipAssociationTenantUserUserRole);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseMembershipAssociationTenantUserUserRole)mappedObjects[key];
            }

            var destination = new DatabaseMembershipAssociationTenantUserUserRole();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Tenant == DtoMembershipTenant.Null)
            {
                destination.Tenant = DatabaseMembershipTenant.Null;
            }
            else if (source.Tenant != null)
            {
                destination.Tenant = source.Tenant.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.User == DtoMembershipUser.Null)
            {
                destination.User = DatabaseMembershipUser.Null;
            }
            else if (source.User != null)
            {
                destination.User = source.User.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.UserRole == DtoAccessControlUserRole.Null)
            {
                destination.UserRole = DatabaseAccessControlUserRole.Null;
            }
            else if (source.UserRole != null)
            {
                destination.UserRole = source.UserRole.ToDatabaseInternal(mappedObjects);
            }

            return destination;
        }

        public static DatabaseAccessControlAuthorization ToDatabase(
            this DtoAccessControlAuthorization source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseAccessControlAuthorization ToDatabaseInternal(
            this DtoAccessControlAuthorization source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseAccessControlAuthorization);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseAccessControlAuthorization)mappedObjects[key];
            }

            var destination = new DatabaseAccessControlAuthorization();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UserRole == DtoAccessControlUserRole.Null)
            {
                destination.UserRole = DatabaseAccessControlUserRole.Null;
            }
            else if (source.UserRole != null)
            {
                destination.UserRole = source.UserRole.ToDatabaseInternal(mappedObjects);
            }
            destination.DataScope = (DatabaseAccessControlDataScope)source.DataScope;
            destination.Permission = (DatabaseAccessControlPermission)source.Permission;

            return destination;
        }

        public static DatabaseResourcesContentResource ToDatabase(
            this DtoResourcesContentResource source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseResourcesContentResource ToDatabaseInternal(
            this DtoResourcesContentResource source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseResourcesContentResource);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseResourcesContentResource)mappedObjects[key];
            }

            var destination = new DatabaseResourcesContentResource();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UploadingUser == DtoMembershipUser.Null)
            {
                destination.UploadingUser = DatabaseMembershipUser.Null;
            }
            else if (source.UploadingUser != null)
            {
                destination.UploadingUser = source.UploadingUser.ToDatabaseInternal(mappedObjects);
            }
            destination.OriginalFilename = source.OriginalFilename;
            destination.Description = source.Description;
            destination.ThumbnailHash = source.ThumbnailHash;
            destination.Hash = source.Hash;
            destination.HashAlgorithmType = (DatabaseResourcesHashAlgorithmTypes)source.HashAlgorithmType;
            destination.MimeType = source.MimeType;
            destination.Length = source.Length;

            return destination;
        }

        public static DatabaseDocumentsDocument ToDatabase(
            this DtoDocumentsDocument source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseDocumentsDocument ToDatabaseInternal(
            this DtoDocumentsDocument source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseDocumentsDocument);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseDocumentsDocument)mappedObjects[key];
            }

            var destination = new DatabaseDocumentsDocument();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;
    
            if (source.Tenant == DtoMembershipTenant.Null)
            {
                destination.Tenant = DatabaseMembershipTenant.Null;
            }
            else if (source.Tenant != null)
            {
                destination.Tenant = source.Tenant.ToDatabaseInternal(mappedObjects);
            }

            if (source.Versions == null)
            {
                destination.Versions = null;
            }
            else
            {
                
                destination.Versions = source
                    .Versions.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            return destination;
        }

        public static DatabaseDocumentsDocumentVersion ToDatabase(
            this DtoDocumentsDocumentVersion source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseDocumentsDocumentVersion ToDatabaseInternal(
            this DtoDocumentsDocumentVersion source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseDocumentsDocumentVersion);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseDocumentsDocumentVersion)mappedObjects[key];
            }

            var destination = new DatabaseDocumentsDocumentVersion();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Document == DtoDocumentsDocument.Null)
            {
                destination.Document = DatabaseDocumentsDocument.Null;
            }
            else if (source.Document != null)
            {
                destination.Document = source.Document.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.CreatingUser == DtoMembershipUser.Null)
            {
                destination.CreatingUser = DatabaseMembershipUser.Null;
            }
            else if (source.CreatingUser != null)
            {
                destination.CreatingUser = source.CreatingUser.ToDatabaseInternal(mappedObjects);
            }
            destination.Major = source.Major;
            destination.Minor = source.Minor;
            destination.Content = source.Content.ToDatabase();
            destination.Description = source.Description;

            return destination;
        }

        public static DatabaseLogLogEntry ToDatabase(
            this DtoLogLogEntry source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseLogLogEntry ToDatabaseInternal(
            this DtoLogLogEntry source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseLogLogEntry);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseLogLogEntry)mappedObjects[key];
            }

            var destination = new DatabaseLogLogEntry();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;

            if (source.Unit != null)
            {
                destination.Unit_Id = source.Unit.Id;
            }
    
            if (source.Unit == DtoUnitsUnit.Null)
            {
                destination.Unit = DatabaseUnitsUnit.Null;
            }
            else if (source.Unit != null)
            {
                destination.Unit = source.Unit.ToDatabaseInternal(mappedObjects);
            }
            destination.Application = source.Application;
            destination.Timestamp = source.Timestamp;
            destination.Level = (DatabaseLogLevel)source.Level;
            destination.Logger = source.Logger;
            destination.Message = source.Message;
            destination.AdditionalData = source.AdditionalData;

            return destination;
        }

        public static DatabaseConfigurationsMediaConfiguration ToDatabase(
            this DtoConfigurationsMediaConfiguration source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseConfigurationsMediaConfiguration ToDatabaseInternal(
            this DtoConfigurationsMediaConfiguration source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseConfigurationsMediaConfiguration);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseConfigurationsMediaConfiguration)mappedObjects[key];
            }

            var destination = new DatabaseConfigurationsMediaConfiguration();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;

            if (source.UpdateGroups == null)
            {
                destination.UpdateGroups = null;
            }
            else
            {
                
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
    
            if (source.Document == DtoDocumentsDocument.Null)
            {
                destination.Document = DatabaseDocumentsDocument.Null;
            }
            else if (source.Document != null)
            {
                destination.Document = source.Document.ToDatabaseInternal(mappedObjects);
            }

            return destination;
        }

        public static DatabaseSoftwarePackage ToDatabase(
            this DtoSoftwarePackage source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseSoftwarePackage ToDatabaseInternal(
            this DtoSoftwarePackage source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseSoftwarePackage);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseSoftwarePackage)mappedObjects[key];
            }

            var destination = new DatabaseSoftwarePackage();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.PackageId = source.PackageId;
            destination.ProductName = source.ProductName;
            destination.Description = source.Description;

            if (source.Versions == null)
            {
                destination.Versions = null;
            }
            else
            {
                
                destination.Versions = source
                    .Versions.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            return destination;
        }

        public static DatabaseSoftwarePackageVersion ToDatabase(
            this DtoSoftwarePackageVersion source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseSoftwarePackageVersion ToDatabaseInternal(
            this DtoSoftwarePackageVersion source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseSoftwarePackageVersion);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseSoftwarePackageVersion)mappedObjects[key];
            }

            var destination = new DatabaseSoftwarePackageVersion();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Package == DtoSoftwarePackage.Null)
            {
                destination.Package = DatabaseSoftwarePackage.Null;
            }
            else if (source.Package != null)
            {
                destination.Package = source.Package.ToDatabaseInternal(mappedObjects);
            }
            destination.SoftwareVersion = source.SoftwareVersion;
            destination.Structure = source.Structure.ToDatabase();
            destination.Description = source.Description;

            return destination;
        }

        public static DatabaseUnitsProductType ToDatabase(
            this DtoUnitsProductType source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUnitsProductType ToDatabaseInternal(
            this DtoUnitsProductType source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUnitsProductType);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUnitsProductType)mappedObjects[key];
            }

            var destination = new DatabaseUnitsProductType();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.UnitType = (DatabaseUnitsUnitTypes)source.UnitType;

            if (source.Units == null)
            {
                destination.Units = null;
            }
            else
            {
                
                destination.Units = source
                    .Units.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
            destination.Name = source.Name;
            destination.Description = source.Description;
            destination.HardwareDescriptor = source.HardwareDescriptor.ToDatabase();

            return destination;
        }

        public static DatabaseResourcesResource ToDatabase(
            this DtoResourcesResource source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseResourcesResource ToDatabaseInternal(
            this DtoResourcesResource source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseResourcesResource);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseResourcesResource)mappedObjects[key];
            }

            var destination = new DatabaseResourcesResource();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UploadingUser == DtoMembershipUser.Null)
            {
                destination.UploadingUser = DatabaseMembershipUser.Null;
            }
            else if (source.UploadingUser != null)
            {
                destination.UploadingUser = source.UploadingUser.ToDatabaseInternal(mappedObjects);
            }
            destination.OriginalFilename = source.OriginalFilename;
            destination.Description = source.Description;
            destination.Hash = source.Hash;
            destination.ThumbnailHash = source.ThumbnailHash;
            destination.MimeType = source.MimeType;
            destination.Length = source.Length;

            return destination;
        }

        public static DatabaseMetaSystemConfig ToDatabase(
            this DtoMetaSystemConfig source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseMetaSystemConfig ToDatabaseInternal(
            this DtoMetaSystemConfig source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseMetaSystemConfig);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseMetaSystemConfig)mappedObjects[key];
            }

            var destination = new DatabaseMetaSystemConfig();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.SystemId = source.SystemId;
            destination.Settings = source.Settings.ToDatabase();

            return destination;
        }

        public static DatabaseMembershipTenant ToDatabase(
            this DtoMembershipTenant source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseMembershipTenant ToDatabaseInternal(
            this DtoMembershipTenant source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseMembershipTenant);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseMembershipTenant)mappedObjects[key];
            }

            var destination = new DatabaseMembershipTenant();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;

            if (source.Users == null)
            {
                destination.Users = null;
            }
            else
            {
                
                destination.Users = source
                    .Users.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            if (source.UpdateGroups == null)
            {
                destination.UpdateGroups = null;
            }
            else
            {
                
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

			if (source.UserDefinedProperties != null)
			{
				foreach (var k in source.UserDefinedProperties.Keys)
				{
					destination.RawUserDefinedProperties.Add(k, source.UserDefinedProperties[k]);
				}
			}

            return destination;
        }

        public static DatabaseUnitsUnit ToDatabase(
            this DtoUnitsUnit source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUnitsUnit ToDatabaseInternal(
            this DtoUnitsUnit source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUnitsUnit);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUnitsUnit)mappedObjects[key];
            }

            var destination = new DatabaseUnitsUnit();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Tenant == DtoMembershipTenant.Null)
            {
                destination.Tenant = DatabaseMembershipTenant.Null;
            }
            else if (source.Tenant != null)
            {
                destination.Tenant = source.Tenant.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.ProductType == DtoUnitsProductType.Null)
            {
                destination.ProductType = DatabaseUnitsProductType.Null;
            }
            else if (source.ProductType != null)
            {
                destination.ProductType = source.ProductType.ToDatabaseInternal(mappedObjects);
            }
            destination.Name = source.Name;
            destination.NetworkAddress = source.NetworkAddress;
            destination.Description = source.Description;
            destination.IsConnected = source.IsConnected;

            if (source.UpdateCommands == null)
            {
                destination.UpdateCommands = null;
            }
            else
            {
                
                destination.UpdateCommands = source
                    .UpdateCommands.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
    
            if (source.UpdateGroup == DtoUpdateUpdateGroup.Null)
            {
                destination.UpdateGroup = DatabaseUpdateUpdateGroup.Null;
            }
            else if (source.UpdateGroup != null)
            {
                destination.UpdateGroup = source.UpdateGroup.ToDatabaseInternal(mappedObjects);
            }

			if (source.UserDefinedProperties != null)
			{
				foreach (var k in source.UserDefinedProperties.Keys)
				{
					destination.RawUserDefinedProperties.Add(k, source.UserDefinedProperties[k]);
				}
			}

            return destination;
        }

        public static DatabaseConfigurationsUnitConfiguration ToDatabase(
            this DtoConfigurationsUnitConfiguration source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseConfigurationsUnitConfiguration ToDatabaseInternal(
            this DtoConfigurationsUnitConfiguration source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseConfigurationsUnitConfiguration);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseConfigurationsUnitConfiguration)mappedObjects[key];
            }

            var destination = new DatabaseConfigurationsUnitConfiguration();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;

            if (source.UpdateGroups == null)
            {
                destination.UpdateGroups = null;
            }
            else
            {
                
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
    
            if (source.Document == DtoDocumentsDocument.Null)
            {
                destination.Document = DatabaseDocumentsDocument.Null;
            }
            else if (source.Document != null)
            {
                destination.Document = source.Document.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.ProductType == DtoUnitsProductType.Null)
            {
                destination.ProductType = DatabaseUnitsProductType.Null;
            }
            else if (source.ProductType != null)
            {
                destination.ProductType = source.ProductType.ToDatabaseInternal(mappedObjects);
            }

            return destination;
        }

        public static DatabaseUpdateUpdateCommand ToDatabase(
            this DtoUpdateUpdateCommand source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUpdateUpdateCommand ToDatabaseInternal(
            this DtoUpdateUpdateCommand source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUpdateUpdateCommand);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUpdateUpdateCommand)mappedObjects[key];
            }

            var destination = new DatabaseUpdateUpdateCommand();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.UpdateIndex = source.UpdateIndex;
    
            if (source.Unit == DtoUnitsUnit.Null)
            {
                destination.Unit = DatabaseUnitsUnit.Null;
            }
            else if (source.Unit != null)
            {
                destination.Unit = source.Unit.ToDatabaseInternal(mappedObjects);
            }
            destination.Command = source.Command.ToDatabase();
            destination.WasTransferred = source.WasTransferred;
            destination.WasInstalled = source.WasInstalled;

            if (source.IncludedParts == null)
            {
                destination.IncludedParts = null;
            }
            else
            {
                
                destination.IncludedParts = source
                    .IncludedParts.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            if (source.Feedbacks == null)
            {
                destination.Feedbacks = null;
            }
            else
            {
                
                destination.Feedbacks = source
                    .Feedbacks.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            return destination;
        }

        public static DatabaseUpdateUpdateFeedback ToDatabase(
            this DtoUpdateUpdateFeedback source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUpdateUpdateFeedback ToDatabaseInternal(
            this DtoUpdateUpdateFeedback source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUpdateUpdateFeedback);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUpdateUpdateFeedback)mappedObjects[key];
            }

            var destination = new DatabaseUpdateUpdateFeedback();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UpdateCommand == DtoUpdateUpdateCommand.Null)
            {
                destination.UpdateCommand = DatabaseUpdateUpdateCommand.Null;
            }
            else if (source.UpdateCommand != null)
            {
                destination.UpdateCommand = source.UpdateCommand.ToDatabaseInternal(mappedObjects);
            }
            destination.Timestamp = source.Timestamp;
            destination.State = (DatabaseUpdateUpdateState)source.State;
            destination.Feedback = source.Feedback.ToDatabase();

            return destination;
        }

        public static DatabaseUpdateUpdateGroup ToDatabase(
            this DtoUpdateUpdateGroup source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUpdateUpdateGroup ToDatabaseInternal(
            this DtoUpdateUpdateGroup source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUpdateUpdateGroup);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUpdateUpdateGroup)mappedObjects[key];
            }

            var destination = new DatabaseUpdateUpdateGroup();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;
    
            if (source.Tenant == DtoMembershipTenant.Null)
            {
                destination.Tenant = DatabaseMembershipTenant.Null;
            }
            else if (source.Tenant != null)
            {
                destination.Tenant = source.Tenant.ToDatabaseInternal(mappedObjects);
            }

            if (source.Units == null)
            {
                destination.Units = null;
            }
            else
            {
                
                destination.Units = source
                    .Units.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            if (source.UpdateParts == null)
            {
                destination.UpdateParts = null;
            }
            else
            {
                
                destination.UpdateParts = source
                    .UpdateParts.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
    
            if (source.UnitConfiguration == DtoConfigurationsUnitConfiguration.Null)
            {
                destination.UnitConfiguration = DatabaseConfigurationsUnitConfiguration.Null;
            }
            else if (source.UnitConfiguration != null)
            {
                destination.UnitConfiguration = source.UnitConfiguration.ToDatabaseInternal(mappedObjects);
            }
    
            if (source.MediaConfiguration == DtoConfigurationsMediaConfiguration.Null)
            {
                destination.MediaConfiguration = DatabaseConfigurationsMediaConfiguration.Null;
            }
            else if (source.MediaConfiguration != null)
            {
                destination.MediaConfiguration = source.MediaConfiguration.ToDatabaseInternal(mappedObjects);
            }

			if (source.UserDefinedProperties != null)
			{
				foreach (var k in source.UserDefinedProperties.Keys)
				{
					destination.RawUserDefinedProperties.Add(k, source.UserDefinedProperties[k]);
				}
			}

            return destination;
        }

        public static DatabaseUpdateUpdatePart ToDatabase(
            this DtoUpdateUpdatePart source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseUpdateUpdatePart ToDatabaseInternal(
            this DtoUpdateUpdatePart source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseUpdateUpdatePart);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseUpdateUpdatePart)mappedObjects[key];
            }

            var destination = new DatabaseUpdateUpdatePart();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UpdateGroup == DtoUpdateUpdateGroup.Null)
            {
                destination.UpdateGroup = DatabaseUpdateUpdateGroup.Null;
            }
            else if (source.UpdateGroup != null)
            {
                destination.UpdateGroup = source.UpdateGroup.ToDatabaseInternal(mappedObjects);
            }
            destination.Type = (DatabaseUpdateUpdatePartType)source.Type;
            destination.Start = source.Start;
            destination.End = source.End;
            destination.Description = source.Description;
            destination.Structure = source.Structure.ToDatabase();
            destination.InstallInstructions = source.InstallInstructions.ToDatabase();
            destination.DynamicContent = source.DynamicContent.ToDatabase();

            if (source.RelatedCommands == null)
            {
                destination.RelatedCommands = null;
            }
            else
            {
                
                destination.RelatedCommands = source
                    .RelatedCommands.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

            return destination;
        }

        public static DatabaseMembershipUser ToDatabase(
            this DtoMembershipUser source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseMembershipUser ToDatabaseInternal(
            this DtoMembershipUser source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseMembershipUser);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseMembershipUser)mappedObjects[key];
            }

            var destination = new DatabaseMembershipUser();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.OwnerTenant == DtoMembershipTenant.Null)
            {
                destination.OwnerTenant = DatabaseMembershipTenant.Null;
            }
            else if (source.OwnerTenant != null)
            {
                destination.OwnerTenant = source.OwnerTenant.ToDatabaseInternal(mappedObjects);
            }

            if (source.AssociationTenantUserUserRoles == null)
            {
                destination.AssociationTenantUserUserRoles = null;
            }
            else
            {
                
                destination.AssociationTenantUserUserRoles = source
                    .AssociationTenantUserUserRoles.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }
            destination.Username = source.Username;
            destination.Domain = source.Domain;
            destination.HashedPassword = source.HashedPassword;
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.Email = source.Email;
            destination.Culture = source.Culture;
            destination.TimeZone = source.TimeZone;
            destination.Description = source.Description;
            destination.LastLoginAttempt = source.LastLoginAttempt;
            destination.LastSuccessfulLogin = source.LastSuccessfulLogin;
            destination.ConsecutiveLoginFailures = source.ConsecutiveLoginFailures;
            destination.IsEnabled = source.IsEnabled;

			if (source.UserDefinedProperties != null)
			{
				foreach (var k in source.UserDefinedProperties.Keys)
				{
					destination.RawUserDefinedProperties.Add(k, source.UserDefinedProperties[k]);
				}
			}

            return destination;
        }

        public static DatabaseMetaUserDefinedProperty ToDatabase(
            this DtoMetaUserDefinedProperty source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseMetaUserDefinedProperty ToDatabaseInternal(
            this DtoMetaUserDefinedProperty source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseMetaUserDefinedProperty);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseMetaUserDefinedProperty)mappedObjects[key];
            }

            var destination = new DatabaseMetaUserDefinedProperty();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.OwnerEntity = (DatabaseMetaUserDefinedPropertyEnabledEntity)source.OwnerEntity;
    
            if (source.Tenant == DtoMembershipTenant.Null)
            {
                destination.Tenant = DatabaseMembershipTenant.Null;
            }
            else if (source.Tenant != null)
            {
                destination.Tenant = source.Tenant.ToDatabaseInternal(mappedObjects);
            }
            destination.Name = source.Name;

            return destination;
        }

        public static DatabaseAccessControlUserRole ToDatabase(
            this DtoAccessControlUserRole source)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDatabaseInternal(mappedObjects);
        }

        internal static DatabaseAccessControlUserRole ToDatabaseInternal(
            this DtoAccessControlUserRole source, IDictionary<EntityKey, object> mappedObjects)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DatabaseAccessControlUserRole);
            if (mappedObjects.ContainsKey(key))
            {
                return (DatabaseAccessControlUserRole)mappedObjects[key];
            }

            var destination = new DatabaseAccessControlUserRole();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;

            if (source.Authorizations == null)
            {
                destination.Authorizations = null;
            }
            else
            {
                
                destination.Authorizations = source
                    .Authorizations.Select(entity => ToDatabaseInternal(entity, mappedObjects))
                    .ToList();
            }

			if (source.UserDefinedProperties != null)
			{
				foreach (var k in source.UserDefinedProperties.Keys)
				{
					destination.RawUserDefinedProperties.Add(k, source.UserDefinedProperties[k]);
				}
			}

            return destination;
        }

        public static DtoMembershipAssociationTenantUserUserRole ToDto(
            this DatabaseMembershipAssociationTenantUserUserRole source, AssociationTenantUserUserRoleFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoMembershipAssociationTenantUserUserRole ToDtoInternal(
            this DatabaseMembershipAssociationTenantUserUserRole source, IDictionary<EntityKey, object> mappedObjects, AssociationTenantUserUserRoleFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoMembershipAssociationTenantUserUserRole);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoMembershipAssociationTenantUserUserRole)mappedObjects[key];
            }

            var destination = new DtoMembershipAssociationTenantUserUserRole();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Tenant != null && (filter == null || filter.Tenant != null))
            {
                destination.Tenant = source.Tenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.Tenant);
            }
    
            if (source.User != null && (filter == null || filter.User != null))
            {
                destination.User = source.User.ToDtoInternal(mappedObjects, filter == null ? null : filter.User);
            }
    
            if (source.UserRole != null && (filter == null || filter.UserRole != null))
            {
                destination.UserRole = source.UserRole.ToDtoInternal(mappedObjects, filter == null ? null : filter.UserRole);
            }

            return destination;
        }

        public static DtoAccessControlAuthorization ToDto(
            this DatabaseAccessControlAuthorization source, AuthorizationFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoAccessControlAuthorization ToDtoInternal(
            this DatabaseAccessControlAuthorization source, IDictionary<EntityKey, object> mappedObjects, AuthorizationFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoAccessControlAuthorization);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoAccessControlAuthorization)mappedObjects[key];
            }

            var destination = new DtoAccessControlAuthorization();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UserRole != null && (filter == null || filter.UserRole != null))
            {
                destination.UserRole = source.UserRole.ToDtoInternal(mappedObjects, filter == null ? null : filter.UserRole);
            }
            destination.DataScope = (DtoAccessControlDataScope)source.DataScope;
            destination.Permission = (DtoAccessControlPermission)source.Permission;

            return destination;
        }

        public static DtoResourcesContentResource ToDto(
            this DatabaseResourcesContentResource source, ContentResourceFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoResourcesContentResource ToDtoInternal(
            this DatabaseResourcesContentResource source, IDictionary<EntityKey, object> mappedObjects, ContentResourceFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoResourcesContentResource);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoResourcesContentResource)mappedObjects[key];
            }

            var destination = new DtoResourcesContentResource();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UploadingUser != null && (filter == null || filter.UploadingUser != null))
            {
                destination.UploadingUser = source.UploadingUser.ToDtoInternal(mappedObjects, filter == null ? null : filter.UploadingUser);
            }
            destination.OriginalFilename = source.OriginalFilename;
            destination.Description = source.Description;
            destination.ThumbnailHash = source.ThumbnailHash;
            destination.Hash = source.Hash;
            destination.HashAlgorithmType = (DtoResourcesHashAlgorithmTypes)source.HashAlgorithmType;
            destination.MimeType = source.MimeType;
            destination.Length = source.Length;

            return destination;
        }

        public static DtoDocumentsDocument ToDto(
            this DatabaseDocumentsDocument source, DocumentFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoDocumentsDocument ToDtoInternal(
            this DatabaseDocumentsDocument source, IDictionary<EntityKey, object> mappedObjects, DocumentFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoDocumentsDocument);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoDocumentsDocument)mappedObjects[key];
            }

            var destination = new DtoDocumentsDocument();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;
    
            if (source.Tenant != null && (filter == null || filter.Tenant != null))
            {
                destination.Tenant = source.Tenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.Tenant);
            }

            if (source.Versions == null || (filter != null && filter.Versions == null))
            {
                destination.Versions = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Versions;
                destination.Versions = source
                    .Versions.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            return destination;
        }

        public static DtoDocumentsDocumentVersion ToDto(
            this DatabaseDocumentsDocumentVersion source, DocumentVersionFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoDocumentsDocumentVersion ToDtoInternal(
            this DatabaseDocumentsDocumentVersion source, IDictionary<EntityKey, object> mappedObjects, DocumentVersionFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoDocumentsDocumentVersion);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoDocumentsDocumentVersion)mappedObjects[key];
            }

            var destination = new DtoDocumentsDocumentVersion();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Document != null && (filter == null || filter.Document != null))
            {
                destination.Document = source.Document.ToDtoInternal(mappedObjects, filter == null ? null : filter.Document);
            }
    
            if (source.CreatingUser != null && (filter == null || filter.CreatingUser != null))
            {
                destination.CreatingUser = source.CreatingUser.ToDtoInternal(mappedObjects, filter == null ? null : filter.CreatingUser);
            }
            destination.Major = source.Major;
            destination.Minor = source.Minor;
            destination.Content = source.Content.ToDto();
            destination.Description = source.Description;

            return destination;
        }

        public static DtoLogLogEntry ToDto(
            this DatabaseLogLogEntry source, LogEntryFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoLogLogEntry ToDtoInternal(
            this DatabaseLogLogEntry source, IDictionary<EntityKey, object> mappedObjects, LogEntryFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoLogLogEntry);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoLogLogEntry)mappedObjects[key];
            }

            var destination = new DtoLogLogEntry();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
    
            if (source.Unit != null && (filter == null || filter.Unit != null))
            {
                destination.Unit = source.Unit.ToDtoInternal(mappedObjects, filter == null ? null : filter.Unit);
            }
            destination.Application = source.Application;
            destination.Timestamp = source.Timestamp;
            destination.Level = (DtoLogLevel)source.Level;
            destination.Logger = source.Logger;
            destination.Message = source.Message;
            destination.AdditionalData = source.AdditionalData;

            return destination;
        }

        public static DtoConfigurationsMediaConfiguration ToDto(
            this DatabaseConfigurationsMediaConfiguration source, MediaConfigurationFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoConfigurationsMediaConfiguration ToDtoInternal(
            this DatabaseConfigurationsMediaConfiguration source, IDictionary<EntityKey, object> mappedObjects, MediaConfigurationFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoConfigurationsMediaConfiguration);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoConfigurationsMediaConfiguration)mappedObjects[key];
            }

            var destination = new DtoConfigurationsMediaConfiguration();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;

            if (source.UpdateGroups == null || (filter != null && filter.UpdateGroups == null))
            {
                destination.UpdateGroups = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.UpdateGroups;
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
    
            if (source.Document != null && (filter == null || filter.Document != null))
            {
                destination.Document = source.Document.ToDtoInternal(mappedObjects, filter == null ? null : filter.Document);
            }

            return destination;
        }

        public static DtoSoftwarePackage ToDto(
            this DatabaseSoftwarePackage source, PackageFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoSoftwarePackage ToDtoInternal(
            this DatabaseSoftwarePackage source, IDictionary<EntityKey, object> mappedObjects, PackageFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoSoftwarePackage);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoSoftwarePackage)mappedObjects[key];
            }

            var destination = new DtoSoftwarePackage();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.PackageId = source.PackageId;
            destination.ProductName = source.ProductName;
            destination.Description = source.Description;

            if (source.Versions == null || (filter != null && filter.Versions == null))
            {
                destination.Versions = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Versions;
                destination.Versions = source
                    .Versions.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            return destination;
        }

        public static DtoSoftwarePackageVersion ToDto(
            this DatabaseSoftwarePackageVersion source, PackageVersionFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoSoftwarePackageVersion ToDtoInternal(
            this DatabaseSoftwarePackageVersion source, IDictionary<EntityKey, object> mappedObjects, PackageVersionFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoSoftwarePackageVersion);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoSoftwarePackageVersion)mappedObjects[key];
            }

            var destination = new DtoSoftwarePackageVersion();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Package != null && (filter == null || filter.Package != null))
            {
                destination.Package = source.Package.ToDtoInternal(mappedObjects, filter == null ? null : filter.Package);
            }
            destination.SoftwareVersion = source.SoftwareVersion;
            destination.Structure = source.Structure.ToDto();
            destination.Description = source.Description;

            return destination;
        }

        public static DtoUnitsProductType ToDto(
            this DatabaseUnitsProductType source, ProductTypeFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUnitsProductType ToDtoInternal(
            this DatabaseUnitsProductType source, IDictionary<EntityKey, object> mappedObjects, ProductTypeFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUnitsProductType);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUnitsProductType)mappedObjects[key];
            }

            var destination = new DtoUnitsProductType();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.UnitType = (DtoUnitsUnitTypes)source.UnitType;

            if (source.Units == null || (filter != null && filter.Units == null))
            {
                destination.Units = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Units;
                destination.Units = source
                    .Units.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
            destination.Name = source.Name;
            destination.Description = source.Description;
            destination.HardwareDescriptor = source.HardwareDescriptor.ToDto();

            return destination;
        }

        public static DtoResourcesResource ToDto(
            this DatabaseResourcesResource source, ResourceFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoResourcesResource ToDtoInternal(
            this DatabaseResourcesResource source, IDictionary<EntityKey, object> mappedObjects, ResourceFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoResourcesResource);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoResourcesResource)mappedObjects[key];
            }

            var destination = new DtoResourcesResource();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UploadingUser != null && (filter == null || filter.UploadingUser != null))
            {
                destination.UploadingUser = source.UploadingUser.ToDtoInternal(mappedObjects, filter == null ? null : filter.UploadingUser);
            }
            destination.OriginalFilename = source.OriginalFilename;
            destination.Description = source.Description;
            destination.Hash = source.Hash;
            destination.ThumbnailHash = source.ThumbnailHash;
            destination.MimeType = source.MimeType;
            destination.Length = source.Length;

            return destination;
        }

        public static DtoMetaSystemConfig ToDto(
            this DatabaseMetaSystemConfig source, SystemConfigFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoMetaSystemConfig ToDtoInternal(
            this DatabaseMetaSystemConfig source, IDictionary<EntityKey, object> mappedObjects, SystemConfigFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoMetaSystemConfig);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoMetaSystemConfig)mappedObjects[key];
            }

            var destination = new DtoMetaSystemConfig();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.SystemId = source.SystemId;
            destination.Settings = source.Settings.ToDto();

            return destination;
        }

        public static DtoMembershipTenant ToDto(
            this DatabaseMembershipTenant source, TenantFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoMembershipTenant ToDtoInternal(
            this DatabaseMembershipTenant source, IDictionary<EntityKey, object> mappedObjects, TenantFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoMembershipTenant);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoMembershipTenant)mappedObjects[key];
            }

            var destination = new DtoMembershipTenant();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;

            if (source.Users == null || (filter != null && filter.Users == null))
            {
                destination.Users = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Users;
                destination.Users = source
                    .Users.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            if (source.UpdateGroups == null || (filter != null && filter.UpdateGroups == null))
            {
                destination.UpdateGroups = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.UpdateGroups;
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            foreach (var userDefinedProperty in source.UserDefinedProperties)
            {
                destination.UserDefinedProperties
                        .Add(userDefinedProperty.PropertyDefinition.Name, userDefinedProperty.Value);
            }

            return destination;
        }

        public static DtoUnitsUnit ToDto(
            this DatabaseUnitsUnit source, UnitFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUnitsUnit ToDtoInternal(
            this DatabaseUnitsUnit source, IDictionary<EntityKey, object> mappedObjects, UnitFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUnitsUnit);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUnitsUnit)mappedObjects[key];
            }

            var destination = new DtoUnitsUnit();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.Tenant != null && (filter == null || filter.Tenant != null))
            {
                destination.Tenant = source.Tenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.Tenant);
            }
    
            if (source.ProductType != null && (filter == null || filter.ProductType != null))
            {
                destination.ProductType = source.ProductType.ToDtoInternal(mappedObjects, filter == null ? null : filter.ProductType);
            }
            destination.Name = source.Name;
            destination.NetworkAddress = source.NetworkAddress;
            destination.Description = source.Description;
            destination.IsConnected = source.IsConnected;

            if (source.UpdateCommands == null || (filter != null && filter.UpdateCommands == null))
            {
                destination.UpdateCommands = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.UpdateCommands;
                destination.UpdateCommands = source
                    .UpdateCommands.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
    
            if (source.UpdateGroup != null && (filter == null || filter.UpdateGroup != null))
            {
                destination.UpdateGroup = source.UpdateGroup.ToDtoInternal(mappedObjects, filter == null ? null : filter.UpdateGroup);
            }

            foreach (var userDefinedProperty in source.UserDefinedProperties)
            {
                destination.UserDefinedProperties
                        .Add(userDefinedProperty.PropertyDefinition.Name, userDefinedProperty.Value);
            }

            return destination;
        }

        public static DtoConfigurationsUnitConfiguration ToDto(
            this DatabaseConfigurationsUnitConfiguration source, UnitConfigurationFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoConfigurationsUnitConfiguration ToDtoInternal(
            this DatabaseConfigurationsUnitConfiguration source, IDictionary<EntityKey, object> mappedObjects, UnitConfigurationFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoConfigurationsUnitConfiguration);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoConfigurationsUnitConfiguration)mappedObjects[key];
            }

            var destination = new DtoConfigurationsUnitConfiguration();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;

            if (source.UpdateGroups == null || (filter != null && filter.UpdateGroups == null))
            {
                destination.UpdateGroups = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.UpdateGroups;
                destination.UpdateGroups = source
                    .UpdateGroups.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
    
            if (source.Document != null && (filter == null || filter.Document != null))
            {
                destination.Document = source.Document.ToDtoInternal(mappedObjects, filter == null ? null : filter.Document);
            }
    
            if (source.ProductType != null && (filter == null || filter.ProductType != null))
            {
                destination.ProductType = source.ProductType.ToDtoInternal(mappedObjects, filter == null ? null : filter.ProductType);
            }

            return destination;
        }

        public static DtoUpdateUpdateCommand ToDto(
            this DatabaseUpdateUpdateCommand source, UpdateCommandFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUpdateUpdateCommand ToDtoInternal(
            this DatabaseUpdateUpdateCommand source, IDictionary<EntityKey, object> mappedObjects, UpdateCommandFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUpdateUpdateCommand);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUpdateUpdateCommand)mappedObjects[key];
            }

            var destination = new DtoUpdateUpdateCommand();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.UpdateIndex = source.UpdateIndex;
    
            if (source.Unit != null && (filter == null || filter.Unit != null))
            {
                destination.Unit = source.Unit.ToDtoInternal(mappedObjects, filter == null ? null : filter.Unit);
            }
            destination.Command = source.Command.ToDto();
            destination.WasTransferred = source.WasTransferred;
            destination.WasInstalled = source.WasInstalled;

            if (source.IncludedParts == null || (filter != null && filter.IncludedParts == null))
            {
                destination.IncludedParts = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.IncludedParts;
                destination.IncludedParts = source
                    .IncludedParts.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            if (source.Feedbacks == null || (filter != null && filter.Feedbacks == null))
            {
                destination.Feedbacks = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Feedbacks;
                destination.Feedbacks = source
                    .Feedbacks.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            return destination;
        }

        public static DtoUpdateUpdateFeedback ToDto(
            this DatabaseUpdateUpdateFeedback source, UpdateFeedbackFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUpdateUpdateFeedback ToDtoInternal(
            this DatabaseUpdateUpdateFeedback source, IDictionary<EntityKey, object> mappedObjects, UpdateFeedbackFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUpdateUpdateFeedback);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUpdateUpdateFeedback)mappedObjects[key];
            }

            var destination = new DtoUpdateUpdateFeedback();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UpdateCommand != null && (filter == null || filter.UpdateCommand != null))
            {
                destination.UpdateCommand = source.UpdateCommand.ToDtoInternal(mappedObjects, filter == null ? null : filter.UpdateCommand);
            }
            destination.Timestamp = source.Timestamp;
            destination.State = (DtoUpdateUpdateState)source.State;
            destination.Feedback = source.Feedback.ToDto();

            return destination;
        }

        public static DtoUpdateUpdateGroup ToDto(
            this DatabaseUpdateUpdateGroup source, UpdateGroupFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUpdateUpdateGroup ToDtoInternal(
            this DatabaseUpdateUpdateGroup source, IDictionary<EntityKey, object> mappedObjects, UpdateGroupFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUpdateUpdateGroup);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUpdateUpdateGroup)mappedObjects[key];
            }

            var destination = new DtoUpdateUpdateGroup();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;
    
            if (source.Tenant != null && (filter == null || filter.Tenant != null))
            {
                destination.Tenant = source.Tenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.Tenant);
            }

            if (source.Units == null || (filter != null && filter.Units == null))
            {
                destination.Units = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Units;
                destination.Units = source
                    .Units.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            if (source.UpdateParts == null || (filter != null && filter.UpdateParts == null))
            {
                destination.UpdateParts = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.UpdateParts;
                destination.UpdateParts = source
                    .UpdateParts.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
    
            if (source.UnitConfiguration != null && (filter == null || filter.UnitConfiguration != null))
            {
                destination.UnitConfiguration = source.UnitConfiguration.ToDtoInternal(mappedObjects, filter == null ? null : filter.UnitConfiguration);
            }
    
            if (source.MediaConfiguration != null && (filter == null || filter.MediaConfiguration != null))
            {
                destination.MediaConfiguration = source.MediaConfiguration.ToDtoInternal(mappedObjects, filter == null ? null : filter.MediaConfiguration);
            }

            foreach (var userDefinedProperty in source.UserDefinedProperties)
            {
                destination.UserDefinedProperties
                        .Add(userDefinedProperty.PropertyDefinition.Name, userDefinedProperty.Value);
            }

            return destination;
        }

        public static DtoUpdateUpdatePart ToDto(
            this DatabaseUpdateUpdatePart source, UpdatePartFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoUpdateUpdatePart ToDtoInternal(
            this DatabaseUpdateUpdatePart source, IDictionary<EntityKey, object> mappedObjects, UpdatePartFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoUpdateUpdatePart);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoUpdateUpdatePart)mappedObjects[key];
            }

            var destination = new DtoUpdateUpdatePart();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.UpdateGroup != null && (filter == null || filter.UpdateGroup != null))
            {
                destination.UpdateGroup = source.UpdateGroup.ToDtoInternal(mappedObjects, filter == null ? null : filter.UpdateGroup);
            }
            destination.Type = (DtoUpdateUpdatePartType)source.Type;
            destination.Start = source.Start;
            destination.End = source.End;
            destination.Description = source.Description;
            destination.Structure = source.Structure.ToDto();
            destination.InstallInstructions = source.InstallInstructions.ToDto();
            destination.DynamicContent = source.DynamicContent.ToDto();

            if (source.RelatedCommands == null || (filter != null && filter.RelatedCommands == null))
            {
                destination.RelatedCommands = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.RelatedCommands;
                destination.RelatedCommands = source
                    .RelatedCommands.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            return destination;
        }

        public static DtoMembershipUser ToDto(
            this DatabaseMembershipUser source, UserFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoMembershipUser ToDtoInternal(
            this DatabaseMembershipUser source, IDictionary<EntityKey, object> mappedObjects, UserFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoMembershipUser);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoMembershipUser)mappedObjects[key];
            }

            var destination = new DtoMembershipUser();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
    
            if (source.OwnerTenant != null && (filter == null || filter.OwnerTenant != null))
            {
                destination.OwnerTenant = source.OwnerTenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.OwnerTenant);
            }

            if (source.AssociationTenantUserUserRoles == null || (filter != null && filter.AssociationTenantUserUserRoles == null))
            {
                destination.AssociationTenantUserUserRoles = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.AssociationTenantUserUserRoles;
                destination.AssociationTenantUserUserRoles = source
                    .AssociationTenantUserUserRoles.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }
            destination.Username = source.Username;
            destination.Domain = source.Domain;
            destination.HashedPassword = source.HashedPassword;
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.Email = source.Email;
            destination.Culture = source.Culture;
            destination.TimeZone = source.TimeZone;
            destination.Description = source.Description;
            destination.LastLoginAttempt = source.LastLoginAttempt;
            destination.LastSuccessfulLogin = source.LastSuccessfulLogin;
            destination.ConsecutiveLoginFailures = source.ConsecutiveLoginFailures;
            destination.IsEnabled = source.IsEnabled;

            foreach (var userDefinedProperty in source.UserDefinedProperties)
            {
                destination.UserDefinedProperties
                        .Add(userDefinedProperty.PropertyDefinition.Name, userDefinedProperty.Value);
            }

            return destination;
        }

        public static DtoMetaUserDefinedProperty ToDto(
            this DatabaseMetaUserDefinedProperty source, UserDefinedPropertyFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoMetaUserDefinedProperty ToDtoInternal(
            this DatabaseMetaUserDefinedProperty source, IDictionary<EntityKey, object> mappedObjects, UserDefinedPropertyFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoMetaUserDefinedProperty);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoMetaUserDefinedProperty)mappedObjects[key];
            }

            var destination = new DtoMetaUserDefinedProperty();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.OwnerEntity = (DtoMetaUserDefinedPropertyEnabledEntity)source.OwnerEntity;
    
            if (source.Tenant != null && (filter == null || filter.Tenant != null))
            {
                destination.Tenant = source.Tenant.ToDtoInternal(mappedObjects, filter == null ? null : filter.Tenant);
            }
            destination.Name = source.Name;

            return destination;
        }

        public static DtoAccessControlUserRole ToDto(
            this DatabaseAccessControlUserRole source, UserRoleFilter filter = null)
        {
            var mappedObjects = new Dictionary<EntityKey, object>();
            return source.ToDtoInternal(mappedObjects, filter);
        }

        internal static DtoAccessControlUserRole ToDtoInternal(
            this DatabaseAccessControlUserRole source, IDictionary<EntityKey, object> mappedObjects, UserRoleFilter filter = null)
        {
            var key = new EntityKey(source.Id, ReferenceTypes.DtoAccessControlUserRole);
            if (mappedObjects.ContainsKey(key))
            {
                return (DtoAccessControlUserRole)mappedObjects[key];
            }

            var destination = new DtoAccessControlUserRole();
            mappedObjects.Add(key, destination);
            destination.Id = source.Id;
            destination.CreatedOn = source.CreatedOn;
            destination.LastModifiedOn = source.LastModifiedOn;
            destination.Version = source.Version;
            destination.Name = source.Name;
            destination.Description = source.Description;

            if (source.Authorizations == null || (filter != null && filter.Authorizations == null))
            {
                destination.Authorizations = null;
            }
            else
            {
                var subFilter = filter == null ? null : filter.Authorizations;
                destination.Authorizations = source
                    .Authorizations.Select(entity => ToDtoInternal(entity, mappedObjects, subFilter))
                    .ToList();
            }

            foreach (var userDefinedProperty in source.UserDefinedProperties)
            {
                destination.UserDefinedProperties
                        .Add(userDefinedProperty.PropertyDefinition.Name, userDefinedProperty.Value);
            }

            return destination;
        }

        public static DatabaseXmlData ToDatabase(this DtoXmlData source)
        {
            return source == default(DtoXmlData) ? default(DatabaseXmlData) : new DatabaseXmlData { Xml = source.Xml, Type = source.Type };
        }

        public static DtoXmlData ToDto(this DatabaseXmlData source)
        {
            return source == default(DatabaseXmlData) ? default(DtoXmlData) : new DtoXmlData(source.Xml, source.Type);
        }
    }
}