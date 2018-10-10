// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeltaOperation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeltaOperation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// Defines the type of the operation.
    /// </summary>
    public enum DeltaOperation
    {
        /// <summary>
        /// Update operation.
        /// </summary>
        Updated = 0,

        /// <summary>
        /// Create operation.
        /// </summary>
        Created = 1,

        /// <summary>
        /// Delete operation.
        /// </summary>
        Deleted = 2
    }
}