// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationManagerFactoryUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManagerFactoryUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using Gorba.Center.Common.MediNotifications;
    using Gorba.Center.Common.ServiceModel.Notifications;

    /// <summary>
    /// Utility class that creates and registers <see cref="NotificationManagerFactory"/> implementations.
    /// </summary>
    public static class NotificationManagerFactoryUtility
    {
        /// <summary>
        /// Configures the default factory to the <see cref="MediNotificationManagerFactory"/>.
        /// </summary>
        /// <param name="isLocal">
        /// Defines if the connections are local.
        /// </param>
        public static void ConfigureMediNotificationManager(bool isLocal = false)
        {
            var mediNotificationManagerFactory = new MediNotificationManagerFactory(isLocal);
            NotificationManagerFactory.Set(mediNotificationManagerFactory);
        }
    }
}