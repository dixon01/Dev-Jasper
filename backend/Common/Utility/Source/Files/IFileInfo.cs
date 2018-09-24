// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files
{
    using System.IO;

    /// <summary>
    /// File information.
    /// </summary>
    public interface IFileInfo : IFileSystemInfo
    {
        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        IDirectoryInfo Directory { get; }

        /// <summary>
        /// Gets the size of this file in bytes.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Opens this file for reading.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream OpenRead();

        /// <summary>
        /// Opens this file to read UTF-8 text.
        /// </summary>
        /// <returns>
        /// The <see cref="TextReader"/>.
        /// </returns>
        TextReader OpenText();
    }
}