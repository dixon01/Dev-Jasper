namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    using Filters;
    
    using Gorba.Center.Common.ServiceModel;
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
    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;

        public static class AuthorizationFilterExtensions
        {
            public static AuthorizationFilter WithId(this AuthorizationFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new AuthorizationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static AuthorizationQuery WithId(this AuthorizationQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new AuthorizationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationFilter WithCreatedOn(this AuthorizationFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new AuthorizationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationQuery WithCreatedOn(this AuthorizationQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new AuthorizationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationFilter WithLastModifiedOn(this AuthorizationFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new AuthorizationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationQuery WithLastModifiedOn(this AuthorizationQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new AuthorizationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationFilter WithVersion(this AuthorizationFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new AuthorizationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationQuery WithVersion(this AuthorizationQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new AuthorizationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationFilter WithUserRole(
                this AuthorizationFilter filter, Gorba.Center.Common.ServiceModel.AccessControl.UserRole value)
            {
                return filter.IncludeUserRole(UserRoleFilter.Create().WithId(value.Id));
            }

            public static AuthorizationQuery WithUserRole(
                this AuthorizationQuery filter, Gorba.Center.Common.ServiceModel.AccessControl.UserRole value)
            {
                return filter.IncludeUserRole(UserRoleFilter.Create().WithId(value.Id));
            }

            public static AuthorizationFilter IncludeUserRole(
                this AuthorizationFilter filter,
                UserRoleFilter filterUserRole = null)
            {
                filter.UserRole = filterUserRole ?? UserRoleFilter.Create();
                return filter;
            }

            public static AuthorizationQuery IncludeUserRole(
                this AuthorizationQuery filter,
                UserRoleFilter filterUserRole = null)
            {
                filter.UserRole = filterUserRole ?? UserRoleFilter.Create();
                return filter;
            }

            public static AuthorizationFilter WithDataScope(this AuthorizationFilter filter, DataScope value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.DataScope = new AuthorizationFilter.DataScopePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationQuery WithDataScope(this AuthorizationQuery filter, DataScope value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.DataScope = new AuthorizationFilter.DataScopePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationFilter WithPermission(this AuthorizationFilter filter, Permission value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Permission = new AuthorizationFilter.PermissionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AuthorizationQuery WithPermission(this AuthorizationQuery filter, Permission value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Permission = new AuthorizationFilter.PermissionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class UserRoleFilterExtensions
        {
            public static UserRoleFilter WithId(this UserRoleFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserRoleFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UserRoleQuery WithId(this UserRoleQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserRoleFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleFilter WithCreatedOn(this UserRoleFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserRoleFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleQuery WithCreatedOn(this UserRoleQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserRoleFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleFilter WithLastModifiedOn(this UserRoleFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserRoleFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleQuery WithLastModifiedOn(this UserRoleQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserRoleFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleFilter WithVersion(this UserRoleFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserRoleFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleQuery WithVersion(this UserRoleQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserRoleFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleFilter IncludeAuthorizations(
                this UserRoleFilter filter,
                AuthorizationFilter filterAuthorizations = null)
            {
                if (filterAuthorizations != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Authorizations = AuthorizationFilter.Create();
                return filter;
            }

            public static UserRoleQuery IncludeAuthorizations(
                this UserRoleQuery filter,
                AuthorizationFilter filterAuthorizations = null)
            {
                filter.Authorizations = filterAuthorizations ?? AuthorizationFilter.Create();
                return filter;
            }

            public static UserRoleFilter WithDescription(this UserRoleFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UserRoleFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleQuery WithDescription(this UserRoleQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UserRoleFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleFilter WithName(this UserRoleFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UserRoleFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserRoleQuery WithName(this UserRoleQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UserRoleFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;

        public static class MediaConfigurationFilterExtensions
        {
            public static MediaConfigurationFilter WithId(this MediaConfigurationFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new MediaConfigurationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static MediaConfigurationQuery WithId(this MediaConfigurationQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new MediaConfigurationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationFilter WithCreatedOn(this MediaConfigurationFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new MediaConfigurationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationQuery WithCreatedOn(this MediaConfigurationQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new MediaConfigurationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationFilter WithLastModifiedOn(this MediaConfigurationFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new MediaConfigurationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationQuery WithLastModifiedOn(this MediaConfigurationQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new MediaConfigurationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationFilter WithVersion(this MediaConfigurationFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new MediaConfigurationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationQuery WithVersion(this MediaConfigurationQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new MediaConfigurationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static MediaConfigurationFilter WithDocument(
                this MediaConfigurationFilter filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(Documents.DocumentFilter.Create().WithId(value.Id));
            }

            public static MediaConfigurationQuery WithDocument(
                this MediaConfigurationQuery filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(Documents.DocumentFilter.Create().WithId(value.Id));
            }

            public static MediaConfigurationFilter IncludeDocument(
                this MediaConfigurationFilter filter,
                Documents.DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? Documents.DocumentFilter.Create();
                return filter;
            }

            public static MediaConfigurationQuery IncludeDocument(
                this MediaConfigurationQuery filter,
                Documents.DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? Documents.DocumentFilter.Create();
                return filter;
            }

            public static MediaConfigurationFilter IncludeUpdateGroups(
                this MediaConfigurationFilter filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                if (filterUpdateGroups != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.UpdateGroups = Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static MediaConfigurationQuery IncludeUpdateGroups(
                this MediaConfigurationQuery filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                filter.UpdateGroups = filterUpdateGroups ?? Update.UpdateGroupFilter.Create();
                return filter;
            }
        }

        public static class UnitConfigurationFilterExtensions
        {
            public static UnitConfigurationFilter WithId(this UnitConfigurationFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UnitConfigurationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UnitConfigurationQuery WithId(this UnitConfigurationQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UnitConfigurationFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationFilter WithCreatedOn(this UnitConfigurationFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UnitConfigurationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationQuery WithCreatedOn(this UnitConfigurationQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UnitConfigurationFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationFilter WithLastModifiedOn(this UnitConfigurationFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UnitConfigurationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationQuery WithLastModifiedOn(this UnitConfigurationQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UnitConfigurationFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationFilter WithVersion(this UnitConfigurationFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UnitConfigurationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationQuery WithVersion(this UnitConfigurationQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UnitConfigurationFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitConfigurationFilter WithDocument(
                this UnitConfigurationFilter filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(Documents.DocumentFilter.Create().WithId(value.Id));
            }

            public static UnitConfigurationQuery WithDocument(
                this UnitConfigurationQuery filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(Documents.DocumentFilter.Create().WithId(value.Id));
            }

            public static UnitConfigurationFilter IncludeDocument(
                this UnitConfigurationFilter filter,
                Documents.DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? Documents.DocumentFilter.Create();
                return filter;
            }

            public static UnitConfigurationQuery IncludeDocument(
                this UnitConfigurationQuery filter,
                Documents.DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? Documents.DocumentFilter.Create();
                return filter;
            }

            public static UnitConfigurationFilter WithProductType(
                this UnitConfigurationFilter filter, Gorba.Center.Common.ServiceModel.Units.ProductType value)
            {
                return filter.IncludeProductType(Units.ProductTypeFilter.Create().WithId(value.Id));
            }

            public static UnitConfigurationQuery WithProductType(
                this UnitConfigurationQuery filter, Gorba.Center.Common.ServiceModel.Units.ProductType value)
            {
                return filter.IncludeProductType(Units.ProductTypeFilter.Create().WithId(value.Id));
            }

            public static UnitConfigurationFilter IncludeProductType(
                this UnitConfigurationFilter filter,
                Units.ProductTypeFilter filterProductType = null)
            {
                filter.ProductType = filterProductType ?? Units.ProductTypeFilter.Create();
                return filter;
            }

            public static UnitConfigurationQuery IncludeProductType(
                this UnitConfigurationQuery filter,
                Units.ProductTypeFilter filterProductType = null)
            {
                filter.ProductType = filterProductType ?? Units.ProductTypeFilter.Create();
                return filter;
            }

            public static UnitConfigurationFilter IncludeUpdateGroups(
                this UnitConfigurationFilter filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                if (filterUpdateGroups != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.UpdateGroups = Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static UnitConfigurationQuery IncludeUpdateGroups(
                this UnitConfigurationQuery filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                filter.UpdateGroups = filterUpdateGroups ?? Update.UpdateGroupFilter.Create();
                return filter;
            }
        }

    }
    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;

        public static class DocumentFilterExtensions
        {
            public static DocumentFilter WithId(this DocumentFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new DocumentFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static DocumentQuery WithId(this DocumentQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new DocumentFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentFilter WithCreatedOn(this DocumentFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new DocumentFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentQuery WithCreatedOn(this DocumentQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new DocumentFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentFilter WithLastModifiedOn(this DocumentFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new DocumentFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentQuery WithLastModifiedOn(this DocumentQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new DocumentFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentFilter WithVersion(this DocumentFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new DocumentFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentQuery WithVersion(this DocumentQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new DocumentFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentFilter WithTenant(
                this DocumentFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static DocumentQuery WithTenant(
                this DocumentQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static DocumentFilter IncludeTenant(
                this DocumentFilter filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static DocumentQuery IncludeTenant(
                this DocumentQuery filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static DocumentFilter IncludeVersions(
                this DocumentFilter filter,
                DocumentVersionFilter filterVersions = null)
            {
                if (filterVersions != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Versions = DocumentVersionFilter.Create();
                return filter;
            }

            public static DocumentQuery IncludeVersions(
                this DocumentQuery filter,
                DocumentVersionFilter filterVersions = null)
            {
                filter.Versions = filterVersions ?? DocumentVersionFilter.Create();
                return filter;
            }

            public static DocumentFilter WithDescription(this DocumentFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new DocumentFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentQuery WithDescription(this DocumentQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new DocumentFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentFilter WithName(this DocumentFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new DocumentFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentQuery WithName(this DocumentQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new DocumentFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class DocumentVersionFilterExtensions
        {
            public static DocumentVersionFilter WithId(this DocumentVersionFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new DocumentVersionFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static DocumentVersionQuery WithId(this DocumentVersionQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new DocumentVersionFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithCreatedOn(this DocumentVersionFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new DocumentVersionFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithCreatedOn(this DocumentVersionQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new DocumentVersionFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithLastModifiedOn(this DocumentVersionFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new DocumentVersionFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithLastModifiedOn(this DocumentVersionQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new DocumentVersionFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithVersion(this DocumentVersionFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new DocumentVersionFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithVersion(this DocumentVersionQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new DocumentVersionFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithCreatingUser(
                this DocumentVersionFilter filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeCreatingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static DocumentVersionQuery WithCreatingUser(
                this DocumentVersionQuery filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeCreatingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static DocumentVersionFilter IncludeCreatingUser(
                this DocumentVersionFilter filter,
                Membership.UserFilter filterCreatingUser = null)
            {
                filter.CreatingUser = filterCreatingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static DocumentVersionQuery IncludeCreatingUser(
                this DocumentVersionQuery filter,
                Membership.UserFilter filterCreatingUser = null)
            {
                filter.CreatingUser = filterCreatingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static DocumentVersionFilter WithDocument(
                this DocumentVersionFilter filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(DocumentFilter.Create().WithId(value.Id));
            }

            public static DocumentVersionQuery WithDocument(
                this DocumentVersionQuery filter, Gorba.Center.Common.ServiceModel.Documents.Document value)
            {
                return filter.IncludeDocument(DocumentFilter.Create().WithId(value.Id));
            }

            public static DocumentVersionFilter IncludeDocument(
                this DocumentVersionFilter filter,
                DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? DocumentFilter.Create();
                return filter;
            }

            public static DocumentVersionQuery IncludeDocument(
                this DocumentVersionQuery filter,
                DocumentFilter filterDocument = null)
            {
                filter.Document = filterDocument ?? DocumentFilter.Create();
                return filter;
            }

            public static DocumentVersionFilter WithDescription(this DocumentVersionFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new DocumentVersionFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithDescription(this DocumentVersionQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new DocumentVersionFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithMajor(this DocumentVersionFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Major = new DocumentVersionFilter.MajorPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithMajor(this DocumentVersionQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Major = new DocumentVersionFilter.MajorPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter WithMinor(this DocumentVersionFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Minor = new DocumentVersionFilter.MinorPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionQuery WithMinor(this DocumentVersionQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Minor = new DocumentVersionFilter.MinorPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static DocumentVersionFilter IncludeContent(this DocumentVersionFilter filter)
            {
                filter.IncludeContent = true;
                return filter;
            }

            public static DocumentVersionQuery IncludeContent(this DocumentVersionQuery query)
            {
                query.IncludeContent = true;
                return query;
            }
        }

    }
    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;

        public static class LogEntryFilterExtensions
        {
            public static LogEntryFilter WithId(this LogEntryFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new LogEntryFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static LogEntryQuery WithId(this LogEntryQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new LogEntryFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithUnit(
                this LogEntryFilter filter, Gorba.Center.Common.ServiceModel.Units.Unit value)
            {
                return filter.IncludeUnit(Units.UnitFilter.Create().WithId(value.Id));
            }

            public static LogEntryQuery WithUnit(
                this LogEntryQuery filter, Gorba.Center.Common.ServiceModel.Units.Unit value)
            {
                return filter.IncludeUnit(Units.UnitFilter.Create().WithId(value.Id));
            }

            public static LogEntryFilter IncludeUnit(
                this LogEntryFilter filter,
                Units.UnitFilter filterUnit = null)
            {
                filter.Unit = filterUnit ?? Units.UnitFilter.Create();
                return filter;
            }

            public static LogEntryQuery IncludeUnit(
                this LogEntryQuery filter,
                Units.UnitFilter filterUnit = null)
            {
                filter.Unit = filterUnit ?? Units.UnitFilter.Create();
                return filter;
            }

            public static LogEntryFilter WithLevel(this LogEntryFilter filter, Level value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Level = new LogEntryFilter.LevelPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithLevel(this LogEntryQuery filter, Level value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Level = new LogEntryFilter.LevelPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithAdditionalData(this LogEntryFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.AdditionalData = new LogEntryFilter.AdditionalDataPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithAdditionalData(this LogEntryQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.AdditionalData = new LogEntryFilter.AdditionalDataPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithApplication(this LogEntryFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Application = new LogEntryFilter.ApplicationPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithApplication(this LogEntryQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Application = new LogEntryFilter.ApplicationPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithLogger(this LogEntryFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Logger = new LogEntryFilter.LoggerPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithLogger(this LogEntryQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Logger = new LogEntryFilter.LoggerPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithMessage(this LogEntryFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Message = new LogEntryFilter.MessagePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithMessage(this LogEntryQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Message = new LogEntryFilter.MessagePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryFilter WithTimestamp(this LogEntryFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Timestamp = new LogEntryFilter.TimestampPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static LogEntryQuery WithTimestamp(this LogEntryQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Timestamp = new LogEntryFilter.TimestampPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;

        public static class AssociationTenantUserUserRoleFilterExtensions
        {
            public static AssociationTenantUserUserRoleFilter WithId(this AssociationTenantUserUserRoleFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new AssociationTenantUserUserRoleFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static AssociationTenantUserUserRoleQuery WithId(this AssociationTenantUserUserRoleQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new AssociationTenantUserUserRoleFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithCreatedOn(this AssociationTenantUserUserRoleFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new AssociationTenantUserUserRoleFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery WithCreatedOn(this AssociationTenantUserUserRoleQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new AssociationTenantUserUserRoleFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithLastModifiedOn(this AssociationTenantUserUserRoleFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new AssociationTenantUserUserRoleFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery WithLastModifiedOn(this AssociationTenantUserUserRoleQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new AssociationTenantUserUserRoleFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithVersion(this AssociationTenantUserUserRoleFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new AssociationTenantUserUserRoleFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery WithVersion(this AssociationTenantUserUserRoleQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new AssociationTenantUserUserRoleFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithTenant(
                this AssociationTenantUserUserRoleFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(TenantFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleQuery WithTenant(
                this AssociationTenantUserUserRoleQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(TenantFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleFilter IncludeTenant(
                this AssociationTenantUserUserRoleFilter filter,
                TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? TenantFilter.Create();
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery IncludeTenant(
                this AssociationTenantUserUserRoleQuery filter,
                TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? TenantFilter.Create();
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithUser(
                this AssociationTenantUserUserRoleFilter filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUser(UserFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleQuery WithUser(
                this AssociationTenantUserUserRoleQuery filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUser(UserFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleFilter IncludeUser(
                this AssociationTenantUserUserRoleFilter filter,
                UserFilter filterUser = null)
            {
                filter.User = filterUser ?? UserFilter.Create();
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery IncludeUser(
                this AssociationTenantUserUserRoleQuery filter,
                UserFilter filterUser = null)
            {
                filter.User = filterUser ?? UserFilter.Create();
                return filter;
            }

            public static AssociationTenantUserUserRoleFilter WithUserRole(
                this AssociationTenantUserUserRoleFilter filter, Gorba.Center.Common.ServiceModel.AccessControl.UserRole value)
            {
                return filter.IncludeUserRole(AccessControl.UserRoleFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleQuery WithUserRole(
                this AssociationTenantUserUserRoleQuery filter, Gorba.Center.Common.ServiceModel.AccessControl.UserRole value)
            {
                return filter.IncludeUserRole(AccessControl.UserRoleFilter.Create().WithId(value.Id));
            }

            public static AssociationTenantUserUserRoleFilter IncludeUserRole(
                this AssociationTenantUserUserRoleFilter filter,
                AccessControl.UserRoleFilter filterUserRole = null)
            {
                filter.UserRole = filterUserRole ?? AccessControl.UserRoleFilter.Create();
                return filter;
            }

            public static AssociationTenantUserUserRoleQuery IncludeUserRole(
                this AssociationTenantUserUserRoleQuery filter,
                AccessControl.UserRoleFilter filterUserRole = null)
            {
                filter.UserRole = filterUserRole ?? AccessControl.UserRoleFilter.Create();
                return filter;
            }
        }

        public static class TenantFilterExtensions
        {
            public static TenantFilter WithId(this TenantFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new TenantFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static TenantQuery WithId(this TenantQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new TenantFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantFilter WithCreatedOn(this TenantFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new TenantFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantQuery WithCreatedOn(this TenantQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new TenantFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantFilter WithLastModifiedOn(this TenantFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new TenantFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantQuery WithLastModifiedOn(this TenantQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new TenantFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantFilter WithVersion(this TenantFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new TenantFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantQuery WithVersion(this TenantQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new TenantFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantFilter IncludeUpdateGroups(
                this TenantFilter filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                if (filterUpdateGroups != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.UpdateGroups = Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static TenantQuery IncludeUpdateGroups(
                this TenantQuery filter,
                Update.UpdateGroupFilter filterUpdateGroups = null)
            {
                filter.UpdateGroups = filterUpdateGroups ?? Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static TenantFilter IncludeUsers(
                this TenantFilter filter,
                UserFilter filterUsers = null)
            {
                if (filterUsers != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Users = UserFilter.Create();
                return filter;
            }

            public static TenantQuery IncludeUsers(
                this TenantQuery filter,
                UserFilter filterUsers = null)
            {
                filter.Users = filterUsers ?? UserFilter.Create();
                return filter;
            }

            public static TenantFilter WithDescription(this TenantFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new TenantFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantQuery WithDescription(this TenantQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new TenantFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantFilter WithName(this TenantFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new TenantFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static TenantQuery WithName(this TenantQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new TenantFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class UserFilterExtensions
        {
            public static UserFilter WithId(this UserFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UserQuery WithId(this UserQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithCreatedOn(this UserFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithCreatedOn(this UserQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithLastModifiedOn(this UserFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithLastModifiedOn(this UserQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithVersion(this UserFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithVersion(this UserQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithOwnerTenant(
                this UserFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeOwnerTenant(TenantFilter.Create().WithId(value.Id));
            }

            public static UserQuery WithOwnerTenant(
                this UserQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeOwnerTenant(TenantFilter.Create().WithId(value.Id));
            }

            public static UserFilter IncludeOwnerTenant(
                this UserFilter filter,
                TenantFilter filterOwnerTenant = null)
            {
                filter.OwnerTenant = filterOwnerTenant ?? TenantFilter.Create();
                return filter;
            }

            public static UserQuery IncludeOwnerTenant(
                this UserQuery filter,
                TenantFilter filterOwnerTenant = null)
            {
                filter.OwnerTenant = filterOwnerTenant ?? TenantFilter.Create();
                return filter;
            }

            public static UserFilter IncludeAssociationTenantUserUserRoles(
                this UserFilter filter,
                AssociationTenantUserUserRoleFilter filterAssociationTenantUserUserRoles = null)
            {
                if (filterAssociationTenantUserUserRoles != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.AssociationTenantUserUserRoles = AssociationTenantUserUserRoleFilter.Create();
                return filter;
            }

            public static UserQuery IncludeAssociationTenantUserUserRoles(
                this UserQuery filter,
                AssociationTenantUserUserRoleFilter filterAssociationTenantUserUserRoles = null)
            {
                filter.AssociationTenantUserUserRoles = filterAssociationTenantUserUserRoles ?? AssociationTenantUserUserRoleFilter.Create();
                return filter;
            }

            public static UserFilter WithConsecutiveLoginFailures(this UserFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.ConsecutiveLoginFailures = new UserFilter.ConsecutiveLoginFailuresPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithConsecutiveLoginFailures(this UserQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.ConsecutiveLoginFailures = new UserFilter.ConsecutiveLoginFailuresPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithCulture(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Culture = new UserFilter.CulturePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithCulture(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Culture = new UserFilter.CulturePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithDescription(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UserFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithDescription(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UserFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithDomain(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Domain = new UserFilter.DomainPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithDomain(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Domain = new UserFilter.DomainPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithEmail(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Email = new UserFilter.EmailPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithEmail(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Email = new UserFilter.EmailPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithFirstName(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.FirstName = new UserFilter.FirstNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithFirstName(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.FirstName = new UserFilter.FirstNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithHashedPassword(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.HashedPassword = new UserFilter.HashedPasswordPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithHashedPassword(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.HashedPassword = new UserFilter.HashedPasswordPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithIsEnabled(this UserFilter filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.IsEnabled = new UserFilter.IsEnabledPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithIsEnabled(this UserQuery filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.IsEnabled = new UserFilter.IsEnabledPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithLastLoginAttempt(this UserFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastLoginAttempt = new UserFilter.LastLoginAttemptPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithLastLoginAttempt(this UserQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastLoginAttempt = new UserFilter.LastLoginAttemptPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithLastName(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.LastName = new UserFilter.LastNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithLastName(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.LastName = new UserFilter.LastNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithLastSuccessfulLogin(this UserFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastSuccessfulLogin = new UserFilter.LastSuccessfulLoginPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithLastSuccessfulLogin(this UserQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastSuccessfulLogin = new UserFilter.LastSuccessfulLoginPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithTimeZone(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.TimeZone = new UserFilter.TimeZonePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithTimeZone(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.TimeZone = new UserFilter.TimeZonePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserFilter WithUsername(this UserFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Username = new UserFilter.UsernamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserQuery WithUsername(this UserQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Username = new UserFilter.UsernamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;

        public static class SystemConfigFilterExtensions
        {
            public static SystemConfigFilter WithId(this SystemConfigFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new SystemConfigFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static SystemConfigQuery WithId(this SystemConfigQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new SystemConfigFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigFilter WithCreatedOn(this SystemConfigFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new SystemConfigFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigQuery WithCreatedOn(this SystemConfigQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new SystemConfigFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigFilter WithLastModifiedOn(this SystemConfigFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new SystemConfigFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigQuery WithLastModifiedOn(this SystemConfigQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new SystemConfigFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigFilter WithVersion(this SystemConfigFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new SystemConfigFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigQuery WithVersion(this SystemConfigQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new SystemConfigFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigFilter WithSystemId(this SystemConfigFilter filter, Guid value, GuidComparison comparison = GuidComparison.ExactMatch)
            {
                filter.SystemId = new SystemConfigFilter.SystemIdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigQuery WithSystemId(this SystemConfigQuery filter, Guid value, GuidComparison comparison = GuidComparison.ExactMatch)
            {
                filter.SystemId = new SystemConfigFilter.SystemIdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static SystemConfigFilter IncludeSettings(this SystemConfigFilter filter)
            {
                filter.IncludeSettings = true;
                return filter;
            }

            public static SystemConfigQuery IncludeSettings(this SystemConfigQuery query)
            {
                query.IncludeSettings = true;
                return query;
            }
        }

        public static class UserDefinedPropertyFilterExtensions
        {
            public static UserDefinedPropertyFilter WithId(this UserDefinedPropertyFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserDefinedPropertyFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UserDefinedPropertyQuery WithId(this UserDefinedPropertyQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UserDefinedPropertyFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyFilter WithCreatedOn(this UserDefinedPropertyFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserDefinedPropertyFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyQuery WithCreatedOn(this UserDefinedPropertyQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UserDefinedPropertyFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyFilter WithLastModifiedOn(this UserDefinedPropertyFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserDefinedPropertyFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyQuery WithLastModifiedOn(this UserDefinedPropertyQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UserDefinedPropertyFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyFilter WithVersion(this UserDefinedPropertyFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserDefinedPropertyFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyQuery WithVersion(this UserDefinedPropertyQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UserDefinedPropertyFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyFilter WithTenant(
                this UserDefinedPropertyFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UserDefinedPropertyQuery WithTenant(
                this UserDefinedPropertyQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UserDefinedPropertyFilter IncludeTenant(
                this UserDefinedPropertyFilter filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UserDefinedPropertyQuery IncludeTenant(
                this UserDefinedPropertyQuery filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UserDefinedPropertyFilter WithOwnerEntity(this UserDefinedPropertyFilter filter, UserDefinedPropertyEnabledEntity value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.OwnerEntity = new UserDefinedPropertyFilter.OwnerEntityPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyQuery WithOwnerEntity(this UserDefinedPropertyQuery filter, UserDefinedPropertyEnabledEntity value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.OwnerEntity = new UserDefinedPropertyFilter.OwnerEntityPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyFilter WithName(this UserDefinedPropertyFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UserDefinedPropertyFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UserDefinedPropertyQuery WithName(this UserDefinedPropertyQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UserDefinedPropertyFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;

        public static class ContentResourceFilterExtensions
        {
            public static ContentResourceFilter WithId(this ContentResourceFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ContentResourceFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static ContentResourceQuery WithId(this ContentResourceQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ContentResourceFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithCreatedOn(this ContentResourceFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ContentResourceFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithCreatedOn(this ContentResourceQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ContentResourceFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithLastModifiedOn(this ContentResourceFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ContentResourceFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithLastModifiedOn(this ContentResourceQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ContentResourceFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithVersion(this ContentResourceFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ContentResourceFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithVersion(this ContentResourceQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ContentResourceFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithUploadingUser(
                this ContentResourceFilter filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUploadingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static ContentResourceQuery WithUploadingUser(
                this ContentResourceQuery filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUploadingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static ContentResourceFilter IncludeUploadingUser(
                this ContentResourceFilter filter,
                Membership.UserFilter filterUploadingUser = null)
            {
                filter.UploadingUser = filterUploadingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static ContentResourceQuery IncludeUploadingUser(
                this ContentResourceQuery filter,
                Membership.UserFilter filterUploadingUser = null)
            {
                filter.UploadingUser = filterUploadingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static ContentResourceFilter WithHashAlgorithmType(this ContentResourceFilter filter, HashAlgorithmTypes value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.HashAlgorithmType = new ContentResourceFilter.HashAlgorithmTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithHashAlgorithmType(this ContentResourceQuery filter, HashAlgorithmTypes value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.HashAlgorithmType = new ContentResourceFilter.HashAlgorithmTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithDescription(this ContentResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ContentResourceFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithDescription(this ContentResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ContentResourceFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithHash(this ContentResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Hash = new ContentResourceFilter.HashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithHash(this ContentResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Hash = new ContentResourceFilter.HashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithLength(this ContentResourceFilter filter, long value, Int64Comparison comparison = Int64Comparison.ExactMatch)
            {
                filter.Length = new ContentResourceFilter.LengthPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithLength(this ContentResourceQuery filter, long value, Int64Comparison comparison = Int64Comparison.ExactMatch)
            {
                filter.Length = new ContentResourceFilter.LengthPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithMimeType(this ContentResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.MimeType = new ContentResourceFilter.MimeTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithMimeType(this ContentResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.MimeType = new ContentResourceFilter.MimeTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithOriginalFilename(this ContentResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.OriginalFilename = new ContentResourceFilter.OriginalFilenamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithOriginalFilename(this ContentResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.OriginalFilename = new ContentResourceFilter.OriginalFilenamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceFilter WithThumbnailHash(this ContentResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ThumbnailHash = new ContentResourceFilter.ThumbnailHashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ContentResourceQuery WithThumbnailHash(this ContentResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ThumbnailHash = new ContentResourceFilter.ThumbnailHashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class ResourceFilterExtensions
        {
            public static ResourceFilter WithId(this ResourceFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ResourceFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static ResourceQuery WithId(this ResourceQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ResourceFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithCreatedOn(this ResourceFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ResourceFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithCreatedOn(this ResourceQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ResourceFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithLastModifiedOn(this ResourceFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ResourceFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithLastModifiedOn(this ResourceQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ResourceFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithVersion(this ResourceFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ResourceFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithVersion(this ResourceQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ResourceFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithUploadingUser(
                this ResourceFilter filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUploadingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static ResourceQuery WithUploadingUser(
                this ResourceQuery filter, Gorba.Center.Common.ServiceModel.Membership.User value)
            {
                return filter.IncludeUploadingUser(Membership.UserFilter.Create().WithId(value.Id));
            }

            public static ResourceFilter IncludeUploadingUser(
                this ResourceFilter filter,
                Membership.UserFilter filterUploadingUser = null)
            {
                filter.UploadingUser = filterUploadingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static ResourceQuery IncludeUploadingUser(
                this ResourceQuery filter,
                Membership.UserFilter filterUploadingUser = null)
            {
                filter.UploadingUser = filterUploadingUser ?? Membership.UserFilter.Create();
                return filter;
            }

            public static ResourceFilter WithDescription(this ResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ResourceFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithDescription(this ResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ResourceFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithHash(this ResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Hash = new ResourceFilter.HashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithHash(this ResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Hash = new ResourceFilter.HashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithLength(this ResourceFilter filter, long value, Int64Comparison comparison = Int64Comparison.ExactMatch)
            {
                filter.Length = new ResourceFilter.LengthPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithLength(this ResourceQuery filter, long value, Int64Comparison comparison = Int64Comparison.ExactMatch)
            {
                filter.Length = new ResourceFilter.LengthPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithMimeType(this ResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.MimeType = new ResourceFilter.MimeTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithMimeType(this ResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.MimeType = new ResourceFilter.MimeTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithOriginalFilename(this ResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.OriginalFilename = new ResourceFilter.OriginalFilenamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithOriginalFilename(this ResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.OriginalFilename = new ResourceFilter.OriginalFilenamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceFilter WithThumbnailHash(this ResourceFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ThumbnailHash = new ResourceFilter.ThumbnailHashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ResourceQuery WithThumbnailHash(this ResourceQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ThumbnailHash = new ResourceFilter.ThumbnailHashPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;

        public static class PackageFilterExtensions
        {
            public static PackageFilter WithId(this PackageFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new PackageFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static PackageQuery WithId(this PackageQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new PackageFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter WithCreatedOn(this PackageFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new PackageFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithCreatedOn(this PackageQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new PackageFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter WithLastModifiedOn(this PackageFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new PackageFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithLastModifiedOn(this PackageQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new PackageFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter WithVersion(this PackageFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new PackageFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithVersion(this PackageQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new PackageFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter IncludeVersions(
                this PackageFilter filter,
                PackageVersionFilter filterVersions = null)
            {
                if (filterVersions != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Versions = PackageVersionFilter.Create();
                return filter;
            }

            public static PackageQuery IncludeVersions(
                this PackageQuery filter,
                PackageVersionFilter filterVersions = null)
            {
                filter.Versions = filterVersions ?? PackageVersionFilter.Create();
                return filter;
            }

            public static PackageFilter WithDescription(this PackageFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new PackageFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithDescription(this PackageQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new PackageFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter WithPackageId(this PackageFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.PackageId = new PackageFilter.PackageIdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithPackageId(this PackageQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.PackageId = new PackageFilter.PackageIdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageFilter WithProductName(this PackageFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ProductName = new PackageFilter.ProductNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageQuery WithProductName(this PackageQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.ProductName = new PackageFilter.ProductNamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class PackageVersionFilterExtensions
        {
            public static PackageVersionFilter WithId(this PackageVersionFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new PackageVersionFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static PackageVersionQuery WithId(this PackageVersionQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new PackageVersionFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter WithCreatedOn(this PackageVersionFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new PackageVersionFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionQuery WithCreatedOn(this PackageVersionQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new PackageVersionFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter WithLastModifiedOn(this PackageVersionFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new PackageVersionFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionQuery WithLastModifiedOn(this PackageVersionQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new PackageVersionFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter WithVersion(this PackageVersionFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new PackageVersionFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionQuery WithVersion(this PackageVersionQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new PackageVersionFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter WithPackage(
                this PackageVersionFilter filter, Gorba.Center.Common.ServiceModel.Software.Package value)
            {
                return filter.IncludePackage(PackageFilter.Create().WithId(value.Id));
            }

            public static PackageVersionQuery WithPackage(
                this PackageVersionQuery filter, Gorba.Center.Common.ServiceModel.Software.Package value)
            {
                return filter.IncludePackage(PackageFilter.Create().WithId(value.Id));
            }

            public static PackageVersionFilter IncludePackage(
                this PackageVersionFilter filter,
                PackageFilter filterPackage = null)
            {
                filter.Package = filterPackage ?? PackageFilter.Create();
                return filter;
            }

            public static PackageVersionQuery IncludePackage(
                this PackageVersionQuery filter,
                PackageFilter filterPackage = null)
            {
                filter.Package = filterPackage ?? PackageFilter.Create();
                return filter;
            }

            public static PackageVersionFilter WithDescription(this PackageVersionFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new PackageVersionFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionQuery WithDescription(this PackageVersionQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new PackageVersionFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter WithSoftwareVersion(this PackageVersionFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.SoftwareVersion = new PackageVersionFilter.SoftwareVersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionQuery WithSoftwareVersion(this PackageVersionQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.SoftwareVersion = new PackageVersionFilter.SoftwareVersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static PackageVersionFilter IncludeStructure(this PackageVersionFilter filter)
            {
                filter.IncludeStructure = true;
                return filter;
            }

            public static PackageVersionQuery IncludeStructure(this PackageVersionQuery query)
            {
                query.IncludeStructure = true;
                return query;
            }
        }

    }
    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;

        public static class ProductTypeFilterExtensions
        {
            public static ProductTypeFilter WithId(this ProductTypeFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ProductTypeFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static ProductTypeQuery WithId(this ProductTypeQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new ProductTypeFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter WithCreatedOn(this ProductTypeFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ProductTypeFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithCreatedOn(this ProductTypeQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new ProductTypeFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter WithLastModifiedOn(this ProductTypeFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ProductTypeFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithLastModifiedOn(this ProductTypeQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new ProductTypeFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter WithVersion(this ProductTypeFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ProductTypeFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithVersion(this ProductTypeQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new ProductTypeFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter IncludeUnits(
                this ProductTypeFilter filter,
                UnitFilter filterUnits = null)
            {
                if (filterUnits != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Units = UnitFilter.Create();
                return filter;
            }

            public static ProductTypeQuery IncludeUnits(
                this ProductTypeQuery filter,
                UnitFilter filterUnits = null)
            {
                filter.Units = filterUnits ?? UnitFilter.Create();
                return filter;
            }

            public static ProductTypeFilter WithUnitType(this ProductTypeFilter filter, UnitTypes value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.UnitType = new ProductTypeFilter.UnitTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithUnitType(this ProductTypeQuery filter, UnitTypes value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.UnitType = new ProductTypeFilter.UnitTypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter WithDescription(this ProductTypeFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ProductTypeFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithDescription(this ProductTypeQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new ProductTypeFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter WithName(this ProductTypeFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new ProductTypeFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeQuery WithName(this ProductTypeQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new ProductTypeFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static ProductTypeFilter IncludeHardwareDescriptor(this ProductTypeFilter filter)
            {
                filter.IncludeHardwareDescriptor = true;
                return filter;
            }

            public static ProductTypeQuery IncludeHardwareDescriptor(this ProductTypeQuery query)
            {
                query.IncludeHardwareDescriptor = true;
                return query;
            }
        }

        public static class UnitFilterExtensions
        {
            public static UnitFilter WithId(this UnitFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UnitFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UnitQuery WithId(this UnitQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UnitFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithCreatedOn(this UnitFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UnitFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithCreatedOn(this UnitQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UnitFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithLastModifiedOn(this UnitFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UnitFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithLastModifiedOn(this UnitQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UnitFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithVersion(this UnitFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UnitFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithVersion(this UnitQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UnitFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithProductType(
                this UnitFilter filter, Gorba.Center.Common.ServiceModel.Units.ProductType value)
            {
                return filter.IncludeProductType(ProductTypeFilter.Create().WithId(value.Id));
            }

            public static UnitQuery WithProductType(
                this UnitQuery filter, Gorba.Center.Common.ServiceModel.Units.ProductType value)
            {
                return filter.IncludeProductType(ProductTypeFilter.Create().WithId(value.Id));
            }

            public static UnitFilter IncludeProductType(
                this UnitFilter filter,
                ProductTypeFilter filterProductType = null)
            {
                filter.ProductType = filterProductType ?? ProductTypeFilter.Create();
                return filter;
            }

            public static UnitQuery IncludeProductType(
                this UnitQuery filter,
                ProductTypeFilter filterProductType = null)
            {
                filter.ProductType = filterProductType ?? ProductTypeFilter.Create();
                return filter;
            }

            public static UnitFilter WithTenant(
                this UnitFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UnitQuery WithTenant(
                this UnitQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UnitFilter IncludeTenant(
                this UnitFilter filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UnitQuery IncludeTenant(
                this UnitQuery filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UnitFilter WithUpdateGroup(
                this UnitFilter filter, Gorba.Center.Common.ServiceModel.Update.UpdateGroup value)
            {
                return filter.IncludeUpdateGroup(Update.UpdateGroupFilter.Create().WithId(value.Id));
            }

            public static UnitQuery WithUpdateGroup(
                this UnitQuery filter, Gorba.Center.Common.ServiceModel.Update.UpdateGroup value)
            {
                return filter.IncludeUpdateGroup(Update.UpdateGroupFilter.Create().WithId(value.Id));
            }

            public static UnitFilter IncludeUpdateGroup(
                this UnitFilter filter,
                Update.UpdateGroupFilter filterUpdateGroup = null)
            {
                filter.UpdateGroup = filterUpdateGroup ?? Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static UnitQuery IncludeUpdateGroup(
                this UnitQuery filter,
                Update.UpdateGroupFilter filterUpdateGroup = null)
            {
                filter.UpdateGroup = filterUpdateGroup ?? Update.UpdateGroupFilter.Create();
                return filter;
            }

            public static UnitFilter IncludeUpdateCommands(
                this UnitFilter filter,
                Update.UpdateCommandFilter filterUpdateCommands = null)
            {
                if (filterUpdateCommands != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.UpdateCommands = Update.UpdateCommandFilter.Create();
                return filter;
            }

            public static UnitQuery IncludeUpdateCommands(
                this UnitQuery filter,
                Update.UpdateCommandFilter filterUpdateCommands = null)
            {
                filter.UpdateCommands = filterUpdateCommands ?? Update.UpdateCommandFilter.Create();
                return filter;
            }

            public static UnitFilter WithDescription(this UnitFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UnitFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithDescription(this UnitQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UnitFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithIsConnected(this UnitFilter filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.IsConnected = new UnitFilter.IsConnectedPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithIsConnected(this UnitQuery filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.IsConnected = new UnitFilter.IsConnectedPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithName(this UnitFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UnitFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithName(this UnitQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UnitFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitFilter WithNetworkAddress(this UnitFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.NetworkAddress = new UnitFilter.NetworkAddressPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UnitQuery WithNetworkAddress(this UnitQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.NetworkAddress = new UnitFilter.NetworkAddressPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

    }
    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;

        public static class UpdateCommandFilterExtensions
        {
            public static UpdateCommandFilter WithId(this UpdateCommandFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateCommandFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UpdateCommandQuery WithId(this UpdateCommandQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateCommandFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithCreatedOn(this UpdateCommandFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateCommandFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithCreatedOn(this UpdateCommandQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateCommandFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithLastModifiedOn(this UpdateCommandFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateCommandFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithLastModifiedOn(this UpdateCommandQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateCommandFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithVersion(this UpdateCommandFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateCommandFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithVersion(this UpdateCommandQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateCommandFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithUnit(
                this UpdateCommandFilter filter, Gorba.Center.Common.ServiceModel.Units.Unit value)
            {
                return filter.IncludeUnit(Units.UnitFilter.Create().WithId(value.Id));
            }

            public static UpdateCommandQuery WithUnit(
                this UpdateCommandQuery filter, Gorba.Center.Common.ServiceModel.Units.Unit value)
            {
                return filter.IncludeUnit(Units.UnitFilter.Create().WithId(value.Id));
            }

            public static UpdateCommandFilter IncludeUnit(
                this UpdateCommandFilter filter,
                Units.UnitFilter filterUnit = null)
            {
                filter.Unit = filterUnit ?? Units.UnitFilter.Create();
                return filter;
            }

            public static UpdateCommandQuery IncludeUnit(
                this UpdateCommandQuery filter,
                Units.UnitFilter filterUnit = null)
            {
                filter.Unit = filterUnit ?? Units.UnitFilter.Create();
                return filter;
            }

            public static UpdateCommandFilter IncludeFeedbacks(
                this UpdateCommandFilter filter,
                UpdateFeedbackFilter filterFeedbacks = null)
            {
                if (filterFeedbacks != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Feedbacks = UpdateFeedbackFilter.Create();
                return filter;
            }

            public static UpdateCommandQuery IncludeFeedbacks(
                this UpdateCommandQuery filter,
                UpdateFeedbackFilter filterFeedbacks = null)
            {
                filter.Feedbacks = filterFeedbacks ?? UpdateFeedbackFilter.Create();
                return filter;
            }

            public static UpdateCommandFilter IncludeIncludedParts(
                this UpdateCommandFilter filter,
                UpdatePartFilter filterIncludedParts = null)
            {
                if (filterIncludedParts != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.IncludedParts = UpdatePartFilter.Create();
                return filter;
            }

            public static UpdateCommandQuery IncludeIncludedParts(
                this UpdateCommandQuery filter,
                UpdatePartFilter filterIncludedParts = null)
            {
                filter.IncludedParts = filterIncludedParts ?? UpdatePartFilter.Create();
                return filter;
            }

            public static UpdateCommandFilter WithUpdateIndex(this UpdateCommandFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.UpdateIndex = new UpdateCommandFilter.UpdateIndexPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithUpdateIndex(this UpdateCommandQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.UpdateIndex = new UpdateCommandFilter.UpdateIndexPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithWasInstalled(this UpdateCommandFilter filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.WasInstalled = new UpdateCommandFilter.WasInstalledPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithWasInstalled(this UpdateCommandQuery filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.WasInstalled = new UpdateCommandFilter.WasInstalledPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter WithWasTransferred(this UpdateCommandFilter filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.WasTransferred = new UpdateCommandFilter.WasTransferredPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandQuery WithWasTransferred(this UpdateCommandQuery filter, bool value, BooleanComparison comparison = BooleanComparison.ExactMatch)
            {
                filter.WasTransferred = new UpdateCommandFilter.WasTransferredPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateCommandFilter IncludeCommand(this UpdateCommandFilter filter)
            {
                filter.IncludeCommand = true;
                return filter;
            }

            public static UpdateCommandQuery IncludeCommand(this UpdateCommandQuery query)
            {
                query.IncludeCommand = true;
                return query;
            }
        }

        public static class UpdateFeedbackFilterExtensions
        {
            public static UpdateFeedbackFilter WithId(this UpdateFeedbackFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateFeedbackFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UpdateFeedbackQuery WithId(this UpdateFeedbackQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateFeedbackFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter WithCreatedOn(this UpdateFeedbackFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateFeedbackFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackQuery WithCreatedOn(this UpdateFeedbackQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateFeedbackFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter WithLastModifiedOn(this UpdateFeedbackFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateFeedbackFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackQuery WithLastModifiedOn(this UpdateFeedbackQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateFeedbackFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter WithVersion(this UpdateFeedbackFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateFeedbackFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackQuery WithVersion(this UpdateFeedbackQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateFeedbackFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter WithUpdateCommand(
                this UpdateFeedbackFilter filter, Gorba.Center.Common.ServiceModel.Update.UpdateCommand value)
            {
                return filter.IncludeUpdateCommand(UpdateCommandFilter.Create().WithId(value.Id));
            }

            public static UpdateFeedbackQuery WithUpdateCommand(
                this UpdateFeedbackQuery filter, Gorba.Center.Common.ServiceModel.Update.UpdateCommand value)
            {
                return filter.IncludeUpdateCommand(UpdateCommandFilter.Create().WithId(value.Id));
            }

            public static UpdateFeedbackFilter IncludeUpdateCommand(
                this UpdateFeedbackFilter filter,
                UpdateCommandFilter filterUpdateCommand = null)
            {
                filter.UpdateCommand = filterUpdateCommand ?? UpdateCommandFilter.Create();
                return filter;
            }

            public static UpdateFeedbackQuery IncludeUpdateCommand(
                this UpdateFeedbackQuery filter,
                UpdateCommandFilter filterUpdateCommand = null)
            {
                filter.UpdateCommand = filterUpdateCommand ?? UpdateCommandFilter.Create();
                return filter;
            }

            public static UpdateFeedbackFilter WithState(this UpdateFeedbackFilter filter, UpdateState value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.State = new UpdateFeedbackFilter.StatePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackQuery WithState(this UpdateFeedbackQuery filter, UpdateState value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.State = new UpdateFeedbackFilter.StatePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter WithTimestamp(this UpdateFeedbackFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Timestamp = new UpdateFeedbackFilter.TimestampPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackQuery WithTimestamp(this UpdateFeedbackQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Timestamp = new UpdateFeedbackFilter.TimestampPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateFeedbackFilter IncludeFeedback(this UpdateFeedbackFilter filter)
            {
                filter.IncludeFeedback = true;
                return filter;
            }

            public static UpdateFeedbackQuery IncludeFeedback(this UpdateFeedbackQuery query)
            {
                query.IncludeFeedback = true;
                return query;
            }
        }

        public static class UpdateGroupFilterExtensions
        {
            public static UpdateGroupFilter WithId(this UpdateGroupFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateGroupFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UpdateGroupQuery WithId(this UpdateGroupQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdateGroupFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupFilter WithCreatedOn(this UpdateGroupFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateGroupFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupQuery WithCreatedOn(this UpdateGroupQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdateGroupFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupFilter WithLastModifiedOn(this UpdateGroupFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateGroupFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupQuery WithLastModifiedOn(this UpdateGroupQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdateGroupFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupFilter WithVersion(this UpdateGroupFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateGroupFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupQuery WithVersion(this UpdateGroupQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdateGroupFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupFilter WithMediaConfiguration(
                this UpdateGroupFilter filter, Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration value)
            {
                return filter.IncludeMediaConfiguration(Configurations.MediaConfigurationFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupQuery WithMediaConfiguration(
                this UpdateGroupQuery filter, Gorba.Center.Common.ServiceModel.Configurations.MediaConfiguration value)
            {
                return filter.IncludeMediaConfiguration(Configurations.MediaConfigurationFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupFilter IncludeMediaConfiguration(
                this UpdateGroupFilter filter,
                Configurations.MediaConfigurationFilter filterMediaConfiguration = null)
            {
                filter.MediaConfiguration = filterMediaConfiguration ?? Configurations.MediaConfigurationFilter.Create();
                return filter;
            }

            public static UpdateGroupQuery IncludeMediaConfiguration(
                this UpdateGroupQuery filter,
                Configurations.MediaConfigurationFilter filterMediaConfiguration = null)
            {
                filter.MediaConfiguration = filterMediaConfiguration ?? Configurations.MediaConfigurationFilter.Create();
                return filter;
            }

            public static UpdateGroupFilter WithTenant(
                this UpdateGroupFilter filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupQuery WithTenant(
                this UpdateGroupQuery filter, Gorba.Center.Common.ServiceModel.Membership.Tenant value)
            {
                return filter.IncludeTenant(Membership.TenantFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupFilter IncludeTenant(
                this UpdateGroupFilter filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UpdateGroupQuery IncludeTenant(
                this UpdateGroupQuery filter,
                Membership.TenantFilter filterTenant = null)
            {
                filter.Tenant = filterTenant ?? Membership.TenantFilter.Create();
                return filter;
            }

            public static UpdateGroupFilter WithUnitConfiguration(
                this UpdateGroupFilter filter, Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration value)
            {
                return filter.IncludeUnitConfiguration(Configurations.UnitConfigurationFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupQuery WithUnitConfiguration(
                this UpdateGroupQuery filter, Gorba.Center.Common.ServiceModel.Configurations.UnitConfiguration value)
            {
                return filter.IncludeUnitConfiguration(Configurations.UnitConfigurationFilter.Create().WithId(value.Id));
            }

            public static UpdateGroupFilter IncludeUnitConfiguration(
                this UpdateGroupFilter filter,
                Configurations.UnitConfigurationFilter filterUnitConfiguration = null)
            {
                filter.UnitConfiguration = filterUnitConfiguration ?? Configurations.UnitConfigurationFilter.Create();
                return filter;
            }

            public static UpdateGroupQuery IncludeUnitConfiguration(
                this UpdateGroupQuery filter,
                Configurations.UnitConfigurationFilter filterUnitConfiguration = null)
            {
                filter.UnitConfiguration = filterUnitConfiguration ?? Configurations.UnitConfigurationFilter.Create();
                return filter;
            }

            public static UpdateGroupFilter IncludeUnits(
                this UpdateGroupFilter filter,
                Units.UnitFilter filterUnits = null)
            {
                if (filterUnits != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.Units = Units.UnitFilter.Create();
                return filter;
            }

            public static UpdateGroupQuery IncludeUnits(
                this UpdateGroupQuery filter,
                Units.UnitFilter filterUnits = null)
            {
                filter.Units = filterUnits ?? Units.UnitFilter.Create();
                return filter;
            }

            public static UpdateGroupFilter IncludeUpdateParts(
                this UpdateGroupFilter filter,
                UpdatePartFilter filterUpdateParts = null)
            {
                if (filterUpdateParts != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.UpdateParts = UpdatePartFilter.Create();
                return filter;
            }

            public static UpdateGroupQuery IncludeUpdateParts(
                this UpdateGroupQuery filter,
                UpdatePartFilter filterUpdateParts = null)
            {
                filter.UpdateParts = filterUpdateParts ?? UpdatePartFilter.Create();
                return filter;
            }

            public static UpdateGroupFilter WithDescription(this UpdateGroupFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UpdateGroupFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupQuery WithDescription(this UpdateGroupQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UpdateGroupFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupFilter WithName(this UpdateGroupFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UpdateGroupFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdateGroupQuery WithName(this UpdateGroupQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Name = new UpdateGroupFilter.NamePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
        }

        public static class UpdatePartFilterExtensions
        {
            public static UpdatePartFilter WithId(this UpdatePartFilter filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdatePartFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }
            
            public static UpdatePartQuery WithId(this UpdatePartQuery filter, Int32 value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Id = new UpdatePartFilter.IdPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithCreatedOn(this UpdatePartFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdatePartFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithCreatedOn(this UpdatePartQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.CreatedOn = new UpdatePartFilter.CreatedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithLastModifiedOn(this UpdatePartFilter filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdatePartFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithLastModifiedOn(this UpdatePartQuery filter, DateTime? value, NullableDateTimeComparison comparison = NullableDateTimeComparison.ExactMatch)
            {
                filter.LastModifiedOn = new UpdatePartFilter.LastModifiedOnPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithVersion(this UpdatePartFilter filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdatePartFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithVersion(this UpdatePartQuery filter, int value, Int32Comparison comparison = Int32Comparison.ExactMatch)
            {
                filter.Version = new UpdatePartFilter.VersionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithUpdateGroup(
                this UpdatePartFilter filter, Gorba.Center.Common.ServiceModel.Update.UpdateGroup value)
            {
                return filter.IncludeUpdateGroup(UpdateGroupFilter.Create().WithId(value.Id));
            }

            public static UpdatePartQuery WithUpdateGroup(
                this UpdatePartQuery filter, Gorba.Center.Common.ServiceModel.Update.UpdateGroup value)
            {
                return filter.IncludeUpdateGroup(UpdateGroupFilter.Create().WithId(value.Id));
            }

            public static UpdatePartFilter IncludeUpdateGroup(
                this UpdatePartFilter filter,
                UpdateGroupFilter filterUpdateGroup = null)
            {
                filter.UpdateGroup = filterUpdateGroup ?? UpdateGroupFilter.Create();
                return filter;
            }

            public static UpdatePartQuery IncludeUpdateGroup(
                this UpdatePartQuery filter,
                UpdateGroupFilter filterUpdateGroup = null)
            {
                filter.UpdateGroup = filterUpdateGroup ?? UpdateGroupFilter.Create();
                return filter;
            }

            public static UpdatePartFilter IncludeRelatedCommands(
                this UpdatePartFilter filter,
                UpdateCommandFilter filterRelatedCommands = null)
            {
                if (filterRelatedCommands != null)
                {
                    throw new NotSupportedException("Filtering on collection properties is not yet supported");
                }

                filter.RelatedCommands = UpdateCommandFilter.Create();
                return filter;
            }

            public static UpdatePartQuery IncludeRelatedCommands(
                this UpdatePartQuery filter,
                UpdateCommandFilter filterRelatedCommands = null)
            {
                filter.RelatedCommands = filterRelatedCommands ?? UpdateCommandFilter.Create();
                return filter;
            }

            public static UpdatePartFilter WithType(this UpdatePartFilter filter, UpdatePartType value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Type = new UpdatePartFilter.TypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithType(this UpdatePartQuery filter, UpdatePartType value, EnumComparison comparison = EnumComparison.ExactMatch)
            {
                filter.Type = new UpdatePartFilter.TypePropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithDescription(this UpdatePartFilter filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UpdatePartFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithDescription(this UpdatePartQuery filter, string value, StringComparison comparison = StringComparison.ExactMatch)
            {
                filter.Description = new UpdatePartFilter.DescriptionPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithEnd(this UpdatePartFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.End = new UpdatePartFilter.EndPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithEnd(this UpdatePartQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.End = new UpdatePartFilter.EndPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter WithStart(this UpdatePartFilter filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Start = new UpdatePartFilter.StartPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartQuery WithStart(this UpdatePartQuery filter, DateTime value, DateTimeComparison comparison = DateTimeComparison.ExactMatch)
            {
                filter.Start = new UpdatePartFilter.StartPropertyValueFilter
                {
                    Comparison = comparison,
                    Value = value
                };
                return filter;
            }

            public static UpdatePartFilter IncludeDynamicContent(this UpdatePartFilter filter)
            {
                filter.IncludeDynamicContent = true;
                return filter;
            }

            public static UpdatePartQuery IncludeDynamicContent(this UpdatePartQuery query)
            {
                query.IncludeDynamicContent = true;
                return query;
            }

            public static UpdatePartFilter IncludeInstallInstructions(this UpdatePartFilter filter)
            {
                filter.IncludeInstallInstructions = true;
                return filter;
            }

            public static UpdatePartQuery IncludeInstallInstructions(this UpdatePartQuery query)
            {
                query.IncludeInstallInstructions = true;
                return query;
            }

            public static UpdatePartFilter IncludeStructure(this UpdatePartFilter filter)
            {
                filter.IncludeStructure = true;
                return filter;
            }

            public static UpdatePartQuery IncludeStructure(this UpdatePartQuery query)
            {
                query.IncludeStructure = true;
                return query;
            }
        }

    }
}