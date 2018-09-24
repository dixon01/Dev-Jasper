// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellFileInfo.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShellFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Wrapper
{
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Api.Structs;

    /// <summary>
    /// Helper class that gives access to Windows shell (Explorer) file information.
    /// </summary>
    public static partial class ShellFileInfo
    {
        /// <summary>
        /// Gets the icon for the given file name.
        /// </summary>
        /// <param name="name">
        /// The name of the file. If the file does not exist, the icon will be chosen by its extension.
        /// </param>
        /// <param name="largeIcon">
        /// A flag indicating if the large 32x32 icon should be returned. If false, the 16x16 icon is returned.
        /// </param>
        /// <param name="linkOverlay">
        /// A flag indicating if the the link overlay is to be added to the icon.
        /// </param>
        /// <returns>
        /// The <see cref="Icon"/> which is to be disposed of by the caller.
        /// </returns>
        public static Icon GetFileIcon(string name, bool largeIcon, bool linkOverlay)
        {
            var fileInfo = new ShFileInfo();
            var flags = ShGetFileInfoFlags.Icon | ShGetFileInfoFlags.UseFileAttributes;

            if (linkOverlay)
            {
                flags |= ShGetFileInfoFlags.LinkOverlay;
            }

            if (largeIcon)
            {
                flags |= ShGetFileInfoFlags.LargeIcon; // include the large icon flag
            }
            else
            {
                flags |= ShGetFileInfoFlags.SmallIcon; // include the small icon flag
            }

            Shell32.SHGetFileInfo(
                name,
                FileAttributes.Normal,
                ref fileInfo,
                (uint)Marshal.SizeOf(fileInfo),
                flags);

            // Copy (clone) the returned icon to a new object, thus allowing us
            // to call DestroyIcon immediately
            var icon = (Icon)Icon.FromHandle(fileInfo.IconHandle).Clone();
            User32.DestroyIcon(fileInfo.IconHandle); // Cleanup
            return icon;
        }
    }
}
