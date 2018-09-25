// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evSpeechDisconnected.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evSpeechDisconnected type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The speech disconnected event.
    /// </summary>
    public class evSpeechDisconnected
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechDisconnected"/> class.
        /// </summary>
        public evSpeechDisconnected()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechDisconnected"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public evSpeechDisconnected(int response)
        {
            this.Response = response;
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public int Response { get; set; }
    }
}