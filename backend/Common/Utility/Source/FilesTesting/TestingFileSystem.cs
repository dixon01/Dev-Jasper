// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// <see cref="IFileSystem"/> and <see cref="IWritableFileSystem"/>
    /// implementation that allows for the creation of virtual files that are just kept in memory.
    /// </summary>
    public class TestingFileSystem : IWritableFileSystem
    {
        private readonly List<TestingDriveInfo> drives = new List<TestingDriveInfo>();

        private readonly Dictionary<string, IWritableFileSystemInfo> items =
            new Dictionary<string, IWritableFileSystemInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingFileSystem"/> class.
        /// </summary>
        public TestingFileSystem()
        {
            for (var name = 'A'; name <= 'Z'; name++)
            {
                var drive = new TestingDriveInfo(this, name);
                this.drives.Add(drive);
                this.items.Add(drive.RootDirectory.FullName, drive.RootDirectory);
            }
        }

        /// <summary>
        /// Creates a dump as a string of the entire file system.
        /// This is useful for debugging to verify the exact structure.
        /// </summary>
        /// <returns>
        /// The file system hierarchy.
        /// </returns>
        public string Dump()
        {
            var builder = new StringBuilder();
            foreach (var drive in this.drives)
            {
                builder.Append(drive.Name).AppendLine(":\\");
                this.AppendDump(builder, drive.RootDirectory, 0);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        public IDriveInfo[] GetDrives()
        {
            return this.drives.Cast<IDriveInfo>().ToArray();
        }

        /// <summary>
        /// Gets a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IFileInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// if the given file can't be found.
        /// </exception>
        public IFileInfo GetFile(string path)
        {
            return ((IWritableFileSystem)this).GetFile(path);
        }

        /// <summary>
        /// Tries to get a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="file">
        /// The <see cref="IFileInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the file was found.
        /// </returns>
        public bool TryGetFile(string path, out IFileInfo file)
        {
            IWritableFileInfo writable;
            this.TryGetFile(path, out writable);
            file = writable;
            return file != null;
        }

        /// <summary>
        /// Tries to get a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="file">
        /// The <see cref="IWritableFileInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the file was found.
        /// </returns>
        public bool TryGetFile(string path, out IWritableFileInfo file)
        {
            IWritableFileSystemInfo item;
            this.items.TryGetValue(path, out item);
            file = item as IWritableFileInfo;

            return file != null;
        }

        /// <summary>
        /// Gets a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// if the given directory can't be found.
        /// </exception>
        public IDirectoryInfo GetDirectory(string path)
        {
            return ((IWritableFileSystem)this).GetDirectory(path);
        }

        /// <summary>
        /// Tries to get a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="directory">
        /// The <see cref="IDirectoryInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the directory was found.
        /// </returns>
        public bool TryGetDirectory(string path, out IDirectoryInfo directory)
        {
            IWritableDirectoryInfo writable;
            this.TryGetDirectory(path, out writable);
            directory = writable;
            return directory != null;
        }

        /// <summary>
        /// Tries to get a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <param name="directory">
        /// The <see cref="IWritableDirectoryInfo"/> if found.
        /// </param>
        /// <returns>
        /// True if the directory was found.
        /// </returns>
        public bool TryGetDirectory(string path, out IWritableDirectoryInfo directory)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }

            IWritableFileSystemInfo item;
            this.items.TryGetValue(path, out item);
            directory = item as IWritableDirectoryInfo;

            return directory != null;
        }

        /// <summary>
        /// Gets or creates a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IFileInfo"/>.
        /// </returns>
        public IWritableFileInfo CreateFile(string path)
        {
            IWritableFileSystemInfo item;
            if (!this.items.TryGetValue(path, out item))
            {
                var parent = this.GetDirectory(path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar) + 1));
                if (parent == null)
                {
                    throw new DirectoryNotFoundException("Parent directory doesn't exist for " + path);
                }

                item = new TestingFileInfo(this, (IWritableDirectoryInfo)parent, path);
                this.items.Add(path, item);
            }
            else if (item is IWritableDirectoryInfo)
            {
                throw new IOException("Can't create file because a directory with the same name exists: " + path);
            }

            return (IWritableFileInfo)item;
        }

        /// <summary>
        /// Gets or creates file a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryInfo"/>.
        /// </returns>
        public IWritableDirectoryInfo CreateDirectory(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }

            IWritableFileSystemInfo item;
            if (!this.items.TryGetValue(path, out item))
            {
                var parent = path.Substring(
                    0,
                    path.LastIndexOf(Path.DirectorySeparatorChar, path.Length - 2, path.Length - 1) + 1);
                this.CreateDirectory(parent);

                item = new TestingDirectoryInfo(this, path);
                this.items.Add(path, item);
            }
            else if (item is IWritableFileInfo)
            {
                throw new IOException("Can't create file because a directory with the same name exists: " + path);
            }

            return (IWritableDirectoryInfo)item;
        }

        IWritableFileInfo IWritableFileSystem.GetFile(string path)
        {
            IWritableFileInfo file;
            if (!this.TryGetFile(path, out file))
            {
                throw new FileNotFoundException("Couldn't find file", path);
            }

            return file;
        }

        IWritableDirectoryInfo IWritableFileSystem.GetDirectory(string path)
        {
            IWritableDirectoryInfo directory;
            if (!this.TryGetDirectory(path, out directory))
            {
                throw new DirectoryNotFoundException("Couldn't find directory " + path);
            }

            return directory;
        }

        IWritableDriveInfo[] IWritableFileSystem.GetDrives()
        {
            return this.drives.Cast<IWritableDriveInfo>().ToArray();
        }

        /// <summary>
        /// Deletes the given directory.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        public void Delete(TestingDirectoryInfo directory)
        {
            var keys =
                this.items.Keys.Where(
                    f => f.StartsWith(directory.FullName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            foreach (var key in keys)
            {
                this.items.Remove(key);
            }
        }

        /// <summary>
        /// Deletes the given file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public void Delete(TestingFileInfo file)
        {
            this.items.Remove(file.FullName);
        }

        /// <summary>
        /// Moves the given directory to a new location.
        /// </summary>
        /// <param name="directory">
        /// The directory to move.
        /// </param>
        /// <param name="newDirectoryName">
        /// The new location where the directory should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableDirectoryInfo"/> that describes this directory at the
        /// new location.
        /// </returns>
        public IWritableDirectoryInfo Move(TestingDirectoryInfo directory, string newDirectoryName)
        {
            if (!newDirectoryName.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                newDirectoryName += Path.DirectorySeparatorChar;
            }

            if (this.ItemExists(newDirectoryName))
            {
                throw new IOException("Directory already exists: " + newDirectoryName);
            }

            var toRename =
                this.items.Where(
                    i => i.Key.StartsWith(directory.FullName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            foreach (var item in toRename)
            {
                this.items.Remove(item.Key);
                var name = newDirectoryName + item.Key.Substring(directory.FullName.Length);
                ((TestingFileSystemInfo)item.Value).Rename(name);
                this.items.Add(name, item.Value);
            }

            return ((IWritableFileSystem)this).GetDirectory(newDirectoryName);
        }

        /// <summary>
        /// Gets all files and directories for a given directory.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// A list of all immediate children of the given directory.
        /// </returns>
        public IEnumerable<IWritableFileSystemInfo> GetFileSystemInfos(TestingDirectoryInfo parent)
        {
            foreach (var item in this.items.Where(p => p.Key.StartsWith(parent.FullName)).Select(p => p.Value))
            {
                if (item.FullName.Equals(parent.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (item.FullName.LastIndexOf(
                    Path.DirectorySeparatorChar,
                    item.FullName.Length - 2,
                    item.FullName.Length - 1) == parent.FullName.Length - 1)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Checks if the given path exists.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// True if there is a file or a directory at the given path.
        /// </returns>
        public bool ItemExists(string path)
        {
            IWritableFileSystemInfo item;
            return this.items.TryGetValue(path, out item);
        }

        private void AppendDump(StringBuilder builder, IDirectoryInfo directory, int indent)
        {
            foreach (var info in directory.GetFileSystemInfos())
            {
                var file = info as IFileInfo;
                if (file != null)
                {
                    builder.Append(new string(' ', indent)).Append("- ").AppendLine(info.Name);
                    continue;
                }

                var dir = info as IDirectoryInfo;
                if (dir != null)
                {
                    builder.Append(new string(' ', indent))
                        .Append("+ ")
                        .Append(info.Name)
                        .AppendLine(Path.DirectorySeparatorChar.ToString());
                    this.AppendDump(builder, dir, indent + 2);
                }
            }
        }
    }
}