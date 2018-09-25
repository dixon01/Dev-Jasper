// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileTransferSession.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileTransferSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File.Local
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Implementation of <see cref="IFileTransferSession"/> to access the local
    /// file system.
    /// </summary>
    internal class LocalFileTransferSession : IFileTransferSession
    {
        private readonly DirectoryInfo baseDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileTransferSession"/> class.
        /// Do not call this constructor directly, but rather use
        /// <see cref="FileTransferSessionProvider.OpenSession"/> instead.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        internal LocalFileTransferSession(Uri baseUrl)
        {
            this.baseDir = new DirectoryInfo(baseUrl.LocalPath);

            if (!this.baseDir.Exists)
            {
                this.baseDir.Create();
                this.baseDir.Refresh();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Lists all files in the working directory.
        /// </summary>
        /// <returns>
        /// A list of file names that can later be used
        /// to call any of the following methods. The file names are
        /// in relative, local format and don't contain directory information.
        /// </returns>
        public string[] ListFiles()
        {
            var files = ArrayUtil.ConvertAll(this.baseDir.GetFiles(), fi => fi.Name);
            return files;
        }

        /// <summary>
        /// Opens the given file for writing, creating it if it doesn't exist yet.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// A stream that can be used to write to the file.
        /// <see cref="Stream.Close"/> should be called before calling any other
        /// method on this session.
        /// </returns>
        public Stream OpenWrite(string fileName)
        {
            var path = this.GetFullPath(fileName);
            return File.OpenWrite(path);
        }

        /// <summary>
        /// Opens the given file for reading.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// A stream that can be used to read from the file.
        /// <see cref="Stream.Close"/> should be called before calling any other
        /// method on this session.
        /// </returns>
        public Stream OpenRead(string fileName)
        {
            var path = this.GetFullPath(fileName);
            return File.OpenRead(path);
        }

        /// <summary>
        /// Renames the file from the original name to the new one.
        /// </summary>
        /// <param name="origFileName">
        /// The original file name.
        /// </param>
        /// <param name="newFileName">
        /// The new file name.
        /// </param>
        public void Rename(string origFileName, string newFileName)
        {
            var source = this.GetFullPath(origFileName);
            var dest = this.GetFullPath(newFileName);
            File.Move(source, dest);
        }

        /// <summary>
        /// Deletes the given file from the file system.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void DeleteFile(string fileName)
        {
            var path = this.GetFullPath(fileName);
            File.Delete(path);
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(this.baseDir.FullName, fileName);
        }
    }
}