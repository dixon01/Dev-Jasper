// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramVerificationException.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramVerificationException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    using System;

    /// <summary>
    /// Exception to be thrown by <see cref="ITelegramRule.Check"/>
    /// and <see cref="TelegramVerifier.Verify"/>.
    /// </summary>
    public class TelegramVerificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramVerificationException"/> class.
        /// </summary>
        public TelegramVerificationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramVerificationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public TelegramVerificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramVerificationException"/> class.
        /// </summary>
        /// <param name="expected">
        ///   The expected value.
        /// </param>
        /// <param name="found">
        ///   The value found in the telegram.
        /// </param>
        /// <param name="telegram">
        ///   The telegram.
        /// </param>
        /// <param name="offset">
        ///   The offset in the telegram.
        /// </param>
        public TelegramVerificationException(object expected, object found, byte[] telegram, int offset)
            : this(string.Format("Expected {0} at offset {1}, but got {2} in <{3}>", expected, offset, found, BitConverter.ToString(telegram)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramVerificationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public TelegramVerificationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
