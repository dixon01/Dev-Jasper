namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Exceptions;

    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

        public static class Extensions
        {
            public static Authorization ToDto(this AuthorizationWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Authorization ToDto(this AuthorizationWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Authorization);
                if (context.ContainsKey(key))
                {
                    return (Authorization)context[key];
                }

                var dto = new Authorization { Id = model.Id };
                dto.DataScope = model.DataScope;
                dto.Permission = model.Permission;

                try
                {
                    if (model.UserRole != null)
                    {
                        dto.UserRole = Extensions.ToDto(model.UserRole, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Authorization ToDto(this AuthorizationReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Authorization ToDto(this AuthorizationReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Authorization);
                if (context.ContainsKey(key))
                {
                    return (Authorization)context[key];
                }

                var dto = new Authorization { Id = model.Id };
                dto.DataScope = model.DataScope;
                dto.Permission = model.Permission;

                try
                {
                    if (model.UserRole != null)
                    {
                        dto.UserRole = Extensions.ToDto(model.UserRole, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UserRole ToDto(this UserRoleWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UserRole ToDto(this UserRoleWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UserRole);
                if (context.ContainsKey(key))
                {
                    return (UserRole)context[key];
                }

                var dto = new UserRole { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;
                dto.UserDefinedProperties = new Dictionary<string, string>(model.UserDefinedProperties);

                context.Add(key, dto);
                return dto;
            }

            public static UserRole ToDto(this UserRoleReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UserRole ToDto(this UserRoleReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UserRole);
                if (context.ContainsKey(key))
                {
                    return (UserRole)context[key];
                }

                var dto = new UserRole { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;

        public static class Extensions
        {
            public static AssociationTenantUserUserRole ToDto(this AssociationTenantUserUserRoleWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static AssociationTenantUserUserRole ToDto(this AssociationTenantUserUserRoleWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.AssociationTenantUserUserRole);
                if (context.ContainsKey(key))
                {
                    return (AssociationTenantUserUserRole)context[key];
                }

                var dto = new AssociationTenantUserUserRole { Id = model.Id };

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.User != null)
                    {
                        dto.User = Extensions.ToDto(model.User, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UserRole != null)
                    {
                        dto.UserRole = AccessControl.Extensions.ToDto(model.UserRole, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static AssociationTenantUserUserRole ToDto(this AssociationTenantUserUserRoleReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static AssociationTenantUserUserRole ToDto(this AssociationTenantUserUserRoleReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.AssociationTenantUserUserRole);
                if (context.ContainsKey(key))
                {
                    return (AssociationTenantUserUserRole)context[key];
                }

                var dto = new AssociationTenantUserUserRole { Id = model.Id };

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.User != null)
                    {
                        dto.User = Extensions.ToDto(model.User, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UserRole != null)
                    {
                        dto.UserRole = AccessControl.Extensions.ToDto(model.UserRole, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Tenant ToDto(this TenantWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Tenant ToDto(this TenantWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Tenant);
                if (context.ContainsKey(key))
                {
                    return (Tenant)context[key];
                }

                var dto = new Tenant { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;
                dto.UserDefinedProperties = new Dictionary<string, string>(model.UserDefinedProperties);

                context.Add(key, dto);
                return dto;
            }

            public static Tenant ToDto(this TenantReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Tenant ToDto(this TenantReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Tenant);
                if (context.ContainsKey(key))
                {
                    return (Tenant)context[key];
                }

                var dto = new Tenant { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                context.Add(key, dto);
                return dto;
            }

            public static User ToDto(this UserWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static User ToDto(this UserWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.User);
                if (context.ContainsKey(key))
                {
                    return (User)context[key];
                }

                var dto = new User { Id = model.Id };
                dto.Username = model.Username;
                dto.Domain = model.Domain;
                dto.HashedPassword = model.HashedPassword;
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Email = model.Email;
                dto.Culture = model.Culture;
                dto.TimeZone = model.TimeZone;
                dto.Description = model.Description;
                dto.LastLoginAttempt = model.LastLoginAttempt;
                dto.LastSuccessfulLogin = model.LastSuccessfulLogin;
                dto.ConsecutiveLoginFailures = model.ConsecutiveLoginFailures;
                dto.IsEnabled = model.IsEnabled;

                try
                {
                    if (model.OwnerTenant != null)
                    {
                        dto.OwnerTenant = Extensions.ToDto(model.OwnerTenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }
                dto.UserDefinedProperties = new Dictionary<string, string>(model.UserDefinedProperties);

                context.Add(key, dto);
                return dto;
            }

            public static User ToDto(this UserReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static User ToDto(this UserReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.User);
                if (context.ContainsKey(key))
                {
                    return (User)context[key];
                }

                var dto = new User { Id = model.Id };
                dto.Username = model.Username;
                dto.Domain = model.Domain;
                dto.HashedPassword = model.HashedPassword;
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Email = model.Email;
                dto.Culture = model.Culture;
                dto.TimeZone = model.TimeZone;
                dto.Description = model.Description;
                dto.LastLoginAttempt = model.LastLoginAttempt;
                dto.LastSuccessfulLogin = model.LastSuccessfulLogin;
                dto.ConsecutiveLoginFailures = model.ConsecutiveLoginFailures;
                dto.IsEnabled = model.IsEnabled;

                try
                {
                    if (model.OwnerTenant != null)
                    {
                        dto.OwnerTenant = Extensions.ToDto(model.OwnerTenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;

        public static class Extensions
        {
            public static ProductType ToDto(this ProductTypeWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static ProductType ToDto(this ProductTypeWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.ProductType);
                if (context.ContainsKey(key))
                {
                    return (ProductType)context[key];
                }

                var dto = new ProductType { Id = model.Id };
                dto.UnitType = model.UnitType;
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    dto.HardwareDescriptorXml = model.HardwareDescriptor.Xml;
                    dto.HardwareDescriptorXmlType = model.HardwareDescriptor.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static ProductType ToDto(this ProductTypeReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static ProductType ToDto(this ProductTypeReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.ProductType);
                if (context.ContainsKey(key))
                {
                    return (ProductType)context[key];
                }

                var dto = new ProductType { Id = model.Id };
                dto.UnitType = model.UnitType;
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    dto.HardwareDescriptorXml = model.HardwareDescriptor.Xml;
                    dto.HardwareDescriptorXmlType = model.HardwareDescriptor.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Unit ToDto(this UnitWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Unit ToDto(this UnitWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Unit);
                if (context.ContainsKey(key))
                {
                    return (Unit)context[key];
                }

                var dto = new Unit { Id = model.Id };
                dto.Name = model.Name;
                dto.NetworkAddress = model.NetworkAddress;
                dto.Description = model.Description;
                dto.IsConnected = model.IsConnected;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.ProductType != null)
                    {
                        dto.ProductType = Extensions.ToDto(model.ProductType, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateGroup != null)
                    {
                        dto.UpdateGroup = Update.Extensions.ToDto(model.UpdateGroup, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }
                dto.UserDefinedProperties = new Dictionary<string, string>(model.UserDefinedProperties);

                context.Add(key, dto);
                return dto;
            }

            public static Unit ToDto(this UnitReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Unit ToDto(this UnitReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Unit);
                if (context.ContainsKey(key))
                {
                    return (Unit)context[key];
                }

                var dto = new Unit { Id = model.Id };
                dto.Name = model.Name;
                dto.NetworkAddress = model.NetworkAddress;
                dto.Description = model.Description;
                dto.IsConnected = model.IsConnected;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.ProductType != null)
                    {
                        dto.ProductType = Extensions.ToDto(model.ProductType, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateGroup != null)
                    {
                        dto.UpdateGroup = Update.Extensions.ToDto(model.UpdateGroup, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;

        public static class Extensions
        {
            public static ContentResource ToDto(this ContentResourceWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static ContentResource ToDto(this ContentResourceWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.ContentResource);
                if (context.ContainsKey(key))
                {
                    return (ContentResource)context[key];
                }

                var dto = new ContentResource { Id = model.Id };
                dto.OriginalFilename = model.OriginalFilename;
                dto.Description = model.Description;
                dto.ThumbnailHash = model.ThumbnailHash;
                dto.Hash = model.Hash;
                dto.HashAlgorithmType = model.HashAlgorithmType;
                dto.MimeType = model.MimeType;
                dto.Length = model.Length;

                try
                {
                    if (model.UploadingUser != null)
                    {
                        dto.UploadingUser = Membership.Extensions.ToDto(model.UploadingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static ContentResource ToDto(this ContentResourceReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static ContentResource ToDto(this ContentResourceReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.ContentResource);
                if (context.ContainsKey(key))
                {
                    return (ContentResource)context[key];
                }

                var dto = new ContentResource { Id = model.Id };
                dto.OriginalFilename = model.OriginalFilename;
                dto.Description = model.Description;
                dto.ThumbnailHash = model.ThumbnailHash;
                dto.Hash = model.Hash;
                dto.HashAlgorithmType = model.HashAlgorithmType;
                dto.MimeType = model.MimeType;
                dto.Length = model.Length;

                try
                {
                    if (model.UploadingUser != null)
                    {
                        dto.UploadingUser = Membership.Extensions.ToDto(model.UploadingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Resource ToDto(this ResourceWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Resource ToDto(this ResourceWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Resource);
                if (context.ContainsKey(key))
                {
                    return (Resource)context[key];
                }

                var dto = new Resource { Id = model.Id };
                dto.OriginalFilename = model.OriginalFilename;
                dto.Description = model.Description;
                dto.Hash = model.Hash;
                dto.ThumbnailHash = model.ThumbnailHash;
                dto.MimeType = model.MimeType;
                dto.Length = model.Length;

                try
                {
                    if (model.UploadingUser != null)
                    {
                        dto.UploadingUser = Membership.Extensions.ToDto(model.UploadingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Resource ToDto(this ResourceReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Resource ToDto(this ResourceReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Resource);
                if (context.ContainsKey(key))
                {
                    return (Resource)context[key];
                }

                var dto = new Resource { Id = model.Id };
                dto.OriginalFilename = model.OriginalFilename;
                dto.Description = model.Description;
                dto.Hash = model.Hash;
                dto.ThumbnailHash = model.ThumbnailHash;
                dto.MimeType = model.MimeType;
                dto.Length = model.Length;

                try
                {
                    if (model.UploadingUser != null)
                    {
                        dto.UploadingUser = Membership.Extensions.ToDto(model.UploadingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;

        public static class Extensions
        {
            public static UpdateCommand ToDto(this UpdateCommandWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateCommand ToDto(this UpdateCommandWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateCommand);
                if (context.ContainsKey(key))
                {
                    return (UpdateCommand)context[key];
                }

                var dto = new UpdateCommand { Id = model.Id };
                dto.UpdateIndex = model.UpdateIndex;
                dto.WasTransferred = model.WasTransferred;
                dto.WasInstalled = model.WasInstalled;

                try
                {
                    dto.CommandXml = model.Command.Xml;
                    dto.CommandXmlType = model.Command.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Unit != null)
                    {
                        dto.Unit = Units.Extensions.ToDto(model.Unit, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdateCommand ToDto(this UpdateCommandReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateCommand ToDto(this UpdateCommandReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateCommand);
                if (context.ContainsKey(key))
                {
                    return (UpdateCommand)context[key];
                }

                var dto = new UpdateCommand { Id = model.Id };
                dto.UpdateIndex = model.UpdateIndex;
                dto.WasTransferred = model.WasTransferred;
                dto.WasInstalled = model.WasInstalled;

                try
                {
                    dto.CommandXml = model.Command.Xml;
                    dto.CommandXmlType = model.Command.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Unit != null)
                    {
                        dto.Unit = Units.Extensions.ToDto(model.Unit, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdateFeedback ToDto(this UpdateFeedbackWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateFeedback ToDto(this UpdateFeedbackWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateFeedback);
                if (context.ContainsKey(key))
                {
                    return (UpdateFeedback)context[key];
                }

                var dto = new UpdateFeedback { Id = model.Id };
                dto.Timestamp = model.Timestamp;
                dto.State = model.State;

                try
                {
                    dto.FeedbackXml = model.Feedback.Xml;
                    dto.FeedbackXmlType = model.Feedback.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateCommand != null)
                    {
                        dto.UpdateCommand = Extensions.ToDto(model.UpdateCommand, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdateFeedback ToDto(this UpdateFeedbackReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateFeedback ToDto(this UpdateFeedbackReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateFeedback);
                if (context.ContainsKey(key))
                {
                    return (UpdateFeedback)context[key];
                }

                var dto = new UpdateFeedback { Id = model.Id };
                dto.Timestamp = model.Timestamp;
                dto.State = model.State;

                try
                {
                    dto.FeedbackXml = model.Feedback.Xml;
                    dto.FeedbackXmlType = model.Feedback.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateCommand != null)
                    {
                        dto.UpdateCommand = Extensions.ToDto(model.UpdateCommand, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdateGroup ToDto(this UpdateGroupWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateGroup ToDto(this UpdateGroupWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateGroup);
                if (context.ContainsKey(key))
                {
                    return (UpdateGroup)context[key];
                }

                var dto = new UpdateGroup { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UnitConfiguration != null)
                    {
                        dto.UnitConfiguration = Configurations.Extensions.ToDto(model.UnitConfiguration, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.MediaConfiguration != null)
                    {
                        dto.MediaConfiguration = Configurations.Extensions.ToDto(model.MediaConfiguration, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }
                dto.UserDefinedProperties = new Dictionary<string, string>(model.UserDefinedProperties);

                context.Add(key, dto);
                return dto;
            }

            public static UpdateGroup ToDto(this UpdateGroupReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdateGroup ToDto(this UpdateGroupReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdateGroup);
                if (context.ContainsKey(key))
                {
                    return (UpdateGroup)context[key];
                }

                var dto = new UpdateGroup { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UnitConfiguration != null)
                    {
                        dto.UnitConfiguration = Configurations.Extensions.ToDto(model.UnitConfiguration, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.MediaConfiguration != null)
                    {
                        dto.MediaConfiguration = Configurations.Extensions.ToDto(model.MediaConfiguration, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdatePart ToDto(this UpdatePartWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdatePart ToDto(this UpdatePartWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdatePart);
                if (context.ContainsKey(key))
                {
                    return (UpdatePart)context[key];
                }

                var dto = new UpdatePart { Id = model.Id };
                dto.Type = model.Type;
                dto.Start = model.Start;
                dto.End = model.End;
                dto.Description = model.Description;

                try
                {
                    dto.StructureXml = model.Structure.Xml;
                    dto.StructureXmlType = model.Structure.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    dto.InstallInstructionsXml = model.InstallInstructions.Xml;
                    dto.InstallInstructionsXmlType = model.InstallInstructions.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    dto.DynamicContentXml = model.DynamicContent.Xml;
                    dto.DynamicContentXmlType = model.DynamicContent.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateGroup != null)
                    {
                        dto.UpdateGroup = Extensions.ToDto(model.UpdateGroup, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UpdatePart ToDto(this UpdatePartReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UpdatePart ToDto(this UpdatePartReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UpdatePart);
                if (context.ContainsKey(key))
                {
                    return (UpdatePart)context[key];
                }

                var dto = new UpdatePart { Id = model.Id };
                dto.Type = model.Type;
                dto.Start = model.Start;
                dto.End = model.End;
                dto.Description = model.Description;

                try
                {
                    dto.StructureXml = model.Structure.Xml;
                    dto.StructureXmlType = model.Structure.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    dto.InstallInstructionsXml = model.InstallInstructions.Xml;
                    dto.InstallInstructionsXmlType = model.InstallInstructions.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    dto.DynamicContentXml = model.DynamicContent.Xml;
                    dto.DynamicContentXmlType = model.DynamicContent.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.UpdateGroup != null)
                    {
                        dto.UpdateGroup = Extensions.ToDto(model.UpdateGroup, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;

        public static class Extensions
        {
            public static Document ToDto(this DocumentWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Document ToDto(this DocumentWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Document);
                if (context.ContainsKey(key))
                {
                    return (Document)context[key];
                }

                var dto = new Document { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static Document ToDto(this DocumentReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Document ToDto(this DocumentReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Document);
                if (context.ContainsKey(key))
                {
                    return (Document)context[key];
                }

                var dto = new Document { Id = model.Id };
                dto.Name = model.Name;
                dto.Description = model.Description;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static DocumentVersion ToDto(this DocumentVersionWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static DocumentVersion ToDto(this DocumentVersionWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.DocumentVersion);
                if (context.ContainsKey(key))
                {
                    return (DocumentVersion)context[key];
                }

                var dto = new DocumentVersion { Id = model.Id };
                dto.Major = model.Major;
                dto.Minor = model.Minor;
                dto.Description = model.Description;

                try
                {
                    dto.ContentXml = model.Content.Xml;
                    dto.ContentXmlType = model.Content.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.CreatingUser != null)
                    {
                        dto.CreatingUser = Membership.Extensions.ToDto(model.CreatingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static DocumentVersion ToDto(this DocumentVersionReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static DocumentVersion ToDto(this DocumentVersionReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.DocumentVersion);
                if (context.ContainsKey(key))
                {
                    return (DocumentVersion)context[key];
                }

                var dto = new DocumentVersion { Id = model.Id };
                dto.Major = model.Major;
                dto.Minor = model.Minor;
                dto.Description = model.Description;

                try
                {
                    dto.ContentXml = model.Content.Xml;
                    dto.ContentXmlType = model.Content.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.CreatingUser != null)
                    {
                        dto.CreatingUser = Membership.Extensions.ToDto(model.CreatingUser, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;

        public static class Extensions
        {
            public static Package ToDto(this PackageWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Package ToDto(this PackageWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Package);
                if (context.ContainsKey(key))
                {
                    return (Package)context[key];
                }

                var dto = new Package { Id = model.Id };
                dto.PackageId = model.PackageId;
                dto.ProductName = model.ProductName;
                dto.Description = model.Description;

                context.Add(key, dto);
                return dto;
            }

            public static Package ToDto(this PackageReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static Package ToDto(this PackageReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.Package);
                if (context.ContainsKey(key))
                {
                    return (Package)context[key];
                }

                var dto = new Package { Id = model.Id };
                dto.PackageId = model.PackageId;
                dto.ProductName = model.ProductName;
                dto.Description = model.Description;

                context.Add(key, dto);
                return dto;
            }

            public static PackageVersion ToDto(this PackageVersionWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static PackageVersion ToDto(this PackageVersionWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.PackageVersion);
                if (context.ContainsKey(key))
                {
                    return (PackageVersion)context[key];
                }

                var dto = new PackageVersion { Id = model.Id };
                dto.SoftwareVersion = model.SoftwareVersion;
                dto.Description = model.Description;

                try
                {
                    dto.StructureXml = model.Structure.Xml;
                    dto.StructureXmlType = model.Structure.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Package != null)
                    {
                        dto.Package = Extensions.ToDto(model.Package, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static PackageVersion ToDto(this PackageVersionReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static PackageVersion ToDto(this PackageVersionReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.PackageVersion);
                if (context.ContainsKey(key))
                {
                    return (PackageVersion)context[key];
                }

                var dto = new PackageVersion { Id = model.Id };
                dto.SoftwareVersion = model.SoftwareVersion;
                dto.Description = model.Description;

                try
                {
                    dto.StructureXml = model.Structure.Xml;
                    dto.StructureXmlType = model.Structure.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.Package != null)
                    {
                        dto.Package = Extensions.ToDto(model.Package, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;

        public static class Extensions
        {
            public static MediaConfiguration ToDto(this MediaConfigurationWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static MediaConfiguration ToDto(this MediaConfigurationWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.MediaConfiguration);
                if (context.ContainsKey(key))
                {
                    return (MediaConfiguration)context[key];
                }

                var dto = new MediaConfiguration { Id = model.Id };

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Documents.Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static MediaConfiguration ToDto(this MediaConfigurationReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static MediaConfiguration ToDto(this MediaConfigurationReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.MediaConfiguration);
                if (context.ContainsKey(key))
                {
                    return (MediaConfiguration)context[key];
                }

                var dto = new MediaConfiguration { Id = model.Id };

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Documents.Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UnitConfiguration ToDto(this UnitConfigurationWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UnitConfiguration ToDto(this UnitConfigurationWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UnitConfiguration);
                if (context.ContainsKey(key))
                {
                    return (UnitConfiguration)context[key];
                }

                var dto = new UnitConfiguration { Id = model.Id };

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Documents.Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.ProductType != null)
                    {
                        dto.ProductType = Units.Extensions.ToDto(model.ProductType, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UnitConfiguration ToDto(this UnitConfigurationReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UnitConfiguration ToDto(this UnitConfigurationReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UnitConfiguration);
                if (context.ContainsKey(key))
                {
                    return (UnitConfiguration)context[key];
                }

                var dto = new UnitConfiguration { Id = model.Id };

                try
                {
                    if (model.Document != null)
                    {
                        dto.Document = Documents.Extensions.ToDto(model.Document, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                try
                {
                    if (model.ProductType != null)
                    {
                        dto.ProductType = Units.Extensions.ToDto(model.ProductType, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;

        public static class Extensions
        {        }
    }

    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;

        public static class Extensions
        {
            public static SystemConfig ToDto(this SystemConfigWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static SystemConfig ToDto(this SystemConfigWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.SystemConfig);
                if (context.ContainsKey(key))
                {
                    return (SystemConfig)context[key];
                }

                var dto = new SystemConfig { Id = model.Id };
                dto.SystemId = model.SystemId;

                try
                {
                    dto.SettingsXml = model.Settings.Xml;
                    dto.SettingsXmlType = model.Settings.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static SystemConfig ToDto(this SystemConfigReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static SystemConfig ToDto(this SystemConfigReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.SystemConfig);
                if (context.ContainsKey(key))
                {
                    return (SystemConfig)context[key];
                }

                var dto = new SystemConfig { Id = model.Id };
                dto.SystemId = model.SystemId;

                try
                {
                    dto.SettingsXml = model.Settings.Xml;
                    dto.SettingsXmlType = model.Settings.Type;
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UserDefinedProperty ToDto(this UserDefinedPropertyWritableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UserDefinedProperty ToDto(this UserDefinedPropertyWritableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UserDefinedProperty);
                if (context.ContainsKey(key))
                {
                    return (UserDefinedProperty)context[key];
                }

                var dto = new UserDefinedProperty { Id = model.Id };
                dto.OwnerEntity = model.OwnerEntity;
                dto.Name = model.Name;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }

            public static UserDefinedProperty ToDto(this UserDefinedPropertyReadableModel model)
            {
                var context = new Dictionary<EntityKey, object>();
                return model.ToDto(context);
            }

            internal static UserDefinedProperty ToDto(this UserDefinedPropertyReadableModel model, Dictionary<EntityKey, object> context)
            {
                var key = new EntityKey(model.Id, ReferenceTypes.UserDefinedProperty);
                if (context.ContainsKey(key))
                {
                    return (UserDefinedProperty)context[key];
                }

                var dto = new UserDefinedProperty { Id = model.Id };
                dto.OwnerEntity = model.OwnerEntity;
                dto.Name = model.Name;

                try
                {
                    if (model.Tenant != null)
                    {
                        dto.Tenant = Membership.Extensions.ToDto(model.Tenant, context);
                    }
                }
                catch (ChangeTrackingException)
                {
                    // the property was probably not loaded, so let's leave it at null
                }

                context.Add(key, dto);
                return dto;
            }
        }
    }

    internal enum ReferenceTypes
    {
        UserRole = 0,
        Authorization = 1,
        Tenant = 2,
        User = 3,
        AssociationTenantUserUserRole = 4,
        ProductType = 5,
        Unit = 6,
        Resource = 7,
        ContentResource = 8,
        UpdateGroup = 9,
        UpdatePart = 10,
        UpdateCommand = 11,
        UpdateFeedback = 12,
        Document = 13,
        DocumentVersion = 14,
        Package = 15,
        PackageVersion = 16,
        UnitConfiguration = 17,
        MediaConfiguration = 18,
        UserDefinedProperty = 19,
        SystemConfig = 20
    }

    internal class EntityKey
    {
        public EntityKey(object id, ReferenceTypes type)
        {
            this.Id = id;
            this.Type = type;
        }

        public object Id { get; private set; }

        public ReferenceTypes Type { get; private set; }

        public bool Equals(EntityKey other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id) && this.Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((EntityKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^ (int)this.Type;
            }
        }
    }
}
