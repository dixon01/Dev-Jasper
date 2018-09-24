// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValues.FX20.cs" company="Gorba AG">
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

    /// <summary>
    /// Allows a range of enumeration values.
    /// Only one of the <see cref="Values"/> is allowed at once (no OR of values).
    /// </summary>
    public partial class EnumValues
    {
        private static IDictionary<int, string> GetEnumValues<T>()
        {
            var dict = new Dictionary<int, string>();
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                dict[((IConvertible)value).ToInt32(null)] = value.ToString();
            }

            return dict;
        }
    }
}