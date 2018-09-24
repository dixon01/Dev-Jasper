// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IByteAccess.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IByteAccess type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    /// <summary>
    /// Interface to access bytes.
    /// This interface was introduced to later support memory mapped files if needed,
    /// because then reading big chunks is unnecessary (they can be accessed directly from the memory map).
    /// </summary>
    internal interface IByteAccess
    {
        /// <summary>
        /// Gets the length of this byte access.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the byte value at the given <see cref="index"/>.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> at the given <see cref="index"/>.
        /// </returns>
        byte this[int index] { get; }

        /// <summary>
        /// Converts this object to a byte array.
        /// Be careful when using this on a large memory-mapped <see cref="IByteAccess"/>
        /// since this might be creating a large byte array that might even end up on the LOH.
        /// </summary>
        /// <returns>
        /// The byte array.
        /// </returns>
        byte[] ToArray();
    }
}