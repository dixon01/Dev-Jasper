// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramRule.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Base class for telegram rule classes.
    /// </summary>
    public abstract class TelegramRule : ITelegramRule
    {
        /// <summary>
        /// Gets or sets the byte information for this check.
        /// This is set before any other methods of this class
        /// are called.
        /// </summary>
        public ByteInfo ByteInfo { get; set; }

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
        public abstract int Check(byte[] telegram, int offset, int length);
    }
}
