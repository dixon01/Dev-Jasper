// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityUtility.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SecurityUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

#if WindowsCE
    using Rfc2898DeriveBytes = OpenNETCF.Security.Cryptography.PasswordDeriveBytes;
#endif

    /// <summary>
    /// Defines utility methods regarding security and encryption.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: all Encrypt and Decrypt methods are incompatible between CF and FX!
    /// </remarks>
    public static class SecurityUtility
    {
        /// <summary>
        /// Evaluates the MD5 hash of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>
        /// The evaluated MD5 hash of the input string in lower chars.
        /// </returns>
        public static string Md5(string input)
        {
            return Md5(input, false);
        }

        /// <summary>
        /// Evaluates the MD5 hash of the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="useCapitalChars">if set to <c>true</c> the result will be in upper chars.</param>
        /// <returns>
        /// The evaluated MD5 hash of the input string in lower chars,
        /// or upper if the <paramref name="useCapitalChars"/> is set to <b>true</b>.
        /// </returns>
        public static string Md5(string input, bool useCapitalChars)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "The input string can't be null.");
            }

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            var format = useCapitalChars ? "X2" : "x2";
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString(format));
            }

            var output = sb.ToString();
            return output;
        }

        /// <summary>
        /// Generates a random password containing upper and lower case letters and digits.
        /// </summary>
        /// <param name="length">
        /// The length of the password.
        /// </param>
        /// <returns>
        /// The generated password.
        /// </returns>
        public static string GenerateRandomPassword(int length)
        {
            const string Valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var password = new StringBuilder(length);
            var rnd = new Random();
            while (0 < length--)
            {
                password.Append(Valid[rnd.Next(Valid.Length)]);
            }

            return password.ToString();
        }

        /// <summary>
        /// Encrypt a byte array into a byte array using a key and an IV.
        /// </summary>
        /// <param name="clearData">
        /// The clear data.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="iv">
        /// The IV.
        /// </param>
        /// <returns>
        /// The encrypted data.
        /// </returns>
        public static byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            var memory = new MemoryStream();
            var algorithm = Rijndael.Create();

            algorithm.Key = key;
            algorithm.IV = iv;

            var cryptoStream = new CryptoStream(memory, algorithm.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(clearData, 0, clearData.Length);
            cryptoStream.Close();

            return memory.ToArray();
        }

        /// <summary>
        /// Encrypt a string into a base-64 string using a password.
        /// </summary>
        /// <param name="clearText">
        /// The clear text.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The encrypted string.
        /// </returns>
        public static string Encrypt(string clearText, string password)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            return Convert.ToBase64String(Encrypt(clearBytes, password));
        }

        /// <summary>
        /// Encrypt a byte array into a byte array using a password.
        /// </summary>
        /// <param name="clearData">
        /// The clear data.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The encrypted data.
        /// </returns>
        public static byte[] Encrypt(byte[] clearData, string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(
                password,
                new byte[] { 0x53, 0x61, 0x6d, 0x75, 0x65, 0x6c, 0x20, 0x57, 0x65, 0x69, 0x62, 0x65, 0x6c });

            return Encrypt(clearData, deriveBytes.GetBytes(32), deriveBytes.GetBytes(16));
        }

        /// <summary>
        /// Decrypts a byte array from a byte array using a key and an IV.
        /// </summary>
        /// <param name="cipherData">
        /// The encrypted data.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="iv">
        /// The IV.
        /// </param>
        /// <returns>
        /// The decrypted data.
        /// </returns>
        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            var memory = new MemoryStream();
            var algorithm = Rijndael.Create();
            algorithm.Key = key;
            algorithm.IV = iv;
            var cryptoStream = new CryptoStream(memory, algorithm.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipherData, 0, cipherData.Length);
            cryptoStream.Close();

            return memory.ToArray();
        }

        /// <summary>
        /// Decrypts a string from a base-64 string using a password.
        /// </summary>
        /// <param name="cipherText">
        /// The encrypted text.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The decrypted text.
        /// </returns>
        public static string Decrypt(string cipherText, string password)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decrypted = Decrypt(cipherBytes, password);
            return Encoding.Unicode.GetString(decrypted, 0, decrypted.Length);
        }

        /// <summary>
        /// Decrypts a byte array from a byte array using a password.
        /// </summary>
        /// <param name="cipherData">
        /// The encrypted data.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The decrypted data.
        /// </returns>
        public static byte[] Decrypt(byte[] cipherData, string password)
        {
            var deriveBytes = new Rfc2898DeriveBytes(
                password,
                new byte[] { 0x53, 0x61, 0x6d, 0x75, 0x65, 0x6c, 0x20, 0x57, 0x65, 0x69, 0x62, 0x65, 0x6c });

            return Decrypt(cipherData, deriveBytes.GetBytes(32), deriveBytes.GetBytes(16));
        }
    }
}
