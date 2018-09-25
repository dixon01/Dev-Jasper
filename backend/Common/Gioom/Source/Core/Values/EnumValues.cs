// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValues.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Allows a range of enumeration values.
    /// Only one of the <see cref="Values"/> is allowed at once (no OR of values).
    /// </summary>
    public partial class EnumValues : ValuesBase
    {
        private readonly Dictionary<int, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValues"/> class.
        /// </summary>
        /// <param name="values">
        /// The valid values.
        /// </param>
        public EnumValues(IDictionary<int, string> values)
        {
            this.values = new Dictionary<int, string>(values);
        }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        public IOValue[] Values
        {
            get
            {
                var list = new IOValue[this.values.Count];
                var index = 0;
                foreach (var value in this.values)
                {
                    list[index++] = new IOValue(value.Value, value.Key);
                }

                return list;
            }
        }

        /// <summary>
        /// Creates an <see cref="EnumValues"/> or <see cref="EnumFlagValues"/> for
        /// the given enum type. If the enum has a <see cref="FlagsAttribute"/>,
        /// an <see cref="EnumFlagValues"/> is returned, otherwise an <see cref="EnumValues"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum, this type must ba an <see cref="Enum"/>.
        /// </typeparam>
        /// <returns>
        /// The <see cref="EnumValues"/> or <see cref="EnumFlagValues"/>.
        /// </returns>
        public static EnumValues FromEnum<T>()
            where T : struct, IConvertible
        {
            var dict = GetEnumValues<T>();

            if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                return new EnumFlagValues(dict);
            }

            return new EnumValues(dict);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name).Append(": ");
            foreach (var value in this.values)
            {
                sb.Append(value.Value).Append(" (").Append(value.Key).Append("), ");
            }

            if (sb.Length > 0)
            {
                sb.Length -= 2;
            }
            else
            {
                sb.Append("<none>");
            }

            return sb.ToString();
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
            if (!this.TryCreateValue(value, out output))
            {
                throw new ArgumentException("Unknown enum value " + value, "value");
            }

            return output;
        }

        /// <summary>
        /// Tries to create an <see cref="IOValue"/> from the given integer.
        /// </summary>
        /// <param name="input">
        /// The integer value.
        /// </param>
        /// <param name="value">
        /// The value or null if this method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the value was successfully created.
        /// </returns>
        protected bool TryCreateValue(int input, out IOValue value)
        {
            string name;
            if (!this.values.TryGetValue(input, out name))
            {
                value = null;
                return false;
            }

            value = new IOValue(name, input);
            return true;
        }
    }
}