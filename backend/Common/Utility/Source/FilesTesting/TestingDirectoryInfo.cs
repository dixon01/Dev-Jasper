// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingDirectoryInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingDirectoryInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting
{
    using System.IO;
    using System.Linq;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// <see cref="IWritableDirectoryInfo"/> implementation for
    /// <see cref="TestingFileSystem"/>.
    /// </summary>
    public class TestingDirectoryInfo : TestingFileSystemInfo, IWritableDirectoryInfo
    {
        private readonly TestingFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingDirectoryInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system creating this object.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        public TestingDirectoryInfo(TestingFileSystem fileSystem, string path)
            : base(fileSystem, path)
        {
            this.fileSystem = fileSystem;
            this.Attributes = FileAttributes.Directory;

            this.Root = path.Length <= 3 ? this : new TestingDirectoryInfo(fileSystem, path.Substring(0, 3));

            this.Name = this.Name.TrimEnd(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the root directory of this directory.
        /// </summary>
        public IWritableDirectoryInfo Root { get; private set; }

        FileAttributes IFileSystemInfo.Attributes
        {
            get
            {
                return this.Attributes;
            }
        }

        IDirectoryInfo IDirectoryInfo.Root
        {
            get
            {
                return this.Root;
            }
        }

        /// <summary>
        /// Deletes this item.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
        /// </summary>
        public override void Delete()
        {
            this.fileSystem.Delete(this);
        }

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        public IWritableFileInfo[] GetFiles()
        {
            var infos = this.fileSystem.GetFileSystemInfos(this);
            return infos.OfType<IWritableFileInfo>().ToArray();
        }

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        public IWritableDirectoryInfo[] GetDirectories()
        {
            var infos = this.fileSystem.GetFileSystemInfos(this);
            return infos.OfType<IWritableDirectoryInfo>().ToArray();
        }

        /// <summary>
        /// Moves the directory to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
        /// After this call, use the returned <see cref="IWritableDirectoryInfo"/>
        /// instead of this object.
        /// </summary>
        /// <param name="newDirectoryName">
        /// The new location where the directory should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableDirectoryInfo"/> that describes this directory at the
        /// new location.
        /// </returns>
        public IWritableDirectoryInfo MoveTo(string newDirectoryName)
        {
            return this.fileSystem.Move(this, newDirectoryName);
        }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        public IWritableFileSystemInfo[] GetFileSystemInfos()
        {
            return this.fileSystem.GetFileSystemInfos(this).ToArray();
        }

        IFileSystemInfo[] IDirectoryInfo.GetFileSystemInfos()
        {
            var infos = this.fileSystem.GetFileSystemInfos(this);
            return infos.Cast<IFileSystemInfo>().ToArray();
        }

        IFileInfo[] IDirectoryInfo.GetFiles()
        {
            var infos = this.fileSystem.GetFileSystemInfos(this);
            return infos.OfType<IFileInfo>().ToArray();
        }

        IDirectoryInfo[] IDirectoryInfo.GetDirectories()
        {
            var infos = this.fileSystem.GetFileSystemInfos(this);
            return infos.OfType<IDirectoryInfo>().ToArray();
        }

        /// <summary>
        /// Renames this item.
        /// </summary>
        /// <param name="path">
        /// The new full path.
        /// </param>
        internal override void Rename(string path)
        {
            base.Rename(path);
            this.Name = this.Name.TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}