// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the INotificationObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Notifications
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// Defines a subscriber for notifications..
    /// </summary>
    public interface INotificationObserver : IDisposable
    {
        /// <summary>
        /// Posts a new notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        Task OnNotificationAsync(Notification notification);
    }
}