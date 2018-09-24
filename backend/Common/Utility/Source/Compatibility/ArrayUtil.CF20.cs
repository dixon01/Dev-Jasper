// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayUtil.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrayUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class for Array methods missing in some frameworks.
    /// </summary>
    public static partial class ArrayUtil
    {
        /// <summary>
        /// Converts an array of one type to an array of another type.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional, zero-based <see cref="Array"/> to convert to a target type.
        /// </param>
        /// <param name="converter">
        /// A <see cref="Converter{TInput,TOutput}"/> that converts each element from one type to another type.
        /// </param>
        /// <typeparam name="TInput">
        /// The type of the elements of the source array.
        /// </typeparam>
        /// <typeparam name="TOutput">
        /// The type of the elements of the target array.
        /// </typeparam>
        /// <returns>
        /// An array of the target type containing the converted elements from the source array.
        /// </returns>
        public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
        {
            var output = new TOutput[array.Length];
            var index = 0;
            foreach (var item in array)
            {
                output[index++] = converter(item);
            }

            return output;
        }

        /// <summary>
        /// Finds the first element matching the given predicate.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <typeparam name="T">
        /// The type of the array elements.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> or default(<see cref="T"/>) if no element matched.
        /// </returns>
        public static T Find<T>(T[] array, Predicate<T> match)
        {
            foreach (var item in array)
            {
                if (match(item))
                {
                    return item;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Splits a string <paramref name="value"/> with the given <paramref name="separator"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <returns>
        /// The split string. Including empty entries.
        /// </returns>
        public static string[] SplitString(string value, string separator)
        {
            var parts = new List<string>();
            int index;
            var startIndex = 0;
            while ((index = value.IndexOf(separator, startIndex, StringComparison.InvariantCulture))
                   >= 0)
            {
                parts.Add(value.Substring(startIndex, index - startIndex));
                startIndex = index + separator.Length;
            }

            parts.Add(value.Substring(startIndex));
            return parts.ToArray();
        }
    }
}
