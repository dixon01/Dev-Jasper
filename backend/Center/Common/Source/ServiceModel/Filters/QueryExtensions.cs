// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Filters
{
    /// <summary>
    /// Defines extension methods for <see cref="IQuery"/> objects.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Updates the Skip property.
        /// </summary>
        /// <typeparam name="T">
        /// The type of query.
        /// </typeparam>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="skip">
        /// The skip.
        /// </param>
        /// <returns>
        /// The updated <see cref="IQuery"/>.
        /// </returns>
        public static T Skip<T>(this T query, int skip)
            where T : IQuery
        {
            query.Skip = skip;
            return query;
        }

        /// <summary>
        /// Updates the Take property.
        /// </summary>
        /// <typeparam name="T">
        /// The type of query.
        /// </typeparam>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="take">
        /// The take.
        /// </param>
        /// <returns>
        /// The updated <see cref="IQuery"/>.
        /// </returns>
        public static T Take<T>(this T query, int? take)
            where T : IQuery
        {
            query.Take = take;
            return query;
        }
    }
}