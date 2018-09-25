// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationSubscriptionConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationSubscriptionConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Notifications;

    /// <summary>
    /// The configuration for a subscription on a notification manager.
    /// </summary>
    public class NotificationSubscriptionConfiguration
    {
        private const int SubscriptionNameMaxLength = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionConfiguration"/> class.
        /// </summary>
        /// <param name="notificationManagerConfiguration">
        /// The notification manager configuration.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <exception cref="ArgumentNullException">The configuration is null.</exception>
        public NotificationSubscriptionConfiguration(
            NotificationManagerConfiguration notificationManagerConfiguration,
            string applicationName,
            string name,
            string sessionId)
        {
            if (notificationManagerConfiguration == null)
            {
                throw new ArgumentNullException("notificationManagerConfiguration");
            }

            this.NotificationManagerConfiguration = notificationManagerConfiguration;

            this.ApplicationName = applicationName;
            this.Name = name;
            this.SessionId = sessionId;
            this.Timeout = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationSubscriptionConfiguration"/> class.
        /// </summary>
        /// <param name="notificationManagerConfiguration">
        /// The notification manager configuration.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentNullException">The configuration is null.</exception>
        /// <remarks>A unique session id will be generated for the subscription.</remarks>
        public NotificationSubscriptionConfiguration(
            NotificationManagerConfiguration notificationManagerConfiguration,
            string applicationName,
            string name)
            : this(
                notificationManagerConfiguration,
                applicationName,
                name,
                NotificationManagerUtility.GenerateUniqueSessionId())
        {
            this.IsUnique = true;
        }

        /// <summary>
        /// Gets the notification manager configuration.
        /// </summary>
        public NotificationManagerConfiguration NotificationManagerConfiguration { get; private set; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the name of the subscription.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a unique identifier for the session by a client of notifications.
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        public NotificationManagerFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscription is unique.
        /// </summary>
        /// <remarks>
        /// This flag should be set when subscription names are generated. This will improve startup time.
        /// </remarks>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets the string representation of the <see cref="Filter"/> property, if specified.
        /// </summary>
        /// <returns>
        /// The string relative to the <see cref="Filter"/> property, if not <c>null</c>; otherwise, <c>null</c>.
        /// </returns>
        public string GetFilterString()
        {
            return this.Filter == null ? null : this.Filter.ToStringFilter();
        }

        /// <summary>
        /// Gets the name of the subscription.
        /// </summary>
        /// <returns>A unique name for the subscription.</returns>
        public string ToSubscriptionName()
        {
            // 2 is for the '_' separators
            var maxApplicationSpecificNameLength = SubscriptionNameMaxLength - this.SessionId.Length - 2;
            var applicationName = this.ApplicationName;
            var name = this.Name;

            Func<int> evalExceed = () => applicationName.Length + name.Length - maxApplicationSpecificNameLength;
            var exceed = evalExceed();
            while (exceed > 0)
            {
                if (applicationName.Length > name.Length)
                {
                    applicationName = applicationName.Substring(0, applicationName.Length - 1);
                }
                else
                {
                    name = name.Substring(0, name.Length - 1);
                }

                exceed = evalExceed();
            }

            var subscriptionName = string.Format("{0}_{1}_{2}", applicationName, name, this.SessionId);
            if (subscriptionName.Length > 50)
            {
                throw new ChangeTrackingException(
                    string.Format(
                        "The subscription name '{0}' exceeds the limit (50 characters, it is {1})",
                        subscriptionName,
                        subscriptionName.Length));
            }

            return subscriptionName;
        }
    }
}