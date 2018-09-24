// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileReceivedLogFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileReceivedLogFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System.IO;

    /// <summary>
    /// Implementation of <see cref="IReceivedLogFile"/> for a file that resides in the local file system.
    /// </summary>
    public class FileReceivedLogFile : IReceivedLogFile
    {
        private readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReceivedLogFile"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        public FileReceivedLogFile(string unitName, string filePath)
            : this(unitName, filePath, filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReceivedLogFile"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name from which the <see cref="FileName"/> will be taken.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        public FileReceivedLogFile(string unitName, string originalFileName, string filePath)
        {
            this.UnitName = unitName;
            this.filePath = filePath;
            this.FileName = Path.GetFileName(originalFileName);
        }

        /// <summary>
        /// Gets the unit name from which this log file comes.
        /// </summary>
        public string UnitName { get; private set; }

        /// <summary>
        /// Gets the file name (without path) of the log file.
        /// </summary>
        public string FileName { get; private set; }

        public string FilePath
        {
            get { return filePath; }
        }

        /// <summary>
        /// Copies this log file to the given path.
        /// </summary>
        /// <param name="destinationPath">
        /// The full file path where to copy the log file to.
        /// </param>
        public virtual void CopyTo(string destinationPath)
        {
            File.Copy(this.filePath, destinationPath, true);
        }

        /// <summary>
        /// Opens this log file for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the log file.
        /// </returns>
        public virtual Stream OpenRead()
        {
            return File.OpenRead(this.filePath);
        }

        public void Delete()
        {
            File.Delete(this.filePath);
        }
    }
}