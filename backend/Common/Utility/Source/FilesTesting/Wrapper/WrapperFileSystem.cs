// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapperFileSystem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrapperFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.FilesTesting.Wrapper
{
    using System.Linq;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Wrapper around an <see cref="IWritableFileSystem"/>.
    /// </summary>
    public class WrapperFileSystem : IWritableFileSystem
    {
        private readonly IWritableFileSystem wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperFileSystem"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped object.
        /// </param>
        public WrapperFileSystem(IWritableFileSystem wrapped)
        {
            this.wrapped = wrapped;
        }

        /// <summary>
        /// Creates a <see cref="WrapperFileInfo"/> for the given file.
        /// You can override this method if you require to return your own wrapper objects.
        /// </summary>
        /// <param name="file">
        /// The <see cref="IWritableFileInfo"/> to wrap.
        /// </param>
        /// <returns>
        /// The wrapping <see cref="WrapperFileInfo"/>.
        /// </returns>
        public virtual WrapperFileInfo CreateFileInfo(IWritableFileInfo file)
        {
            return new WrapperFileInfo(file, this);
        }

        /// <summary>
        /// Creates a <see cref="WrapperDirectoryInfo"/> for the given directory.
        /// You can override this method if you require to return your own wrapper objects.
        /// </summary>
        /// <param name="directory">
        /// The <see cref="IWritableDirectoryInfo"/> to wrap.
        /// </param>
        /// <returns>
        /// The wrapping <see cref="WrapperDirectoryInfo"/>.
        /// </returns>
        public virtual WrapperDirectoryInfo CreateDirectoryInfo(IWritableDirectoryInfo directory)
        {
            return new WrapperDirectoryInfo(directory, this);
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
            IWritableFileInfo writableFile;
            if (this.TryGetFile(path, out writableFile))
            {
                file = writableFile;
                return true;
            }

            file = null;
            return false;
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
            IWritableDirectoryInfo writableDirectory;
            if (this.TryGetDirectory(path, out writableDirectory))
            {
                directory = writableDirectory;
                return true;
            }

            directory = null;
            return false;
        }

        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        public virtual IWritableDriveInfo[] GetDrives()
        {
            return this.wrapped.GetDrives().Select(d => (IWritableDriveInfo)new WrapperDriveInfo(d, this)).ToArray();
        }

        /// <summary>
        /// Gets a file for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IWritableFileInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// if the given file can't be found.
        /// </exception>
        public virtual IWritableFileInfo GetFile(string path)
        {
            return this.wrapped.GetFile(path);
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
        public virtual bool TryGetFile(string path, out IWritableFileInfo file)
        {
            if (!this.wrapped.TryGetFile(path, out file))
            {
                return false;
            }

            file = this.CreateFileInfo(file);
            return true;
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
        public virtual IWritableFileInfo CreateFile(string path)
        {
            return this.CreateFileInfo(this.wrapped.CreateFile(path));
        }

        /// <summary>
        /// Gets a directory for a given path.
        /// </summary>
        /// <param name="path">
        /// The relative or absolute path.
        /// </param>
        /// <returns>
        /// The <see cref="IWritableDirectoryInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// if the given directory can't be found.
        /// </exception>
        public virtual IWritableDirectoryInfo GetDirectory(string path)
        {
            return this.CreateDirectoryInfo(this.wrapped.GetDirectory(path));
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
        public virtual bool TryGetDirectory(string path, out IWritableDirectoryInfo directory)
        {
            if (!this.wrapped.TryGetDirectory(path, out directory))
            {
                return false;
            }

            directory = this.CreateDirectoryInfo(directory);
            return true;
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
        public virtual IWritableDirectoryInfo CreateDirectory(string path)
        {
            return this.CreateDirectoryInfo(this.wrapped.CreateDirectory(path));
        }

        IFileInfo IFileSystem.GetFile(string path)
        {
            return this.GetFile(path);
        }

        IDirectoryInfo IFileSystem.GetDirectory(string path)
        {
            return this.GetDirectory(path);
        }

        IDriveInfo[] IFileSystem.GetDrives()
        {
            return this.GetDrives().Cast<IDriveInfo>().ToArray();
        }
    }
}
