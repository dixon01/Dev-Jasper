// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evIraMessageReceived.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evIraMessageReceived type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The IRA message received event.
    /// </summary>
    public class evIraMessageReceived
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evIraMessageReceived"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public evIraMessageReceived(int message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evIraMessageReceived"/> class.
        /// </summary>
        public evIraMessageReceived()
        {
        }

        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        public int Message { get; set; }
    }
}