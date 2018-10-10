// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerValues.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Allows a range of integer values between <see cref="MinValue"/> and
    /// <see cref="MaxValue"/>.
    /// </summary>
    public class IntegerValues : ValuesBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerValues"/> class.
        /// </summary>
        /// <param name="minValue">
        /// The min value.
        /// </param>
        /// <param name="maxValue">
        /// The max value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <see cref="minValue"/> is greater than or equal to <see cref="maxValue"/>.
        /// </exception>
        public IntegerValues(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("MinValue has to be less than maxValue");
            }

            this.MaxValue = maxValue;
            this.MinValue = minValue;
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public int MinValue { get; private set; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public int MaxValue { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("IntegerValues: {0}..{1}", this.MinValue, this.MaxValue);
        }

        /// <summary>
        /// Creates a valid I/O value for the given <see cref="value"/>.
        /// </summary>
        /// <param name="value">
        /// The integer value to be converted into an <see cref="IOValue"/>.
        /// </param>
        /// <returns>
        /// The resulting <see cref="IOValue"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <see cref="value"/> is outside the allowed range of I/O values.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <see cref="value"/> is not a I/O valid value.
        /// </exception>
        internal override IOValue CreateValue(int value)
        {
            if (value < this.MinValue || value > this.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    "value",
                    string.Format("Only values between {0} and {1} are supported", this.MinValue, this.MaxValue));
            }

            return new IOValue(value.ToString(CultureInfo.InvariantCulture), value);
        }
    }
}