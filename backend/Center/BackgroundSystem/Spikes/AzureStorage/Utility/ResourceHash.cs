// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHash.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceHash type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.Utility
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Helper class that allows to create a string representing the MD5 hash.
    /// </summary>
    public static class ResourceHash
    {
        /// <summary>
        /// Creates a string representing the MD5 hash from the contents of a local file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(string fileName)
        {
            using (var input = File.OpenRead(fileName))
            {
                return Create(input);
            }
        }

        /// <summary>
        /// Creates a string representing the MD5 hash from the contents of a stream.
        /// </summary>
        /// <param name="input">
        /// The stream to read from.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(Stream input)
        {
            using (var md5 = CreateAlgorithm())
            {
                var result = md5.ComputeHash(input);
                return HashToString(result);
            }
        }

        /// <summary>
        /// Create a string representing the MD5 hash from the contents of a byte array.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset into the <see cref="buffer"/>.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(byte[] buffer, int offset, int count)
        {
            using (var md5 = CreateAlgorithm())
            {
                var result = md5.ComputeHash(buffer, offset, count);
                return HashToString(result);
            }
        }

        /// <summary>
        /// Creates a new instance of the MD5 hash.
        /// </summary>
        /// <returns>
        /// The <see cref="HashAlgorithm"/> for MD5.
        /// </returns>
        public static HashAlgorithm CreateAlgorithm()
        {
            var md5 = new MD5CryptoServiceProvider();
            md5.Initialize();
            return md5;
        }

        private static string HashToString(byte[] hash)
        {
            var sb = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
