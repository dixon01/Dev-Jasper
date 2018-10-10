// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndOfTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EndOfTelegram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Special telegram check that always throws a
    /// <see cref="TelegramVerificationException"/> if Check() is
    /// called. The <see cref="TelegramVerifier"/> checks if
    /// the telegram was checked until the end when this
    /// check is reached.
    /// </summary>
    public sealed class EndOfTelegram : TelegramRule
    {
        /// <summary>
        /// Verifies the contents of the telegram at the given offset
        /// and advances the offset.
        /// </summary>
        /// <param name="telegram">
        /// The telegram data including header, marker and checksum.
        /// </param>
        /// <param name="offset">
        /// The offset. It will be updated to the next position.
        /// </param>
        /// <param name="length">
        /// The number of bytes available in the telegram from the offset.
        /// </param>
        /// <exception cref="TelegramVerificationException">
        /// If the check was unsuccessful.
        /// </exception>
        /// <returns>
        /// The new offset where to check next.
        /// </returns>
        public override int Check(byte[] telegram, int offset, int length)
        {
            throw new TelegramVerificationException("Expected end of telegram");
        }
    }
}
