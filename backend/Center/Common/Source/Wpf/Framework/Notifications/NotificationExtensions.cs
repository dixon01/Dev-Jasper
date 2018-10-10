// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    /// <summary>
    /// Defines extension methods for <see cref="Notification"/>s.
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Updates the progress of the given <paramref name="progressNotification"/> based on a given
        /// <see cref="ProgressNotification"/>.
        /// </summary>
        /// <param name="progressNotification">The progress notification.</param>
        /// <param name="updatedProgressNotification">The updated progress notification.</param>
        public static void UpdateProgress(
            this ProgressNotification progressNotification, ProgressNotification updatedProgressNotification)
        {
            progressNotification.IsCompleted = updatedProgressNotification.IsCompleted;
            progressNotification.Progress = updatedProgressNotification.Progress;
        }
    }
}