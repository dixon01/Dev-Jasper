// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constant.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Constant type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Checks that the telegram contains exactly the given
    /// constant (only ASCII is supported).
    /// </summary>
    public class Constant : TelegramRule
    {
        private readonly string constant;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="constant">
        /// The expected constant. This has to be an ASCII string.
        /// </param>
        public Constant(string constant)
        {
            this.constant = constant;
        }

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
            int byteSize = this.ByteInfo.ByteSize;
            var endOffset = offset + length;
            foreach (var c in this.constant)
            {
                if (offset >= endOffset)
                {
                    throw new TelegramVerificationException(c, "end of telegram", telegram, offset);
                }

                if (byteSize == 2 && telegram[offset++] != 0)
                {
                    throw new TelegramVerificationException(this, "non-null first byte", telegram, offset - 1);
                }

                byte b = telegram[offset++];
                if (b != c)
                {
                    throw new TelegramVerificationException(c, string.Format("<{0:X2}>", b), telegram, offset - 1);
                }
            }

            return offset;
        }
    }
}
