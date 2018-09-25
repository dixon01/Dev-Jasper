// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkRepository{T,TFilter}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBulkRepository&lt;T&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository that supports bulk operations.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TFilter">The type of the filter to apply.</typeparam>
    public interface IBulkRepository<T, in TFilter> : IRepository<T>
        where T : ICloneable
    {
        /// <summary>
        /// Adds a set of entities.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Deletes the items matching the given filter.
        /// </summary>
        /// <param name="filter">The filter to select the items to delete.</param>
        /// <returns>The number of deleted items.</returns>
        Task<int> DeleteAsync(TFilter filter);
    }
}