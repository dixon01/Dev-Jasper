// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting
{
    using System.IO;

    using Gorba.Common.Utility.Core.IO;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// <see cref="IWritableFileInfo"/> implementation for
    /// <see cref="TestingFileSystem"/>.
    /// </summary>
    public class TestingFileInfo : TestingFileSystemInfo, IWritableFileInfo
    {
        private readonly TestingFileSystem fileSystem;

        private byte[] contents = new byte[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingFileInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">
        /// The file system creating this object.
        /// </param>
        /// <param name="parent">
        /// The parent directory.
        /// </param>
        /// <param name="path">
        /// The path of the file.
        /// </param>
        public TestingFileInfo(TestingFileSystem fileSystem, IWritableDirectoryInfo parent, string path)
            : base(fileSystem, path)
        {
            this.fileSystem = fileSystem;
            this.Directory = parent;

            int index = this.Name.LastIndexOf('.');
            if (index > 0)
            {
                this.Extension = this.Name.Substring(index);
            }
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public IWritableDirectoryInfo Directory { get; private set; }

        /// <summary>
        /// Gets the size of this file in bytes.
        /// </summary>
        public long Size
        {
            get
            {
                return this.contents == null ? 0L : this.contents.LongLength;
            }
        }

        IDirectoryInfo IFileInfo.Directory
        {
            get
            {
                return this.Directory;
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
        /// Opens this file for writing.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public Stream OpenWrite()
        {
            this.contents = null;
            return new TestFileWriteStream(this);
        }

        /// <summary>
        /// Opens this file for writing by appending to an existing file.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public Stream OpenAppend()
        {
            var oldContents = this.contents;
            var stream = this.OpenWrite();
            stream.Write(oldContents, 0, oldContents.Length);
            return stream;
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
        /// <exception cref="System.IO.IOException">
        /// If there is already a file at <see cref="newFileName"/>.
        /// </exception>
        public IWritableFileInfo CopyTo(string newFileName)
        {
            if (this.fileSystem.ItemExists(newFileName))
            {
                throw new IOException("File already exists: " + newFileName);
            }

            if (this.contents == null)
            {
                throw new IOException("File is currently open: " + this.FullName);
            }

            var copy = (TestingFileInfo)this.fileSystem.CreateFile(newFileName);
            copy.Attributes = this.Attributes;
            copy.contents = (byte[])this.contents.Clone();
            return copy;
        }

        /// <summary>
        /// Moves the file to a new location.
        /// This method invalidates this object, any further calls to this
        /// object will result in an <see cref="System.IO.IOException"/> being thrown.
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
            var copy = this.CopyTo(newFileName);
            this.Delete();
            return copy;
        }

        /// <summary>
        /// Opens this file for reading.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public Stream OpenRead()
        {
            return new MemoryStream(this.contents, false);
        }

        /// <summary>
        /// Opens this file to read UTF-8 text.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.TextReader"/>.
        /// </returns>
        public TextReader OpenText()
        {
            return new StreamReader(this.OpenRead());
        }

        private class TestFileWriteStream : WrapperStream
        {
            private readonly TestingFileInfo parent;

            private readonly MemoryStream stream = new MemoryStream();

            public TestFileWriteStream(TestingFileInfo parent)
            {
                this.parent = parent;
                this.Open(this.stream);
            }

            public override void Close()
            {
                base.Close();
                this.parent.contents = this.stream.ToArray();
            }
        }
    }
}