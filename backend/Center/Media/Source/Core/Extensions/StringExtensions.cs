// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The string extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes all invalid filename characters from a string.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> without invalid filename characters.
        /// </returns>
        public static string GetValidFileName(this string source)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(
                    source,
                    (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty));
        }
    }
}
