namespace Gorba.Center.Common.Wpf.Client.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.Filters;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Log;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Meta;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
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

    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    public static partial class QueryExtensions
    {

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, AssociationTenantUserUserRoleFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Apply(filter.Tenant, path + "Tenant");
            query = query.Apply(filter.User, path + "User");
            query = query.Apply(filter.UserRole, path + "UserRole");

            return query;
        }

        private static IQueryable<AssociationTenantUserUserRole> Sort(this IQueryable<AssociationTenantUserUserRole> query, IEnumerable<AssociationTenantUserUserRoleQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<AssociationTenantUserUserRole> Sort(this IQueryable<AssociationTenantUserUserRole> query, AssociationTenantUserUserRoleQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<AssociationTenantUserUserRole> Sort(this IOrderedQueryable<AssociationTenantUserUserRole> query, AssociationTenantUserUserRoleQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, AuthorizationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Apply(filter.UserRole, path + "UserRole");
            query = query.Apply(filter.DataScope, prefix, path + "DataScope");
            query = query.Apply(filter.Permission, prefix, path + "Permission");

            return query;
        }

        private static IQueryable<Authorization> Sort(this IQueryable<Authorization> query, IEnumerable<AuthorizationQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Authorization> Sort(this IQueryable<Authorization> query, AuthorizationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case AuthorizationQuery.SortingProperties.DataScope:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.DataScope);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.DataScope);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case AuthorizationQuery.SortingProperties.Permission:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Permission);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Permission);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Authorization> Sort(this IOrderedQueryable<Authorization> query, AuthorizationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case AuthorizationQuery.SortingProperties.DataScope:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.DataScope);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.DataScope);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case AuthorizationQuery.SortingProperties.Permission:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Permission);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Permission);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, ContentResourceFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Hash, prefix, path + "Hash");
            query = query.Apply(filter.Length, prefix, path + "Length");
            query = query.Apply(filter.MimeType, prefix, path + "MimeType");
            query = query.Apply(filter.OriginalFilename, prefix, path + "OriginalFilename");
            query = query.Apply(filter.ThumbnailHash, prefix, path + "ThumbnailHash");
            query = query.Apply(filter.UploadingUser, path + "UploadingUser");
            query = query.Apply(filter.HashAlgorithmType, prefix, path + "HashAlgorithmType");

            return query;
        }

        private static IQueryable<ContentResource> Sort(this IQueryable<ContentResource> query, IEnumerable<ContentResourceQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<ContentResource> Sort(this IQueryable<ContentResource> query, ContentResourceQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ContentResourceQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.Hash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Hash);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Hash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.Length:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Length);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Length);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.MimeType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.MimeType);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.MimeType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.OriginalFilename:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.OriginalFilename);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.OriginalFilename);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.ThumbnailHash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.ThumbnailHash);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.ThumbnailHash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.HashAlgorithmType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.HashAlgorithmType);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.HashAlgorithmType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<ContentResource> Sort(this IOrderedQueryable<ContentResource> query, ContentResourceQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ContentResourceQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.Hash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Hash);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Hash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.Length:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Length);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Length);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.MimeType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.MimeType);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.MimeType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.OriginalFilename:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.OriginalFilename);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.OriginalFilename);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.ThumbnailHash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.ThumbnailHash);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.ThumbnailHash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ContentResourceQuery.SortingProperties.HashAlgorithmType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.HashAlgorithmType);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.HashAlgorithmType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, DocumentFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.Tenant, path + "Tenant");
            query = query.Apply(filter.Versions, path + "Versions");

            return query;
        }

        private static IQueryable<Document> Sort(this IQueryable<Document> query, IEnumerable<DocumentQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Document> Sort(this IQueryable<Document> query, DocumentQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case DocumentQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Document> Sort(this IOrderedQueryable<Document> query, DocumentQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case DocumentQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, DocumentVersionFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Major, prefix, path + "Major");
            query = query.Apply(filter.Minor, prefix, path + "Minor");
            query = query.Apply(filter.CreatingUser, path + "CreatingUser");
            query = query.Apply(filter.Document, path + "Document");

            return query;
        }

        private static IQueryable<DocumentVersion> Sort(this IQueryable<DocumentVersion> query, IEnumerable<DocumentVersionQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<DocumentVersion> Sort(this IQueryable<DocumentVersion> query, DocumentVersionQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case DocumentVersionQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentVersionQuery.SortingProperties.Major:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Major);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Major);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentVersionQuery.SortingProperties.Minor:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Minor);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Minor);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<DocumentVersion> Sort(this IOrderedQueryable<DocumentVersion> query, DocumentVersionQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case DocumentVersionQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentVersionQuery.SortingProperties.Major:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Major);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Major);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case DocumentVersionQuery.SortingProperties.Minor:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Minor);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Minor);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, LogEntryFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.AdditionalData, prefix, path + "AdditionalData");
            query = query.Apply(filter.Application, prefix, path + "Application");
            query = query.Apply(filter.Logger, prefix, path + "Logger");
            query = query.Apply(filter.Message, prefix, path + "Message");
            query = query.Apply(filter.Timestamp, prefix, path + "Timestamp");
            query = query.Apply(filter.Unit, path + "Unit");
            query = query.Apply(filter.Level, prefix, path + "Level");

            return query;
        }

        private static IQueryable<LogEntry> Sort(this IQueryable<LogEntry> query, IEnumerable<LogEntryQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<LogEntry> Sort(this IQueryable<LogEntry> query, LogEntryQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case LogEntryQuery.SortingProperties.AdditionalData:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.AdditionalData);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.AdditionalData);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Application:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Application);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Application);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Logger:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Logger);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Logger);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Message:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Message);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Message);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Timestamp:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Timestamp);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Timestamp);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Level:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Level);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Level);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<LogEntry> Sort(this IOrderedQueryable<LogEntry> query, LogEntryQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case LogEntryQuery.SortingProperties.AdditionalData:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.AdditionalData);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.AdditionalData);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Application:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Application);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Application);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Logger:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Logger);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Logger);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Message:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Message);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Message);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Timestamp:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Timestamp);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Timestamp);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case LogEntryQuery.SortingProperties.Level:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Level);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Level);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, MediaConfigurationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Apply(filter.Document, path + "Document");
            query = query.Apply(filter.UpdateGroups, path + "UpdateGroups");

            return query;
        }

        private static IQueryable<MediaConfiguration> Sort(this IQueryable<MediaConfiguration> query, IEnumerable<MediaConfigurationQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<MediaConfiguration> Sort(this IQueryable<MediaConfiguration> query, MediaConfigurationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<MediaConfiguration> Sort(this IOrderedQueryable<MediaConfiguration> query, MediaConfigurationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, PackageFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.PackageId, prefix, path + "PackageId");
            query = query.Apply(filter.ProductName, prefix, path + "ProductName");
            query = query.Apply(filter.Versions, path + "Versions");

            return query;
        }

        private static IQueryable<Package> Sort(this IQueryable<Package> query, IEnumerable<PackageQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Package> Sort(this IQueryable<Package> query, PackageQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case PackageQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageQuery.SortingProperties.PackageId:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.PackageId);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.PackageId);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageQuery.SortingProperties.ProductName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.ProductName);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.ProductName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Package> Sort(this IOrderedQueryable<Package> query, PackageQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case PackageQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageQuery.SortingProperties.PackageId:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.PackageId);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.PackageId);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageQuery.SortingProperties.ProductName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.ProductName);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.ProductName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, PackageVersionFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.SoftwareVersion, prefix, path + "SoftwareVersion");
            query = query.Apply(filter.Package, path + "Package");

            return query;
        }

        private static IQueryable<PackageVersion> Sort(this IQueryable<PackageVersion> query, IEnumerable<PackageVersionQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<PackageVersion> Sort(this IQueryable<PackageVersion> query, PackageVersionQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case PackageVersionQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageVersionQuery.SortingProperties.SoftwareVersion:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.SoftwareVersion);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.SoftwareVersion);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<PackageVersion> Sort(this IOrderedQueryable<PackageVersion> query, PackageVersionQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case PackageVersionQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case PackageVersionQuery.SortingProperties.SoftwareVersion:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.SoftwareVersion);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.SoftwareVersion);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, ProductTypeFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.Units, path + "Units");
            query = query.Apply(filter.UnitType, prefix, path + "UnitType");

            return query;
        }

        private static IQueryable<ProductType> Sort(this IQueryable<ProductType> query, IEnumerable<ProductTypeQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<ProductType> Sort(this IQueryable<ProductType> query, ProductTypeQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ProductTypeQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ProductTypeQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ProductTypeQuery.SortingProperties.UnitType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.UnitType);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.UnitType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<ProductType> Sort(this IOrderedQueryable<ProductType> query, ProductTypeQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ProductTypeQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ProductTypeQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ProductTypeQuery.SortingProperties.UnitType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.UnitType);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.UnitType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, ResourceFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Hash, prefix, path + "Hash");
            query = query.Apply(filter.Length, prefix, path + "Length");
            query = query.Apply(filter.MimeType, prefix, path + "MimeType");
            query = query.Apply(filter.OriginalFilename, prefix, path + "OriginalFilename");
            query = query.Apply(filter.ThumbnailHash, prefix, path + "ThumbnailHash");
            query = query.Apply(filter.UploadingUser, path + "UploadingUser");

            return query;
        }

        private static IQueryable<Resource> Sort(this IQueryable<Resource> query, IEnumerable<ResourceQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Resource> Sort(this IQueryable<Resource> query, ResourceQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ResourceQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.Hash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Hash);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Hash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.Length:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Length);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Length);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.MimeType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.MimeType);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.MimeType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.OriginalFilename:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.OriginalFilename);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.OriginalFilename);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.ThumbnailHash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.ThumbnailHash);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.ThumbnailHash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Resource> Sort(this IOrderedQueryable<Resource> query, ResourceQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case ResourceQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.Hash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Hash);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Hash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.Length:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Length);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Length);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.MimeType:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.MimeType);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.MimeType);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.OriginalFilename:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.OriginalFilename);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.OriginalFilename);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ResourceQuery.SortingProperties.ThumbnailHash:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.ThumbnailHash);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.ThumbnailHash);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, SystemConfigFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.SystemId, prefix, path + "SystemId");

            return query;
        }

        private static IQueryable<SystemConfig> Sort(this IQueryable<SystemConfig> query, IEnumerable<SystemConfigQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<SystemConfig> Sort(this IQueryable<SystemConfig> query, SystemConfigQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case SystemConfigQuery.SortingProperties.SystemId:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.SystemId);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.SystemId);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<SystemConfig> Sort(this IOrderedQueryable<SystemConfig> query, SystemConfigQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case SystemConfigQuery.SortingProperties.SystemId:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.SystemId);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.SystemId);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, TenantFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.UpdateGroups, path + "UpdateGroups");
            query = query.Apply(filter.Users, path + "Users");

            return query;
        }

        private static IQueryable<Tenant> Sort(this IQueryable<Tenant> query, IEnumerable<TenantQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Tenant> Sort(this IQueryable<Tenant> query, TenantQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case TenantQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case TenantQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Tenant> Sort(this IOrderedQueryable<Tenant> query, TenantQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case TenantQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case TenantQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UnitFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.IsConnected, prefix, path + "IsConnected");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.NetworkAddress, prefix, path + "NetworkAddress");
            query = query.Apply(filter.ProductType, path + "ProductType");
            query = query.Apply(filter.Tenant, path + "Tenant");
            query = query.Apply(filter.UpdateGroup, path + "UpdateGroup");
            query = query.Apply(filter.UpdateCommands, path + "UpdateCommands");

            return query;
        }

        private static IQueryable<Unit> Sort(this IQueryable<Unit> query, IEnumerable<UnitQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<Unit> Sort(this IQueryable<Unit> query, UnitQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UnitQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.IsConnected:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.IsConnected);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.IsConnected);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.NetworkAddress:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.NetworkAddress);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.NetworkAddress);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<Unit> Sort(this IOrderedQueryable<Unit> query, UnitQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UnitQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.IsConnected:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.IsConnected);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.IsConnected);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UnitQuery.SortingProperties.NetworkAddress:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.NetworkAddress);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.NetworkAddress);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UnitConfigurationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Apply(filter.Document, path + "Document");
            query = query.Apply(filter.ProductType, path + "ProductType");
            query = query.Apply(filter.UpdateGroups, path + "UpdateGroups");

            return query;
        }

        private static IQueryable<UnitConfiguration> Sort(this IQueryable<UnitConfiguration> query, IEnumerable<UnitConfigurationQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UnitConfiguration> Sort(this IQueryable<UnitConfiguration> query, UnitConfigurationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UnitConfiguration> Sort(this IOrderedQueryable<UnitConfiguration> query, UnitConfigurationQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UpdateCommandFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.UpdateIndex, prefix, path + "UpdateIndex");
            query = query.Apply(filter.WasInstalled, prefix, path + "WasInstalled");
            query = query.Apply(filter.WasTransferred, prefix, path + "WasTransferred");
            query = query.Apply(filter.Unit, path + "Unit");
            query = query.Apply(filter.Feedbacks, path + "Feedbacks");
            query = query.Apply(filter.IncludedParts, path + "IncludedParts");

            return query;
        }

        private static IQueryable<UpdateCommand> Sort(this IQueryable<UpdateCommand> query, IEnumerable<UpdateCommandQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UpdateCommand> Sort(this IQueryable<UpdateCommand> query, UpdateCommandQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateCommandQuery.SortingProperties.UpdateIndex:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.UpdateIndex);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.UpdateIndex);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateCommandQuery.SortingProperties.WasInstalled:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.WasInstalled);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.WasInstalled);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateCommandQuery.SortingProperties.WasTransferred:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.WasTransferred);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.WasTransferred);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UpdateCommand> Sort(this IOrderedQueryable<UpdateCommand> query, UpdateCommandQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateCommandQuery.SortingProperties.UpdateIndex:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.UpdateIndex);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.UpdateIndex);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateCommandQuery.SortingProperties.WasInstalled:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.WasInstalled);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.WasInstalled);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateCommandQuery.SortingProperties.WasTransferred:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.WasTransferred);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.WasTransferred);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UpdateFeedbackFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Timestamp, prefix, path + "Timestamp");
            query = query.Apply(filter.UpdateCommand, path + "UpdateCommand");
            query = query.Apply(filter.State, prefix, path + "State");

            return query;
        }

        private static IQueryable<UpdateFeedback> Sort(this IQueryable<UpdateFeedback> query, IEnumerable<UpdateFeedbackQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UpdateFeedback> Sort(this IQueryable<UpdateFeedback> query, UpdateFeedbackQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateFeedbackQuery.SortingProperties.Timestamp:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Timestamp);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Timestamp);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateFeedbackQuery.SortingProperties.State:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.State);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.State);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UpdateFeedback> Sort(this IOrderedQueryable<UpdateFeedback> query, UpdateFeedbackQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateFeedbackQuery.SortingProperties.Timestamp:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Timestamp);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Timestamp);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateFeedbackQuery.SortingProperties.State:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.State);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.State);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UpdateGroupFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.MediaConfiguration, path + "MediaConfiguration");
            query = query.Apply(filter.Tenant, path + "Tenant");
            query = query.Apply(filter.UnitConfiguration, path + "UnitConfiguration");
            query = query.Apply(filter.Units, path + "Units");
            query = query.Apply(filter.UpdateParts, path + "UpdateParts");

            return query;
        }

        private static IQueryable<UpdateGroup> Sort(this IQueryable<UpdateGroup> query, IEnumerable<UpdateGroupQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UpdateGroup> Sort(this IQueryable<UpdateGroup> query, UpdateGroupQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateGroupQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateGroupQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UpdateGroup> Sort(this IOrderedQueryable<UpdateGroup> query, UpdateGroupQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdateGroupQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdateGroupQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UpdatePartFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.End, prefix, path + "End");
            query = query.Apply(filter.Start, prefix, path + "Start");
            query = query.Apply(filter.UpdateGroup, path + "UpdateGroup");
            query = query.Apply(filter.RelatedCommands, path + "RelatedCommands");
            query = query.Apply(filter.Type, prefix, path + "Type");

            return query;
        }

        private static IQueryable<UpdatePart> Sort(this IQueryable<UpdatePart> query, IEnumerable<UpdatePartQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UpdatePart> Sort(this IQueryable<UpdatePart> query, UpdatePartQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdatePartQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.End:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.End);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.End);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.Start:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Start);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Start);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.Type:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Type);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Type);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UpdatePart> Sort(this IOrderedQueryable<UpdatePart> query, UpdatePartQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UpdatePartQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.End:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.End);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.End);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.Start:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Start);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Start);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UpdatePartQuery.SortingProperties.Type:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Type);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Type);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UserFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.ConsecutiveLoginFailures, prefix, path + "ConsecutiveLoginFailures");
            query = query.Apply(filter.Culture, prefix, path + "Culture");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Domain, prefix, path + "Domain");
            query = query.Apply(filter.Email, prefix, path + "Email");
            query = query.Apply(filter.FirstName, prefix, path + "FirstName");
            query = query.Apply(filter.HashedPassword, prefix, path + "HashedPassword");
            query = query.Apply(filter.IsEnabled, prefix, path + "IsEnabled");
            query = query.Apply(filter.LastLoginAttempt, prefix, path + "LastLoginAttempt");
            query = query.Apply(filter.LastName, prefix, path + "LastName");
            query = query.Apply(filter.LastSuccessfulLogin, prefix, path + "LastSuccessfulLogin");
            query = query.Apply(filter.TimeZone, prefix, path + "TimeZone");
            query = query.Apply(filter.Username, prefix, path + "Username");
            query = query.Apply(filter.OwnerTenant, path + "OwnerTenant");
            query = query.Apply(filter.AssociationTenantUserUserRoles, path + "AssociationTenantUserUserRoles");

            return query;
        }

        private static IQueryable<User> Sort(this IQueryable<User> query, IEnumerable<UserQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<User> Sort(this IQueryable<User> query, UserQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserQuery.SortingProperties.ConsecutiveLoginFailures:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.ConsecutiveLoginFailures);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.ConsecutiveLoginFailures);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Culture:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Culture);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Culture);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Domain:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Domain);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Domain);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Email:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Email);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Email);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.FirstName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.FirstName);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.FirstName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.HashedPassword:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.HashedPassword);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.HashedPassword);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.IsEnabled:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.IsEnabled);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.IsEnabled);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastLoginAttempt:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.LastLoginAttempt);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.LastLoginAttempt);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.LastName);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.LastName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastSuccessfulLogin:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.LastSuccessfulLogin);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.LastSuccessfulLogin);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.TimeZone:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.TimeZone);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.TimeZone);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Username:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Username);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Username);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<User> Sort(this IOrderedQueryable<User> query, UserQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserQuery.SortingProperties.ConsecutiveLoginFailures:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.ConsecutiveLoginFailures);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.ConsecutiveLoginFailures);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Culture:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Culture);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Culture);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Domain:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Domain);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Domain);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Email:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Email);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Email);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.FirstName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.FirstName);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.FirstName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.HashedPassword:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.HashedPassword);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.HashedPassword);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.IsEnabled:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.IsEnabled);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.IsEnabled);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastLoginAttempt:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.LastLoginAttempt);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.LastLoginAttempt);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastName:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.LastName);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.LastName);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.LastSuccessfulLogin:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.LastSuccessfulLogin);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.LastSuccessfulLogin);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.TimeZone:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.TimeZone);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.TimeZone);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserQuery.SortingProperties.Username:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Username);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Username);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UserDefinedPropertyFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.Tenant, path + "Tenant");
            query = query.Apply(filter.OwnerEntity, prefix, path + "OwnerEntity");

            return query;
        }

        private static IQueryable<UserDefinedProperty> Sort(this IQueryable<UserDefinedProperty> query, IEnumerable<UserDefinedPropertyQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UserDefinedProperty> Sort(this IQueryable<UserDefinedProperty> query, UserDefinedPropertyQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserDefinedPropertyQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserDefinedPropertyQuery.SortingProperties.OwnerEntity:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.OwnerEntity);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.OwnerEntity);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UserDefinedProperty> Sort(this IOrderedQueryable<UserDefinedProperty> query, UserDefinedPropertyQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserDefinedPropertyQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserDefinedPropertyQuery.SortingProperties.OwnerEntity:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.OwnerEntity);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.OwnerEntity);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> query, UserRoleFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                path += ".";
            }

            query = query.Apply(filter.Id, prefix, path + "Id");
            query = query.Apply(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Apply(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Apply(filter.Description, prefix, path + "Description");
            query = query.Apply(filter.Name, prefix, path + "Name");
            query = query.Apply(filter.Authorizations, path + "Authorizations");

            return query;
        }

        private static IQueryable<UserRole> Sort(this IQueryable<UserRole> query, IEnumerable<UserRoleQuery.OrderClause> sorting)
        {
            var orderedQuery = query.Sort(sorting.First());
            var others = sorting.Skip(1);
            if (others.Any())
            {
                foreach (var orderClause in others)
                {
                    orderedQuery = orderedQuery.Sort(orderClause);
                }
            }

            return orderedQuery;
        }

        private static IOrderedQueryable<UserRole> Sort(this IQueryable<UserRole> query, UserRoleQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserRoleQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserRoleQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.OrderBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.OrderByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IOrderedQueryable<UserRole> Sort(this IOrderedQueryable<UserRole> query, UserRoleQuery.OrderClause clause)
        {
            switch (clause.Property)
            {
                case UserRoleQuery.SortingProperties.Description:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Description);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Description);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case UserRoleQuery.SortingProperties.Name:
                    switch (clause.Direction)
                    {
                        case SortDirection.Ascending:
                            return query.ThenBy(entity => entity.Name);
                        case SortDirection.Descending:
                            return query.ThenByDescending(entity => entity.Name);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
