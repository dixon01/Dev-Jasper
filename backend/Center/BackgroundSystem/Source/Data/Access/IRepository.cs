// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The Repository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The Repository interface.
    /// </summary>
    /// <typeparam name="T">The type of entities in the repository.</typeparam>
    public interface IRepository<T> : IQueryableRepository<T>
        where T : ICloneable
    {
        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The added entity.
        /// </returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Removed an item from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>A task that can be awaited.</returns>
        Task RemoveAsync(T entity);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <returns>The updated entity.</returns>
        /// <remarks>
        /// Only the primitive properties of the entity are updated.
        /// </remarks>
        Task<T> UpdateAsync(T entity);
    }
}