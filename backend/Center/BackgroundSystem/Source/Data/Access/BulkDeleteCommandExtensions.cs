// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkDeleteCommandExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BulkDeleteCommandExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Gorba.Center.Common.ServiceModel.Filters.Log;

    /// <summary>
    /// Extension methods to generate bulk delete commands.
    /// </summary>
    public static partial class BulkDeleteCommandExtensions
    {
        private static string CreateDeleteCommand<T>(this ObjectQuery<T> query, LogEntryFilter filter, string tableName)
        {
            var innerSelect = query.Where(filter);
            var s = innerSelect.ToTraceString();
            var sqlBuilder = new StringBuilder(s.Length * 2);
            sqlBuilder.AppendFormat("DELETE [{0}]\n", tableName);
            sqlBuilder.AppendFormat("FROM [{0}] as j0 INNER JOIN(\n", tableName);
            sqlBuilder.AppendLine(s);
            sqlBuilder.Append(") as j1 ON j0.Id = j1.Id");
            s = sqlBuilder.ToString();
            return s;
        }

        private static ObjectQuery<T> ToObjectQuery<T>(this IQueryable<T> query) where T : class
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var objectQuery = query as ObjectQuery<T>;
            if (objectQuery != null)
            {
                return objectQuery;
            }

            var databaseQuery = (DbQuery<T>)query;
            var internalQuery =
                databaseQuery.GetType()
                    .GetProperty("InternalQuery", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(databaseQuery);
            if (internalQuery == null)
            {
                throw new InvalidOperationException("Can't use the internal query");
            }

            objectQuery = internalQuery.GetType().GetProperty("ObjectQuery").GetValue(internalQuery) as ObjectQuery<T>;
            if (objectQuery == null)
            {
                throw new InvalidOperationException("Can't use the internal object query");
            }

            return objectQuery;
        }
    }
}
