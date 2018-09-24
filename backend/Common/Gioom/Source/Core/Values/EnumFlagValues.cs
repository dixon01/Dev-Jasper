// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlagValues.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumFlagValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Allows a range of enumeration values.
    /// One or multiple of the <see cref="Values"/> can be combined with OR.
    /// </summary>
    public class EnumFlagValues : EnumValues
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFlagValues"/> class.
        /// </summary>
        /// <param name="values">
        /// The valid values.
        /// </param>
        public EnumFlagValues(IDictionary<int, string> values)
            : base(values)
        {
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
            IOValue output;
            if (this.TryCreateValue(value, out output))
            {
                return output;
            }

            // it seems to be a composite value (multiple flags),
            // so let's create the new value
            var nameBuilder = new StringBuilder();
            var values = new List<IOValue>(this.Values);
            values.Sort((a, b) => a.Value.CompareTo(b.Value));
            foreach (var existingValue in values)
            {
                if (existingValue.Value == 0 && value != 0)
                {
                    continue;
                }

                if ((value & existingValue.Value) == existingValue.Value)
                {
                    nameBuilder.Append(existingValue.Name).Append(",");
                }
            }

            if (nameBuilder.Length == 0)
            {
                throw new ArgumentException("Unknown enum flag value " + value, "value");
            }

            nameBuilder.Length--;

            return new IOValue(nameBuilder.ToString(), value);
        }
    }
}