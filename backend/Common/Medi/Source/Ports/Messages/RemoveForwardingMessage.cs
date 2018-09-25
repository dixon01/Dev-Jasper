// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveForwardingMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoveForwardingMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Messages
{
    /// <summary>
    /// The remove forwarding message.
    /// Don't use this class outside this library.
    /// </summary>
    public class RemoveForwardingMessage
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}: Id: {1}", this.GetType().Name, this.Id);
        }
    }
}