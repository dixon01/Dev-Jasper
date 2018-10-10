// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuedMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic
{
    using System;

    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Represents the request to send a live update message.
    /// </summary>
    public class QueuedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedMessage"/> class.
        /// </summary>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        /// <param name="updateGroupId">
        /// The update group id.
        /// </param>
        /// <param name="message">
        /// The update.
        /// </param>
        public QueuedMessage(string applicationName, int updateGroupId, GorbaMessage message)
        {
            this.ApplicationName = applicationName;
            this.UpdateGroupId = updateGroupId;
            this.Message = message;
            this.Timestamp = TimeProvider.Current.UtcNow;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the update group id.
        /// </summary>
        public int UpdateGroupId { get; private set; }

        /// <summary>
        /// Gets the update.
        /// </summary>
        public GorbaMessage Message { get; private set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}