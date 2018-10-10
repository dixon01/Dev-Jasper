// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationManagerFilter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManagerFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// Defines an object to describe a filter for notifications.
    /// </summary>
    public abstract class NotificationManagerFilter
    {
        /// <summary>
        /// Returns a string definition of the filter.
        /// </summary>
        /// <returns>
        /// The string definition of the filter.
        /// </returns>
        public abstract string ToStringFilter();
    }
}