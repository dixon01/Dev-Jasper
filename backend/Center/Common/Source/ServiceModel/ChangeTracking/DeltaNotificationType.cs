// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeltaNotificationType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeltaNotificationType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// The delta notification type.
    /// </summary>
    public enum DeltaNotificationType
    {
        /// <summary>
        /// Properties have changed.
        /// </summary>
        PropertiesChanged = 0,

        /// <summary>
        /// The entity was added.
        /// </summary>
        EntityAdded = 1,

        /// <summary>
        /// The entity was removed.
        /// </summary>
        EntityRemoved = 2
    }
}