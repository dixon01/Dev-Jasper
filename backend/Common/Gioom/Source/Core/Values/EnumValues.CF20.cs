// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValues.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Allows a range of enumeration values.
    /// Only one of the <see cref="Values"/> is allowed at once (no OR of values).
    /// </summary>
    public partial class EnumValues
    {
        private static IDictionary<int, string> GetEnumValues<T>()
        {
            var dict = new Dictionary<int, string>();
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                dict[((IConvertible)field.GetValue(null)).ToInt32(null)] = field.Name;
            }

            return dict;
        }
    }
}