// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumUtil.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.CommonEmb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// The enum util.
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Returns all values of an enum type
        /// </summary>
        /// <typeparam name="TEnum">the Enum to get all values from</typeparam>
        /// <returns>A list of all values.</returns>
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            var enumerations = new List<TEnum>();
            foreach (FieldInfo fieldInfo in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumerations.Add((TEnum)fieldInfo.GetValue(null));
            }

            return enumerations;
        }

        /// <summary>
        /// Returns all values in the defined Enum with separated comma.
        /// </summary>
        /// <typeparam name="TEnum">the Enum to get all values from</typeparam>
        /// <returns>A string listing all values.</returns>
        public static string GetAllEnumValues<TEnum>() where TEnum : struct
        {
            var sb = new StringBuilder();
            foreach (TEnum e in GetValues<TEnum>())
            {
                sb.Append(e);
                sb.Append(", ");
            }

            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }

            return sb.ToString();
        }

       
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }
        
    }
}