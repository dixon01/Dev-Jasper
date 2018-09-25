namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    using Filters;
    
    using Gorba.Center.Common.ServiceModel;
    namespace AccessControl
    {
        using Gorba.Center.Common.ServiceModel.AccessControl;

                public static class AuthorizationQueryExtensions
        {

            public static AuthorizationQuery OrderByCreatedOn(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByCreatedOnDescending(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByLastModifiedOn(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByLastModifiedOnDescending(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByVersion(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByVersionDescending(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByDataScope(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.DataScope, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByDataScopeDescending(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.DataScope, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByPermission(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.Permission, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AuthorizationQuery OrderByPermissionDescending(this AuthorizationQuery query)
            {
                var clause = new AuthorizationQuery.OrderClause(AuthorizationQuery.SortingProperties.Permission, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UserRoleQueryExtensions
        {

            public static UserRoleQuery OrderByCreatedOn(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByCreatedOnDescending(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByLastModifiedOn(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByLastModifiedOnDescending(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByVersion(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByVersionDescending(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByDescription(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByDescriptionDescending(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByName(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserRoleQuery OrderByNameDescending(this UserRoleQuery query)
            {
                var clause = new UserRoleQuery.OrderClause(UserRoleQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Configurations
    {
        using Gorba.Center.Common.ServiceModel.Configurations;

                public static class MediaConfigurationQueryExtensions
        {

            public static MediaConfigurationQuery OrderByCreatedOn(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static MediaConfigurationQuery OrderByCreatedOnDescending(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static MediaConfigurationQuery OrderByLastModifiedOn(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static MediaConfigurationQuery OrderByLastModifiedOnDescending(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static MediaConfigurationQuery OrderByVersion(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static MediaConfigurationQuery OrderByVersionDescending(this MediaConfigurationQuery query)
            {
                var clause = new MediaConfigurationQuery.OrderClause(MediaConfigurationQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UnitConfigurationQueryExtensions
        {

            public static UnitConfigurationQuery OrderByCreatedOn(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitConfigurationQuery OrderByCreatedOnDescending(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitConfigurationQuery OrderByLastModifiedOn(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitConfigurationQuery OrderByLastModifiedOnDescending(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitConfigurationQuery OrderByVersion(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitConfigurationQuery OrderByVersionDescending(this UnitConfigurationQuery query)
            {
                var clause = new UnitConfigurationQuery.OrderClause(UnitConfigurationQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Documents
    {
        using Gorba.Center.Common.ServiceModel.Documents;

                public static class DocumentQueryExtensions
        {

            public static DocumentQuery OrderByCreatedOn(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByCreatedOnDescending(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByLastModifiedOn(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByLastModifiedOnDescending(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByVersion(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByVersionDescending(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByDescription(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByDescriptionDescending(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByName(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentQuery OrderByNameDescending(this DocumentQuery query)
            {
                var clause = new DocumentQuery.OrderClause(DocumentQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class DocumentVersionQueryExtensions
        {

            public static DocumentVersionQuery OrderByCreatedOn(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByCreatedOnDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByLastModifiedOn(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByLastModifiedOnDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByVersion(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByVersionDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByDescription(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByDescriptionDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByMajor(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Major, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByMajorDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Major, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByMinor(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Minor, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static DocumentVersionQuery OrderByMinorDescending(this DocumentVersionQuery query)
            {
                var clause = new DocumentVersionQuery.OrderClause(DocumentVersionQuery.SortingProperties.Minor, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Log
    {
        using Gorba.Center.Common.ServiceModel.Log;

                public static class LogEntryQueryExtensions
        {

            public static LogEntryQuery OrderByLevel(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Level, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByLevelDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Level, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByAdditionalData(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.AdditionalData, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByAdditionalDataDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.AdditionalData, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByApplication(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Application, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByApplicationDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Application, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByLogger(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Logger, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByLoggerDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Logger, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByMessage(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Message, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByMessageDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Message, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByTimestamp(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Timestamp, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static LogEntryQuery OrderByTimestampDescending(this LogEntryQuery query)
            {
                var clause = new LogEntryQuery.OrderClause(LogEntryQuery.SortingProperties.Timestamp, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Membership
    {
        using Gorba.Center.Common.ServiceModel.Membership;

                public static class AssociationTenantUserUserRoleQueryExtensions
        {

            public static AssociationTenantUserUserRoleQuery OrderByCreatedOn(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AssociationTenantUserUserRoleQuery OrderByCreatedOnDescending(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AssociationTenantUserUserRoleQuery OrderByLastModifiedOn(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AssociationTenantUserUserRoleQuery OrderByLastModifiedOnDescending(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AssociationTenantUserUserRoleQuery OrderByVersion(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static AssociationTenantUserUserRoleQuery OrderByVersionDescending(this AssociationTenantUserUserRoleQuery query)
            {
                var clause = new AssociationTenantUserUserRoleQuery.OrderClause(AssociationTenantUserUserRoleQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class TenantQueryExtensions
        {

            public static TenantQuery OrderByCreatedOn(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByCreatedOnDescending(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByLastModifiedOn(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByLastModifiedOnDescending(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByVersion(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByVersionDescending(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByDescription(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByDescriptionDescending(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByName(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static TenantQuery OrderByNameDescending(this TenantQuery query)
            {
                var clause = new TenantQuery.OrderClause(TenantQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UserQueryExtensions
        {

            public static UserQuery OrderByCreatedOn(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByCreatedOnDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastModifiedOn(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastModifiedOnDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByVersion(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByVersionDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByConsecutiveLoginFailures(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.ConsecutiveLoginFailures, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByConsecutiveLoginFailuresDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.ConsecutiveLoginFailures, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByCulture(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Culture, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByCultureDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Culture, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByDescription(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByDescriptionDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByDomain(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Domain, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByDomainDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Domain, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByEmail(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Email, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByEmailDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Email, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByFirstName(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.FirstName, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByFirstNameDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.FirstName, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByHashedPassword(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.HashedPassword, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByHashedPasswordDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.HashedPassword, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByIsEnabled(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.IsEnabled, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByIsEnabledDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.IsEnabled, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastLoginAttempt(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastLoginAttempt, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastLoginAttemptDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastLoginAttempt, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastName(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastName, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastNameDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastName, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastSuccessfulLogin(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastSuccessfulLogin, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByLastSuccessfulLoginDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.LastSuccessfulLogin, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByTimeZone(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.TimeZone, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByTimeZoneDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.TimeZone, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByUsername(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Username, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserQuery OrderByUsernameDescending(this UserQuery query)
            {
                var clause = new UserQuery.OrderClause(UserQuery.SortingProperties.Username, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Meta
    {
        using Gorba.Center.Common.ServiceModel.Meta;

                public static class SystemConfigQueryExtensions
        {

            public static SystemConfigQuery OrderByCreatedOn(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderByCreatedOnDescending(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderByLastModifiedOn(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderByLastModifiedOnDescending(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderByVersion(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderByVersionDescending(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderBySystemId(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.SystemId, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static SystemConfigQuery OrderBySystemIdDescending(this SystemConfigQuery query)
            {
                var clause = new SystemConfigQuery.OrderClause(SystemConfigQuery.SortingProperties.SystemId, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UserDefinedPropertyQueryExtensions
        {

            public static UserDefinedPropertyQuery OrderByCreatedOn(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByCreatedOnDescending(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByLastModifiedOn(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByLastModifiedOnDescending(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByVersion(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByVersionDescending(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByOwnerEntity(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.OwnerEntity, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByOwnerEntityDescending(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.OwnerEntity, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByName(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UserDefinedPropertyQuery OrderByNameDescending(this UserDefinedPropertyQuery query)
            {
                var clause = new UserDefinedPropertyQuery.OrderClause(UserDefinedPropertyQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Resources
    {
        using Gorba.Center.Common.ServiceModel.Resources;

                public static class ContentResourceQueryExtensions
        {

            public static ContentResourceQuery OrderByCreatedOn(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByCreatedOnDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByLastModifiedOn(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByLastModifiedOnDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByVersion(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByVersionDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByHashAlgorithmType(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.HashAlgorithmType, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByHashAlgorithmTypeDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.HashAlgorithmType, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByDescription(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByDescriptionDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByHash(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Hash, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByHashDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Hash, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByLength(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Length, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByLengthDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.Length, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByMimeType(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.MimeType, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByMimeTypeDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.MimeType, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByOriginalFilename(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.OriginalFilename, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByOriginalFilenameDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.OriginalFilename, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByThumbnailHash(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.ThumbnailHash, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ContentResourceQuery OrderByThumbnailHashDescending(this ContentResourceQuery query)
            {
                var clause = new ContentResourceQuery.OrderClause(ContentResourceQuery.SortingProperties.ThumbnailHash, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class ResourceQueryExtensions
        {

            public static ResourceQuery OrderByCreatedOn(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByCreatedOnDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByLastModifiedOn(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByLastModifiedOnDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByVersion(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByVersionDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByDescription(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByDescriptionDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByHash(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Hash, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByHashDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Hash, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByLength(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Length, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByLengthDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.Length, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByMimeType(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.MimeType, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByMimeTypeDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.MimeType, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByOriginalFilename(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.OriginalFilename, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByOriginalFilenameDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.OriginalFilename, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByThumbnailHash(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.ThumbnailHash, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ResourceQuery OrderByThumbnailHashDescending(this ResourceQuery query)
            {
                var clause = new ResourceQuery.OrderClause(ResourceQuery.SortingProperties.ThumbnailHash, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Software
    {
        using Gorba.Center.Common.ServiceModel.Software;

                public static class PackageQueryExtensions
        {

            public static PackageQuery OrderByCreatedOn(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByCreatedOnDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByLastModifiedOn(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByLastModifiedOnDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByVersion(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByVersionDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByDescription(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByDescriptionDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByPackageId(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.PackageId, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByPackageIdDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.PackageId, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByProductName(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.ProductName, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageQuery OrderByProductNameDescending(this PackageQuery query)
            {
                var clause = new PackageQuery.OrderClause(PackageQuery.SortingProperties.ProductName, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class PackageVersionQueryExtensions
        {

            public static PackageVersionQuery OrderByCreatedOn(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByCreatedOnDescending(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByLastModifiedOn(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByLastModifiedOnDescending(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByVersion(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByVersionDescending(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByDescription(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderByDescriptionDescending(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderBySoftwareVersion(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.SoftwareVersion, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static PackageVersionQuery OrderBySoftwareVersionDescending(this PackageVersionQuery query)
            {
                var clause = new PackageVersionQuery.OrderClause(PackageVersionQuery.SortingProperties.SoftwareVersion, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Units
    {
        using Gorba.Center.Common.ServiceModel.Units;

                public static class ProductTypeQueryExtensions
        {

            public static ProductTypeQuery OrderByCreatedOn(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByCreatedOnDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByLastModifiedOn(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByLastModifiedOnDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByVersion(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByVersionDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByUnitType(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.UnitType, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByUnitTypeDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.UnitType, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByDescription(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByDescriptionDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByName(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static ProductTypeQuery OrderByNameDescending(this ProductTypeQuery query)
            {
                var clause = new ProductTypeQuery.OrderClause(ProductTypeQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UnitQueryExtensions
        {

            public static UnitQuery OrderByCreatedOn(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByCreatedOnDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByLastModifiedOn(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByLastModifiedOnDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByVersion(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByVersionDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByDescription(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByDescriptionDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByIsConnected(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.IsConnected, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByIsConnectedDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.IsConnected, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByName(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByNameDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByNetworkAddress(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.NetworkAddress, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UnitQuery OrderByNetworkAddressDescending(this UnitQuery query)
            {
                var clause = new UnitQuery.OrderClause(UnitQuery.SortingProperties.NetworkAddress, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
    namespace Update
    {
        using Gorba.Center.Common.ServiceModel.Update;

                public static class UpdateCommandQueryExtensions
        {

            public static UpdateCommandQuery OrderByCreatedOn(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByCreatedOnDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByLastModifiedOn(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByLastModifiedOnDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByVersion(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByVersionDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByUpdateIndex(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.UpdateIndex, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByUpdateIndexDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.UpdateIndex, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByWasInstalled(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.WasInstalled, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByWasInstalledDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.WasInstalled, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByWasTransferred(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.WasTransferred, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateCommandQuery OrderByWasTransferredDescending(this UpdateCommandQuery query)
            {
                var clause = new UpdateCommandQuery.OrderClause(UpdateCommandQuery.SortingProperties.WasTransferred, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UpdateFeedbackQueryExtensions
        {

            public static UpdateFeedbackQuery OrderByCreatedOn(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByCreatedOnDescending(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByLastModifiedOn(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByLastModifiedOnDescending(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByVersion(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByVersionDescending(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByState(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.State, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByStateDescending(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.State, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByTimestamp(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.Timestamp, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateFeedbackQuery OrderByTimestampDescending(this UpdateFeedbackQuery query)
            {
                var clause = new UpdateFeedbackQuery.OrderClause(UpdateFeedbackQuery.SortingProperties.Timestamp, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UpdateGroupQueryExtensions
        {

            public static UpdateGroupQuery OrderByCreatedOn(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByCreatedOnDescending(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByLastModifiedOn(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByLastModifiedOnDescending(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByVersion(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByVersionDescending(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByDescription(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByDescriptionDescending(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByName(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Name, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdateGroupQuery OrderByNameDescending(this UpdateGroupQuery query)
            {
                var clause = new UpdateGroupQuery.OrderClause(UpdateGroupQuery.SortingProperties.Name, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

        public static class UpdatePartQueryExtensions
        {

            public static UpdatePartQuery OrderByCreatedOn(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.CreatedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByCreatedOnDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.CreatedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByLastModifiedOn(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.LastModifiedOn, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByLastModifiedOnDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.LastModifiedOn, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByVersion(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Version, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByVersionDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Version, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByType(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Type, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByTypeDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Type, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByDescription(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Description, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByDescriptionDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Description, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByEnd(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.End, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByEndDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.End, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByStart(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Start, SortDirection.Ascending);
                query.Sorting.Add(clause);
                return query;
            }

            public static UpdatePartQuery OrderByStartDescending(this UpdatePartQuery query)
            {
                var clause = new UpdatePartQuery.OrderClause(UpdatePartQuery.SortingProperties.Start, SortDirection.Descending);
                query.Sorting.Add(clause);
                return query;
            }
        }

    }
}