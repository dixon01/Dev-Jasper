// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryableRepository.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines a readonly interface for a repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a readonly interface for a repository.
    /// </summary>
    /// <typeparam name="T">The type of items in the repository.</typeparam>
    public interface IQueryableRepository<T> : IDisposable
        where T : ICloneable
    {
        /// <summary>
        /// Find an item in the repository by its key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The item with the given key values, if found; otherwise, <c>null</c>.</returns>
        Task<T> FindAsync(params object[] keyValues);

        /// <summary>
        /// The query.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        IQueryable<T> Query();
    }
}