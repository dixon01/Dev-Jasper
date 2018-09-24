// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceHash.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContentResourceHash type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System;
    using System.Data.HashFunction;
    using System.IO;
    using System.Text;

    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Helper class that allows to create a string representing a hash created
    /// with the selected <see cref="HashAlgorithmTypes"/>.
    /// </summary>
    public static class ContentResourceHash
    {
        /// <summary>
        /// Creates a string representing the hash of the given type from the contents of a local file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(string fileName, HashAlgorithmTypes hashType)
        {
            using (var input = File.OpenRead(fileName))
            {
                return Create(input, hashType);
            }
        }

        /// <summary>
        /// Creates a string representing the MD5 hash from the contents of a stream.
        /// </summary>
        /// <param name="input">
        /// The stream to read from.
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(Stream input, HashAlgorithmTypes hashType)
        {
            switch (hashType)
            {
                case HashAlgorithmTypes.MD5:
                    return ResourceHash.Create(input);
                case HashAlgorithmTypes.xxHash64:
                    var hash64 = new xxHash(64);
                    var result64 = hash64.ComputeHash(input);
                    return HashToString(result64);
                case HashAlgorithmTypes.xxHash32:
                    var hash32 = new xxHash(32);
                    var result32 = hash32.ComputeHash(input);
                    return HashToString(result32);
                default:
                    throw new ArgumentOutOfRangeException("hashType", hashType, null);
            }
        }

        /// <summary>
        /// Create a string representing the MD5 hash from the contents of a byte array.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="offset">
        /// The offset into the <see cref="buffer"/>. This is only used if the has type is MD5.
        /// </param>
        /// <param name="count">
        /// The count. This is only used if the has type is MD5.
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// The hash string.
        /// </returns>
        public static string Create(byte[] buffer, int offset, int count, HashAlgorithmTypes hashType)
        {
            switch (hashType)
            {
                case HashAlgorithmTypes.MD5:
                    return ResourceHash.Create(buffer, offset, count);
                case HashAlgorithmTypes.xxHash64:
                    var hash = new xxHash(64);
                    var result = hash.ComputeHash(buffer);
                    return HashToString(result);
                case HashAlgorithmTypes.xxHash32:
                    var hash32 = new xxHash(32);
                    var result32 = hash32.ComputeHash(buffer);
                    return HashToString(result32);
                default:
                    throw new ArgumentOutOfRangeException("hashType", hashType, null);
            }
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
