// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileInfoExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Files;

    using NLog;

    using Shell32;

    /// <summary>
    /// Extension methods for <see cref="IFileInfo"/>.
    /// </summary>
    public static class FileInfoExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the duration of the file in the format 'minute(s):seconds'. Only tested on Windows 7.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <param name="shellAppType">
        /// The shell application type.
        /// </param>
        /// <param name="shell">
        /// The shell32 object.
        /// </param>
        /// <returns>
        /// The duration in the format 'minute(s):seconds'. If no duration could be found, '?' is returned.
        /// </returns>
        public static string GetDuration(this IFileInfo fileInfo, Type shellAppType, object shell)
        {
            Folder folder = null;
            FolderItem folderItem = null;
            var result = "?";
            try
            {
                folder =
                    (Folder)
                    shellAppType.InvokeMember(
                        "NameSpace",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null,
                        shell,
                        new object[] { fileInfo.Directory.FullName });

                folderItem = folder.ParseName(fileInfo.Name);
                var length = folder.GetDetailsOf(folderItem, 27);
                TimeSpan duration;
                if (TimeSpan.TryParse(length, out duration))
                {
                    result = string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting duration of a video file.", exception);
            }
            finally
            {
                if (folder != null)
                {
                    Marshal.ReleaseComObject(folder);
                }

                if (folderItem != null)
                {
                    Marshal.ReleaseComObject(folderItem);
                }
            }

            if (result.Equals(":"))
            {
                result = "?";
            }

            return result;
        }

        /// <summary>
        /// Gets the dimensions of a file in the format 'Width x Height'.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <param name="shellAppType">
        /// The shell application type.
        /// </param>
        /// <param name="shell">
        /// The shell32 object.
        /// </param>
        /// <returns>
        /// The dimensions in the format 'Width x Height'. If the information could not be found, '?' is returned.
        /// </returns>
        public static string GetDimensions(this IFileInfo fileInfo, Type shellAppType, object shell)
        {
            Folder folder = null;
            FolderItem2 folderItem = null;
            var dimensions = "?";
            try
            {
                folder =
                    (Folder)
                    shellAppType.InvokeMember(
                        "NameSpace",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null,
                        shell,
                        new object[] { fileInfo.Directory.FullName });
                folderItem = (FolderItem2)folder.Items().Item(fileInfo.Name);
                dimensions = (string)folderItem.ExtendedProperty("Dimensions");
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting dimensions of an image file.", exception);
            }
            finally
            {
                if (folder != null)
                {
                    Marshal.ReleaseComObject(folder);
                }

                if (folderItem != null)
                {
                    Marshal.ReleaseComObject(folderItem);
                }
            }

            return dimensions;
        }

        /// <summary>
        /// Gets the dimensions of a file in the format 'Width x Height'.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <param name="shellAppType">
        /// The shell application type.
        /// </param>
        /// <param name="shell">
        /// The shell32 object.
        /// </param>
        /// <returns>
        /// The dimensions in the format 'Width x Height'. If the information could not be found, '?' is returned.
        /// </returns>
        public static string GetVideoDimensions(this IFileInfo fileInfo, Type shellAppType, object shell)
        {
            Folder folder = null;
            FolderItem folderItem = null;
            var dimensions = "?";
            var os = Environment.OSVersion;
            var isWindows8OrNewer = os.Platform == PlatformID.Win32NT &&
                   (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
            try
            {
                folder =
                    (Folder)
                    shellAppType.InvokeMember(
                        "NameSpace",
                        System.Reflection.BindingFlags.InvokeMethod,
                        null,
                        shell,
                        new object[] { fileInfo.Directory.FullName });
                folderItem = folder.ParseName(fileInfo.Name);
                string frameHeight;
                string frameWidth;
                if (isWindows8OrNewer)
                {
                    frameHeight = folder.GetDetailsOf(folderItem, 299);
                    frameWidth = folder.GetDetailsOf(folderItem, 301);
                }
                else
                {
                    frameHeight = folder.GetDetailsOf(folderItem, 283);
                    frameWidth = folder.GetDetailsOf(folderItem, 285);
                }

                if (frameHeight.Equals(string.Empty))
                {
                    frameHeight = "?";
                }

                if (frameWidth.Equals(string.Empty))
                {
                    frameWidth = "?";
                }

                dimensions = string.Format("{0} x {1}", frameWidth, frameHeight);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting dimensions of a video file.", exception);
            }
            finally
            {
                if (folder != null)
                {
                    Marshal.ReleaseComObject(folder);
                }

                if (folderItem != null)
                {
                    Marshal.ReleaseComObject(folderItem);
                }
            }

            return dimensions;
        }
    }
}
