// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationPersistence.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationPersistence type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Persistence
{
    /// <summary>
    /// Persistence object (XML serializable) to persist information about an application.
    /// </summary>
    public class ApplicationPersistence : PersistenceBase
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string Name { get; set; }
    }
}
