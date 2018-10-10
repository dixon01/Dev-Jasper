// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeltaNotification.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeltaNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// A delta notification.
    /// </summary>
    /// <typeparam name="T">
    /// The type of delta.
    /// </typeparam>
    public abstract class DeltaNotification<T> : Notification
        where T : DeltaMessageBase
    {
        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        public DeltaNotificationType NotificationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the change was accepted.
        /// </summary>
        public bool WasAccepted { get; set; }

        /// <summary>
        /// Gets or sets the delta message.
        /// </summary>
        public T Delta { get; set; }
    }
}