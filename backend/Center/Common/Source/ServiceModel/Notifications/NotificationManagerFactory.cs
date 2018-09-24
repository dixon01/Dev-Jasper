// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Notifications
{
    using System;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// Defines a factory for notification managers.
    /// </summary>
    public abstract class NotificationManagerFactory
    {
        static NotificationManagerFactory()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static NotificationManagerFactory Current { get; private set; }

        /// <summary>
        /// Sets the current factory.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(NotificationManagerFactory instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Resets to the default factory.
        /// </summary>
        public static void Reset()
        {
            Set(DefaultNotificationManagerFactory.Instance);
        }

        /// <summary>
        /// Creates a notification manager.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The <see cref="INotificationManager"/>.
        /// </returns>
        public abstract INotificationManager Create(NotificationManagerConfiguration configuration);

        private class DefaultNotificationManagerFactory : NotificationManagerFactory
        {
            static DefaultNotificationManagerFactory()
            {
                Instance = new DefaultNotificationManagerFactory();
            }

            public static DefaultNotificationManagerFactory Instance { get; private set; }

            public override INotificationManager Create(NotificationManagerConfiguration configuration)
            {
                throw new NotSupportedException("There's not a default notification manager factory");
            }
        }
    }
}