// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressNotification.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    using System.Diagnostics;

    /// <summary>
    /// Defines a notification containing progress information.
    /// </summary>
    [DebuggerDisplay("ActivityId: {ActivityId}, Progress: {Progress}, IsCompleted: {IsCompleted}, Title: '{Title}',"
                     + " Priority: {Priority}, DateTime: {DateTime}, IsAcknowledged: {IsAcknowledged}")]
    public class ProgressNotification : Notification
    {
        private bool isCompleted;

        private double progress;

        /// <summary>
        /// Gets or sets the activity id this progress information refers to.
        /// </summary>
        /// <value>
        /// The activity id.
        /// </value>
        public int ActivityId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the activity is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }

            set
            {
                this.SetProperty(ref this.isCompleted, value, () => this.IsCompleted);
            }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>
        /// The progress as value between 0 (not/just started) and 1 (completed).
        /// </value>
        public double Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                this.SetProperty(ref this.progress, value, () => this.Progress);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                string.Format(
                    "[ProgressNotification ActivityId: {0}, IsCompleted: {1}, Progress: {2}, Title:'{3}',"
                    + " Priority: {4}, DateTime: {5}, IsAcknowledged: {6}]",
                    this.ActivityId,
                    this.IsCompleted,
                    this.Progress,
                    this.Title,
                    this.Priority,
                    this.DateTime,
                    this.IsAcknowledged);
        }
    }
}