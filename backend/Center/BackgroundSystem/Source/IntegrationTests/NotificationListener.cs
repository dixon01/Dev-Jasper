// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationListener.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The test notification listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// The test notification listener.
    /// </summary>
    public class NotificationListener : INotificationObserver
    {
        private readonly IntegrationTestContext context;

        private readonly BackgroundSystemConfiguration configuration;

        private readonly List<ExpectationBase> expectations;

        private readonly List<Notification> receivedNotifications;

        private readonly Dictionary<string, INotificationSubscriber> subscriptions;

        private NotificationSubscriptionConfiguration notificationSubscriptionConfiguration;
        private INotificationManager notificationManager;

        private bool isStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationListener"/> class.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        /// <param name="configuration">
        /// The BackgroundSystem configuration
        /// </param>
        public NotificationListener(IntegrationTestContext testContext, BackgroundSystemConfiguration configuration)
        {
            this.context = testContext;
            this.configuration = configuration;
            this.receivedNotifications = new List<Notification>();
            this.expectations = new List<ExpectationBase>();
            this.subscriptions = new Dictionary<string, INotificationSubscriber>();
        }

        /// <summary>
        /// Gets the received notifications.
        /// </summary>
        public IReadOnlyCollection<Notification> ReceivedNotifications
        {
            get
            {
                return this.receivedNotifications.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include pong notifications to test.
        /// </summary>
        public bool IncludePongNotifications { get; set; }

        /// <summary>
        /// Adds an expectation.
        /// </summary>
        /// <param name="expectation">
        /// The expectation.
        /// </param>
        public void ExpectNotification(ExpectationBase expectation)
        {
            this.expectations.Add(expectation);
        }

        /// <summary>
        /// Clears the list of expected notifications.
        /// </summary>
        public void ClearExpectations()
        {
            this.expectations.Clear();
        }

        /// <summary>
        /// Clears the list of received notifications.
        /// </summary>
        public void ClearReceivedNotifications()
        {
           this.receivedNotifications.Clear();
        }

        /// <summary>
        /// Adds a subscription for the given path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public void Subscribe(string path)
        {
            if (this.subscriptions.ContainsKey(path))
            {
                return;
            }

            this.CreateAdditionalSubscriptionAsync(path).Wait();
        }

        /// <summary>
        /// Adds subscriptions for the paths.
        /// </summary>
        /// <param name="paths">
        /// The paths.
        /// </param>
        public void Subscribe(IEnumerable<string> paths)
        {
            paths.ToList().ForEach(this.Subscribe);
        }

        /// <summary>
        /// Verifies if all expected notifications have been received and adds a fail message to the test context
        /// for all remaining notifications.
        /// </summary>
        /// <returns>
        /// The list of expectations.
        /// </returns>
        public IEnumerable<ExpectationBase> VerifyAllExpectationsReceived()
        {
            if (!this.expectations.Any())
            {
                return this.expectations;
            }

            foreach (var expectation in this.expectations)
            {
                var message = string.Format(
                    "Expectation with Id {0} was expected, but never received.",
                    expectation.ExpectationId);
                this.context.Fail(message);
            }

            return this.expectations;
        }

        /// <summary>
        /// Creates the notification manager used to subscribe.
        /// </summary>
        /// <returns>The <see cref="Task"/> that can be awaited.</returns>
        public async Task StartAsync()
        {
            if (this.isStarted)
            {
                return;
            }

            var notificationConfiguration = new NotificationManagerConfiguration
                                                {
                                                    ConnectionString =
                                                        this.configuration
                                                        .NotificationsConnectionString,
                                                    Path = "Units"
                                                };
            var subscriptionConfiguration = new NotificationSubscriptionConfiguration(
                notificationConfiguration,
                "NotificationListener",
                "Unit",
                NotificationManagerUtility.GenerateUniqueSessionId())
                                                {
                                                    Filter =
                                                        new SqlNotificationManagerFilter(
                                                        "[sys].ReplyToSessionId IS NULL"),
                                                    Timeout = TimeSpan.FromMinutes(30)
                                                };
            this.notificationSubscriptionConfiguration = subscriptionConfiguration;
            this.notificationManager = NotificationManagerFactory.Current.Create(notificationConfiguration);
            this.subscriptions["Units"] =
                    await this.notificationManager.SubscribeAsync(this, subscriptionConfiguration);
            await this.CreateAdditionalSubscriptionAsync("UnitGroups");
            await this.CreateAdditionalSubscriptionAsync("Tenants");
            await this.CreateAdditionalSubscriptionAsync("Users");
            await this.CreateAdditionalSubscriptionAsync("ProductTypes");

            this.isStarted = true;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Handles a received notification
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task OnNotificationAsync(Notification notification)
        {
            if (!this.IncludePongNotifications && notification is PongNotification)
            {
                return;
            }

            this.context.Debug(
                "Received notification of type {0} with Id: {1}",
                notification.GetType().Name,
                notification.Id);
            this.receivedNotifications.Add(notification);
            var expectation = this.expectations.FirstOrDefault(e => e.Match(this.context, notification));
            if (expectation == null)
            {
                var failMessage = string.Format(
                    "Received notification with Id '{0}', type '{1}' was not expected.",
                    notification.Id,
                    notification.GetType().Name);
                this.context.Fail(failMessage);
            }
            else
            {
                if (this.expectations.IndexOf(expectation) == 0)
                {
                    this.context.Info(string.Format("Found match for notification with Id {0}", notification.Id));
                    this.expectations.Remove(expectation);
                    return;
                }

                this.context.Fail(string.Format("Missed at least one expected notification"));
            }
        }

        /// <summary>
        /// Asynchronously creates an additional subscription on the current notification manager.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected virtual async Task CreateAdditionalSubscriptionAsync(string path)
        {
            var notificationManagerConfiguration = new NotificationManagerConfiguration
            {
                ConnectionString = this.configuration.NotificationsConnectionString,
                Path = path
            };
            var subscriptionConfiguration =
                new NotificationSubscriptionConfiguration(
                    notificationManagerConfiguration,
                    this.notificationSubscriptionConfiguration.ApplicationName,
                    this.notificationSubscriptionConfiguration.Name,
                    this.notificationSubscriptionConfiguration.SessionId)
                {
                    Filter = new SqlNotificationManagerFilter("[sys].ReplyToSessionId IS NULL"),
                    IsUnique = true,
                    Timeout = TimeSpan.FromMinutes(30)
                };
            var notificationSubscriber =
                await this.notificationManager.SubscribeAsync(this, subscriptionConfiguration);
            this.subscriptions[path] = notificationSubscriber;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose unmanaged resources
            }

            this.isStarted = false;
            foreach (
                var notificationSubscriber in
                    this.subscriptions.Values.Where(notificationSubscriber => notificationSubscriber != null))
            {
                notificationSubscriber.Dispose();
            }

            this.notificationManager.Dispose();
        }
    }
}
