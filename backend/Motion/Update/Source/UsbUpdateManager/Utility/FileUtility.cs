// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUtility.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Utility
{
    /// <summary>
    /// Utility class for file specific tasks.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// The number of bytes in a kilobyte (1'024).
        /// </summary>
        public static readonly double KiloBytes = 1024;

        /// <summary>
        /// The number of bytes in a megabyte (1'048'576).
        /// </summary>
        public static readonly double MegaBytes = 1024 * 1024;

        /// <summary>
        /// Gets the file size string for a given number of bytes.
        /// </summary>
        /// <param name="size">
        /// The size in bytes.
        /// </param>
        /// <returns>
        /// The string representing the size (in bytes, KB or MB).
        /// </returns>
        public static string GetFileSizeString(long size)
        {
            if (size < KiloBytes)
            {
                return string.Format("{0} bytes", size);
            }

            if (size < MegaBytes)
            {
                return string.Format("{0:0.00} KB", size / KiloBytes);
            }

            return string.Format("{0:0.00} MB", size / MegaBytes);
        }
    }
}
