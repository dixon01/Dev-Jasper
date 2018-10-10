// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;

    using NLog;

    /// <summary>
    /// Defines a base class for tracking services.
    /// </summary>
    public abstract class ChangeTrackingServiceBase : INotificationObserver
    {
        /// <summary>
        /// The <see cref="Logger"/> used for logging.
        /// </summary>
        protected readonly Logger Logger;

        private readonly Dictionary<string, INotificationManager> notificationManagers =
            new Dictionary<string, INotificationManager>();

        private readonly AsyncLock asyncLocker = new AsyncLock();

        private volatile bool isStarted;

        private INotificationSubscriber subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingServiceBase"/> class.
        /// </summary>
        /// <param name="backgroundSystemConfiguration">
        /// The background System Configuration.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        protected ChangeTrackingServiceBase(
            BackgroundSystemConfiguration backgroundSystemConfiguration,
            NotificationSubscriptionConfiguration configuration)
        {
            this.BackgroundSystemConfiguration = backgroundSystemConfiguration;
            this.Configuration = configuration;
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Event raised when the service is ready (both topic and subscription available).
        /// </summary>
        public event EventHandler ServiceReady;

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public NotificationSubscriptionConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets the background system configuration.
        /// </summary>
        protected BackgroundSystemConfiguration BackgroundSystemConfiguration { get; private set; }

        /// <summary>
        /// Gets the <see cref="INotificationManager"/> used by the service.
        /// </summary>
        protected INotificationManager NotificationManager { get; private set; }

        /// <summary>
        /// Starts the service asynchronously.
        /// </summary>
        public void Start()
        {
            if (this.isStarted)
            {
                return;
            }

            using (this.asyncLocker.LockAsync().ConfigureAwait(false).GetAwaiter().GetResult())
            {
                if (this.isStarted)
                {
                    return;
                }

                this.isStarted = true;
            }

            try
            {
                this.NotificationManager =
                    NotificationManagerFactory.Current.Create(this.Configuration.NotificationManagerConfiguration);
                this.subscription = this.NotificationManager.SubscribeAsync(this, this.Configuration).Result;
                this.GetReferenceNotificationManagers()
                    .ToList()
                    .ForEach(tuple => this.notificationManagers.Add(tuple.Item1, tuple.Item2));

                this.OnServiceReady();
            }
            catch
            {
                this.isStarted = false;
                throw;
            }
        }

        /// <summary>
        /// Starts the service asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task StartAsync()
        {
            if (this.isStarted)
            {
                return;
            }

            using (await this.asyncLocker.LockAsync())
            {
                if (this.isStarted)
                {
                    return;
                }

                this.isStarted = true;
            }

            try
            {
                this.NotificationManager =
                    NotificationManagerFactory.Current.Create(this.Configuration.NotificationManagerConfiguration);
                this.subscription = await this.NotificationManager.SubscribeAsync(this, this.Configuration);
                this.GetReferenceNotificationManagers()
                    .ToList()
                    .ForEach(tuple => this.notificationManagers.Add(tuple.Item1, tuple.Item2));

                this.OnServiceReady();
            }
            catch
            {
                this.isStarted = false;
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        async Task INotificationObserver.OnNotificationAsync(Notification notification)
        {
            var pingNotification = notification as PingNotification;
            if (pingNotification == null)
            {
                await this.OnNotification(notification);
                return;
            }

            this.Logger.Trace("Ping ({0})?", pingNotification.Id);
            var notificationId =
                await this.PostNotificationAsync(new PongNotification { ReplyTo = pingNotification.Id.ToString() });
            this.Logger.Trace("Pong ({0})!", notificationId);
        }

        /// <summary>
        /// Gets the notification managers for reference properties.
        /// </summary>
        /// <returns>
        /// The notification managers to add.
        /// </returns>
        protected abstract IEnumerable<Tuple<string, INotificationManager>> GetReferenceNotificationManagers();

        /// <summary>
        /// Handles a notification.
        /// </summary>
        /// <param name="value">The notification.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        protected abstract Task OnNotification(Notification value);

        /// <summary>
        /// Posts a notification.
        /// The notification is posted to the notification manager relative to the current service.
        /// </summary>
        /// <param name="notification">The notification to post.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        protected async Task<Guid> PostNotificationAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            await this.NotificationManager.PostAsync(notification);
            return notification.Id;
        }

        /// <summary>
        /// Raises the <see cref="ServiceReady"/> event.
        /// </summary>
        protected virtual void OnServiceReady()
        {
            var handler = this.ServiceReady;
            if (handler == null)
            {
                return;
            }

            handler(this, EventArgs.Empty);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose unmanaged resources
            }

            if (this.NotificationManager != null)
            {
                this.NotificationManager.Dispose();
            }

            foreach (var notificationManager in this.notificationManagers.Values)
            {
                notificationManager.Dispose();
            }

            if (this.subscription != null)
            {
                this.subscription.Dispose();
            }
        }
    }
}