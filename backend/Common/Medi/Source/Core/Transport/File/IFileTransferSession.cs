// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileTransferSession.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFileTransferSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;
    using System.IO;

    /// <summary>
    /// Session to a file system that can be used to 
    /// query information from it and read/write files.
    /// A session has always a fixed directory in which
    /// it is working. This directory can't be changed 
    /// through this interface and is in most cases defined
    /// in the constructor of the implementation of this interface.
    /// </summary>
    internal interface IFileTransferSession : IDisposable
    {
        /// <summary>
        /// Lists all files in the working directory.
        /// </summary>
        /// <returns>
        /// A list of file names that can later be used
        /// to call any of the following methods. The file names are
        /// in relative, local format and don't contain directory information.
        /// </returns>
        string[] ListFiles();

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
        Stream OpenWrite(string fileName);

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
        Stream OpenRead(string fileName);

        /// <summary>
        /// Renames the file from the original name to the new one.
        /// </summary>
        /// <param name="origFileName">
        /// The original file name.
        /// </param>
        /// <param name="newFileName">
        /// The new file name.
        /// </param>
        void Rename(string origFileName, string newFileName);

        /// <summary>
        /// Deletes the given file from the file system.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        void DeleteFile(string fileName);
    }
}
