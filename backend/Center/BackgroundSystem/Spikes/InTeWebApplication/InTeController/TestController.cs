// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.InTeController
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// The test controller.
    /// </summary>
    public class TestController : IObserver<Notification>
    {
        private INotificationManager notificationManager;

        private INotificationSubscriber notificationSubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        public TestController(UserCredentials userCredentials)
        {
            this.UserCredentials = userCredentials;
        }

        /// <summary>
        /// Gets the user credentials.
        /// </summary>
        public UserCredentials UserCredentials { get; private set; }

        /// <summary>
        /// Runs the controller asynchronously.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task RunAsync()
        {
            const string ConnectionString = "Endpoint=sb://pc1234.gorba.com/ServiceBusDefaultNamespace;"
                                            + "StsEndpoint=https://pc1234.gorba.com:9355/ServiceBusDefaultNamespace;"
                                            + "RuntimePort=9354;ManagementPort=9355";
            var configuration = new NotificationManagerConfiguration
                                                                 {
                                                                     ConnectionString = ConnectionString,
                                                                     Path = "InTe"
                                                                 };
            this.notificationManager = NotificationManagerFactory.Current.Create(configuration);
            var c = new NotificationSubscriptionConfiguration(
                configuration,
                "TestController",
                "Controller",
                NotificationManagerUtility.GenerateUniqueSessionId(),
                this.UserCredentials);
            this.notificationSubscriber = await this.notificationManager.SubscribeAsync(this, c);
        }

        /// <summary>
        /// Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(Notification value)
        {
            var testRunNotification = value as TestRunNotification;
            if (testRunNotification == null)
            {
                return;
            }

            Task.Run(() => RunAsync(testRunNotification.Content));
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted()
        {
        }

        private static async Task RunAsync(TestRun run)
        {
            await Task.FromResult(0);
        }
    }
}