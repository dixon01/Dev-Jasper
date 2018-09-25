// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDirectoryInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalDirectoryInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System.IO;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local directory info.
    /// </summary>
    internal class LocalDirectoryInfo : LocalFileSystemInfo<DirectoryInfo>, IWritableDirectoryInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDirectoryInfo"/> class.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        /// <param name="fileSystem">
        /// The file system creating this info.
        /// </param>
        public LocalDirectoryInfo(DirectoryInfo directory, IWritableFileSystem fileSystem)
            : base(directory, fileSystem)
        {
        }

        /// <summary>
        /// Gets the root.
        /// </summary>
        public IDirectoryInfo Root
        {
            get
            {
                this.VerifyVaild();
                return ((IWritableDirectoryInfo)this).Root;
            }
        }

        IWritableDirectoryInfo IWritableDirectoryInfo.Root
        {
            get
            {
                this.VerifyVaild();
                DirectoryInfo directory = this.Item.Root;
                return new LocalDirectoryInfo(directory, this.FileSystem);
            }
        }

        /// <summary>
        /// Gets all file system items in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files and directories.
        /// </returns>
        public IFileSystemInfo[] GetFileSystemInfos()
        {
            this.VerifyVaild();
            FileSystemInfo[] array = this.Item.GetFileSystemInfos();
            return ArrayUtil.ConvertAll(
                array, i => (IFileSystemInfo)LocalFileSystem.CreateFileSystemInfo(i, this.FileSystem));
        }

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns>
        /// A list of all files.
        /// </returns>
        public IFileInfo[] GetFiles()
        {
            this.VerifyVaild();
            FileInfo[] array = this.Item.GetFiles();
            return ArrayUtil.ConvertAll(array, i => (IFileInfo)new LocalFileInfo(i, this.FileSystem));
        }

        /// <summary>
        /// Gets all directories in this directory.
        /// </summary>
        /// <returns>
        /// A list of all directories.
        /// </returns>
        public IDirectoryInfo[] GetDirectories()
        {
            this.VerifyVaild();
            DirectoryInfo[] array = this.Item.GetDirectories();
            return ArrayUtil.ConvertAll(array, i => (IDirectoryInfo)new LocalDirectoryInfo(i, this.FileSystem));
        }

        /// <summary>
        /// Moves the directory to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
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
            this.VerifyVaild();
            this.Item.MoveTo(newDirectoryName);
            return new LocalDirectoryInfo(new DirectoryInfo(newDirectoryName), this.FileSystem);
        }

        IWritableFileSystemInfo[] IWritableDirectoryInfo.GetFileSystemInfos()
        {
            this.VerifyVaild();
            FileSystemInfo[] array = this.Item.GetFileSystemInfos();
            return ArrayUtil.ConvertAll(array, i => LocalFileSystem.CreateFileSystemInfo(i, this.FileSystem));
        }

        IWritableFileInfo[] IWritableDirectoryInfo.GetFiles()
        {
            this.VerifyVaild();
            FileInfo[] array = this.Item.GetFiles();
            return ArrayUtil.ConvertAll(array, i => (IWritableFileInfo)new LocalFileInfo(i, this.FileSystem));
        }

        IWritableDirectoryInfo[] IWritableDirectoryInfo.GetDirectories()
        {
            this.VerifyVaild();
            DirectoryInfo[] array = this.Item.GetDirectories();
            return ArrayUtil.ConvertAll(array, i => (IWritableDirectoryInfo)new LocalDirectoryInfo(i, this.FileSystem));
        }
    }
}