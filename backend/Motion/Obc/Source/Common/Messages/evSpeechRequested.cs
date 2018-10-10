// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evSpeechRequested.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evSpeechRequested type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The speech requested event.
    /// </summary>
    public class evSpeechRequested
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechRequested"/> class.
        /// </summary>
        /// <param name="addressList">
        /// The address list.
        /// </param>
        public evSpeechRequested(string addressList)
        {
            this.AddressList = addressList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evSpeechRequested"/> class.
        /// </summary>
        public evSpeechRequested()
        {
            this.AddressList = string.Empty;
        }

        /// <summary>
        /// Gets or sets the address list.
        /// </summary>
        public string AddressList { get; set; }
    }
}