// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumerableExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Some nice extensions that allow us to work efficiently with enumerable collections.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Does an action for each item in the <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enumerable.
        /// </typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Gets the item before the last one matching the given <paramref name="condition"/>.
        /// If no item exists after the match or if no match is found, the default value is returned.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to search.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enumerable to search.
        /// </typeparam>
        /// <returns>
        /// The item before the last one matching the given <paramref name="condition"/>.
        /// If no item exists after the match or if no match is found, the default value is returned.
        /// </returns>
        public static T PreviousBefore<T>(this IEnumerable<T> enumerable, Predicate<T> condition)
        {
            return enumerable.Reverse().NextAfter(condition);
        }

        /// <summary>
        /// Gets the item after the first one matching the given <paramref name="condition"/>.
        /// If no item exists before the match or if no match is found, the default value is returned.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable to search.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enumerable to search.
        /// </typeparam>
        /// <returns>
        /// The item after the first one matching the given <paramref name="condition"/>.
        /// If no item exists before the match or if no match is found, the default value is returned.
        /// </returns>
        public static T NextAfter<T>(this IEnumerable<T> enumerable, Predicate<T> condition)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (condition(enumerator.Current))
                {
                    if (!enumerator.MoveNext())
                    {
                        return default(T);
                    }

                    return enumerator.Current;
                }
            }

            return default(T);
        }
    }
}
