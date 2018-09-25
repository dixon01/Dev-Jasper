// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharRange.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharRange type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Checks that the telegram contains any of the given characters
    /// </summary>
    public class CharRange : DigitBase
    {
        private readonly string allowedCharacters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharRange"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to 1.
        /// </summary>
        /// <param name="allowedCharacters">
        /// The allowed characters.
        /// </param>
        public CharRange(string allowedCharacters)
            : this(1, allowedCharacters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharRange"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to the given value.
        /// </summary>
        /// <param name="count">
        /// The number of digits allowed.
        /// </param>
        /// <param name="allowedCharacters">
        /// The allowed characters.
        /// </param>
        public CharRange(int count, string allowedCharacters)
            : this(count, count, allowedCharacters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharRange"/> class.
        /// </summary>
        /// <param name="minCount">
        /// The minimum number of allowed digits.
        /// </param>
        /// <param name="maxCount">
        /// The maximum number of allowed digits.
        /// </param>
        /// <param name="allowedCharacters">
        /// The allowed characters.
        /// </param>
        public CharRange(int minCount, int maxCount, string allowedCharacters)
            : base(minCount, maxCount)
        {
            this.allowedCharacters = allowedCharacters;
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
                return string.Format("{0} of [{1}]", this.MinCount, this.allowedCharacters);
            }

            return string.Format("{0}-{1} of [{2}]", this.MinCount, this.MaxCount, this.allowedCharacters);
        }

        /// <summary>
        /// Checks a single byte.
        /// </summary>
        /// <param name="b">
        /// The byte to check.
        /// </param>
        /// <returns>
        /// true if the byte is a valid character.
        /// </returns>
        protected override bool CheckDigit(byte b)
        {
            return this.allowedCharacters.IndexOf((char)b) >= 0;
        }
    }
}
