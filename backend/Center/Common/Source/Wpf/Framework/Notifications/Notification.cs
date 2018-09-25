// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Notification.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Notification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    using System;
    using System.Diagnostics;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Defines a notification that should be displayed to inform the user about any event occurring within the
    /// application.
    /// </summary>
    [DebuggerDisplay(
        "Title: '{Title}', Priority: {Priority},"
        + " DateTime: {DateTime}, IsAcknowledged: {IsAcknowledged}")]
    public abstract class Notification : ViewModelBase
    {
        private bool isAcknowledged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        protected Notification()
        {
            this.DateTime = TimeProvider.Current.UtcNow;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this notification is acknowledged by the user, meaning that he
        /// &quot;read&quot; and &quot;accepted&quot; the notification.
        /// </summary>
        /// <value>
        /// <c>true</c> if this notification is acknowledged by the user; otherwise, <c>false</c>.
        /// </value>
        public bool IsAcknowledged
        {
            get
            {
                return this.isAcknowledged;
            }

            set
            {
                this.SetProperty(ref this.isAcknowledged, value, () => this.IsAcknowledged);
            }
        }

        /// <summary>
        /// Gets or sets the date time when the notification was created.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the priority of the notification.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "[Notification Title:'{0}', Priority: {1}, DateTime: {2}, IsAcknowledged: {3}]",
                this.Title,
                this.Priority,
                this.IsAcknowledged,
                this.DateTime);
        }
    }
}