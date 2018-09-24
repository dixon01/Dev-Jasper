// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileIconManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileIconManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// Manager for file icons to be used with <see cref="ListView"/>.
    /// </summary>
    public class FileIconManager
    {
        private readonly Dictionary<string, int> icons =
            new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ImageList smallImages;
        private readonly ImageList largeImages;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIconManager"/> class.
        /// </summary>
        /// <param name="smallImages">
        /// The small image list (<see cref="ListView.SmallImageList"/>).
        /// </param>
        /// <param name="largeImages">
        /// The large image list (<see cref="ListView.LargeImageList"/>).
        /// </param>
        public FileIconManager(ImageList smallImages, ImageList largeImages)
        {
            smallImages.ImageSize = new Size(16, 16);
            smallImages.ColorDepth = ColorDepth.Depth32Bit;

            largeImages.ImageSize = new Size(32, 32);
            largeImages.ColorDepth = ColorDepth.Depth32Bit;

            this.smallImages = smallImages;
            this.largeImages = largeImages;
        }

        /// <summary>
        /// Creates an icon for a file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="small">
        /// A flag indicating if a small or large icon is required.
        /// </param>
        /// <returns>
        /// The <see cref="Icon"/>.
        /// </returns>
        public static Icon CreateFileIcon(string filePath, bool small)
        {
            return CreateIcon(filePath, NativeMethods.FILE_ATTRIBUTE_NORMAL, small);
        }

        /// <summary>
        /// Creates an icon for a folder.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path.
        /// </param>
        /// <param name="small">
        /// A flag indicating if a small or large icon is required.
        /// </param>
        /// <returns>
        /// The <see cref="Icon"/>.
        /// </returns>
        public static Icon CreateFolderIcon(string folderPath, bool small)
        {
            return CreateIcon(folderPath, NativeMethods.FILE_ATTRIBUTE_DIRECTORY, small);
        }

        /// <summary>
        /// Adds the icon for the given path to the lists or returns an existing
        /// index if the icon already exists.
        /// </summary>
        /// <param name="filePath">
        /// The file path (this doesn't have to be a valid path).
        /// </param>
        /// <returns>
        /// The index into the small and large image icon list.
        /// </returns>
        public int AddFileIcon(string filePath)
        {
            return this.AddIcon(filePath, NativeMethods.FILE_ATTRIBUTE_NORMAL);
        }

        /// <summary>
        /// Adds the icon for the given path to the lists or returns an existing
        /// index if the icon already exists.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path (this doesn't have to be a valid path).
        /// </param>
        /// <returns>
        /// The index into the small and large image icon list.
        /// </returns>
        public int AddFolderIcon(string folderPath)
        {
            return this.AddIcon(folderPath, NativeMethods.FILE_ATTRIBUTE_DIRECTORY);
        }

        private static Icon CreateIcon(string filePath, uint attributes, bool small)
        {
            var info = new NativeMethods.SHFILEINFO
                           {
                               hIcon = IntPtr.Zero,
                               iIcon = 0,
                               dwAttributes = 0,
                               szDisplayName = string.Empty,
                               szTypeName = string.Empty
                           };

            int sizeOfInfo = Marshal.SizeOf(info);
            NativeMethods.SHGFI flags;
            if (small)
            {
                flags = NativeMethods.SHGFI.Icon
                    | NativeMethods.SHGFI.SmallIcon
                    | NativeMethods.SHGFI.UseFileAttributes;
            }
            else
            {
                flags = NativeMethods.SHGFI.Icon
                    | NativeMethods.SHGFI.LargeIcon
                    | NativeMethods.SHGFI.UseFileAttributes;
            }

            NativeMethods.SHGetFileInfo(filePath, attributes, out info, (uint)sizeOfInfo, flags);
            return Icon.FromHandle(info.hIcon);
        }

        private int AddIcon(string path, uint attributes)
        {
            int index;
            if (this.icons.TryGetValue(path, out index))
            {
                return index;
            }

            index = this.smallImages.Images.Count;
            this.smallImages.Images.Add(CreateIcon(path, attributes, true));
            this.largeImages.Images.Add(CreateIcon(path, attributes, false));
            return index;
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable UnusedMember.Local
            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

            /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
            private const int MaxPath = 260;

            /// <summary>Maximal Length of unmanaged type name</summary>
            private const int MaxType = 80;

            [Flags]
            public enum SHGFI
            {
                /// <summary>get icon</summary>
                Icon = 0x000000100,

                /// <summary>get display name</summary>
                DisplayName = 0x000000200,

                /// <summary>get type name</summary>
                TypeName = 0x000000400,

                /// <summary>get attributes</summary>
                Attributes = 0x000000800,

                /// <summary>get icon location</summary>
                IconLocation = 0x000001000,

                /// <summary>return exe type</summary>
                ExeType = 0x000002000,

                /// <summary>get system icon index</summary>
                SysIconIndex = 0x000004000,

                /// <summary>put a link overlay on icon</summary>
                LinkOverlay = 0x000008000,

                /// <summary>show icon in selected state</summary>
                Selected = 0x000010000,

                /// <summary>get only specified attributes</summary>
                Attr_Specified = 0x000020000,

                /// <summary>get large icon</summary>
                LargeIcon = 0x000000000,

                /// <summary>get small icon</summary>
                SmallIcon = 0x000000001,

                /// <summary>get open icon</summary>
                OpenIcon = 0x000000002,

                /// <summary>get shell size icon</summary>
                ShellIconSize = 0x000000004,

                /// <summary>path is a PIDL</summary>
                PIDL = 0x000000008,

                /// <summary>use passed file attribute</summary>
                UseFileAttributes = 0x000000010,

                /// <summary>apply the appropriate overlays</summary>
                AddOverlays = 0x000000020,

                /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
                OverlayIndex = 0x000000040,
            }

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern int SHGetFileInfo(
                string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;

                public int iIcon;

                public uint dwAttributes;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
                public string szDisplayName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxType)]
                public string szTypeName;
            }

            // ReSharper restore UnusedMember.Local
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local
            // ReSharper restore InconsistentNaming
        }
    }
}
