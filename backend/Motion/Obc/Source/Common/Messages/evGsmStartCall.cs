// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evGsmStartCall.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evGsmStartCall type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// COS: 21 October 2010
    /// This class represents an event launched
    /// when someone wants to make a GSM voice call.
    /// </summary>
    public class evGsmStartCall
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evGsmStartCall"/> class.
        /// </summary>
        public evGsmStartCall()
        {
            this.PhoneNumber = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evGsmStartCall"/> class.
        /// </summary>
        /// <param name="phoneNumber">
        /// The phone number to call.
        /// </param>
        public evGsmStartCall(string phoneNumber)
        {
            this.PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Gets or sets the phone number to call.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
