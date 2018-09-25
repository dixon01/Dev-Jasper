// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemMock.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemMock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Utility
{
    using System.IO;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Helper methods to mock file system operations.
    /// </summary>
    public static class FileSystemMock
    {
        /// <summary>
        /// Creates a file with the given contents.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="filePath">
        /// The file path (either relative to to the root or absolute).
        /// </param>
        /// <param name="contents">
        /// The contents of the file.
        /// </param>
        public static void CreateFile(IWritableDirectoryInfo root, string filePath, string contents)
        {
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(root.FullName, filePath);
            root.FileSystem.CreateDirectory(Path.GetDirectoryName(fullPath));
            var file = root.FileSystem.CreateFile(fullPath);
            using (var output = new StreamWriter(file.OpenWrite()))
            {
                output.Write(contents);
            }
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="dirPath">
        /// The directory path (either relative to to the root or absolute).
        /// </param>
        public static void CreateDirectory(IWritableDirectoryInfo root, string dirPath)
        {
            var fullPath = Path.IsPathRooted(dirPath) ? dirPath : Path.Combine(root.FullName, dirPath);
            root.FileSystem.CreateDirectory(fullPath);
        }

        /// <summary>
        /// Gets the contents of a file.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="filePath">
        /// The file path (either relative to to the root or absolute).
        /// </param>
        /// <returns>
        /// The contents of the file in a single <see cref="string"/>.
        /// </returns>
        public static string GetFile(IDirectoryInfo root, string filePath)
        {
            var path = Path.IsPathRooted(filePath) ? filePath : Path.Combine(root.FullName, filePath);
            using (var reader = new StreamReader(root.FileSystem.GetFile(path).OpenRead()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Changes the contents of a file.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="filePath">
        /// The file path (either relative to to the root or absolute).
        /// </param>
        /// <param name="contents">
        /// The new contents of the file.
        /// </param>
        public static void ChangeFile(IWritableDirectoryInfo root, string filePath, string contents)
        {
            var path = Path.IsPathRooted(filePath) ? filePath : Path.Combine(root.FullName, filePath);
            using (var writer = new StreamWriter(root.FileSystem.GetFile(path).OpenWrite()))
            {
                writer.Write(contents);
            }
        }
    }
}