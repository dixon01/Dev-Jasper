// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationSubscriber.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationSubscriber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel;
    using Gorba.Center.Common.Client.ChangeTracking;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;

    /// <summary>
    /// Handles subscriptions.
    /// </summary>
    public partial class NotificationSubscriber : IDisposable
    {
        private readonly List<Tuple<INotificationObserver, INotificationSubscriber>> notificationManagers =
            new List<Tuple<INotificationObserver, INotificationSubscriber>>();

        /// <summary>
        /// Creates the subscriptions subscriptions.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CreateSubscriptions(CancellationToken token)
        {
            var portal = ConfigurationManager.AppSettings["CenterPortal"];
            var backgroundSystemConfiguration = BackgroundSystemConfigurationProvider.Current.GetConfiguration(portal);
            await this.CreateSubscription(backgroundSystemConfiguration, "NotificationManager", true);
            var tasks = this.GetPaths()
                .Select(s => this.CreateSubscription(backgroundSystemConfiguration, s)).ToArray();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        private async Task CreateSubscription(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            string path,
            bool skipReadyGate = false)
        {
            var configuration = new NotificationManagerConfiguration
                                    {
                                        ConnectionString =
                                            backgroundSystemConfiguration
                                            .ServiceBusConnectionString,
                                        Path = path
                                    };
            var notificationManager = NotificationManagerFactory.Current.Create(configuration);
            var c = new NotificationSubscriptionConfiguration(configuration, "NotificationObserver", "Observer", "LEF");
            var observer = new NotificationObserver(this, path);
            var notificationSubscriber = await notificationManager.SubscribeAsync(observer, c);
            if (!skipReadyGate)
            {
                Func<Notification, Task<Guid>> sendNotification = async notification =>
                    {
                        notification.Id = Guid.NewGuid();
                        notification.ReplyToSessionId = c.SessionId;
                        await notificationManager.PostAsync(notification);
                        return notification.Id;
                    };
                var readyGate = ReadyGateFactory.Current.Create(path, sendNotification);
                EventHandler<PongEventArgs> observerOnPong = (sender, e) => readyGate.PongAsync(e.Pong);
                observer.Pong += observerOnPong;
                await readyGate.PingPongAsync();
                observer.Pong -= observerOnPong;
            }

            var shell = DependencyResolver.Current.Get<Shell>();
            shell.Status = string.Format("Created subscription '{0}'", path);
            this.notificationManagers.Add(
                new Tuple<INotificationObserver, INotificationSubscriber>(observer, notificationSubscriber));
        }

        private void OnNotificationInfo(NotificationInfo notificationInfo)
        {
            var shell = DependencyResolver.Current.Get<Shell>();
            shell.AddNotification(notificationInfo);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose unmanaged resources here
            }

            this.notificationManagers.Select(tuple => tuple.Item2).ToList().ForEach(subscriber => subscriber.Dispose());
        }

        private sealed class NotificationObserver : INotificationObserver
        {
            private readonly NotificationSubscriber notificationSubscriber;

            private readonly string path;

            public NotificationObserver(NotificationSubscriber notificationSubscriber, string path)
            {
                this.notificationSubscriber = notificationSubscriber;
                this.path = path;
            }

            public event EventHandler<PongEventArgs> Pong;

            // ReSharper disable once CSharpWarnings::CS1998
            public async Task OnNotificationAsync(Notification notification)
            {
                var notificationInfo = new NotificationInfo(this.path, notification.Id, notification)
                                           {
                                               ReplyTo =
                                                   notification
                                                   .ReplyTo,
                                               ReplyToSessionId
                                                   =
                                                   notification
                                                   .ReplyToSessionId
                                           };
                this.notificationSubscriber.OnNotificationInfo(notificationInfo);
                var pongNotification = notification as PongNotification;
                if (pongNotification == null)
                {
                    return;
                }

                this.RaisePong(pongNotification);
            }

            public void Dispose()
            {
            }

            private void RaisePong(PongNotification notification)
            {
                this.RaisePong(new PongEventArgs(notification));
            }

            private void RaisePong(PongEventArgs e)
            {
                var handler = this.Pong;
                if (handler == null)
                {
                    return;
                }

                handler(this, e);
            }
        }

        private class PongEventArgs : EventArgs
        {
            public PongEventArgs(PongNotification pong)
            {
                this.Pong = pong;
            }

            public PongNotification Pong { get; private set; }
        }
    }
}
