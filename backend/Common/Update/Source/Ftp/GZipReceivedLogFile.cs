// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GZipReceivedLogFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GZipReceivedLogFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System.IO;

    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core.IO;

    using ICSharpCode.SharpZipLib.GZip;

    /// <summary>
    /// Implementation of <see cref="IReceivedLogFile"/> that decompresses the received file
    /// when copying or opening.
    /// </summary>
    internal class GZipReceivedLogFile : FileReceivedLogFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GZipReceivedLogFile"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        public GZipReceivedLogFile(string unitName, string filePath)
            : base(unitName, filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GZipReceivedLogFile"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name from which the <see cref="FileReceivedLogFile.FileName"/> will be taken.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        public GZipReceivedLogFile(string unitName, string originalFileName, string filePath)
            : base(unitName, originalFileName, filePath)
        {
        }

        /// <summary>
        /// Copies this log file to the given path.
        /// </summary>
        /// <param name="destinationPath">
        /// The full file path where to copy the log file to.
        /// </param>
        public override void CopyTo(string destinationPath)
        {
            using (var input = this.OpenRead())
            {
                using (var output = File.Create(destinationPath))
                {
                    StreamCopy.Copy(input, output);
                }
            }
        }

        /// <summary>
        /// Opens this log file for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the log file.
        /// </returns>
        public override Stream OpenRead()
        {
            return new GZipInputStream(base.OpenRead());
        }
    }
}