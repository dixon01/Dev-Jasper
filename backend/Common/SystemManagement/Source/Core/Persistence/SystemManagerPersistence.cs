// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerPersistence.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerPersistence type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Persistence
{
    /// <summary>
    /// The system manager persistence.
    /// </summary>
    public class SystemManagerPersistence : PersistenceBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether System Manager is running.
        /// This flag is used to know if System Manager has exited properly.
        /// When starting up, this flag should always be false, if not, we know
        /// that something went wrong the last time.
        /// </summary>
        public bool Running { get; set; }
    }
}