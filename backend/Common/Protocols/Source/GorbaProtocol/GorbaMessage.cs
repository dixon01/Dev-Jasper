// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GorbaMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.GorbaProtocol
{
    using System;

    /// <summary>
    /// A live update message.
    /// </summary>
    [Serializable]
    public class GorbaMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the (UTC) time until this message is valid.
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("GorbaMessage {0}", this.Id);
        }
    }
}