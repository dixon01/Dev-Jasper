// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagValues.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FlagValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;

    /// <summary>
    /// Allows true (1) or false (0) values only.
    /// </summary>
    public class FlagValues : ValuesBase
    {
        /// <summary>
        /// The value representing true (1).
        /// </summary>
        public static readonly IOValue True = new IOValue("1", 1);

        /// <summary>
        /// The value representing false (0).
        /// </summary>
        public static readonly IOValue False = new IOValue("0", 0);

        /// <summary>
        /// Gets the <see cref="IOValue"/> for the given boolean value.
        /// </summary>
        /// <param name="value">
        /// The boolean value.
        /// </param>
        /// <returns>
        /// Either <see cref="True"/> or <see cref="False"/>.
        /// </returns>
        public static IOValue GetValue(bool value)
        {
            return value ? True : False;
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
            return "FlagValues: 0 / 1";
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
            switch (value)
            {
                case 0:
                    return False;
                case 1:
                    return True;
                default:
                    throw new ArgumentOutOfRangeException("value", "Only 0 or 1 supported");
            }
        }
    }
}