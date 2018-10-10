// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Any.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Any type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers.Verification
{
    /// <summary>
    /// Checks that the telegram contains any characters
    /// </summary>
    public class Any : DigitBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to 1.
        /// </summary>
        public Any()
            : this(1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> class.
        /// This sets <see cref="DigitBase.MinCount"/> and 
        /// <see cref="DigitBase.MaxCount"/> to the given value.
        /// </summary>
        /// <param name="count">
        /// The number of characters allowed.
        /// </param>
        public Any(int count)
            : this(count, count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Any"/> class.
        /// </summary>
        /// <param name="minCount">
        /// The minimum number of allowed characters.
        /// </param>
        /// <param name="maxCount">
        /// The maximum number of allowed characters.
        /// </param>
        public Any(int minCount, int maxCount)
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
                return string.Format("{0} character(s)", this.MinCount);
            }
            
            return string.Format("{0}-{1} characters", this.MinCount, this.MaxCount);
        }

        /// <summary>
        /// Checks a single byte.
        /// </summary>
        /// <param name="b">
        /// The byte to check.
        /// </param>
        /// <returns>
        /// always true.
        /// </returns>
        protected override bool CheckDigit(byte b)
        {
            return true;
        }
    }
}