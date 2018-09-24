// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUtility.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Utility
{
    using System.IO;

    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Utility class for handling files.
    /// </summary>
    public class FileUtility
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FileUtility>();

        private readonly IWritableFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileUtility"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        public FileUtility(IWritableFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Deletes the given directory recursively, making sure all files are deleted first.
        /// </summary>
        /// <param name="directory">
        /// The directory to delete.
        /// </param>
        public static void DeleteDirectory(IWritableDirectoryInfo directory)
        {
            ClearDirectory(directory);

            Logger.Trace("Deleting directory '{0}'", directory.FullName);
            directory.Delete();
        }

        /// <summary>
        /// Clears the given directory recursively, making sure all files
        /// and folders inside the given directory are deleted.
        /// </summary>
        /// <param name="directory">
        /// The directory to clear.
        /// </param>
        public static void ClearDirectory(IWritableDirectoryInfo directory)
        {
            foreach (var subdir in directory.GetDirectories())
            {
                DeleteDirectory(subdir);
            }

            foreach (var file in directory.GetFiles())
            {
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Copies the given source directory to the target directory recursively,
        /// making sure the copied files don't have any special file attributes.
        /// </summary>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="target">
        /// The target directory.
        /// </param>
        public static void CopyDirectory(IWritableDirectoryInfo source, string target)
        {
            Logger.Trace("Copying directory '{0}' to '{1}'", source.FullName, target);

            source.FileSystem.CreateDirectory(target);

            foreach (var file in source.GetFiles())
            {
                var fileTarget = Path.Combine(target, file.Name);
                Logger.Trace("Copying file '{0}' to '{1}'", file.FullName, fileTarget);
                var targetInfo = file.CopyTo(fileTarget);
                targetInfo.Attributes = FileAttributes.Normal;
            }

            foreach (var directory in source.GetDirectories())
            {
                CopyDirectory(directory, Path.Combine(target, directory.Name));
            }
        }

        /// <summary>
        /// Moves recursively all the files from one directory to another directory.
        /// </summary>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="target">
        /// The target directory.
        /// </param>
        public static void MoveDirectoryContents(IWritableDirectoryInfo source, string target)
        {
            Logger.Trace("Moving contents from '{0}' to '{1}'", source.FullName, target);
            source.FileSystem.CreateDirectory(target);
            foreach (var file in source.GetFiles())
            {
                var fileTarget = Path.Combine(target, file.Name);
                Logger.Trace("Moving file '{0}' to '{1}'", file.FullName, fileTarget);
                file.Attributes = FileAttributes.Normal;
                file.MoveTo(fileTarget);
            }

            foreach (var directory in source.GetDirectories())
            {
                MoveDirectoryContents(directory, Path.Combine(target, directory.Name));
            }
        }

        /// <summary>
        /// Securely deletes the given file by first resetting its file attributes.
        /// </summary>
        /// <param name="file">
        /// The file to delete.
        /// </param>
        public static void DeleteFile(IWritableFileInfo file)
        {
            Logger.Trace("Deleting file '{0}'", file.FullName);
            file.Attributes = FileAttributes.Normal;
            file.Delete();
        }

        /// <summary>
        /// Deletes the given directory recursively, making sure all files are deleted first.
        /// </summary>
        /// <param name="directory">
        /// The directory to delete.
        /// </param>
        public void DeleteDirectory(string directory)
        {
            IWritableDirectoryInfo dir;
            if (this.fileSystem.TryGetDirectory(directory, out dir))
            {
                DeleteDirectory(dir);
            }
        }

        /// <summary>
        /// Clears the given directory recursively, making sure all files
        /// and folders inside the given directory are deleted.
        /// </summary>
        /// <param name="directory">
        /// The directory to clear.
        /// </param>
        public void ClearDirectory(string directory)
        {
            IWritableDirectoryInfo dir;
            if (this.fileSystem.TryGetDirectory(directory, out dir))
            {
                ClearDirectory(dir);
            }
        }

        /// <summary>
        /// Copies the given source directory to the target directory recursively,
        /// making sure the copied files don't have any special file attributes.
        /// </summary>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="target">
        /// The target directory.
        /// </param>
        public void CopyDirectory(string source, string target)
        {
            IWritableDirectoryInfo sourceDir;
            if (this.fileSystem.TryGetDirectory(source, out sourceDir))
            {
                CopyDirectory(sourceDir, target);
            }
        }

        /// <summary>
        /// Moves a directory to a new place.
        /// </summary>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="target">
        /// The target directory.
        /// </param>
        public void MoveDirectory(string source, string target)
        {
            IWritableDirectoryInfo sourceDir;
            if (this.fileSystem.TryGetDirectory(source, out sourceDir))
            {
                sourceDir.MoveTo(target);
            }
        }

        /// <summary>
        /// Moves recursively all the files from one directory to another directory.
        /// </summary>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="target">
        /// The target directory.
        /// </param>
        public void MoveDirectoryContents(string source, string target)
        {
            IWritableDirectoryInfo sourceDir;
            if (this.fileSystem.TryGetDirectory(source, out sourceDir))
            {
                MoveDirectoryContents(sourceDir, target);
            }
        }

        /// <summary>
        /// Securely deletes the given file by first resetting its file attributes.
        /// </summary>
        /// <param name="filePath">
        /// The path to the file to delete.
        /// </param>
        public void DeleteFile(string filePath)
        {
            IWritableFileInfo file;
            if (this.fileSystem.TryGetFile(filePath, out file))
            {
                DeleteFile(file);
            }
        }
    }
}
