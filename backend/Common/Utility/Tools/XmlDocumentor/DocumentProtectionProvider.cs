// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentProtectionProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentProtectionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    /// <summary>
    /// Class that can make a word document read-only by applying a password.
    /// Mostly taken from:
    /// <c>http://blogs.msdn.com/b/vsod/archive/2010/04/05/
    /// how-to-set-the-editing-restrictions-in-word-using-open-xml-sdk-2-0.aspx</c>
    /// </summary>
    public class DocumentProtectionProvider
    {
        private static readonly int[] InitialCodeArray =
            {
                0xE1F0, 0x1D0F, 0xCC9C, 0x84C0, 0x110C, 0x0E10, 0xF1CE,
                0x313E, 0x1872, 0xE139, 0xD40F, 0x84F9, 0x280C, 0xA96A,
                0x4EC3
            };

        private static readonly int[,] EncryptionMatrix = new[,]
            {
                /* char 1  */ { 0xAEFC, 0x4DD9, 0x9BB2, 0x2745, 0x4E8A, 0x9D14, 0x2A09 },
                /* char 2  */ { 0x7B61, 0xF6C2, 0xFDA5, 0xEB6B, 0xC6F7, 0x9DCF, 0x2BBF },
                /* char 3  */ { 0x4563, 0x8AC6, 0x05AD, 0x0B5A, 0x16B4, 0x2D68, 0x5AD0 },
                /* char 4  */ { 0x0375, 0x06EA, 0x0DD4, 0x1BA8, 0x3750, 0x6EA0, 0xDD40 },
                /* char 5  */ { 0xD849, 0xA0B3, 0x5147, 0xA28E, 0x553D, 0xAA7A, 0x44D5 },
                /* char 6  */ { 0x6F45, 0xDE8A, 0xAD35, 0x4A4B, 0x9496, 0x390D, 0x721A },
                /* char 7  */ { 0xEB23, 0xC667, 0x9CEF, 0x29FF, 0x53FE, 0xA7FC, 0x5FD9 },
                /* char 8  */ { 0x47D3, 0x8FA6, 0x0F6D, 0x1EDA, 0x3DB4, 0x7B68, 0xF6D0 },
                /* char 9  */ { 0xB861, 0x60E3, 0xC1C6, 0x93AD, 0x377B, 0x6EF6, 0xDDEC },
                /* char 10 */ { 0x45A0, 0x8B40, 0x06A1, 0x0D42, 0x1A84, 0x3508, 0x6A10 },
                /* char 11 */ { 0xAA51, 0x4483, 0x8906, 0x022D, 0x045A, 0x08B4, 0x1168 },
                /* char 12 */ { 0x76B4, 0xED68, 0xCAF1, 0x85C3, 0x1BA7, 0x374E, 0x6E9C },
                /* char 13 */ { 0x3730, 0x6E60, 0xDCC0, 0xA9A1, 0x4363, 0x86C6, 0x1DAD },
                /* char 14 */ { 0x3331, 0x6662, 0xCCC4, 0x89A9, 0x0373, 0x06E6, 0x0DCC },
                /* char 15 */ { 0x1021, 0x2042, 0x4084, 0x8108, 0x1231, 0x2462, 0x48C4 }
            };

        private readonly WordprocessingDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentProtectionProvider"/> class.
        /// </summary>
        /// <param name="document">
        /// The document to which to add protection.
        /// </param>
        public DocumentProtectionProvider(WordprocessingDocument document)
        {
            this.document = document;
        }

        /// <summary>
        /// Makes the given document read-only.
        /// </summary>
        /// <param name="password">
        /// The password to protect the document with.
        /// </param>
        public void MakeReadOnly(string password)
        {
            // Generate the Salt
            var arrSalt = new byte[16];
            RandomNumberGenerator rand = new RNGCryptoServiceProvider();
            rand.GetNonZeroBytes(arrSalt);

            var generatedKey = GenerateKey(password);

            // Implementation Notes List:
            // --> In this third stage, the reversed byte order legacy hash from the second stage
            // shall be converted to Unicode hex
            // --> string representation
            var sb = new StringBuilder();
            for (int intTemp = 0; intTemp < 4; intTemp++)
            {
                sb.Append(Convert.ToString(generatedKey[intTemp], 16));
            }

            generatedKey = Encoding.Unicode.GetBytes(sb.ToString().ToUpper());

            // Implementation Notes List:
            // Word appends the binary form of the salt attribute and not the base64 string representation when hashing
            // Before calculating the initial hash, you are supposed to prepend (not append) the salt to the key
            byte[] tmpArray1 = generatedKey;
            byte[] tmpArray2 = arrSalt;
            var tempKey = new byte[tmpArray1.Length + tmpArray2.Length];
            Buffer.BlockCopy(tmpArray2, 0, tempKey, 0, tmpArray2.Length);
            Buffer.BlockCopy(tmpArray1, 0, tempKey, tmpArray2.Length, tmpArray1.Length);
            generatedKey = tempKey;

            // Iterations specifies the number of times the hashing function shall be iteratively run (using each
            // iteration's result as the input for the next iteration).
            const int Iterations = 50000;

            // Implementation Notes List:
            // Word requires that the initial hash of the password with the salt not be considered in the count.
            //    The initial hash of salt + key is not included in the iteration count.
            HashAlgorithm sha1 = new SHA1Managed();
            generatedKey = sha1.ComputeHash(generatedKey);
            var iterator = new byte[4];
            for (int intTmp = 0; intTmp < Iterations; intTmp++)
            {
                // When iterating on the hash, you are supposed to append the current iteration number.
                iterator[0] = Convert.ToByte((intTmp & 0x000000FF) >> 0);
                iterator[1] = Convert.ToByte((intTmp & 0x0000FF00) >> 8);
                iterator[2] = Convert.ToByte((intTmp & 0x00FF0000) >> 16);
                iterator[3] = Convert.ToByte((intTmp & 0xFF000000) >> 24);

                generatedKey = ConcatByteArrays(iterator, generatedKey);
                generatedKey = sha1.ComputeHash(generatedKey);
            }

            // Apply the element
            var documentProtection = new DocumentProtection { Edit = DocumentProtectionValues.ReadOnly };

            var docProtection = new OnOffValue(true);
            documentProtection.Enforcement = docProtection;

            documentProtection.CryptographicAlgorithmClass = CryptAlgorithmClassValues.Hash;
            documentProtection.CryptographicProviderType = CryptProviderValues.RsaFull;
            documentProtection.CryptographicAlgorithmType = CryptAlgorithmValues.TypeAny;
            documentProtection.CryptographicAlgorithmSid = 4; // SHA1
            //    The iteration count is unsigned
            var uintVal = new UInt32Value { Value = Iterations };
            documentProtection.CryptographicSpinCount = uintVal;
            documentProtection.Hash = Convert.ToBase64String(generatedKey);
            documentProtection.Salt = Convert.ToBase64String(arrSalt);
            this.document.MainDocumentPart.DocumentSettingsPart.Settings.AppendChild(documentProtection);
            this.document.MainDocumentPart.DocumentSettingsPart.Settings.Save();
        }

        private static byte[] GenerateKey(string password)
        {
            // Array to hold Key Values
            var generatedKey = new byte[4];

            // Maximum length of the password is 15 chars.
            const int MaxPasswordLength = 15;

            if (string.IsNullOrEmpty(password))
            {
                return generatedKey;
            }

            // Truncate the password to 15 characters
            password = password.Substring(0, Math.Min(password.Length, MaxPasswordLength));

            // Construct a new NULL-terminated string consisting of single-byte characters:
            //  -- > Get the single-byte values by iterating through the Unicode characters of the truncated Password.
            //   --> For each character, if the low byte is not equal to 0, take it. Otherwise, take the high byte.
            var chars = new byte[password.Length];
            for (int intLoop = 0; intLoop < password.Length; intLoop++)
            {
                int intTemp = Convert.ToInt32(password[intLoop]);
                chars[intLoop] = Convert.ToByte(intTemp & 0x00FF);
                if (chars[intLoop] == 0)
                {
                    chars[intLoop] = Convert.ToByte((intTemp & 0xFF00) >> 8);
                }
            }

            // Compute the high-order word of the new key:

            // --> Initialize from the initial code array (see below), depending on the password’s length.
            int intHighOrderWord = InitialCodeArray[chars.Length - 1];

            // --> For each character in the password:
            //      --> For every bit in the character, starting with the least significant and progressing
            //          to (but excluding) the most significant, if the bit is set, XOR the key’s high-order word
            //          with the corresponding word from the Encryption Matrix
            for (int intLoop = 0; intLoop < chars.Length; intLoop++)
            {
                int tmp = MaxPasswordLength - chars.Length + intLoop;
                for (int intBit = 0; intBit < 7; intBit++)
                {
                    if ((chars[intLoop] & (0x0001 << intBit)) != 0)
                    {
                        intHighOrderWord ^= EncryptionMatrix[tmp, intBit];
                    }
                }
            }

            // Compute the low-order word of the new key:
            int intLowOrderWord = 0;

            // For each character in the password, going backwards
            for (int intLoopChar = chars.Length - 1; intLoopChar >= 0; intLoopChar--)
            {
                // low-order word = (((low-order word SHR 14) AND 0x0001) OR (low-order word SHL 1) AND 0x7FFF))
                // XOR character
                intLowOrderWord = (((intLowOrderWord >> 14) & 0x0001) | ((intLowOrderWord << 1) & 0x7FFF))
                                  ^ chars[intLoopChar];
            }

            // Lastly,low-order word = (((low-order word SHR 14) AND 0x0001) OR (low-order word SHL 1) AND 0x7FFF))
            // XOR password length XOR 0xCE4B.
            intLowOrderWord = (((intLowOrderWord >> 14) & 0x0001) | ((intLowOrderWord << 1) & 0x7FFF))
                              ^ chars.Length ^ 0xCE4B;

            // Combine the Low and High Order Word
            int intCombinedkey = (intHighOrderWord << 16) + intLowOrderWord;

            // The byte order of the result shall be reversed [Example: 0x64CEED7E becomes 7EEDCE64. end example],
            // and that value shall be hashed as defined by the attribute values.
            for (int intTemp = 0; intTemp < 4; intTemp++)
            {
                generatedKey[intTemp] =
                    Convert.ToByte(((uint)(intCombinedkey & (0x000000FF << (intTemp * 8)))) >> (intTemp * 8));
            }

            return generatedKey;
        }

        private static byte[] ConcatByteArrays(byte[] array1, byte[] array2)
        {
            var result = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array2, 0, result, 0, array2.Length);
            Buffer.BlockCopy(array1, 0, result, array2.Length, array1.Length);
            return result;
        }
    }
}