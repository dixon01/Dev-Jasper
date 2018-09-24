// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel
{
    /// <summary>
    /// The notification type.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// The type is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The ping.
        /// </summary>
        Ping,

        /// <summary>
        /// The pong.
        /// </summary>
        Pong
    }
}