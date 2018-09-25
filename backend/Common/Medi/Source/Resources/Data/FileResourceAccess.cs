// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceAccess.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResourceAccess type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System.IO;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The resource access implementation that uses a file.
    /// </summary>
    public class FileResourceAccess : IResourceAccess
    {
        private readonly IWritableFileInfo file;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResourceAccess"/> class.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public FileResourceAccess(IWritableFileInfo file)
        {
            this.file = file;
        }

        /// <summary>
        /// Copies the resource to the given file location.
        /// </summary>
        /// <param name="newFileName">
        /// The local file name to copy the file to.
        /// </param>
        public void CopyTo(string newFileName)
        {
            this.file.CopyTo(newFileName);
        }

        /// <summary>
        /// Opens the stream to read the resource from it.
        /// </summary>
        /// <returns>
        /// A stream from which the resource can be read.
        /// </returns>
        public Stream OpenRead()
        {
            return this.file.OpenRead();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.file.FullName;
        }
    }
}