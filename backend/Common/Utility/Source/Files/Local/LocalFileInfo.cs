// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System.IO;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local file info.
    /// </summary>
    internal class LocalFileInfo : LocalFileSystemInfo<FileInfo>, IWritableFileInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileInfo"/> class.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="fileSystem">
        /// The file system creating this info.
        /// </param>
        public LocalFileInfo(FileInfo file, IWritableFileSystem fileSystem)
            : base(file, fileSystem)
        {
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public IDirectoryInfo Directory
        {
            get
            {
                this.VerifyVaild();
                return ((IWritableFileInfo)this).Directory;
            }
        }

        /// <summary>
        /// Gets the size of this file in bytes.
        /// </summary>
        public long Size
        {
            get
            {
                this.VerifyVaild();
                return this.Item.Length;
            }
        }

        IWritableDirectoryInfo IWritableFileInfo.Directory
        {
            get
            {
                this.VerifyVaild();
                DirectoryInfo directory = this.Item.Directory;
                return new LocalDirectoryInfo(directory, this.FileSystem);
            }
        }

        /// <summary>
        /// Opens this file for reading.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream OpenRead()
        {
            this.VerifyVaild();
            return this.Item.OpenRead();
        }

        /// <summary>
        /// Opens this file to read UTF-8 text.
        /// </summary>
        /// <returns>
        /// The <see cref="TextReader"/>.
        /// </returns>
        public TextReader OpenText()
        {
            this.VerifyVaild();
            return this.Item.OpenText();
        }

        /// <summary>
        /// Opens this file for writing.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream OpenWrite()
        {
            this.VerifyVaild();
            return new FileStream(this.Item.FullName, FileMode.Truncate);
        }

        /// <summary>
        /// Opens this file for writing by appending to an existing file.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public Stream OpenAppend()
        {
            this.VerifyVaild();
            var stream = new FileStream(this.Item.FullName, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.End);
            return stream;
        }

        /// <summary>
        /// Moves the file to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="IOException"/> being thrown.
        /// After this call, use the returned <see cref="IWritableFileInfo"/>
        /// instead of this object.
        /// </summary>
        /// <param name="newFileName">
        /// The new location where the file should be moved to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableFileInfo"/> that describes this file at the
        /// new location.
        /// </returns>
        public IWritableFileInfo MoveTo(string newFileName)
        {
            this.VerifyVaild();
            this.Item.MoveTo(newFileName);
            this.IsValid = false;
            return new LocalFileInfo(new FileInfo(newFileName), this.FileSystem);
        }

        /// <summary>
        /// Copies this file to a new location.
        /// </summary>
        /// <param name="newFileName">
        /// The new location where the file should be copied to.
        /// </param>
        /// <returns>
        /// An <see cref="IWritableFileInfo"/> that describes the copy of this
        /// file at the new location.
        /// </returns>
        /// <exception cref="IOException">
        /// If there is already a file at <see cref="newFileName"/>.
        /// </exception>
        public IWritableFileInfo CopyTo(string newFileName)
        {
            this.VerifyVaild();
            var info = this.Item.CopyTo(newFileName, false);
            return new LocalFileInfo(info, this.FileSystem);
        }
    }
}