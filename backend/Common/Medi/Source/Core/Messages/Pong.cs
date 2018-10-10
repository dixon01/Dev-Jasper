// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pong.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Pong type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    /// <summary>
    /// For internal use only.
    /// Response to a <see cref="Ping"/> request.
    /// </summary>
    public class Pong
    {
        /// <summary>
        /// Gets or sets the timestamp of the <see cref="Ping"/> request related
        /// to this response.
        /// </summary>
        public long RequestTimestamp { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Pong[{0}]", this.RequestTimestamp);
        }
    }
}
