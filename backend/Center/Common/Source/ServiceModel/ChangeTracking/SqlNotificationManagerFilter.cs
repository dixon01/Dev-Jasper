// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlNotificationManagerFilter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SqlNotificationManagerFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// Defines a filter based on a sql string.
    /// </summary>
    public class SqlNotificationManagerFilter : NotificationManagerFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlNotificationManagerFilter"/> class.
        /// </summary>
        /// <param name="sqlFilter">
        /// The sql filter.
        /// </param>
        public SqlNotificationManagerFilter(string sqlFilter)
        {
            this.SqlFilter = sqlFilter;
        }

        /// <summary>
        /// Gets the sql filter.
        /// </summary>
        public string SqlFilter { get; private set; }

        /// <summary>
        /// Returns a string definition of the filter.
        /// </summary>
        /// <returns>
        /// The string definition of the filter.
        /// </returns>
        public override string ToStringFilter()
        {
            return this.SqlFilter;
        }
    }
}