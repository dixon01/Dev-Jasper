// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.MediNotifications
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// A message sent over Medi that contains a single <see cref="Notification"/>.
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
        /// </summary>
        public NotificationMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public NotificationMessage(Notification notification)
        {
            this.Notification = notification;
        }

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        public Notification Notification { get; set; }
    }
}
