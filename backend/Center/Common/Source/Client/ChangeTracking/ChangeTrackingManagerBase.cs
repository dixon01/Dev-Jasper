// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Defines a base class for change tracking managers.
    /// </summary>
    public abstract class ChangeTrackingManagerBase : IChangeTrackingManager, INotificationObserver
    {
        /// <summary>
        /// The <see cref="Logger"/> used for logging.
        /// </summary>
        protected readonly Logger Logger;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly AsyncLock asyncLocker = new AsyncLock();

        private readonly IList<INotificationSubscriber> additionalSubscribers = new List<INotificationSubscriber>();

        private readonly NotificationSubscriptionConfiguration configuration;

        private readonly Lazy<IReadyGate> readyGate;

        private volatile bool isRunning;

        private UserCredentials userCredentials;

        private TaskCompletionSource<bool> wait = new TaskCompletionSource<bool>();

        private TaskCompletionSource<bool> notificationCompletionSource;

        private INotificationSubscriber subscription;

        /// <summary>
        /// Gets the notification manager.
        /// </summary>
        private INotificationManager notificationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingManagerBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="userCredentials">The user credentials</param>
        /// <exception cref="ArgumentNullException">The configuration is null.</exception>
        protected ChangeTrackingManagerBase(
            NotificationSubscriptionConfiguration configuration, UserCredentials userCredentials)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = configuration;
            this.userCredentials = userCredentials;
            this.Logger = LogManager.GetCurrentClassLogger();
            var name = configuration.ToSubscriptionName();
            this.readyGate = new Lazy<IReadyGate>(() => this.CreateReadyGate(name));
        }

        /// <summary>
        /// Event raised when the instance is running.
        /// </summary>
        public event EventHandler Running;

        /// <summary>
        /// Gets the <see cref="IReadyGate"/>.
        /// </summary>
        public IReadyGate ReadyGate
        {
            get
            {
                return this.readyGate.Value;
            }
        }

        /// <summary>
        /// Stops the running manager.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public async Task CancelAsync()
        {
            var waitedTask = this.wait.Task;
            this.wait = new TaskCompletionSource<bool>();
            await waitedTask.ConfigureAwait(false);
            await waitedTask;
            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Asynchronously runs the manager.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task RunAsync()
        {
            if (this.isRunning)
            {
                return;
            }

            using (await this.asyncLocker.LockAsync())
            {
                if (this.isRunning)
                {
                    return;
                }

                this.isRunning = true;
            }

            this.notificationManager =
                NotificationManagerFactory.Current.Create(this.configuration.NotificationManagerConfiguration);
            this.subscription = await this.notificationManager.SubscribeAsync(this, this.configuration);
            await this.CreateAdditionalSubscriptions().ConfigureAwait(false);
            this.RaiseRunning();
            await this.cancellationTokenSource.Token.WaitHandle.AsTask().ConfigureAwait(false);
            this.notificationManager.Dispose();
        }

        /// <summary>
        /// Changes the credentials used by this change tracking manager to access the background system.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        public void ChangeCredentials(UserCredentials credentials)
        {
            this.userCredentials = credentials;
        }

        /// <summary>
        /// Tests the server asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public virtual async Task TestServerAsync()
        {
            this.Logger.Info("Testing manager {0}", this.GetType().Name);

            try
            {
                await this.readyGate.Value.PingPongAsync();
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error in PingPongAsync");
            }

            this.Logger.Info("Manager {0} tested", this.GetType().Name);
        }

        /// <summary>
        /// Waits until the change tracking mechanism is ready to be used.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public virtual async Task WaitReadyAsync()
        {
            await this.ReadyGate.WaitReadyAsync();
        }

        /// <summary>
        /// Posts a new notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        async Task INotificationObserver.OnNotificationAsync(Notification notification)
        {
            var pongNotification = notification as PongNotification;
            if (pongNotification != null)
            {
                await this.ReadyGate.PongAsync(pongNotification);
                return;
            }

            // TODO: replace with semaphore?
            var previousNotificationCompletionSource = this.notificationCompletionSource;
            this.notificationCompletionSource = new TaskCompletionSource<bool>();
            if (previousNotificationCompletionSource != null)
            {
                await previousNotificationCompletionSource.Task;
            }

            this.notificationCompletionSource.TrySetResult(true);
            try
            {
                await this.OnNotificationInternalAsync(notification);
            }
            catch (Exception exception)
            {
                var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                this.Logger.Warn(exceptionDispatchInfo.SourceException, "Ex");
            }
        }

        /// <summary>
        /// Raises the Running event.
        /// </summary>
        protected virtual void RaiseRunning()
        {
            var handler = this.Running;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error in the Running event handler");
            }
        }

        /// <summary>
        /// Creates the additional subscriptions.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task CreateAdditionalSubscriptions()
        {
            var tasks = this.GetAdditionalSubscriptions().Select(this.CreateAdditionalSubscription).ToList();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates the additional subscription for the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task CreateAdditionalSubscription(string path)
        {
            var notificationManagerConfiguration = new NotificationManagerConfiguration
                {
                    ConnectionString = this.configuration.NotificationManagerConfiguration.ConnectionString,
                    Path = path
                };
            var notificationSubscriptionConfiguration =
                new NotificationSubscriptionConfiguration(
                    notificationManagerConfiguration,
                    this.configuration.ApplicationName,
                    this.configuration.Name,
                    this.configuration.SessionId)
                    {
                        Filter =
                            new SqlNotificationManagerFilter(
                            "[sys].ReplyToSessionId IS NULL"),
                        IsUnique = true,
                        Timeout = Consts.SubscriptionTimeout
                    };
            var notificationSubscriber =
                await this.notificationManager.SubscribeAsync(this, notificationSubscriptionConfiguration);
            this.additionalSubscribers.Add(notificationSubscriber);
        }

        /// <summary>
        /// Gets the paths of the additional subscriptions this manager should subscribe to.
        /// </summary>
        /// <returns>
        /// The enumeration of additional subscriptions.
        /// </returns>
        protected virtual IEnumerable<string> GetAdditionalSubscriptions()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Handles internally a notification.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        protected abstract Task OnNotificationInternalAsync(Notification notification);

        /// <summary>
        /// Posts a notification.
        /// </summary>
        /// <param name="notification">The notification to post.</param>
        /// <returns>The identifier of the notification sent.</returns>
        protected virtual async Task<Guid> PostNotificationAsync(Notification notification)
        {
            if (!this.isRunning)
            {
                throw new Exception("Manager '" + this.GetType().Name + "' not started!");
            }

            notification.Id = Guid.NewGuid();
            notification.TimeToLive = TimeSpan.FromMinutes(1);
            notification.ReplyToSessionId = this.configuration.SessionId;
            this.Logger.Trace(
                "Marked notification '{0}' of type '{1}' with session Id '{2}'",
                notification.Id,
                notification.GetType(),
                notification.ReplyToSessionId);
            await this.notificationManager.PostAsync(notification);
            return notification.Id;
        }

        /// <summary>
        /// Gets the result of the last commit.
        /// </summary>
        /// <param name="result">The result.</param>
        protected void SetResult(bool result)
        {
            var w = this.wait;
            this.wait = new TaskCompletionSource<bool>();
            w.SetResult(result);
        }

        /// <summary>
        /// Creates a channel scope for the given type of service.
        /// </summary>
        /// <typeparam name="T">
        /// The type of service.
        /// </typeparam>
        /// <returns>
        /// A new channel scope.
        /// </returns>
        protected ChannelScope<T> CreateChannelScope<T>() where T : class
        {
            return ChannelScopeFactory<T>.Current.Create(this.userCredentials);
        }

        private IReadyGate CreateReadyGate(string name)
        {
            return ReadyGateFactory.Current.Create(name, this.PostNotificationAsync);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose unmanaged resources
            }

            this.isRunning = false;

            if (this.subscription == null)
            {
                return;
            }

            this.subscription.Dispose();
            this.additionalSubscribers.AsParallel().ForAll(subscriber => subscriber.Dispose());
            this.additionalSubscribers.Clear();
            this.cancellationTokenSource.Cancel();
        }
    }
}