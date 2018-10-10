namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    using Gorba.Center.BackgroundSystem.Data;
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
    using AssociationTenantUserUserRole = Gorba.Center.BackgroundSystem.Data.Model.Membership.AssociationTenantUserUserRole;
    using Authorization = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.Authorization;
    using Document = Gorba.Center.BackgroundSystem.Data.Model.Documents.Document;
    using DocumentVersion = Gorba.Center.BackgroundSystem.Data.Model.Documents.DocumentVersion;
    using LogEntry = Gorba.Center.BackgroundSystem.Data.Model.Log.LogEntry;
    using MediaConfiguration = Gorba.Center.BackgroundSystem.Data.Model.Configurations.MediaConfiguration;
    using Package = Gorba.Center.BackgroundSystem.Data.Model.Software.Package;
    using PackageVersion = Gorba.Center.BackgroundSystem.Data.Model.Software.PackageVersion;
    using ProductType = Gorba.Center.BackgroundSystem.Data.Model.Units.ProductType;
    using Resource = Gorba.Center.BackgroundSystem.Data.Model.Resources.Resource;
    using SystemConfig = Gorba.Center.BackgroundSystem.Data.Model.Meta.SystemConfig;
    using Tenant = Gorba.Center.BackgroundSystem.Data.Model.Membership.Tenant;
    using Unit = Gorba.Center.BackgroundSystem.Data.Model.Units.Unit;
    using UnitConfiguration = Gorba.Center.BackgroundSystem.Data.Model.Configurations.UnitConfiguration;
    using UpdateCommand = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateCommand;
    using UpdateFeedback = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateFeedback;
    using UpdateGroup = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateGroup;
    using UpdatePart = Gorba.Center.BackgroundSystem.Data.Model.Update.UpdatePart;
    using User = Gorba.Center.BackgroundSystem.Data.Model.Membership.User;
    using UserDefinedProperty = Gorba.Center.BackgroundSystem.Data.Model.Meta.UserDefinedProperty;
    using UserRole = Gorba.Center.BackgroundSystem.Data.Model.AccessControl.UserRole;

    using NLog;

    using DynamicExpression = DynamicQuery.DynamicExpression;
    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    public static partial class WhereExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ObjectQuery<AssociationTenantUserUserRole> Where(this ObjectQuery<AssociationTenantUserUserRole> query, AssociationTenantUserUserRoleQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, AssociationTenantUserUserRoleFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Where(filter.Tenant, path + "Tenant");
            query = query.Where(filter.User, path + "User");
            query = query.Where(filter.UserRole, path + "UserRole");

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

        public static ObjectQuery<Authorization> Where(this ObjectQuery<Authorization> query, AuthorizationQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, AuthorizationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Where(filter.UserRole, path + "UserRole");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.AccessControl.Permission>(filter.Permission, prefix, path + "Permission");

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

        public static ObjectQuery<Document> Where(this ObjectQuery<Document> query, DocumentQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, DocumentFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.Tenant, path + "Tenant");
            query = query.Where(filter.Versions, path + "Versions");

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

        public static ObjectQuery<DocumentVersion> Where(this ObjectQuery<DocumentVersion> query, DocumentVersionQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, DocumentVersionFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Major, prefix, path + "Major");
            query = query.Where(filter.Minor, prefix, path + "Minor");

            if (filter.IncludeContent)
            {
                query = query.Include(path + "Content");
            }
            query = query.Where(filter.CreatingUser, path + "CreatingUser");
            query = query.Where(filter.Document, path + "Document");

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

        public static ObjectQuery<LogEntry> Where(this ObjectQuery<LogEntry> query, LogEntryQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, LogEntryFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.AdditionalData, prefix, path + "AdditionalData");
            query = query.Where(filter.Application, prefix, path + "Application");
            query = query.Where(filter.Logger, prefix, path + "Logger");
            query = query.Where(filter.Message, prefix, path + "Message");
            query = query.Where(filter.Timestamp, prefix, path + "Timestamp");
            query = query.Where(filter.Unit, path + "Unit");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.Log.Level>(filter.Level, prefix, path + "Level");

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

        public static ObjectQuery<MediaConfiguration> Where(this ObjectQuery<MediaConfiguration> query, MediaConfigurationQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, MediaConfigurationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Where(filter.Document, path + "Document");
            query = query.Where(filter.UpdateGroups, path + "UpdateGroups");

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

        public static ObjectQuery<Package> Where(this ObjectQuery<Package> query, PackageQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, PackageFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.PackageId, prefix, path + "PackageId");
            query = query.Where(filter.ProductName, prefix, path + "ProductName");
            query = query.Where(filter.Versions, path + "Versions");

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

        public static ObjectQuery<PackageVersion> Where(this ObjectQuery<PackageVersion> query, PackageVersionQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, PackageVersionFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.SoftwareVersion, prefix, path + "SoftwareVersion");

            if (filter.IncludeStructure)
            {
                query = query.Include(path + "Structure");
            }
            query = query.Where(filter.Package, path + "Package");

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

        public static ObjectQuery<ProductType> Where(this ObjectQuery<ProductType> query, ProductTypeQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, ProductTypeFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Name, prefix, path + "Name");

            if (filter.IncludeHardwareDescriptor)
            {
                query = query.Include(path + "HardwareDescriptor");
            }
            query = query.Where(filter.Units, path + "Units");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.Units.UnitTypes>(filter.UnitType, prefix, path + "UnitType");

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

        public static ObjectQuery<Resource> Where(this ObjectQuery<Resource> query, ResourceQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, ResourceFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Hash, prefix, path + "Hash");
            query = query.Where(filter.Length, prefix, path + "Length");
            query = query.Where(filter.MimeType, prefix, path + "MimeType");
            query = query.Where(filter.OriginalFilename, prefix, path + "OriginalFilename");
            query = query.Where(filter.ThumbnailHash, prefix, path + "ThumbnailHash");
            query = query.Where(filter.UploadingUser, path + "UploadingUser");

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

        public static ObjectQuery<SystemConfig> Where(this ObjectQuery<SystemConfig> query, SystemConfigQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, SystemConfigFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.SystemId, prefix, path + "SystemId");

            if (filter.IncludeSettings)
            {
                query = query.Include(path + "Settings");
            }

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

        public static ObjectQuery<Tenant> Where(this ObjectQuery<Tenant> query, TenantQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, TenantFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.UpdateGroups, path + "UpdateGroups");
            query = query.Where(filter.Users, path + "Users");

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

        public static ObjectQuery<Unit> Where(this ObjectQuery<Unit> query, UnitQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UnitFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.IsConnected, prefix, path + "IsConnected");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.NetworkAddress, prefix, path + "NetworkAddress");
            query = query.Where(filter.ProductType, path + "ProductType");
            query = query.Where(filter.Tenant, path + "Tenant");
            query = query.Where(filter.UpdateGroup, path + "UpdateGroup");
            query = query.Where(filter.UpdateCommands, path + "UpdateCommands");

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

        public static ObjectQuery<UnitConfiguration> Where(this ObjectQuery<UnitConfiguration> query, UnitConfigurationQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UnitConfigurationFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");

            query = query.Where(filter.Document, path + "Document");
            query = query.Where(filter.ProductType, path + "ProductType");
            query = query.Where(filter.UpdateGroups, path + "UpdateGroups");

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

        public static ObjectQuery<UpdateCommand> Where(this ObjectQuery<UpdateCommand> query, UpdateCommandQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UpdateCommandFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.UpdateIndex, prefix, path + "UpdateIndex");
            query = query.Where(filter.WasInstalled, prefix, path + "WasInstalled");
            query = query.Where(filter.WasTransferred, prefix, path + "WasTransferred");

            if (filter.IncludeCommand)
            {
                query = query.Include(path + "Command");
            }
            query = query.Where(filter.Unit, path + "Unit");
            query = query.Where(filter.Feedbacks, path + "Feedbacks");
            query = query.Where(filter.IncludedParts, path + "IncludedParts");

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

        public static ObjectQuery<UpdateFeedback> Where(this ObjectQuery<UpdateFeedback> query, UpdateFeedbackQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UpdateFeedbackFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Timestamp, prefix, path + "Timestamp");

            if (filter.IncludeFeedback)
            {
                query = query.Include(path + "Feedback");
            }
            query = query.Where(filter.UpdateCommand, path + "UpdateCommand");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.Update.UpdateState>(filter.State, prefix, path + "State");

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

        public static ObjectQuery<UpdateGroup> Where(this ObjectQuery<UpdateGroup> query, UpdateGroupQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UpdateGroupFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.MediaConfiguration, path + "MediaConfiguration");
            query = query.Where(filter.Tenant, path + "Tenant");
            query = query.Where(filter.UnitConfiguration, path + "UnitConfiguration");
            query = query.Where(filter.Units, path + "Units");
            query = query.Where(filter.UpdateParts, path + "UpdateParts");

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

        public static ObjectQuery<UpdatePart> Where(this ObjectQuery<UpdatePart> query, UpdatePartQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UpdatePartFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.End, prefix, path + "End");
            query = query.Where(filter.Start, prefix, path + "Start");

            if (filter.IncludeDynamicContent)
            {
                query = query.Include(path + "DynamicContent");
            }

            if (filter.IncludeInstallInstructions)
            {
                query = query.Include(path + "InstallInstructions");
            }

            if (filter.IncludeStructure)
            {
                query = query.Include(path + "Structure");
            }
            query = query.Where(filter.UpdateGroup, path + "UpdateGroup");
            query = query.Where(filter.RelatedCommands, path + "RelatedCommands");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.Update.UpdatePartType>(filter.Type, prefix, path + "Type");

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

        public static ObjectQuery<User> Where(this ObjectQuery<User> query, UserQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UserFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.ConsecutiveLoginFailures, prefix, path + "ConsecutiveLoginFailures");
            query = query.Where(filter.Culture, prefix, path + "Culture");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Domain, prefix, path + "Domain");
            query = query.Where(filter.Email, prefix, path + "Email");
            query = query.Where(filter.FirstName, prefix, path + "FirstName");
            query = query.Where(filter.HashedPassword, prefix, path + "HashedPassword");
            query = query.Where(filter.IsEnabled, prefix, path + "IsEnabled");
            query = query.Where(filter.LastLoginAttempt, prefix, path + "LastLoginAttempt");
            query = query.Where(filter.LastName, prefix, path + "LastName");
            query = query.Where(filter.LastSuccessfulLogin, prefix, path + "LastSuccessfulLogin");
            query = query.Where(filter.TimeZone, prefix, path + "TimeZone");
            query = query.Where(filter.Username, prefix, path + "Username");
            query = query.Where(filter.OwnerTenant, path + "OwnerTenant");
            query = query.Where(filter.AssociationTenantUserUserRoles, path + "AssociationTenantUserUserRoles");

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

        public static ObjectQuery<UserDefinedProperty> Where(this ObjectQuery<UserDefinedProperty> query, UserDefinedPropertyQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UserDefinedPropertyFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.Tenant, path + "Tenant");
            query = query.Where<T, Gorba.Center.BackgroundSystem.Data.Model.Meta.UserDefinedPropertyEnabledEntity>(filter.OwnerEntity, prefix, path + "OwnerEntity");

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

        public static ObjectQuery<UserRole> Where(this ObjectQuery<UserRole> query, UserRoleQuery filter)
        {
            if (filter == null)
            {
                return query;
            }

            query = query.Where(filter, null);

            if (filter.Sorting.Any())
            {
                //query = query.Sort(filter.Sorting);
            }
            
            if (filter.Skip > 0)
            {
                Logger.Trace("Skipping {0} item(s)", filter.Skip);
                //query = query.Skip(filter.Skip);
            }

            if (filter.Take.HasValue)
            {
                Logger.Trace("Taking {0} item(s)", filter.Take.Value);
                //query = query.Take(filter.Take.Value);
            }

            return query;
        }

        public static ObjectQuery<T> Where<T>(this ObjectQuery<T> query, UserRoleFilter filter, string path = null)
        {
            if (filter == null)
            {
                return query;
            }

            var prefix = string.Empty;
            if (path != null)
            {
                prefix = string.Format("{0} != null && ", path);
                query = query.Include(path);
                path += ".";
            }

            query = query.Where(filter.Id, prefix, path + "Id");
            query = query.Where(filter.CreatedOn, prefix, path + "CreatedOn");
            query = query.Where(filter.LastModifiedOn, prefix, path + "LastModifiedOn");
            query = query.Where(filter.Description, prefix, path + "Description");
            query = query.Where(filter.Name, prefix, path + "Name");
            query = query.Where(filter.Authorizations, path + "Authorizations");

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
