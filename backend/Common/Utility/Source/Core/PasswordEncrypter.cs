// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordEncrypter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PasswordEncrypter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Defines a component which provides methods to encrypt a given password.
    /// </summary>
    public class PasswordEncrypter
    {
        private static readonly Guid DefaultSalt = new Guid("a479a06a-bd80-46f3-a1a6-6f3385edc8e0");

        /// <summary>
        /// List of supported hash algorithms.
        /// </summary>
        public enum HashAlgorithm
        {
            /// <summary>
            /// MD5 algorithm as specified in RFC 1321.
            /// </summary>
            Md5 = 0
        }

        /// <summary>
        /// Evaluates the digest of the <paramref name="input"/> string using the
        /// specified algorithm and appending a salt to the input.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="algorithm">The algorithm to be used.</param>
        /// <returns>
        /// The evaluated hash.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The input string is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified algorithm is invalid or not supported.
        /// </exception>
        /// <remarks>
        /// The hash will be evaluated using the following format: "{input}_{salt}".
        /// The output will contain lower chars.
        /// Even if an empty salt is specified, the '_' char is appended to the input string.
        /// </remarks>
        public string SaltedHash(string input, HashAlgorithm algorithm)
        {
            return this.SaltedHash(input, algorithm, null);
        }

        /// <summary>
        /// Evaluates the digest of the <paramref name="input"/> string using the
        /// specified algorithm and appending a salt to the input.
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
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The specified algorithm is invalid or not supported.
        /// </exception>
        /// <remarks>
        /// The hash will be evaluated using the following format: "{input}_{salt}".
        /// The output will contain lower chars.
        /// Even if an empty salt is specified, the '_' char is appended to the input string.
        /// </remarks>
        public string SaltedHash(string input, HashAlgorithm algorithm, string salt)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "The input string can't be null.");
            }

            if (salt == null)
            {
                salt = DefaultSalt.ToString("N");
            }

            var salted = string.Concat(input, "_", salt);
            string result;
            switch (algorithm)
            {
                case HashAlgorithm.Md5:
                    result = SecurityUtility.Md5(salted);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "algorithm", "The specified algorithm is invalid or not supported.");
            }

            return result;
        }
    }
}
