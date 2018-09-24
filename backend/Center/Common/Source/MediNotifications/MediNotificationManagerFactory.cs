// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediNotificationManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediNotificationManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.MediNotifications
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;

    /// <summary>
    /// Defines a <see cref="NotificationManagerFactory"/> that creates objects of type
    /// <see cref="MediNotificationManager"/>.
    /// </summary>
    public class MediNotificationManagerFactory : NotificationManagerFactory
    {
        private readonly Dictionary<string, MediConnectionManager> connectionManagers =
            new Dictionary<string, MediConnectionManager>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="MediNotificationManagerFactory"/> class.
        /// </summary>
        /// <param name="isLocal">
        /// Defines if the connections are local.
        /// </param>
        public MediNotificationManagerFactory(bool isLocal)
        {
            this.IsLocal = isLocal;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connections are local.
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// Creates a notification manager.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The <see cref="INotificationManager"/>.
        /// </returns>
        public override INotificationManager Create(NotificationManagerConfiguration configuration)
        {
            MediConnectionManager connectionManager;
            lock (this.connectionManagers)
            {
                if (!this.connectionManagers.TryGetValue(configuration.ConnectionString, out connectionManager))
                {
                    connectionManager = new MediConnectionManager(configuration.ConnectionString, this.IsLocal);
                    connectionManager.Disposing += (s, e) =>
                        {
                            lock (this.connectionManagers)
                            {
                                this.connectionManagers.Remove(configuration.ConnectionString);
                            }
                        };
                    this.connectionManagers.Add(configuration.ConnectionString, connectionManager);
                }
            }

            return connectionManager.CreateNotificationManager(configuration.Path);
        }
    }
}
