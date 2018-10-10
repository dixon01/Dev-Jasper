// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Notification.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Notification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines a message exchanged between remote peers.
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        protected Notification()
        {
            this.TimeToLive = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a string identifying the original notification replied by this one.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets a session identifier for the notification.
        /// </summary>
        public string ReplyToSessionId { get; set; }

        /// <summary>
        /// Gets or sets the time when the message was enqueued.
        /// </summary>
        /// <remarks>
        /// This value should not be set by the producer of the notification. It should be filled by the implementation
        /// of the notification layer.
        /// </remarks>
        public DateTime EnqueuedTimeUtc { get; set; }

        /// <summary>
        /// Gets the UTC time when this notification expires (<see cref="EnqueuedTimeUtc"/> + <see cref="TimeToLive"/>).
        /// </summary>
        public DateTime ExpiresAtUtc
        {
            get
            {
                var ticks = this.EnqueuedTimeUtc.Ticks;
                var maxTicks = Math.Min(this.TimeToLive.Ticks, DateTime.MaxValue.Subtract(this.EnqueuedTimeUtc).Ticks);
                var expiresAtUtcTicks = ticks + maxTicks;
                return new DateTime(expiresAtUtcTicks, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the validity time span for this notification.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }
    }
}