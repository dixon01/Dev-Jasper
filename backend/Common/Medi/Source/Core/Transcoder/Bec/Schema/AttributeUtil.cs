// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeUtil.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AttributeUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    using System.Reflection;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Utility class to query attributes for a given reflection information.
    /// </summary>
    internal static class AttributeUtil
    {
        /// <summary>
        /// Gets all attributes of a given type from a given member.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <typeparam name="T">
        /// The type of attribute to be requested.
        /// </typeparam>
        /// <returns>
        /// An array of all attributes (can never be null, but might be empty).
        /// </returns>
        public static T[] GetAttributes<T>(MemberInfo member)
        {
            object[] attrs = member.GetCustomAttributes(typeof(T), false);
            if (attrs.Length == 0)
            {
                return new T[0];
            }

            return ArrayUtil.ConvertAll(attrs, attr => (T)attr);
        }
    }
}
