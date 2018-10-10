// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell32.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell32 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Api.Structs;

    /// <summary>
    /// Wrapper for the <c>shell32.dll</c>.
    /// </summary>
    public static partial class Shell32
    {
        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="path">
        /// String that contains the path and file name. Both absolute and relative paths are valid.
        /// </param>
        /// <param name="fileAttributes">
        /// A combination of one or more file attribute flags.
        /// If <paramref name="flags"/> does not include the <see cref="ShGetFileInfoFlags.UseFileAttributes"/> flag,
        /// this parameter is ignored.
        /// </param>
        /// <param name="fileInfo">
        /// Pointer to a <see cref="ShFileInfo"/> structure to receive the file information.
        /// </param>
        /// <param name="fileInfoSize">
        /// The size, in bytes, of the <see cref="ShFileInfo"/> structure pointed to
        /// by the <paramref name="fileInfo"/> parameter.
        /// </param>
        /// <param name="flags">
        /// The flags that specify the file information to retrieve.
        /// </param>
        /// <returns>
        /// Returns a value whose meaning depends on the <paramref name="flags"/> parameter.
        /// If <paramref name="flags"/> does not contain <see cref="ShGetFileInfoFlags.ExeType"/> or
        /// <see cref="ShGetFileInfoFlags.SysIconIndex"/>, the return value is nonzero if successful, or zero otherwise.
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(
            string path,
            FileAttributes fileAttributes,
            ref ShFileInfo fileInfo,
            uint fileInfoSize,
            ShGetFileInfoFlags flags);
    }
}
