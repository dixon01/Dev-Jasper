// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a file object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShFileInfo
    {
        /// <summary>
        /// A handle to the icon that represents the file.
        /// You are responsible for destroying this handle with
        /// <see cref="Gorba.Common.Utility.Win32.Api.DLLs.User32.DestroyIcon"/> when you no longer need it.
        /// </summary>
        public IntPtr IconHandle;

        /// <summary>
        /// The index of the icon image within the system image list.
        /// </summary>
        public int IconIndex;

        /// <summary>
        /// An array of values that indicates the attributes of the file object.
        /// </summary>
        public uint Attributes;

        /// <summary>
        /// A string that contains the name of the file as it appears in the Windows Shell,
        /// or the path and file name of the file that contains the icon representing the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string DisplayName;

        /// <summary>
        /// A string that describes the type of file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string TypeName;
    }
}
