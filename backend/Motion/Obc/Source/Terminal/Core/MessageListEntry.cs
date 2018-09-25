// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageListEntry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageListEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// An entry for the IMessageList
    /// </summary>
    public class MessageListEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListEntry"/> class.
        /// </summary>
        public MessageListEntry()
        {
            this.Message = string.Empty;
            this.Date = TimeProvider.Current.Now;
            this.IconType = MessageType.Info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListEntry"/> class.
        /// </summary>
        /// <param name="iconType">
        /// The icon type.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public MessageListEntry(MessageType iconType, DateTime date, string message)
        {
            this.IconType = iconType;
            this.Date = date;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the icon type.
        /// </summary>
        public MessageType IconType { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }
}