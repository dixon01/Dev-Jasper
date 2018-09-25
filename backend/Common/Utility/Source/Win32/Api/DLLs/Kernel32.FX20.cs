// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Kernel32.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Kernel32 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Wrapper for the <c>kernel32.dll</c>.
    /// </summary>
    public static partial class Kernel32
    {
        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="destination">
        /// A pointer to the starting address of the move destination.
        /// </param>
        /// <param name="source">
        /// A pointer to the starting address of the block of memory to be moved.
        /// </param>
        /// <param name="length">
        /// The size of the block of memory to move, in bytes.
        /// </param>
        /// <remarks>
        /// This function is defined as the <c>RtlMoveMemory</c> function. Its C implementation is provided inline.
        /// </remarks>
        public static void MoveMemory(IntPtr destination, IntPtr source, int length)
        {
            RtlMoveMemory(destination, source, length);
        }

        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="destination">
        /// A pointer to the starting address of the move destination.
        /// </param>
        /// <param name="source">
        /// A pointer to the starting address of the block of memory to be moved.
        /// </param>
        /// <param name="length">
        /// The size of the block of memory to move, in bytes.
        /// </param>
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void RtlMoveMemory(IntPtr destination, IntPtr source, int length);
    }
}