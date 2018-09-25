// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassNameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClassNameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Text;

    /// <summary>
    /// Provides readable class names for generic types.
    /// </summary>
    public static class ClassNameProvider
    {
        /// <summary>
        /// Get the class name of a generic type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// a readable class name.
        /// </returns>
        public static string GetGenericClassName(Type type)
        {
            var sb = new StringBuilder();
            GetGenericClassName(type, sb, true);
            return sb.ToString();
        }

        private static void GetGenericClassName(Type type, StringBuilder sb, bool fullName)
        {
            sb.Append(fullName ? type.FullName : type.Name);
            if (!type.IsGenericType)
            {
                return;
            }

            var index = sb.ToString().LastIndexOf('`');
            if (index > 0)
            {
                sb.Length = index;
            }

            sb.Append('<');
            foreach (var genericArgument in type.GetGenericArguments())
            {
                GetGenericClassName(genericArgument, sb, false);
                sb.Append(',');
            }

            sb.Length--;
            sb.Append('>');
        }
    }
}
