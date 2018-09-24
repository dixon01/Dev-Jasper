// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Messages
{
    /// <summary>
    /// The data message.
    /// Don't use this class outside this library.
    /// </summary>
    public class DataMessage : ForwarderMessageBase
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "{0}: {1} [{2}] {3} bytes",
                this.GetType().Name,
                this.ForwardingId,
                this.StreamId,
                this.Data.Length);
        }
    }
}