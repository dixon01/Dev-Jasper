// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamCopy.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamCopy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Utility class to copy streams.
    /// </summary>
    public static class StreamCopy
    {
        /// <summary>
        /// Copies all data available in <see cref="input"/> to output
        /// using a buffer of 4096 bytes.
        /// </summary>
        /// <param name="input">
        /// The input stream to read from.
        /// </param>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        public static void Copy(Stream input, Stream output)
        {
            Copy(input, output, 4096);
        }

        /// <summary>
        /// Copies all data available in <see cref="input"/> to output
        /// using a buffer of the given size.
        /// </summary>
        /// <param name="input">
        /// The input stream to read from.
        /// </param>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size to use.
        /// </param>
        public static void Copy(Stream input, Stream output, int bufferSize)
        {
            if (bufferSize >= 80000)
            {
                throw new ArgumentException(
                    "Buffer size can't be bigger than 80'000 bytes otherwise it ends up on the LOH");
            }

            var buffer = new byte[bufferSize];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
    }
}
