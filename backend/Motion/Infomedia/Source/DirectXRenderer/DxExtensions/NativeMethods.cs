// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NativeMethods type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.DxExtensions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods for accessing native DLLs.
    /// </summary>
    internal static class NativeMethods
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// GDI value <code>FR_PRIVATE</code> (0x10).
        /// Used for <see cref="AddFontResourceEx"/>.
        /// </summary>
        public static readonly uint FR_PRIVATE = 0x10;

        /// <summary>
        /// GDI value <code>FR_NOT_ENUM</code> (0x20).
        /// Used for <see cref="AddFontResourceEx"/>.
        /// </summary>
        public static readonly uint FR_NOT_ENUM = 0x20;

        /// <summary>
        /// Retrieves the window styles.
        /// Used for <see cref="GetWindowLong"/>.
        /// </summary>
        public static readonly int GWL_STYLE = -16;

        /// <summary>
        /// Retrieves the extended window styles.
        /// Used for <see cref="GetWindowLong"/>.
        /// </summary>
        public static readonly int GWL_EXSTYLE = -20;

        /// <summary>
        /// The window is (initially) visible.
        /// Used with the <see cref="GetWindowLong"/> return value.
        /// </summary>
        public static readonly uint WS_VISIBLE = 0x10000000;

        /// <summary>
        /// The AddFontResourceEx function adds the font resource from the specified file to the system.
        /// Fonts added with the AddFontResourceEx function can be marked as private and not enumerable.
        /// </summary>
        /// <param name="filename">
        /// String that contains a valid font file name.
        /// </param>
        /// <param name="fl">
        /// The characteristics of the font to be added to the system.
        /// This parameter can be one of the following values:
        /// - <see cref="FR_PRIVATE"/>
        /// - <see cref="FR_NOT_ENUM"/>
        /// </param>
        /// <param name="pdv">
        /// Reserved. Must be zero.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value specifies the number of fonts added.
        /// If the function fails, the return value is zero. No extended error information is available.
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern int AddFontResourceEx(string filename, uint fl, IntPtr pdv);

        /// <summary>
        /// Removes the fonts in the specified file from the system font table.
        /// </summary>
        /// <param name="filename">
        /// String that contains a valid font file name.
        /// </param>
        /// <param name="fl">
        /// The characteristics of the font to be removed from the system.
        /// In order for the font to be removed, the flags used must be the same as when the font was added with the
        /// <see cref="AddFontResourceEx"/> function.
        /// </param>
        /// <param name="pdv">Reserved. Must be zero.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. No extended error information is available.
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern int RemoveFontResourceEx(string filename, uint fl, IntPtr pdv);

        /// <summary>
        /// Retrieves information about the specified window.
        /// The function also retrieves the 32-bit (DWORD) value at the specified
        /// offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window belongs.
        /// </param>
        /// <param name="nIndex">
        /// The zero-based offset to the value to be retrieved.
        /// Valid values are in the range zero through the number of bytes of extra window memory, minus four.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the requested value.
        /// If the function fails, the return value is zero.
        /// To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        // ReSharper restore InconsistentNaming
    }
}
