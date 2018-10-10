// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting.Wrapper
{
    using System.IO;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Wrapper around an <see cref="IWritableFileInfo"/>.
    /// </summary>
    public class WrapperFileInfo : WrapperFileSystemInfo, IWritableFileInfo
    {
        private readonly IWritableFileInfo wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperFileInfo"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped object.
        /// </param>
        /// <param name="fileSystem">
        /// The file system.
        /// </param>
        public WrapperFileInfo(IWritableFileInfo wrapped, WrapperFileSystem fileSystem)
            : base(wrapped, fileSystem)
        {
            this.wrapped = wrapped;
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public virtual IWritableDirectoryInfo Directory
        {
            get
            {
                return this.wrapped.Directory;
            }
        }

        /// <summary>
        /// Gets the size of this file in bytes.
        /// </summary>
        public virtual long Size
        {
            get
            {
                return this.wrapped.Size;
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
        /// Opens this file for reading.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public virtual Stream OpenRead()
        {
            return this.wrapped.OpenRead();
        }

        /// <summary>
        /// Opens this file to read UTF-8 text.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.TextReader"/>.
        /// </returns>
        public virtual TextReader OpenText()
        {
            return this.wrapped.OpenText();
        }

        /// <summary>
        /// Opens this file for writing.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public virtual Stream OpenWrite()
        {
            return this.wrapped.OpenWrite();
        }

        /// <summary>
        /// Opens this file for writing by appending to an existing file.
        /// </summary>
        /// <returns>
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public virtual Stream OpenAppend()
        {
            return this.wrapped.OpenAppend();
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
        public virtual IWritableFileInfo MoveTo(string newFileName)
        {
            return this.wrapped.MoveTo(newFileName);
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
        public virtual IWritableFileInfo CopyTo(string newFileName)
        {
            return this.wrapped.CopyTo(newFileName);
        }
    }
}