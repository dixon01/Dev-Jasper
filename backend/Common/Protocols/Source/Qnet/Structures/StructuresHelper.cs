// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuresHelper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Helper class for structures
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System;
    using System.Text;

    /// <summary>
    /// Helper class for structures
    /// </summary>
    public class StructuresHelper
    {
        /// <summary>
        /// Copy the specified source string to the specified byte array.
        /// </summary>
        /// <param name="src">
        /// The string source to copy from.
        /// </param>
        /// <param name="dest">
        /// The destination byte array to copy into.
        /// </param>
        /// <param name="lenght">
        /// The maximum lenght to copy if src lenght is greater that the specified lenght.
        /// </param>
        public static unsafe void UnsafeCopyStringToByteArrayPtr(string src, byte* dest, int lenght)
        {
            var buffer = Encoding.Default.GetBytes(src);
            var len = Math.Min(src.Length, lenght);
            byte* ps = dest;
            for (int i = 0; i < len; i++)
            {
                *ps = buffer[i];
                ps++;
            }
        }

        /// <summary>
        /// Convert the specified pointer on byte array that should be represent a string into a string
        /// </summary>
        /// <param name="array">
        /// The pointer on byte array.
        /// </param>
        /// <param name="lenght">
        /// The lenght of the specidfied array of bytes.
        /// </param>
        /// <returns>
        /// The converted string.
        /// </returns>
        public static unsafe string UnsafeCopyByteArrayPtrToString(byte* array, int lenght)
        {
            var text = new byte[lenght];
            var len = 0;
            byte* ps = array;
            for (int i = 0; i < lenght; i++)
            {
                if (*ps == '\0')
                {
                    break;
                }

                len = i + 1;
                text[i] = *ps;
                ps++;
            }

            return Encoding.Default.GetString(text).Substring(0, len);
        }
    }
}
