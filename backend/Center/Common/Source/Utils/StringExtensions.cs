// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System;
    using System.Diagnostics.Contracts;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Defines string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Evaluates the MD5 hash of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="useCapitalChars">if set to <c>true</c> the result will .</param>
        /// <returns>
        /// The evaluated MD5 hash in lower chars, or upper if the <paramref name="useCapitalChars"/>
        /// is set to <b>true</b>.
        /// </returns>
        public static string Md5(this string input, bool useCapitalChars = false)
        {
            Contract.Requires(input != null, "The input string can't be null.");
            var md5 = SecurityUtility.Md5(input);
            return md5;
        }

        /// <summary>
        /// Evaluates the digest of the <paramref name="input"/> string using the specified
        /// algorithm and appending a salt to the input.
        /// The salt can be specified as parameter or, if null, the default salt will be appended.
        /// An empty salt is allowed, even though not recommended.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="algorithm">The algorithm to be used.</param>
        /// <param name="salt">The optional salt to be appended.</param>
        /// <returns>
        /// The evaluated hash.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The input string is null.
        ///   </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified algorithm is invalid or not supported.
        /// </exception>
        /// <remarks>
        /// The hash will be evaluated using the following format: "{input}_{salt}".
        /// The output will contain lower chars.
        /// Even if an empty salt is specified, the '_' char is appended to the input string.
        /// </remarks>
        public static string Encrypt(this string input, PasswordEncrypter.HashAlgorithm algorithm, string salt = null)
        {
            var encrypter = new PasswordEncrypter();
            var result = encrypter.SaltedHash(input, algorithm, salt);
            return result;
        }
    }
}
