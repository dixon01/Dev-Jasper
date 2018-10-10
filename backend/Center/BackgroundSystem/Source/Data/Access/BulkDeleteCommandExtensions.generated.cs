namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using EntityFramework.BulkInsert.Extensions;

    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    
    public static partial class BulkDeleteCommandExtensions
    {
        public static string BuildBulkDeleteCommand(this CenterDataContext context, LogEntryFilter filter)
        {
            var query = context.LogEntries.ToObjectQuery();
            return query.CreateDeleteCommand(filter, "LogEntries");
        }
    }
}