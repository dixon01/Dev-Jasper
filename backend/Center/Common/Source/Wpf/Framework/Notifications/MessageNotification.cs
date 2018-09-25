// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageNotification.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Notifications
{
    using System.Diagnostics;

    /// <summary>
    /// Defines a message notification.
    /// </summary>
    [DebuggerDisplay(
        "Type: {Type}, Message: '{Message}', Title: '{Title}', Priority: {Priority},"
        + " DateTime: {DateTime}, IsAcknowledged: {IsAcknowledged}")]
    public class MessageNotification : Notification
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type of the message notification.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public MessageNotificationType Type { get; set; }

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
                    "[MessageNotification Type: {0}, Message: '{1}', Title: '{2}', Priority: {3}, DateTime: {4},"
                    + " IsAcknowledged: {5}]",
                    this.Type,
                    this.Message,
                    this.Title,
                    this.Priority,
                    this.IsAcknowledged,
                    this.DateTime);
        }
    }
}