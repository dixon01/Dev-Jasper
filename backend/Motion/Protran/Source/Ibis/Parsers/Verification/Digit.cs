// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Digit.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Digit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Checks that the telegram contains a digit (0-9).
    /// </summary>
    public class Digit : DigitBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Digit"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to 1.
        /// </summary>
        public Digit()
            : this(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Digit"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to the given value.
        /// </summary>
        /// <param name="count">
        /// The number of digits allowed.
        /// </param>
        public Digit(int count)
            : this(count, count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Digit"/> class.
        /// </summary>
        /// <param name="minCount">
        /// The minimum number of allowed digits.
        /// </param>
        /// <param name="maxCount">
        /// The maximum number of allowed digits.
        /// </param>
        public Digit(int minCount, int maxCount)
            : base(minCount, maxCount)
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.MinCount == this.MaxCount)
            {
                return string.Format("{0} digits", this.MinCount);
            }
            
            return string.Format("{0}-{1} digits", this.MinCount, this.MaxCount);
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
        protected override bool CheckDigit(byte b)
        {
            return b >= '0' && b <= '9';
        }
    }
}
