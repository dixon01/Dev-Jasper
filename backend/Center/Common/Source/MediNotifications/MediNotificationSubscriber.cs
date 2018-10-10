// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediNotificationSubscriber.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediNotificationSubscriber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.MediNotifications
{
    using System;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Common.Medi.Core;

    using NLog;

    /// <summary>
    /// Notification subscriber using Medi.
    /// </summary>
    internal class MediNotificationSubscriber : INotificationSubscriber
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly INotificationObserver notificationObserver;

        private readonly IMessageDispatcher messageDispatcher;

        private readonly Predicate<Notification> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediNotificationSubscriber"/> class.
        /// </summary>
        /// <param name="notificationObserver">
        /// The notification observer for which the subscriber is created.
        /// </param>
        /// <param name="configuration">
        /// The subscription configuration.
        /// </param>
        /// <param name="messageDispatcher">
        /// The message dispatcher on which to subscribe.
        /// </param>
        public MediNotificationSubscriber(
            INotificationObserver notificationObserver,
            NotificationSubscriptionConfiguration configuration,
            IMessageDispatcher messageDispatcher)
        {
            this.notificationObserver = notificationObserver;
            this.messageDispatcher = messageDispatcher;
            var filterString = configuration.GetFilterString();
            if ("[sys].ReplyToSessionId IS NOT NULL".Equals(
                filterString,
                StringComparison.InvariantCultureIgnoreCase))
            {
                this.filter = n => n.ReplyToSessionId != null;
            }
            else if ("[sys].ReplyToSessionId IS NULL".Equals(
                filterString,
                StringComparison.InvariantCultureIgnoreCase))
            {
                this.filter = n => n.ReplyToSessionId == null;
            }
            else
            {
                this.filter = n => true;
            }

            this.messageDispatcher.Subscribe<NotificationMessage>(this.HandleNotificationMessage);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.messageDispatcher.Unsubscribe<NotificationMessage>(this.HandleNotificationMessage);
        }

        private async void HandleNotificationMessage(object sender, MessageEventArgs<NotificationMessage> e)
        {
            var notification = e.Message.Notification;
            if (!this.filter(notification))
            {
                return;
            }

            try
            {
                await this.notificationObserver.OnNotificationAsync(notification);
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't notify observer {0}", ex.Message);
            }
        }
    }
}