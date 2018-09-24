// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationManager{TObserver}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Notifications
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// Defines a notification manager.
    /// </summary>
    /// <typeparam name="TObserver">The observer of notifications.</typeparam>
    public interface INotificationManager<TObserver>
        where TObserver : INotificationObserver
    {
        /// <summary>
        /// Subscribes to notifications.
        /// </summary>
        /// <returns>The observer.</returns>
        Task<TObserver> SubscribeAsync();

        /// <summary>
        /// Posts a notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        Task SendAsync(Notification notification);
    }
}