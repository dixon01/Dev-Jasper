// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistenceServiceImpl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPersistenceServiceImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Persistence
{
    using System;

    /// <summary>
    /// Interface for the creator of the persistence service.
    /// This interface allows to interact with the persistence service
    /// on a global level which is not used by "normal" clients of the persistence service.
    /// </summary>
    public interface IPersistenceServiceImpl : IPersistenceService
    {
        /// <summary>
        /// Configures a new instance of <see cref="PersistenceService"/> class.
        /// </summary>
        /// <param name="configFileName">The file Name.</param>
        /// <param name="defaultValidityTime">The default Validity Time.</param>
        /// <param name="enabled">The enabled.</param>
        void Configure(string configFileName, TimeSpan defaultValidityTime, bool enabled);

        /// <summary>
        /// Save the persistence data to the disk.
        /// </summary>
        void Save();
    }
}