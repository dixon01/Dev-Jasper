// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DigitBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DigitBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all digit rules.
    /// </summary>
    public abstract class DigitBase : TelegramRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitBase"/> class.
        /// </summary>
        /// <param name="minCount">
        /// The minimum number of allowed digits.
        /// </param>
        /// <param name="maxCount">
        /// The maximum number of allowed digits.
        /// </param>
        protected DigitBase(int minCount, int maxCount)
        {
            this.MinCount = minCount;
            this.MaxCount = maxCount;
        }

        /// <summary>
        /// Gets the minimum number of allowed digits.
        /// </summary>
        [XmlIgnore]
        public int MinCount { get; private set; }

        /// <summary>
        /// Gets the maximum number of allowed digits.
        /// </summary>
        [XmlIgnore]
        public int MaxCount { get; private set; }

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
            int count;
            int endOffset = offset + length;
            for (count = 0; count < this.MaxCount && offset < endOffset; count++)
            {
                if (byteSize == 2 && telegram[offset++] != 0)
                {
                    throw new TelegramVerificationException(this, "non-null first byte", telegram, offset - 1);
                }

                if (!this.CheckDigit(telegram[offset++]))
                {
                    offset -= byteSize;
                    break;
                }
            }

            if (count < this.MinCount)
            {
                throw new TelegramVerificationException(
                    this, string.Format("<{0:X2}>", telegram[offset - 1]), telegram, offset - 1);
            }

            return offset;
        }

        /// <summary>
        /// Checks a single byte.
        /// </summary>
        /// <param name="b">
        /// The byte to check.
        /// </param>
        /// <returns>
        /// true if the byte is a valid digit.
        /// </returns>
        protected abstract bool CheckDigit(byte b);
    }
}