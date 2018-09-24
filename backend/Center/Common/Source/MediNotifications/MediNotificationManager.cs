// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediNotificationManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediNotificationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.MediNotifications
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Notification manager that uses Medi for subscriptions and sending messages.
    /// </summary>
    internal class MediNotificationManager : INotificationManager
    {
        private readonly IRootMessageDispatcher messageDispatcher;

        private readonly Task routingTableUpdated;

        private readonly MediAddress sendAddress;

        private readonly List<MediNotificationSubscriber> subscribers = new List<MediNotificationSubscriber>();

        private readonly List<IMessageDispatcher> namedDispatchers = new List<IMessageDispatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediNotificationManager"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher through which the messages are to be sent.
        /// </param>
        /// <param name="path">
        /// The path on which to post and subscribe.
        /// </param>
        /// <param name="routingTableUpdated">
        /// Task to be awaited to be sure that the routing table of the dispatcher was initialized.
        /// </param>
        public MediNotificationManager(IRootMessageDispatcher messageDispatcher, string path, Task routingTableUpdated)
        {
            this.messageDispatcher = messageDispatcher;
            this.routingTableUpdated = routingTableUpdated;
            this.sendAddress = new MediAddress("*", path);
        }

        /// <summary>
        /// Event that is risen when this notification manager is being disposed of.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Subscribes to notifications.
        /// </summary>
        /// <param name="notificationObserver">The observer of notifications.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A handle that can be disposed.</returns>
        public async Task<INotificationSubscriber> SubscribeAsync(
            INotificationObserver notificationObserver,
            NotificationSubscriptionConfiguration configuration)
        {
            await this.routingTableUpdated;
            var dispatcher =
                this.messageDispatcher.GetNamedDispatcher(configuration.NotificationManagerConfiguration.Path);
            this.namedDispatchers.Add(dispatcher);
            var subscriber = new MediNotificationSubscriber(notificationObserver, configuration, dispatcher);
            this.subscribers.Add(subscriber);

            return (INotificationSubscriber)subscriber;
        }

        /// <summary>
        /// Posts a notification.
        /// </summary>
        /// <param name="notification">The notification to post.</param>
        /// <returns>The identifier of the posted notification.</returns>
        public Task<Guid> PostAsync(Notification notification)
        {
            notification.EnqueuedTimeUtc = TimeProvider.Current.UtcNow;
            this.messageDispatcher.Send(this.sendAddress, new NotificationMessage(notification));
            return Task.FromResult(notification.Id);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            foreach (var subscriber in this.subscribers)
            {
                subscriber.Dispose();
            }

            this.subscribers.Clear();

            foreach (var namedDispatcher in this.namedDispatchers)
            {
                namedDispatcher.Dispose();
            }

            this.namedDispatchers.Clear();
        }
    }
}