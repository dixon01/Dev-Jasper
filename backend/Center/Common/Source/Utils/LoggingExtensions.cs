// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggingExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension utility methods for logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs an enumeration of objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <param name="items">The enumeration.</param>
        /// <returns>A concatenation of the string representations for the given objects.</returns>
        public static string LogEnumeration<T>(this IEnumerable<T> items)
        {
            var list = items.ToList();
            const string ContainmentFormat = "[{0}]";
            if (list.Count == 0)
            {
                return string.Format(ContainmentFormat, '-');
            }

            var s = list.Select(arg => arg.ToString()).Aggregate((s1, s2) => s1 + ", " + s2);
            return string.Format(ContainmentFormat, s);
        }
    }
}