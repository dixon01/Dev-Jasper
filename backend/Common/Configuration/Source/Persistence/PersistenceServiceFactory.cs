// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceServiceFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PersistenceServiceFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    /// <summary>
    /// Factory for <see cref="IPersistenceServiceImpl"/> objects.
    /// </summary>
    public static class PersistenceServiceFactory
    {
        /// <summary>
        /// Creates a new instance of the persistence service.
        /// This method should only be called once by the "bootstrapper".
        /// </summary>
        /// <returns>
        /// The <see cref="IPersistenceServiceImpl"/>.
        /// </returns>
        public static IPersistenceServiceImpl CreatePersistenceService()
        {
            return new PersistenceService();
        }
    }
}