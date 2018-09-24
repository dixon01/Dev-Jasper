// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RxLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Groups items according to the selector key.
        /// Order is preserved for both groups and items in groups.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <typeparam name="T">The type of the items.
        /// </typeparam>
        /// <typeparam name="TKey">The type of the key for the selection.
        /// </typeparam>
        /// <returns>
        /// An enumeration of lists representing the grouped items.
        /// </returns>
        public static IEnumerable<IList<T>> ToRuns<T, TKey>(
               this IEnumerable<T> source,
               Func<T, TKey> keySelector)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                var currentSet = new List<T>();

                // inspect the first item
                var lastKey = keySelector(enumerator.Current);
                currentSet.Add(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    var newKey = keySelector(enumerator.Current);
                    if (!Equals(newKey, lastKey))
                    {
                        // A difference == new run; return what we've got thus far
                        yield return currentSet;
                        lastKey = newKey;
                        currentSet = new List<T>();
                    }

                    currentSet.Add(enumerator.Current);
                }

                // Return the last run.
                yield return currentSet;

                // and clean up
                currentSet = new List<T>();
                lastKey = default(TKey);
            }
        }
    }
}
