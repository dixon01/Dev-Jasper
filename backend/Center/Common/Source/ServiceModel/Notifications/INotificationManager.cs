// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the INotificationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Notifications
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// Defines the notification manager.
    /// </summary>
    public interface INotificationManager : IDisposable
    {
        /// <summary>
        /// Subscribes to notifications.
        /// </summary>
        /// <param name="notificationObserver">The observer of notifications.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A handle that can be disposed.</returns>
        Task<INotificationSubscriber> SubscribeAsync(
            INotificationObserver notificationObserver,
            NotificationSubscriptionConfiguration configuration);

        /// <summary>
        /// Posts a notification.
        /// </summary>
        /// <param name="notification">The notification to post.</param>
        /// <returns>The identifier of the posted notification.</returns>
        Task<Guid> PostAsync(Notification notification);
    }
}